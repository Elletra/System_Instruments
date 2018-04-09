function InstrumentsClient::deleteSelectedFile(%this, %localOrServer) {
  %type = $Instruments::GUI::FileListMode;
  %filename = InstrumentsClient.getSelectedFile();

  if (%type $= "Phrases") {
    %type = "phrase";
  }
  else if (%type $= "Songs") {
    %type = "song";
  }
  else if (%type $= "Bindsets") {
    %type = "bindset";
  }
  else {
    return;
  }

  if (_strEmpty(%filename)) {
    return;
  }

  if (%localOrServer $= "local") {
    Instruments.deleteFile(%type, %filename, "", 0);
  }
  else if (%localOrServer $= "server") {
    commandToServer('Instruments_DeleteFile', %type, %filename, 0);
  }
}

function InstrumentsClient::clickRenameFile(%this, %localOrServer) {
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

  InstrumentsClient.openRenameDialog(%mode, %localOrServer);
}

function InstrumentsClient::openRenameDialog(%this, %type, %localOrServer) {
  InstrumentsEditTextDlg_Button.command = %command;
  InstrumentsEditTextDlg_Button.setText("Rename");
  InstrumentsEditTextDlg_Window.setText("Rename" SPC capitalizeFirstLetter(%type));

  %title = "Rename" SPC capitalizeFirstLetter(%type);
  %label = "Filename";
  %editVar = "$Instruments::GUI::Filename";
  %editCmd = "";
  %btnText = "Rename";
  %btnCmd = "InstrumentsClient.renameFile(\"" @ %type @ "\", \"" @ %localOrServer @ "\");";
  %footer = "(A-Z, 0-9, spaces, dashes, and underscores only)";

  %filename = _cleanFilename(InstrumentsClient.getSelectedFile());

  InstrumentsClient.openEditTextDialog(%title, %label, %editVar, %editCmd, %btnText, %btnCmd, %footer);
  InstrumentsEditTextDlg_TextEdit.setValue(%filename);
}

function InstrumentsClient::renameFile(%this, %type, %localOrServer) {
  %newFilename = InstrumentsEditTextDlg_TextEdit.getValue();

  if (!Instruments.validateFilename(%newFilename, "", 1)) {
    return;
  }

  %filename = _cleanFilename(InstrumentsClient.getSelectedFile());

  if (%localOrServer $= "local") {
    Instruments.renameFile(%type, %filename, %newFilename, "");
  }
  else if (%localOrServer $= "server") {
    commandToServer('Instruments_RenameFile', %type, %filename, %newFilename);
  }

  Canvas.popDialog(InstrumentsEditTextDlg);
}
