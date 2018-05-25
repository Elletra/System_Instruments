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

// Not my best idea, in retrospect...
function InstrumentsClient::openEditTextDialog(%this, %title, %label, %editVar, %editCmd, %btnText, %btnCmd, %footer) {
  InstrumentsEditTextDlg_Window.setText(%title);
  InstrumentsEditTextDlg_Label.setText("<font:impact:18>" @ %label @ ": ");
  InstrumentsEditTextDlg_Button.setText(%btnText);
  InstrumentsEditTextDlg_BottomText.setText(%footer);
  InstrumentsEditTextDlg_BottomText._centerX(-50);

  InstrumentsEditTextDlg_TextEdit.variable = %editVar;
  InstrumentsEditTextDlg_TextEdit.command = %editCmd;

  InstrumentsEditTextDlg_Button.command = "Canvas.popDialog(InstrumentsEditTextDlg); " @ %btnCmd;

  Canvas.pushDialog(InstrumentsEditTextDlg);
}

function InstrumentsClient::copyEditText(%this) {
  setClipboard(InstrumentsEditTextDlg_TextEdit.getValue());

  InstrumentsEditTextDlg_Copy.disable();
  InstrumentsEditTextDlg_Copy.schedule(500, enable);
}

function InstrumentsClient::canPlayLive(%this) {
  return $Instruments::GUI::Instrument $= $Instruments::Client::Instrument;
}
