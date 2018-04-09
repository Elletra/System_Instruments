// This enables the client to use almost their entire keyboard to play instruments

$Instruments::Client::KeyboardMode = "default";

if (!isObject(InstrumentsMap)) {
  new ActionMap(InstrumentsMap) {
    _chordNoteIndex = 0;
  };

  InstrumentsClient.keyboard = InstrumentsMap;
}

if (!isObject(InstrumentsBlockingMap)) {
  new ActionMap(InstrumentsBlockingMap);
}

function InstrumentsClient_SwitchKeyboardMode(%down) {
  if (!%down) {
    return;
  }

  if ($Instruments::Client::KeyboardMode $= "default") {
    InstrumentsClient.configureKeyboard();
    
    HUD_InstrumentsNoteIcon.visible = true;
    
    // I don't pop MoveMap or GlobalActionMap because I still want certain basic bindings to work
    InstrumentsBlockingMap.push();
    InstrumentsMap.push();
    $Instruments::Client::KeyboardMode = "instruments";
  }
  else {
    HUD_InstrumentsNoteIcon.visible = false;
    InstrumentsBlockingMap.pop();
    InstrumentsMap.pop();
    $Instruments::Client::KeyboardMode = "default";
  }
}

function InstrumentsClient::configureKeyboard(%this) {
  for (%i = 0; %i < $remapCount; %i++) {
    %cmd = $remapCmd[%i];
    %binding = MoveMap.getBinding(%cmd);

    if (!_strEmpty(%binding) && getFieldCount(%binding) == 2) {
      %device = getField(%binding, 0);
      %key = getField(%binding, 1);

      // If the key is already bound to the InstrumentsMap, we don't want to accidentally unbind it
      if (!_strEmpty(InstrumentsMap.getCommand(%device, %key))) {
        continue;
      }

      if (Instruments.isKeyAllowed(%key) && !_strEmpty(%device) && !_strEmpty(%key)) {

        // Disable standard inputs if the preference is set

        if ($Pref::Client::Instruments::DisableMoveMap) {
          %makeCmd = "InstrumentsClient.onMoveMapKey(\"" @ %device @ "\", \"" @ %key @ "\", \"1\");";
          %breakCmd = "InstrumentsClient.onMoveMapKey(\"" @ %device @ "\", \"" @ %key @ "\", \"0\");";

          InstrumentsBlockingMap.bindCmd(%device, %key, %makeCmd, %breakCmd);
        }
        else {
          InstrumentsBlockingMap.unbind(%device, %key);
        }
      }
    }
  }

  // getBinding returns two fields: 1. the device 2. the key(s) it's bound to
  %switchDevice = getField(MoveMap.getBinding("InstrumentsClient_SwitchKeyboardMode"), 0);
  %switchAction = getField(MoveMap.getBinding("InstrumentsClient_SwitchKeyboardMode"), 1);

  // We don't want to overwrite the bind that switches ActionMaps, for obvious reasons
  InstrumentsMap.bind(%switchDevice, %switchAction, "InstrumentsClient_SwitchKeyboardMode");

  // getBinding returns two fields: 1. the device 2. the key(s) it's bound to
  %toggleDevice = getField(MoveMap.getBinding("InstrumentsDlg_Toggle"), 0);
  %toggleAction = getField(MoveMap.getBinding("InstrumentsDlg_Toggle"), 1);

  // We don't want to overwrite the bind that toggles the GUI, for obvious reasons
  InstrumentsMap.bind(%toggleDevice, %toggleAction, "InstrumentsDlg_Toggle");
}

function InstrumentsClient::bindToKey(%this, %phraseOrNote, %key, %showMessage, %serverCmd) {
  if (_strEmpty(%key)) { 
    return;
  }

  if (%key $= "ALL") {
    InstrumentsClient.bindToAllKeys(%phraseOrNote, 1);
    return;
  }

  if (isMacintosh()) {
    if (%key $= "lalt" || %key $= "ralt") {
      %key = "cmd";
    }
  }
  else if (isWindows()) {
    if (%key $= "cmd") {
      if (_strEmpty(InstrumentsMap.getCommand("keyboard", "lalt"))) {
        %key = "lalt";
      }
      else {
        %key = "ralt";
      }
    }
  }

  %control = InstrumentsMap.keyControl[%key];

  if (!isObject(%control)) {
    return;
  }

  if (MoveMap.getCommand("keyboard", %key) $= "InstrumentsClient_SwitchKeyboardMode") {
    if (%showMessage) {
      %red = "<color:FF0000>";
      %black = "<color:000000>";

      MessageBoxOK("REMAP FAILED", %red @ strUpr(%key) @ %black @ 
        " is already bound to switching keyboard modes.");
    }

    return;
  }

  if (MoveMap.getCommand("keyboard", %key) $= "InstrumentsDlg_Toggle") {
    if (%showMessage) {
      %red = "<color:FF0000>";
      %black = "<color:000000>";

      MessageBoxOK("REMAP FAILED", %red @ strUpr(%key) @ %black @ 
        " is already bound to toggling the instruments GUI.");
    }

    return;
  }

  if (!Instruments.isKeyAllowed(%key) || getWordCount(%key) > 1 || getWordCount(%key) < 1) {
    return;
  }

  %phraseOrNote = expandEscape(_cleanPhrase(%phraseOrNote));
  %fieldCount = getFieldCount(strReplace(%phraseOrNote, ",", "\t"));

  if (!_strEmpty(%phraseOrNote)) {
    if ($Pref::Client::Instruments::ColoredKeys) {
      // Quick and dirty hack to turn the bound phrase or note into a color
      %color = __hexToRGB(getSubStr(sha1(%phraseOrNote), 0, 6)) SPC "1"; 
    }
    else {
      %color = "0.97 0.97 1.0 1.0";
    }
  }

  if (_strEmpty(%phraseOrNote)) {
    %label = %control.keyLabel;
  }
  else {
    %label = "";

    %truncateAt = getWord(%control.extent, 0) - 8;
    %truncateAt = mRound(%truncateAt / 8);
    %truncateAt = %truncateAt >= 0 ? %truncateAt : 0;

    if (%truncateAt > 1 && strLen(%phraseOrNote) > %truncateAt) {
      %truncateAt--;
      %label = "...";
    }
    
    %label = getSubStr(%phraseOrNote, 0, %truncateAt) @ %label;
  }

  if ($Pref::Client::Instruments::ChangeKeyLabels) {
    %control.setText(%label);
  }
  else {
    %control.setText(%control.keyLabel);
  }

  if (_strEmpty(%phraseOrNote)) {
    InstrumentsMap._key[%key] = "";
    InstrumentsMap.unbind("keyboard", %key);

    %control.setProfile(GuiButtonProfile);
    %control.setColor("1 1 1 1");
    %control.phraseBoundTo = "";
  }
  else {
    if (%fieldCount == 1) {
      InstrumentsMap._key[%key] = %phraseOrNote;
      InstrumentsMap.bindCmd("keyboard", %key, "InstrumentsClient.playNote(\"" @ %phraseOrNote @ "\");", "");

      %control.setProfile(InstrumentsBoundKeyProfile);
      %control.setColor(%color);
      %control.phraseBoundTo = %phraseOrNote;
    }
    else if (%fieldCount > 1) {
      InstrumentsMap._key[%key] = %phraseOrNote;
      InstrumentsMap.bindCmd("keyboard", %key, "commandToServer('playPhrase', \"" @ %phraseOrNote @ "\");", "");

      %control.setProfile(InstrumentsBoundKeyProfile);
      %control.setColor(%color);
      %control.phraseBoundTo = %phraseOrNote;
    }
  }

  if (%serverCmd) {
    commandToServer('Instruments_BindToKey', %key, %phraseOrNote);
  }
  
  if (_strEmpty(%phraseOrNote)) {
    InstrumentsClient.binds.removeBind(%key);
  }
  else {
    InstrumentsClient.binds.addBind(%key, %phraseOrNote);
  }
}

function InstrumentsClient::rebindKey(%this, %key) {
  %control = InstrumentsMap.keyControl[%key];

  if (!isObject(%control)) {
    return;
  }

  InstrumentsClient.bindToKey(%control.phraseBoundTo, %key, 0, 0);
}

function InstrumentsClient::rebindKeyControl(%this, %control) {
  if (!isObject(%control)) {
    return;
  }

  InstrumentsClient.bindToKey(%control.phraseBoundTo, %control.keyBoundTo, 0, 0);
}

function InstrumentsClient::bindToAllKeys(%this, %phraseOrNote, %serverCmd) {
  for (%row = 0; %row < Instruments.const["NUM_KEY_ROWS"]; %row++) {
    for (%col = 0; %col < Instruments.const["NUM_KEY_COLUMNS"]; %col++) {
      %control = "InstrumentsDlg_Key" @ %row @ "_" @ %col;
      InstrumentsClient.bindToKey(%phraseOrNote, %control.keyBoundTo, 0, %serverCmd);
 
      %control = "InstrumentsDlg_KeyNumPad" @ %row @ "_" @ %col;
      InstrumentsClient.bindToKey(%phraseOrNote, %control.keyBoundTo, 0, %serverCmd);
 
      %control = "InstrumentsDlg_KeyArrow" @ %row @ "_" @ %col;
      InstrumentsClient.bindToKey(%phraseOrNote, %control.keyBoundTo, 0, %serverCmd);
    }
  }
}

function InstrumentsClient::rebindAllKeys(%this) {
  for (%row = 0; %row < Instruments.const["NUM_KEY_ROWS"]; %row++) {
    for (%col = 0; %col < Instruments.const["NUM_KEY_COLUMNS"]; %col++) {
      InstrumentsClient.rebindKeyControl("InstrumentsDlg_Key" @ %row @ "_" @ %col);
      InstrumentsClient.rebindKeyControl("InstrumentsDlg_KeyNumPad" @ %row @ "_" @ %col);
      InstrumentsClient.rebindKeyControl("InstrumentsDlg_KeyArrow" @ %row @ "_" @ %col);
    }
  }
}

function InstrumentsClient::playNote(%this, %note) {
  %note = _cleanNote(%note);
  %index = InstrumentsMap._chordNoteIndex;

  %chord = strReplace(%note, "+", "\t");
  %chordCount = getFieldCount(%chord);

  if (%chordCount <= 4 && %index < 4) {
    if (%chordCount <= 1) {
      InstrumentsMap._chordNote[%index] = %note;
      InstrumentsMap._chordNoteIndex++;
    }
    else {
      for (%i = 0; %i < %chordCount; %i++) {
        %index = InstrumentsMap._chordNoteIndex;

        InstrumentsMap._chordNote[%index] = getField(%chord, %i);
        InstrumentsMap._chordNoteIndex++;

        if (InstrumentsMap._chordNoteIndex >= 4) {
          break;
        }
      }
    }

    // So this is kind of a weird compromise I've had to make for keybinds

    // I want users to be able to press more than one key at once (like for drums or something)
    // but I need there to be a playNote timeout or else players will be able to lag/crash the server

    // So my compromise is combining the notes pressed into one chord
    // In order to do that, I have to do this weird schedule shit

    if ($InstrumentsClient_PlayChordSchedule $= "") {
      $InstrumentsClient_PlayChordSchedule = InstrumentsClient.schedule(1, playChord);
    }
  }
  else if ($InstrumentsClient_PlayChordSchedule $= "") {
    commandToServer('playNote', %note);
  }
}

function InstrumentsClient::playChord(%this) {
  cancel($InstrumentsClient_PlayChordSchedule);

  %chord = InstrumentsMap._chordNote[0];

  for (%i = 1; %i < 4; %i++) {
    %note = InstrumentsMap._chordNote[%i];

    if (_strEmpty(%note)) {
      break;
    }

    %chord = %chord @ "+" @ %note;

    InstrumentsMap._chordNote[%i] = "";
  }

  $InstrumentsClient_PlayChordSchedule = "";
  InstrumentsMap._chordNote[0] = "";
  InstrumentsMap._chordNoteIndex = 0;

  commandToServer('playNote', %chord);
}

function InstrumentsClient::onMoveMapKey(%this, %device, %key, %isDown) {
  // callback
}
