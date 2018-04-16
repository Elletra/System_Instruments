function Player::playPhrase(%this, %phrase) {
  if (%this.instrument $= "") { 
    return; 
  }

  if (%this.isPlayingInstrument) {
    %this.stopPlaying();
  }

  InstrumentsServer.playPhrase(%this, %phrase);
}

function GameConnection::playPhrase(%this, %phrase, %instrument) {
  %instrumentObj = InstrumentsServer._(%instrument);

  if (!isObject(%instrumentObj)) {
    return;
  }

  if (%instrumentObj.length $= "") { 
    return; 
  }

  if (%instrumentObj.length < 1) { 
    return; 
  }

  if (%this.isPlayingInstrument) {
    %this.stopPlaying();
  }
  
  %this.instrument = %instrument;
  InstrumentsServer.playPhrase(%this, %phrase);
}

function serverCmdPlayPhrase(%client, %phrase, %instrument, %preview) {
  if (!InstrumentsServer.checkInstrumentPermissions(%client, 1)) {
    return;
  }

  if (strLen(%phrase) > Instruments.const["MAX_PHRASE_LENGTH"]) {
    messageClient(%client, '', "\c6Maximum phrase length is \c3" @ Instruments.const["MAX_PHRASE_LENGTH"] @ " characters");
    return;
  }

  %timeout = Instruments.const["LOWEST_DELAY"] / 1.5;

  if (%client.playPhraseTimeout !$= "" && getSimTime() - %client.playPhraseTimeout < %timeout) {
    InstrumentsServer.stopPlaying(%client);
    return;
  }

  %client.playPhraseTimeout = getSimTime();

  if (%preview) {
    %client.playPhrase(%phrase, %instrument);
  }
  else if (isObject(%player = %client.player)) {
    %player.playPhrase(%phrase);
  }
}

// Main function

function InstrumentsServer::playPhrase(%this, %obj, %phrase, %phraseDelay, %delay) {
  if (!isObject(%obj)) { 
    return; 
  }

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer") {
    %client = %obj.client;

    if (%obj.isDisabled()) {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }

    // If the client is previewing a phrase or a song, stop them
    if (isObject(%client) && %client.isPlayingInstrument) {
      InstrumentsServer.stopPlaying(%client);
    }
  }
  else if (%obj.getClassName() $= "GameConnection") {
    %player = %obj.player;

    // If the player is playing a phrase or a song, stop them
    if (isObject(%player) && %player.isPlayingInstrument) {
      InstrumentsServer.stopPlaying(%player);
    }
  }

  cancel(%obj.phraseSchedule);
  cancel(%obj.songSchedule);

  // Which note we are on
  %noteIndex = %obj.noteIndex;

  if (%noteIndex $= "") {
    %noteIndex = 0;
    %obj.noteIndex = 0;
  }

  // Separate notes into fields and remove spaces
  %phrase = strReplace(strReplace(%phrase, ",", "\t"), " ", "");
  %obj.instrumentPhrase = %phrase;

  // Get note at current %noteIndex
  %note = getField(%phrase, %noteIndex);

  %noteCount = getFieldCount(%phrase);
  %notesLeft = %noteCount - %noteIndex;

  // Check to see if there are no more notes left
  if (%notesLeft < 1) {

    // If there are no notes left and the client is not playing a song, stop playing...
    if (%obj.instrumentSong $= "") {
      InstrumentsServer.stopPlaying(%obj);
      %obj.processInputEvent("onPhraseEnd");
      return;
    }

    // ...otherwise we're probably gonna be going to the next phrase in the song
    %phraseIndex = %obj.phraseIndex;

    if (%phraseIndex $= "") {
      %phraseIndex = 0;
      %obj.phraseIndex = 0;
    }

    // Separate the song phrase indices into fields and remove spaces
    %song = strReplace(strReplace(%obj.instrumentSong, ",", "\t"), " ", "");

    %phraseCount = getFieldCount(%song);
    %phrasesLeft = %phraseCount - %phraseIndex;

    // Check if the song is over
    if (%phrasesLeft <= 1) {

      // If there's a repeat, go back to the beginning of the song...
      if (strPos(%phrase, "%") != -1) {
        %obj.phraseIndex = 0;
        %obj.processInputEvent("onSongLoop");
      }
      // ...otherwise stop playing
      else {
        InstrumentsServer.stopPlaying(%obj);
        %obj.processInputEvent("onSongEnd");
        return;
      }
    }
    else {
      %obj.phraseIndex++;
    }

    %obj.noteIndex = 0;
    %obj.songSchedule = InstrumentsServer.playSong(%obj, %song, %phraseDelay, %delay);

    return;
  }

  // Phrase/song preview handling for the GUI...
  if (%obj.getClassName() $= "GameConnection") {

    // If isPlayingInstrument isn't set, let the client know they are playing an instrument
    // No need to tell them every single note, though
    if (!%obj.isPlayingInstrument) {
      commandToClient(%obj, 'startPlayingInstrument', 1);
    }
  }

  // ...otherwise it's probably a player or a bot
  else if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer") {

    // If the player is not holding an item, stop playing
    if (!isObject(%instrument = %obj.getMountedImage(0))) {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }

    // If the item they are holding is not an instrument, stop playing
    if (%instrument.instrumentType $= "") {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }

    // If isPlayingInstrument isn't set, let the client know they are playing an instrument
    // No need to tell them every single note, though
    if (!%obj.isPlayingInstrument && isObject(%client)) {
      commandToClient(%client, 'startPlayingInstrument', 0);
    }
  }
  
  %instrument = %obj.instrument;
  %obj.isPlayingInstrument = true;
  %skipToNextNote = false;

  // Tempo change
  if (strLwr(getSubStr(%note, 0, 2)) $= "t:") {

    // Beats per minute to millisecond delay conversion
    // (1 / bpm) * 60,000 = delay
    %phraseDelay = mRound((1 / getSubStr(%note, 2, strLen(%note))) * 60000);

    // FYI: The inverse would be 60,000 / delay = bpm

    %skipToNextNote = true;
  }
  // Delay change
  else if (strLwr(getSubStr(%note, 0, 2)) $= "d:") {
    %phraseDelay = getSubStr(%note, 2, strLen(%note));

    // If the delay has been changed, just skip to the next note
    // Delay and tempo changes must be by themselves
    %skipToNextNote = true;
  }

  // Tempo and delay changes are the same under the hood; some people just like having options


  if (%phraseDelay $= "") {
    %phraseDelay = Instruments.const["DEFAULT_DELAY"];
  }

  // %phraseDelay is the delay throughout the entire phrase or song, whereas %delay resets after every note
  %delay = %phraseDelay;

  // Strip note of rests and other irrelevant characters,
  // except + for chords, : for pitches, and . for decimal numbers for pitches
  // (as well as # for sharps, ? for random, and some other ones)
  %playNote = _cleanNote(%note);

  // Check to see if there is even a note to be played, and whether or not we're skipping to the next one
  // No use playing a section that has no musical notes
  if (%playNote !$= "" && !%skipToNextNote) {
    InstrumentsServer.playNote(%obj, %playNote, %instrument);
  }

  // If it's just a repeat, just skip to the next note
  if (strReplace(%note, "%", "") $= "") {
    %skipToNextNote = true;
  }


  if (!%skipToNextNote) {
    // Adjust %delay based on the value and number of rests
    %delay += (strCount(%note, ";") * 0.25) * %phraseDelay;
    %delay += (strCount(%note, "/") * 0.5) * %phraseDelay;
    %delay += (strCount(%note, "$")) * %phraseDelay;
    %delay += (strCount(%note, "-") * 2) * %phraseDelay;
    %delay += (strCount(%note, "_") * 4) * %phraseDelay;

    // `>` cuts %delay in half, quarters, eighths, etc. depending on how many you put (2^n)

    if ((%halves = strCount(%note, ">")) > 0) {
      %delay /= (mPow(2, %halves));
    }
  }

  %delay = mRound(%delay);
  %phraseDelay = mRound(%phraseDelay);


  if (%skipToNextNote) {
    // schedules with a delay of 0 lag for some reason
    %delay = 1;
  }
  // Hard-coded limits
  else if (%delay < Instruments.const["LOWEST_DELAY"]) {
    %delay = Instruments.const["LOWEST_DELAY"];
  }
  else if (%delay > Instruments.const["HIGHEST_DELAY"]) {
    %delay = Instruments.const["HIGHEST_DELAY"];
  }

  if (%phraseDelay < Instruments.const["LOWEST_DELAY"]) {
    %phraseDelay = Instruments.const["LOWEST_DELAY"];
  }
  else if (%phraseDelay > Instruments.const["HIGHEST_DELAY"]) {
    %phraseDelay = Instruments.const["HIGHEST_DELAY"];
  }

  // There is special handling for repeats when a song is being played, so we need to make sure that 
  // they aren't playing a song at the moment
  if (strPos(%note, "%") != -1 && %obj.instrumentSong $= "") {
    %obj.noteIndex = 0;
    %obj.processInputEvent("onPhraseLoop");
  }
  else {
    %obj.noteIndex++;
  }

  %obj.phraseSchedule = InstrumentsServer.schedule(%delay, playPhrase, %obj, %phrase, %phraseDelay, %delay);
}
