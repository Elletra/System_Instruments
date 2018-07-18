
function InstrumentsClient::clickLoadFile(%this, %localOrServer) {
  %type = $Instruments::GUI::FileListMode;

  if (%type $= "phrases") {
    %type = "phrase";
  }
  else if (%type $= "songs") {
    %type = "song";
  }
  else if (%type $= "bindsets") {
    %type = "bindset";
  }
  else {
    return;
  }

  %filename = _cleanFilename(InstrumentsClient.getSelectedFile());
  InstrumentsClient.loadFile(%type, %filename, %localOrServer);
}

function InstrumentsClient::loadFile(%this, %type, %filename, %localOrServer) {
  %filename = _cleanFilename(%filename);
  %phraseOrSong = "";

  $Instruments::GUI::isLoading = true;

  if (%localOrServer $= "local") {
    Instruments.loadFile(%type, %filename);
  }
  else if (%localOrServer $= "server") {
    commandToServer('Instruments_LoadFile', %type, %filename);
  }
}

function InstrumentsClient::setLoadedAuthor(%this, %type, %author, %bl_id) {
  $Instruments::GUI::LoadedAuthorName[%type] = %author;
  $Instruments::GUI::LoadedAuthorBL_ID[%type] = %bl_id;
}

function InstrumentsClient::onFileLoadStart(%this, %type, %filename, %useServerCmd) {
  if (!$Instruments::GUI::isLoading) {
    return;
  }

  if (%type $= "song") {
    InstrumentsClient.clearAllSongPhrases(%useServerCmd);
  }
  else if (%type $= "bindset") {
    InstrumentsClient.clearAllKeys(%useServerCmd);
  }
}

// None of this is DRY at all but I don't care enough to fix it

function InstrumentsClient::onPhraseLoaded(%this, %phrase, %filename, %author, %bl_id, %unusedArg, %failure) {
  if (!$Instruments::GUI::isLoading) {
    $Instruments::Client::Warning["Loading"] = "";
    return;
  }

  if (%failure $= "") {
    %phrase = _cleanPhrase(%phrase);
    InstrumentsClient.setPhrase(%phrase);


    if ($Instruments::Client::Warning["Loading"] $= "") {
      %header = "Phrase Loaded Successfully";
      %body = "Successfully loaded " @ %filename @ " by " @ %author;
    }
    else {
      %header = "[!] Phrase Loaded";
      %body = "Loaded " @ %filename @ " by " @ %author;
    }

    if (%bl_id >= 0 && %bl_id != 888888 && %bl_id != 999999) {
       %body = %body @ " (BL_ID: " @ %bl_id @ ")";
    }

    if ($Instruments::Client::Warning["Loading"] !$= "") {
      %body = %body @ "\n\n<font:Arial Bold:14><color:FF0000>WARNING: " @ 
                      "<font:Arial:14>" @ $Instruments::Client::Warning["Loading"];
    }

    InstrumentsClient.setLoadedAuthor("phrase", %author, %bl_id);
    InstrumentsClient.updateSaveButtons();

    Instruments.messageBoxOK(%header, %body);
  }
  else {
    Instruments.messageBoxOK("Error Loading File", %failure);
  }

  $Instruments::GUI::isLoading = false;
  $Instruments::Client::Warning["Loading"] = "";
}

// Good attitude to have, I know

function InstrumentsClient::onSongLoaded(%this, %song, %filename, %author, %bl_id, %unusedArg, %failure) {
  if (!$Instruments::GUI::isLoading) {
    $Instruments::Client::Warning["Loading"] = "";
    return;
  }

  if (%failure $= "") {
    InstrumentsClient.textToSong(%song);


    if ($Instruments::Client::Warning["Loading"] $= "") {
      %header = "Song Loaded Successfully";
      %body = "Successfully loaded " @ %filename @ " by " @ %author;
    }
    else {
      %header = "[!] Song Loaded";
      %body = "Loaded " @ %filename @ " by " @ %author;
    }

    if (%bl_id >= 0 && %bl_id != 888888 && %bl_id != 999999) {
       %body = %body @ " (BL_ID: " @ %bl_id @ ")";
    }

    if ($Instruments::Client::Warning["Loading"] !$= "") {
      %body = %body @ "\n\n<font:Arial Bold:14><color:FF0000>WARNING: " @ 
                      "<font:Arial:14>" @ $Instruments::Client::Warning["Loading"];
    }

    InstrumentsClient.setLoadedAuthor("song", %author, %bl_id);
    InstrumentsClient.updateSongOrderList();

    Instruments.messageBoxOK(%header, %body);
  }
  else {
    Instruments.messageBoxOK("Error Loading File", %failure);
  }

  $Instruments::GUI::isLoading = false;
  $Instruments::Client::Warning["Loading"] = "";
}

function InstrumentsClient::onSongPhraseData(%this, %index, %phrase, %useServerCmd) {
  if (!$Instruments::GUI::isLoading) {
    return;
  }

  if (%index < 0 || %index >= $Instruments::Client::ServerPref::MaxSongPhrases) {
    return;
  }

  if (%useServerCmd) {
    commandToServer('setSongPhrase', %index, %phrase, 1);
  }

  %phrase = _cleanPhrase(%phrase);
  InstrumentsClient.setSongPhrase(%index, %phrase);
}

function InstrumentsClient::onBindsetLoaded(%this, %filename, %author, %bl_id, %unusedArg, %failure) {
  if (!$Instruments::GUI::isLoading) {
    $Instruments::Client::Warning["Loading"] = "";
    return;
  }

  if (%failure $= "") {
    if ($Instruments::Client::Warning["Loading"] $= "") {
      %header = "Bindset Loaded Successfully";
      %body = "Successfully loaded " @ %filename @ " by " @ %author;
    }
    else {
      %header = "[!] Bindset Loaded";
      %body = "Loaded " @ %filename @ " by " @ %author;
    }

    if (%bl_id >= 0 && %bl_id != 888888 && %bl_id != 999999) {
       %body = %body @ " (BL_ID: " @ %bl_id @ ")";
    }

    if ($Instruments::Client::Warning["Loading"] !$= "") {
      %body = %body @ "\n\n<font:Arial Bold:14><color:FF0000>WARNING: " @ 
                      "<font:Arial:14>" @ $Instruments::Client::Warning["Loading"];
    }

    InstrumentsClient.setLoadedAuthor("bindset", %author, %bl_id);
    InstrumentsClient.updateSaveButtons();

    Instruments.messageBoxOK(%header, %body);
  }
  else {
    Instruments.messageBoxOK("Error Loading File", %failure);
  }

  $Instruments::GUI::isLoading = false;
  $Instruments::Client::Warning["Loading"] = "";
}

function InstrumentsClient::onBindsetData(%this, %key, %phraseOrNote, %useServerCmd) {
  if (!$Instruments::GUI::isLoading) {
    return;
  }

  %control = InstrumentsMap.keyControl[%key];
  InstrumentsClient.bindToKey(%phraseOrNote, %key, 0, %useServerCmd);
}

function clientCmdInstruments_onFileLoadStart(%type, %filename) {
  InstrumentsClient.onFileLoadStart(%type, %filename, 0);
}

function clientCmdInstruments_onPhraseLoaded(%phrase, %filename, %author, %bl_id, %failure) {
  InstrumentsClient.onPhraseLoaded(%phrase, %filename, %author, %bl_id, "", %failure);
}

function clientCmdInstruments_onSongLoaded(%song, %filename, %author, %bl_id, %failure) {
  InstrumentsClient.onSongLoaded(%song, %filename, %author, %bl_id, "", %failure);
}

function clientCmdInstruments_onSongPhraseData(%index, %phrase) {
  InstrumentsClient.onSongPhraseData(%index, %phrase, 0);
}

function clientCmdInstruments_onBindsetLoaded(%filename, %author, %bl_id, %failure) {
  InstrumentsClient.onBindsetLoaded(%filename, %author, %bl_id, "", %failure);
}

function clientCmdInstruments_onBindsetData(%index, %phrase) {
  InstrumentsClient.onBindsetData(%index, %phrase, 0);
}
