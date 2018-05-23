function InstrumentsDlg::onWake(%this) {
  InstrumentsDlg_Window.setText("Playable Instruments Client  (v" @ $Instruments::Version @ ")");

  commandToServer('Instruments_CanIUse', "all", false);
  
  if ($Instruments::Client::Instrument !$= "") {
    $Instruments::GUI::Instrument = $Instruments::Client::Instrument;
  }
  
  InstrumentsClient.setPhrase(InstrumentsClient.getPhrase());
  InstrumentsClient.selectInstrument();

  if (InstrumentsDlg_SongPhraseList.rowCount() <= 0) {
    for (%i = 0; %i < 20; %i++) {
      InstrumentsDlg_SongPhraseList.addRow(%i, (%i + 1) @ "." TAB "");
    }
  }

  if (InstrumentsDlg_SongOrderList.rowCount() <= 0) {
    InstrumentsDlg_PreviewSong.disable();
  }

  %toggleGui = MoveMap.getBinding("InstrumentsDlg_Toggle");
  %toggleMode = MoveMap.getBinding("InstrumentsClient_SwitchKeyboardMode");

  // Only do this check if one of the keybinds has changed
  if ($Instruments::GUI::ToggleGuiKeybind !$= %toggleGui || $Instruments::GUI::ToggleModeKeybind !$= %toggleMode) {
    for (%row = 0; %row < Instruments.const["NUM_KEY_ROWS"]; %row++) {
      for (%col = 0; %col < Instruments.const["NUM_KEY_COLUMNS"]; %col++) {
        InstrumentsClient.checkGuiKey("InstrumentsDlg_Key" @ %row @ "_" @ %col);
        InstrumentsClient.checkGuiKey("InstrumentsDlg_KeyNumPad" @ %row @ "_" @ %col);
        InstrumentsClient.checkGuiKey("InstrumentsDlg_KeyArrow" @ %row @ "_" @ %col);
      }
    }
  }

  $Instruments::GUI::ToggleGuiKeybind = %toggleGui;
  $Instruments::GUI::ToggleModeKeybind = %toggleMode;

  // Refresh list
  if ($Instruments::GUI::FileListMode $= "root") {
    InstrumentsClient.refreshFileLists(%this);
  }

  InstrumentsClient.populateHelpCategoriesList();
}

// Underscore because apparently there's already some mod-defined centerX method
function GuiControl::_centerX(%this, %offset) {
  if (!isObject(%parent = %this.getGroup())) {
    return;
  }

  if (%offset $= "") {
    %offset = 0;
  }

  %parentWidth = getWord(%parent.getExtent(), 0) / 2;
  %width = getWord(%this.getExtent(), 0) / 2;
  %y = getWord(%this.getPosition(), 1);

  %this.position = (mAbs(%parentWidth - %width) + %offset) SPC %y;
}

// Underscore because apparently there's already some mod-defined centerY method
function GuiControl::_centerY(%this, %offset) {
  if (!isObject(%parent = %this.getGroup())) {
    return;
  }

  if (%offset $= "") {
    %offset = 0;
  }

  %parentHeight = getWord(%parent.getExtent(), 1) / 2;
  %height = getWord(%this.getExtent(), 1) / 2;
  %x = getWord(%this.getPosition(), 0);

  %this.position = %x SPC (%offset + mAbs(%parentHeight - %height));
}

function GuiBitmapButtonCtrl::enable(%this) {
  if (%this.enabled && %this.enabledColor $= "") { 
    return;
  }

  %color = %this.enabledColor;

  if (_strEmpty(%color)) {
    %color = "1 1 1 1";
  }

  %this.enabled = true;
  %this.setColor(%color);
}

function GuiBitmapButtonCtrl::disable(%this) {
  if (!%this.enabled) { 
    return;
  }

  if (%this.enabledColor $= "") {
    %this.enabledColor = %this.getColor();
  }
  
  %this.enabled = false;
  %this.setColor("0.5 0.5 0.5 1.0");
}

function GuiBitmapButtonCtrl::toggle(%this) {
  if (%this.enabled) {
    %this.disable();
  }
  else {
    %this.enable();
  }
}

function GuiBitmapButtonCtrl::doDisableCheck(%this, %bool) {
  if (%bool) {
    %this.disable();
  }
  else {
    %this.enable();
  }
}

function GuiTextListCtrl::getSelectedRow(%this) {
  %id = %this.getSelectedId();
  return %this.getRowNumById(%id);
}

function GuiTextListCtrl::setRow(%this, %index, %value) {
  %id = %this.getRowId(%index);

  if (%id == -1) {
    %this.addRow(%this.rowCount(), %value);
  }
  else {
    %this.setRowById(%id, %value);
  }
}

function GuiTextListCtrl::insertAfter(%this, %index, %value) {
  %count = %this.rowCount();

  if (%index >= %count - 1) {
    %this.addRow(%count, %value);
  }
  else {
    %index++;
    %previousRow = %this.getRowText(%index);

    for (%i = %index; %i < %count; %i++) {
      if (%i == %index) {
        %this.setRow(%i, %value);
      }
      else {
        %currentRow = %this.getRowText(%i);
        %this.setRow(%i, %previousRow);

        %previousRow = %currentRow;
      }
    }

    %this.addRow(%count, %previousRow);
  }
}

function GuiTextListCtrl::insertBefore(%this, %index, %value) {
  %count = %this.rowCount();

  if (%index >= %count) {
    %this.addRow(%count, %value);
  }
  else {
    %previousRow = %this.getRowText(%index);

    for (%i = %index; %i < %count; %i++) {
      if (%i == %index) {
        %this.setRow(%i, %value);
      }
      else {
        %currentRow = %this.getRowText(%i);
        %this.setRow(%i, %previousRow);

        %previousRow = %currentRow;
      }
    }

    %this.addRow(%count, %previousRow);
  }
}

function GuiTextListCtrl::swapRows(%this, %rowIndex1, %rowIndex2) {
  %selected = %this.getSelectedRow();

  if (%rowIndex1 < 0 || %rowIndex1 >= %this.rowCount()) {
    return;
  }

  if (%rowIndex2 < 0 || %rowIndex2 >= %this.rowCount()) {
    return;
  }

  %rowText1 = %this.getRowText(%rowIndex1);
  %rowText2 = %this.getRowText(%rowIndex2);

  %this.setRow(%rowIndex1, %rowText2);
  %this.setRow(%rowIndex2, %rowText1);

  // Swap selected row too
  if (%selected == %rowIndex1) {
    %this.setSelectedRow(%rowIndex2);
  }
  else if (%selected == %rowIndex2) {
    %this.setSelectedRow(%rowIndex1);
  }
}
