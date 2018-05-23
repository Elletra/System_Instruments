function serverCmdInstruments_SaveFile(%client, %type, %filename, %phraseOrSong, %overwrite, %author) {
  if (!%client.hasInstrumentsClient) {
    return;
  }

  if (!InstrumentsServer.checkSavingPermissions(%client, 1)) {
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

  if (%type $= "phrase" || %type $= "song") {
    if (_strEmpty(%phraseOrSong)) {
      Instruments.messageBoxOK("Error", "No" SPC %type SPC "to save!", %client);
      return;
    }
  }

  if (%type $= "bindset") {
    if (%client.instrumentBinds.bindCount <= 0) {
      Instruments.messageBoxOK("Error", "No bindset to save!", %client);
      return;
    }

    if (%client.instrumentBinds.bindCount < 3) {
      Instruments.messageBoxOK("Error", "You need at least 3 keybinds before you can save.", %client);
      return;
    }
  }

  %timeLeft = InstrumentsServer.getTimeLeft(%client.lastInstrumentsSaveTime, $Pref::Server::Instruments::SavingTimeout * 1000);

  if (!_strEmpty(%client.lastInstrumentsSaveTime) && %timeLeft > 0) {
    Instruments.messageBoxOK("Saving Timeout", 
      "Please wait " @ mCeil(%timeLeft / 1000) @ " second(s) before attempting to save again.", %client);
    return;
  }

  Instruments.saveFile(%type, %filename, %phraseOrSong, %client, %overwrite, %author);
}

function serverCmdSaveSongOverwrite(%client) {
  %author = %client.instrumentFileAuthor;
  %filename = %client.instrumentFileName;
  %song = %client.instrumentSong;
  
  serverCmdInstruments_SaveFile(%client, "song", %filename, %song, 1, %author);
}

function serverCmdSavePhraseOverwrite(%client) {
  %author = %client.instrumentFileAuthor;
  %filename = %client.instrumentFileName;
  %phrase = %client.instrumentPhrase;

  serverCmdInstruments_SaveFile(%client, "phrase", %filename, %phrase, 1, %author);
}

function serverCmdSaveBindsetOverwrite(%client) {
  %author = %client.instrumentFileAuthor;
  %filename = %client.instrumentFileName;
  
  serverCmdInstruments_SaveFile(%client, "bindset", %filename, "", 1, %author);
}
