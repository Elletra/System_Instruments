function InstrumentsClient::clickClearPhrase() {
  %yes = "InstrumentsClient.clearPhrase();";
  messageBoxYesNo("Clear phrase?", "Are you sure you want to clear the phrase?", %yes);
}

function InstrumentsClient::clearPhrase() {
  InstrumentsClient.setPhrase("");
  commandToServer('stopPlayingInstrument');
}

function InstrumentsClient::getPhrase(%this) {
  return InstrumentsDlg_Phrase.getValue();
}

function InstrumentsClient::setPhrase(%this, %phrase, %cursorOffset) {
  %phrase = _cleanPhrase(%phrase);
  %pos = InstrumentsDlg_Phrase.getCursorPos();  

  InstrumentsDlg_Phrase.setValue(%phrase);
  InstrumentsDlg_Phrase.setCursorPos(%pos + %cursorOffset);

  InstrumentsDlg_PhraseCharacterLimit.setValue(strLen(InstrumentsClient.getPhrase()) @ "/255");

  if (_strEmpty(%phrase)) {
    InstrumentsDlg_SetSongPhrase.disable();
    InstrumentsDlg_QuickBind.disable();
    InstrumentsDlg_QuickBind.setText("Quick Bind");

    $Instruments::GUI::QuickBind = false;
  }
  else {
    InstrumentsDlg_SetSongPhrase.enable();

    if (!InstrumentsDlg_QuickBind.enabled) {
      InstrumentsDlg_QuickBind.enable();
    }
  }

  // Doesn't work if you copy and paste into the phrase textbox :(
  // InstrumentsClient.updateSaveButtons();
}

function InstrumentsClient::copyPhraseToClipboard(%this, %copied) {
  if (%copied) {
    cancel($InstrumentsClient_CopySchedule);

    InstrumentsDlg_CopyToClipboard.enable();
    InstrumentsDlg_CopyToClipboard.setText("Copy to Clipboard");
  }
  else {
    setClipboard(InstrumentsClient.getPhrase());

    InstrumentsDlg_CopyToClipboard.disable();
    InstrumentsDlg_CopyToClipboard.setText("Copied!");
    $InstrumentsClient_CopySchedule = InstrumentsClient.schedule(750, copyPhraseToClipboard, true);
  }
}

function InstrumentsClient::clickPreviewPhrase() {
  if ($Instruments::Client::isPlaying) {
    commandToServer('stopPlayingInstrument');
  }
  else {
    commandToServer('playPhrase', InstrumentsClient.getPhrase(), $Instruments::GUI::Instrument, 1);
  }
}

function InstrumentsClient::clickPlayPhrase() {
  if ($Instruments::Client::isPlaying) {
    commandToServer('stopPlayingInstrument');
  }
  else {
    commandToServer('playPhrase', InstrumentsClient.getPhrase());
  }
}
