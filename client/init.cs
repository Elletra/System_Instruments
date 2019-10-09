// Client-specific

if (!isObject(InstrumentsClient)) {
  new ScriptObject(InstrumentsClient) {
    class = "InstrumentsNamespace";
  };

  %binds = new ScriptObject() {
    class = "InstrumentsBindset";
  };

  InstrumentsClient.binds = %binds;
}

if (!isObject(HUD_InstrumentsNoteIcon)) {
  new GuiBitmapCtrl(HUD_InstrumentsNoteIcon) {
    profile = "GuiDefaultProfile";
    horizSizing = "left";
    vertSizing = "bottom";
    position = "0 376";
    extent = "80 80";
    minExtent = "8 2";
    enabled = "1";
    visible = "0";
    clipToParent = "1";
    bitmap = "./images/noteIcon.png";
    wrap = "0";
    lockAspectRatio = "0";
    alignLeft = "0";
    alignTop = "0";
    overflowImage = "0";
    keepCached = "0";
    mColor = "255 255 255 255";
    mMultiply = "0";
  };
  
  PlayGui.add(HUD_InstrumentsNoteIcon);
}


if (!$Instruments::Client::Keybinds) {
  $remapDivision[$remapCount] = "Playable Instruments";
  $remapName[$remapCount] = "Toggle GUI";
  $remapCmd[$remapCount] = "InstrumentsDlg_Toggle";
  $remapCount++;

  $remapName[$remapCount] = "Switch Keyboard Mode";
  $remapCmd[$remapCount] = "InstrumentsClient_SwitchKeyboardMode";
  $remapCount++;

  $Instruments::Client::Keybinds = true;
}

// Default GUI values
if ($Instruments::GUI::AddToPhrase $= "") {
  $Instruments::GUI::AddToPhrase = true;
}

if ($Instruments::GUI::Delay $= "") {
  $Instruments::GUI::Delay = Instruments.const["DEFAULT_DELAY"];
}

if ($Instruments::GUI::Tempo $= "") {
  $Instruments::GUI::Tempo = Instruments.const["DEFAULT_TEMPO"];
}

// Default preferences
if ($Pref::Client::Instruments::DisableMoveMap $= "") {
  $Pref::Client::Instruments::DisableMoveMap = true;
}

if ($Pref::Client::Instruments::ColoredKeys $= "") {
  $Pref::Client::Instruments::ColoredKeys = true;
}

if ($Pref::Client::Instruments::ChangeKeyLabels $= "") {
  $Pref::Client::Instruments::ChangeKeyLabels = true;
}

if ($Pref::Client::Instruments::MuteByDefault $= "") {
  $Pref::Client::Instruments::MuteByDefault = false;
}

if ($Pref::Client::Instruments::OpenGuiOnEquip $= "") {
  $Pref::Client::Instruments::OpenGuiOnEquip = true;
}

function InstrumentsDlg_Toggle(%down) {
  if (%down) {
    if (!InstrumentsDlg.isAwake()) {
      if ($Instruments::Client::ServerVersion $= "") {
        Instruments.messageBoxOK("Failure", "This server does not have the instruments mod.");
      }
      else {
        Canvas.pushDialog(InstrumentsDlg);
      }
    }
    else {
      Canvas.popDialog(InstrumentsDlg);
    }
  }
}
