function GameConnection::playSong(%this, %song, %instrument) {
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
  InstrumentsServer.playSong(%this, %song);
}

function Player::playSong(%this, %song) {
  if (%this.instrument $= "") { 
    return; 
  }
  
  InstrumentsServer.playSong(%this, %song);
}

function serverCmdPlaySong(%client, %song, %instrument, %preview) {
  if (!InstrumentsServer.checkSongPermissions(%client, 1)) { 
    return; 
  }

  %timeout = Instruments.const["LOWEST_DELAY"] / 1.5;

  if (%client.playPhraseTimeout !$= "" && getSimTime() - %client.playPhraseTimeout < %timeout) {
    InstrumentsServer.stopPlaying(%client);
    return;
  }

  %client.playPhraseTimeout = getSimTime();

  if (%preview) {
    %client.playSong(%song, %instrument);
  }
  else if (isObject(%player = %client.player)) {
    %player.playSong(%song);
  }
}

// Main function

function InstrumentsServer::playSong(%this, %obj, %song, %songDelay, %delay) {
  if (!isObject(%obj)) {
    return;
  }

  cancel(%obj.phraseSchedule);
  cancel(%obj.songSchedule);

  %song = strReplace(strReplace(%song, ",", "\t"), " ", "");
  %phraseCount = getFieldCount(%song);

  if (%phraseCount < 1) {
    InstrumentsServer.stopPlaying(%obj);
    return;
  }

  if (%phraseCount > Instruments.const["SONG_ORDER_LIMIT"]) {
    if (%obj.getClassName() $= "GameConnection") {
      Instruments.messageBoxOK("Phrase Limit Exceeded", 
        "You have exceeded the maximum number (" @ 
        Instruments.const["SONG_ORDER_LIMIT"] @ ") of phrases in a song.", %obj);
    }
    else {
      if (isObject(%client = %obj.client)) {
        Instruments.messageBoxOK("Phrase Limit Exceeded", 
          "You have exceeded the maximum number (" @ 
          Instruments.const["SONG_ORDER_LIMIT"] @ ") of phrases in a song.", %client);
      }
    }
    return;
  }

  if (%songDelay $= "") {
    %songDelay = Instruments.const["DEFAULT_DELAY"];
  }

  if (%delay $= "") {
    %delay = Instruments.const["DEFAULT_DELAY"];
  }

  %phraseIndex = %obj.phraseIndex;
  %noteIndex = %obj.noteIndex;

  if (%phraseIndex $= "") {
    %obj.phraseIndex = 0;
    %phraseIndex = 0;
  }

  if (%noteIndex $= "") {
    %obj.noteIndex = 0;
    %noteIndex = 0;
  }

  %index = getField(%song, %phraseIndex);

  if (%obj.getClassName() $= "GameConnection") {
    %phrase = %obj.songPhrase[%index];
  }
  else if (%obj.getClassName() $= "Player") {
    if (!isObject(%client = %obj.client)) {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }

    %phrase = %client.songPhrase[%index];
  }
  else if (%obj.getClassName() $= "AIPlayer") {
    %phrase = %obj.songPhrase[%index];
  }
  else {
    return;
  }
  
  %obj.instrumentSong = %song;
  InstrumentsServer.playPhrase(%obj, %phrase, %songDelay, %delay);
}
