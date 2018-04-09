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

  %where = "to Local";

  if (%localOrServer $= "server") {
    %where = "to Server";
  }

  InstrumentsEditTextDlg_Button.command = %command;
  InstrumentsEditTextDlg_Button.setText("Save");
  InstrumentsEditTextDlg_Window.setText("Save" SPC capitalizeFirstLetter(%type) SPC %where);

  %title = "Save" SPC capitalizeFirstLetter(%type) SPC %where;
  %label = "Filename";
  %editVar = "$Instruments::GUI::Filename";
  %editCmd = "";
  %btnText = "Save";
  %btnCmd = "InstrumentsClient.saveFile(\"" @ %type @ "\", \"" @ %localOrServer @ "\");";
  %footer = "(A-Z, 0-9, spaces, dashes, and underscores only)";

  InstrumentsEditTextDlg_TextEdit.setValue("");
  InstrumentsClient.openEditTextDialog(%title, %label, %editVar, %editCmd, %btnText, %btnCmd, %footer);
}

function InstrumentsClient::saveFile(%this, %type, %localOrServer) {
  %filename = InstrumentsEditTextDlg_TextEdit.getValue();

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

  if (%localOrServer $= "local") {
    Instruments.saveFile(%type, %filename, %phraseOrSong, "", 0);
  }
  else if (%localOrServer $= "server") {
    commandToServer('Instruments_SaveFile', %type, %filename, %phraseOrSong, 0);
  }

  Canvas.popDialog(InstrumentsEditTextDlg);
}
