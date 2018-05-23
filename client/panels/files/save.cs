function InstrumentsClient::clickSaveFile(%this, %localOrServer) {
  %mode = $Instruments::GUI::FileListMode;

  if (%mode $= "Phrases") {
    %mode = "phrase";
  }
  else if (%mode $= "Songs") {
    %mode = "song";
  }
  else if (%mode $= "Bindsets") {
    %mode = "bindset";
  }
  else {
    return;
  }

  InstrumentsClient.openSaveDialog(%mode, %localOrServer);
}

function InstrumentsClient::openSaveDialog(%this, %type, %localOrServer) {
  if (%type $= "phrase" && _strEmpty(InstrumentsClient.getPhrase())) {
    Instruments.messageBoxOK("Error", "No phrase to save!");
    return;
  }

  if (%type $= "song" && InstrumentsDlg_SongOrderList.rowCount() <= 0) {
    Instruments.messageBoxOK("Error", "No song to save!");
    return;
  }

  // <3
  if (%type $= "bindset" && InstrumentsClient.binds.bindCount < 3) {
    Instruments.messageBoxOK("Error", "Not enough binds to save!");
    return;
  }

  %authorName = $Instruments::GUI::LoadedAuthorName[%type];

  if (%authorName $= "") {
    %authorName = $Pref::Player::NetName;
  }

  %authorBL_ID = $Instruments::GUI::LoadedAuthorBL_ID[%type];

  if (%authorBL_ID $= "") {
    %authorBL_ID = getNumKeyID();
  }

  $Instruments::GUI::FileType = %type;
  $Instruments::GUI::FileLocalOrServer = %localOrServer;

  InstrumentsSaveDlg_Filename.setValue("");
  InstrumentsSaveDlg_AuthorName.setValue(%authorName);
  InstrumentsSaveDlg_AuthorBL_ID.setValue(%authorBL_ID);

  Canvas.pushDialog(InstrumentsSaveDlg);
}

function InstrumentsClient::clickSaveButton(%this) {
  %filename = InstrumentsSaveDlg_Filename.getValue();
  %authorName = InstrumentsSaveDlg_AuthorName.getValue();
  %authorBL_ID = InstrumentsSaveDlg_AuthorBL_ID.getValue();
  %author = %authorName TAB %authorBL_ID;

  Canvas.popDialog(InstrumentsSaveDlg);

  InstrumentsClient.saveFile($Instruments::GUI::FileType, $Instruments::GUI::FileLocalOrServer, %filename, %author);
}

function InstrumentsClient::saveFile(%this, %type, %localOrServer, %filename, %author) {
  if (!Instruments.validateFilename(%filename, "", 1)) {
    return;
  }

  %phraseOrSong = "";

  if (%type $= "phrase") {
    %phraseOrSong = _cleanPhrase(InstrumentsClient.getPhrase());
  }
  else if (%type $= "song") {
    %phraseOrSong = _cleanPhrase(InstrumentsClient.songToText());
  }

  if (%author $= "") {
    %author = $Pref::Player::NetName TAB getNumKeyID();
  }

  if (%localOrServer $= "local") {
    Instruments.saveFile(%type, %filename, %phraseOrSong, "", 0, %author);
  }
  else if (%localOrServer $= "server") {
    commandToServer('Instruments_SaveFile', %type, %filename, %phraseOrSong, 0, %author);
  }

  Canvas.popDialog(InstrumentsEditTextDlg);
}
