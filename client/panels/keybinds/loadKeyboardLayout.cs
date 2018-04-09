function InstrumentsClient::loadKeyboardLayout(%this, %path) {
  if (!isFile(%path)) {
    return;
  }

  %file = new FileObject();
  %file.openForRead(%path);

  while(!%file.isEoF()) {
    %line = %file.readLine();

    if (getFieldCount(%line) == 4) {
      %row = getField(%line, 0);
      %col = getField(%line, 1);
      %key = getField(%line, 2);
      %label = getField(%line, 3);

      %keyControl = "InstrumentsDlg_Key" @ %row @ "_" @ %col;

      if (isMacintosh()) {
        if (%key $= "lalt") {
          %key = "cmd";
          %label = "Cmd";
        }
        else if (%key $= "ralt") {
          // Mac version does not differentiate between left and right cmd :(
          if (isObject(%keyControl)) {
            %keyControl.visible = false;
            %keyControl.enabled = false;
          }

          continue;
        }
      }

      if (isObject(%keyControl)) {
        %command = "InstrumentsClient.clickKeyboardKey(\"" @ expandEscape(%key) @ "\", " @ %keyControl @ ");";

        %keyControl.keyboardRow = %row;
        %keyControl.keyboardColumn = %col;
        %keyControl.keyBoundTo = %key;
        %keyControl.keyLabel = %label;
        %keyControl.command = %command;
        %keyControl.setText(%label);

        InstrumentsMap.keyControl[%key] = %keyControl;

        if (%key $= moveMap.getCommand("keyboard", %key) $= "InstrumentsClient_SwitchKeyboardMode") {
          %keyControl.visible = false;
          %keyControl.enabled = false;
        }

        if (%key $= moveMap.getCommand("keyboard", %key) $= "InstrumentsDlg_Toggle") {
          %keyControl.visible = false;
          %keyControl.enabled = false;
        }

        if (!Instruments.isKeyAllowed(%key)) {
          %keyControl.visible = false;
          %keyControl.enabled = false;
        }
      }
    }
  }

  %file.close();
  %file.delete();
}
