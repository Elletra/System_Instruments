function serverCmdInstruments_DeleteFile(%client, %type, %filename, %overwrite) {
  if (!InstrumentsServer.checkDeletingPermissions(%client, 1)) {
    return;
  }

  if (_strEmpty(%filename) || _strEmpty(%type)) {
    return;
  }

  if (%type $= "song" && !InstrumentsServer.checkSongPermissions(%client, 1)) { 
    return; 
  }

  if (%type $= "bindset" && !InstrumentsServer.checkBindsetPermissions(%client, 1)) { 
    return; 
  }

  %timeLeft = InstrumentsServer.getTimeLeft(%client.lastInstrumentsDeleteTime, $Pref::Server::Instruments::DeletingTimeout * 1000);

  if (!_strEmpty(%client.lastInstrumentsDeleteTime) && %timeLeft > 0) {
    Instruments.messageBoxOK("Deleting Timeout", 
      "Please wait " @ mCeil(%timeLeft / 1000) @ " second(s) before attempting to delete again.", %client);
    return;
  }

  Instruments.deleteFile(%type, %filename, %client, %overwrite);
}

// Renaming uses deleting permissions because it makes no sense to have both deleting and renaming permissions
// If renaming is disabled but deleting isn't, you can just delete the old file and save a new one with a different name

function serverCmdInstruments_RenameFile(%client, %type, %filename, %newFilename) {
  if (!InstrumentsServer.checkDeletingPermissions(%client, 1)) {
    return;
  }

  if (_strEmpty(%filename) || _strEmpty(%type)) {
    return;
  }

  if (%type $= "song" && !InstrumentsServer.checkSongPermissions(%client, 1)) { 
    return; 
  }

  if (%type $= "bindset" && !InstrumentsServer.checkBindsetPermissions(%client, 1)) { 
    return; 
  }

  %timeLeft = InstrumentsServer.getTimeLeft(%client.lastInstrumentsDeleteTime, $Pref::Server::Instruments::DeletingTimeout * 1000);

  if (!_strEmpty(%client.lastInstrumentsDeleteTime) && %timeLeft > 0) {
    Instruments.messageBoxOK("Renaming Timeout", 
      "Please wait " @ mFloatLength(%timeLeft / 1000, 1) @ " second(s) before attempting to rename again.", %client);
    return;
  }

  Instruments.renameFile(%type, %filename, %newFilename, %client);
}

function serverCmdDeleteSongOverwrite(%client) {
  %filename = %client.instrumentFileName;
  %song = %client.instrumentSong;
  
  serverCmdInstruments_DeleteFile(%client, "song", %filename, 1);
}

function serverCmdDeletePhraseOverwrite(%client) {
  %filename = %client.instrumentFileName;
  %phrase = %client.instrumentPhrase;

  serverCmdInstruments_DeleteFile(%client, "phrase", %filename, 1);
}

function serverCmdDeleteBindsetOverwrite(%client) {
  %filename = %client.instrumentFileName;
  serverCmdInstruments_DeleteFile(%client, "bindset", %filename, 1);
}
