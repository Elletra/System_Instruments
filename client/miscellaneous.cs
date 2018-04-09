function InstrumentsClient::setGuiPanel(%this, %panel) {
  InstrumentsDlg_PianoPanel.visible = false;
  InstrumentsDlg_SongsPanel.visible = false;
  InstrumentsDlg_KeybindsPanel.visible = false;
  InstrumentsDlg_FilesPanel.visible = false;
  InstrumentsDlg_SettingsPanel.visible = false;
  InstrumentsDlg_HelpPanel.visible = false;

  %panelObj = ("InstrumentsDlg_" @ %panel @ "Panel");

  if (isObject(%panelObj)) {
    %panelObj.visible = true;
  }

  // Refresh list
  if (%panel $= "Files") {
    InstrumentsClient.refreshFileLists();
  }
}

function InstrumentsClient::openEditTextDialog(%this, %title, %label, %editVar, %editCmd, %btnText, %btnCmd, %footer) {
  InstrumentsEditTextDlg_Window.setText(%title);
  InstrumentsEditTextDlg_Label.setText("<font:impact:18>" @ %label @ ": ");
  InstrumentsEditTextDlg_Button.setText(%btnText);
  InstrumentsEditTextDlg_BottomText.setText(%footer);

  InstrumentsEditTextDlg_TextEdit.variable = %editVar;
  InstrumentsEditTextDlg_TextEdit.command = %editCmd;

  InstrumentsEditTextDlg_Button.command = "Canvas.popDialog(InstrumentsEditTextDlg); " @ %btnCmd;

  Canvas.pushDialog(InstrumentsEditTextDlg);
}

function InstrumentsClient::canPlayLive(%this) {
  return $Instruments::GUI::Instrument $= $Instruments::Client::Instrument;
}

function InstrumentsClient::applyPreference(%this, %pref) {
  %value = $Pref::Client::Instruments["::" @ %pref];

  if (%pref $= "DisableMoveMap") {
    InstrumentsClient.configureKeyboard();
  }
  else if (%pref $= "ColoredKeys") {
    InstrumentsClient.rebindAllKeys();
  }
  else if (%pref $= "ChangeKeyLabels") {
    InstrumentsClient.rebindAllKeys();
  }
}
