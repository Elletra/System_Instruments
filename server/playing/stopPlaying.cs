function GameConnection::stopPlaying(%this) {
  InstrumentsServer.stopPlaying(%this);
}

function Player::stopPlaying(%this) {
  InstrumentsServer.stopPlaying(%this);
}

function serverCmdStopPlayingInstrument(%client) {
  InstrumentsServer.stopPlaying(%client);

  if (isObject(%player = %client.player)) {
    InstrumentsServer.stopPlaying(%player);
  }
}

// Main function

function InstrumentsServer::stopPlaying(%this, %obj) {
  if (!isObject(%obj)) {
    return;
  }
  
  cancel(%obj.phraseSchedule);
  cancel(%obj.songSchedule);

  %obj.isPlayingInstrument = false;
  %obj.instrumentPhrase = "";
  %obj.instrumentSong = "";
  %obj.noteIndex = 0;
  %obj.phraseIndex = 0;

  if (%obj.getClassName() $= "GameConnection") {
    commandToClient(%obj, 'stopPlayingInstrument');
  }
  else if (isObject(%client = %obj.client)) {
    commandToClient(%client, 'stopPlayingInstrument');
  }
}
