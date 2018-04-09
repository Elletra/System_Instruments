if ($Instruments::GUI::QuickBind $= "") {
  $Instruments::GUI::QuickBind = false;
}


function InstrumentsClient::shouldDisableKey(%this, %key) {
  if (moveMap.getCommand("keyboard", %key) $= "InstrumentsClient_SwitchKeyboardMode") {
    return true;
  }

  if (moveMap.getCommand("keyboard", %key) $= "InstrumentsDlg_Toggle") {
    return true;
  }

  if (isMacintosh() && %key $= "ralt") {
    return true;
  }

  return false;
}

function InstrumentsClient::checkGuiKey(%this, %control) {
  if (!isObject(%control) || %control.keyBoundTo $= "") {
    return;
  }

  %key = %control.keyBoundTo;

  if (InstrumentsClient.shouldDisableKey(%key)) {
    %control.visible = false;
    %control.enabled = false;
  }
  else {
    %control.visible = true;
    %control.enabled = true;
  }
}

function InstrumentsClient::toggleQuickBind(%this) {
  $Instruments::GUI::QuickBind = !$Instruments::GUI::QuickBind;

  if ($Instruments::GUI::QuickBind) {
    InstrumentsDlg_QuickBind.setText("Stop Bind");
    InstrumentsDlg_QuickBind.setColor("0.75 0.75 0.5 1.0");
  }
  else {
    InstrumentsDlg_QuickBind.setText("Quick Bind");
    InstrumentsDlg_QuickBind.setColor("1 1 0 1");
  }
}

function InstrumentsClient::clickKeyboardKey(%this, %key, %control, %rightClick) {
  if (%rightClick) {
    InstrumentsClient.bindToKey("", %key, 1, 1);
  }
  else if ($Instruments::GUI::QuickBind) {
    %phrase = InstrumentsClient.getPhrase();
    InstrumentsClient.bindToKey(%phrase, %key, 1, 1);
  }
  else {
    InstrumentsClient.openKeybindDialog(%key, %control);
  }
}

function InstrumentsClient::clearAllKeys(%this, %serverCmd) {
  if (%serverCmd) {
    commandToServer('Instruments_clearAllKeys');
  }

  InstrumentsClient.bindToAllKeys("", 0);
}

function clientCmdInstruments_clearAllKeys() {
  InstrumentsClient.clearAllKeys(0);
}
