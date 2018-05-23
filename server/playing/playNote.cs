function GameConnection::playNote(%this, %note, %instrument) {
  %instrumentObj = InstrumentsServer._(%instrument);

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
  InstrumentsServer.playNote(%this, %note, %instrument);
}

function Player::playNote(%this, %note) {
  %instrument = %this.instrument;
  
  if (%instrument $= "") { 
    return; 
  }

  if (%this.isPlayingInstrument) {
    %this.stopPlaying();
  }

  InstrumentsServer.playNote(%this, %note, %instrument);
}

function serverCmdPlayNote(%client, %note, %instrument, %preview) {
  if (%client.playNoteTimeout !$= "" && getSimTime() - %client.playNoteTimeout < 10) { 
    return; 
  }

  %note = _cleanNote(%note);
  
  if (%preview) {
    %client.playNote(%note, %instrument);
  }
  else if (isObject(%player = %client.player)) {
    %player.playNote(%note);
  }
}

function InstrumentsServer::playRandomNote(%this, %obj, %instrument) {
  InstrumentsServer.playNote(%obj, "?", %instrument);
}

function Instruments_Play3D(%obj, %sound, %position) {
  %count = ClientGroup.getCount();

  for (%i = 0; %i < %count; %i++) {
    %client = ClientGroup.getObject(%i);

    if (!%client.mutedInstruments[%obj]) {
      %client.play3D(%sound, %position);
    }
  }
}

// thx shock
function InstrumentsServer::playPitchedSound(%this, %obj, %sound, %pitch, %position) {
  %oldTimescale = getTimescale();
  setTimescale(%pitch);

  // Swollow says WebComPostServerUpdateLoop and pingMatchMakerLoop are needed,
  // or else eventually things get fucked up? idk



  // But they lag the server so I commented them out so let's see if it breaks anything!
  // What could possibly go wrong?????

  //if (!$Server::LAN) {
    //WebCom_PostServerUpdateLoop();
  //}

  //pingMatchMakerLoop();

  if (%position $= "") {
    %obj.playSound(%sound);
  }
  else {
    Instruments_Play3D(%obj, %sound, %position);
  }

  setTimescale(%oldTimescale);

  //if (!$Server::LAN) {
    //WebCom_PostServerUpdateLoop();
  //}

  //pingMatchMakerLoop();
}

// Main function

function InstrumentsServer::playNote(%this, %obj, %note, %instrument) {
  if (_strEmpty(%note)) {
    return;
  }
  
  // Hard-coded timeout
  if (%obj.playNoteTimeout !$= "" && getSimTime() - %obj.playNoteTimeout < 10) { 
    return; 
  }

  if (isObject(%client = %obj.client)) {
    if (%client.playNoteTimeout !$= "" && getSimTime() - %client.playNoteTimeout < 10) { 
      return; 
    }
  }

  if (strLwr(getSubStr(%note, 0, 2)) $= "d:" || strLwr(getSubStr(%note, 0, 2)) $= "t:") { 
    return; 
  }

  if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer") {
    // If the player is not holding an item, stop playing
    if (!isObject(%item = %obj.getMountedImage(0))) {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }

    // If the item they are holding is not an instrument, stop playing
    if (%item.instrumentType $= "") {
      InstrumentsServer.stopPlaying(%obj);
      return;
    }
  }

  %note = strUpr(%note);
  %color = "\c2";

  %bottomPrint = "<font:palatino linotype:64>";
  %chords = strReplace(%note, "+", "\t");
  %chordCount = getFieldCount(%chords);
  // Hard-coded max of 4 notes per chord
  %chordCount = %chordCount <= 4 ? %chordCount : 4;

  for (%c = 0; %c < %chordCount; %c++) {
    %sound = "";
    %note = getField(%chords, %c);

    // Pitch-changing
    %colonPos = strPos(%note, ":");
    %pitch = 1;

    if (%colonPos != -1) {
      %pitch = getSubStr(%note, %colonPos + 1, strLen(%note));
      %note = getSubStr(%note, 0, %colonPos);
      %customPitch = true;
    }

    // Random note
    if (%note $= "?") {
      %instrumentObj = InstrumentsServer._(%instrument);
      %index = getRandom(0, %instrumentObj.length - 1);
      %note = %instrumentObj.note(%index);

      if (Instruments.isMelodicNote(%note)) {
        %octave = Instruments.getNoteOctave(%note);
        %index = getRandom(0, getFieldCount($Instruments::Notes) - 1);

        %randomNote = getField($Instruments::Notes, %index) @ %octave;

        // Let's assign %sound now so we don't call getNoteSound twice
        %sound = InstrumentsServer.getNoteSound(%instrument, %randomNote);

        // If there is a sound file that can be used, then use that
        if (isObject(%sound)) {
          %note = %randomNote;
        }
        else {
          // Otherwise, %randomNote won't be used and we'll set
          // %sound from %note instead
          %sound = InstrumentsServer.getNoteSound(%instrument, %note);
        }
      }
    }

    if (%sound $= "") {
      %sound = InstrumentsServer.getNoteSound(%instrument, %note);
    }


    if (!isObject(%sound)) {
      if (%customPitch) {
        %pitchPrint = ":" @ %pitch;
      }

      %bottomPrint = %bottomPrint @ "\c7" @ %note @ %pitchPrint;
    }
    else {
      if (%pitch $= "") {
        %pitch = InstrumentsServer.getNotePitch(%note);
      }
      else {
        // If the user specifies a pitch, we're going to have to account for that
        %pitch--;
        %pitch += InstrumentsServer.getNotePitch(%note);

        // Examples:

        // If the user-specified pitch is 1.1, then it would subtract 1 from 1.1, which makes 0.1
        // then it adds the pitch of the note, which, if it's C, would just be 1, so 0.1 + 1 = 1.1
        // as our final pitch.

        // If the note is D and the user-specified pitch is 1.1 again, then it would subtract 1 from 1.1, 0.1
        // It then adds the pitch of the note (which for D is a C note set to the pitch 1.12), to 0.1, so
        // 0.1 + 1.12 = 1.22 as our final pitch.
      }

      if (%obj.getClassName() $= "GameConnection") {
        InstrumentsServer.playPitchedSound(%obj, %sound, %pitch);
      }
      else {
        %zScale = mRound(getWord(%obj.getScale(), 2) / 2);
        %pos = vectorAdd(%obj.getPosition(), "0 0" SPC %zScale);

        InstrumentsServer.playPitchedSound(%obj, %sound, %pitch, %pos);
      }

      if (%customPitch) {
        %pitchPrint = "\c4:" @ %pitch;
      }

      %bottomPrint = %bottomPrint @ "\c2" @ %note @ %pitchPrint;
    }

    if (%chordCount > 1 && %c < %chordCount - 1) {
      %bottomPrint = %bottomPrint @ "\c6+";
    }
  }

  %timeout = getSimTime();
  %obj.playNoteTimeout = %timeout;

  if (%obj.getClassName() $= "GameConnection") {
    %obj.bottomPrint("<font:palatino linotype:64>\c7[Preview]" SPC %bottomPrint, 1, true);
  }
  else if (%obj.getClassName() $= "Player" || %obj.getClassName() $= "AIPlayer") {
    %obj.playThread(1, plant);

    if (isObject(%client = %obj.client)) {
      %client.playNoteTimeout = %timeout;
      %client.bottomPrint(%bottomPrint, 1, true);
    }
  }
}
