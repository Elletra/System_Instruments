function InstrumentsClient::openKeybindDialog(%this, %key, %control) {
  $Instruments::GUI::KeyToBind = %key;
  %command = "InstrumentsClient.clickKeybindDone(\"" @ expandEscape(%key) @ "\", " @ %control @ ", 1);";

  InstrumentsKeybindDlg_Done.command = %command;
  InstrumentsKeybindDlg_PhraseOrNote.setValue(InstrumentsMap._key[%key]);

  Canvas.pushDialog(InstrumentsKeybindDlg);
}

function InstrumentsClient::clickKeybindDone(%this, %key, %control) {
  %phraseOrNote = InstrumentsClient.getPhraseToBind();
  
  InstrumentsClient.bindToKey(%phraseOrNote, %key, 1, 1);

  Canvas.popDialog(InstrumentsKeybindDlg);
  InstrumentsKeybindDlg_PhraseOrNote.setValue("");
}

function InstrumentsClient::getPhraseToBind(%this) {
  return InstrumentsKeybindDlg_PhraseOrNote.getValue();
}
