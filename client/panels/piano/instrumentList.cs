// $Instruments::GUI::Instrument is the currently selected instrument, and
// $Instruments::Client::Instrument is the currently equipped instrument 

function InstrumentsClient::selectInstrument(%this) {
  %id = InstrumentsDlg_InstrumentList.getSelected();
  $Instruments::GUI::Instrument = InstrumentsDlg_InstrumentList.getTextById(%id);
  %instrument = $Instruments::GUI::Instrument;

  if (InstrumentsClient.canPlayLive()) {
    InstrumentsDlg_PlayLive.enabled = true;
    InstrumentsDlg_Play.enable();

    if (InstrumentsDlg_SongOrderList.rowCount() > 0) {
      InstrumentsDlg_PlaySong.enable();
      InstrumentsDlg_PreviewSong.enable();
    }
  }
  else {
    InstrumentsDlg_PlayLive.enabled = false;
    InstrumentsDlg_PlayLive.setValue(false);
    InstrumentsDlg_Play.disable();
    InstrumentsDlg_PlaySong.disable();
  }

  InstrumentsDlg_NoPianoNotesAreAvailable.visible = false;
  
  %instrumentObj = InstrumentsClient._(%instrument);
  %sounds = %instrumentObj.length;
  %disabledCount = 0;

  if (!isObject(%instrumentObj)) { 
    return; 
  }

  for (%i = 3; %i <= 6; %i++) {
    %noteC = %instrumentObj.index("C" @ %i);
    %noteF = %instrumentObj.index("F" @ %i);
    %noteLowerF = %instrumentObj.index("F" @ %i - 1);

    if (%noteC $= "") {
      InstrumentsClient.setIntervalEnabled("C", %i, false);
      %disabledCount++;
    }
    else {
      InstrumentsClient.setIntervalEnabled("C", %i, true);
    }

    if (%noteF $= "") {
      InstrumentsClient.setIntervalEnabled("F", %i, false);
      %disabledCount++;
    }
    else {
      InstrumentsClient.setIntervalEnabled("F", %i, true);
    }
  }

  if (%disabledCount >= 8) {
    InstrumentsDlg_NoPianoNotesAreAvailable.visible = true;
  }

  InstrumentsClient.populateNoteList();

  if ($Instruments::Client::SelectInstrumentServerCmd) {
    commandToServer('onSelectInstrument', %instrument);
  }

  $Instruments::Client::SelectInstrumentServerCmd = true;
}

function InstrumentsClient::populateInstrumentList(%this) {
  %count = InstrumentsClient.instrumentCount;

  for (%i = 0; %i < %count; %i++) {
    InstrumentsDlg_InstrumentList.add(InstrumentsClient.name(%i), %i);
  }

  InstrumentsDlg_InstrumentList.sort();

  %index = InstrumentsDlg_InstrumentList.findText("Piano");

  if (%index < 0) {
    %index = 0;
  }

  InstrumentsDlg_InstrumentList.setSelected(%index);
}

function InstrumentsClient::populateNoteList(%this) {
  InstrumentsDlg_NoteList.clear();
  
  %instrument = $Instruments::GUI::Instrument;
  %index = 0;
  %obj = InstrumentsClient._(%instrument);
  %length = %obj.length;

  for (%i = 0; %i < %length; %i++) {
    %note = %obj.note(%i);

    if (!Instruments.isMelodicNote(%note)) {
      InstrumentsDlg_NoteList.addRow(%index, %note, %index);
      %index++;
    }
  }

  InstrumentsDlg_NoteList.sort(0);
}

function clientCmdReceiveInstrumentListStart() {
  InstrumentsClient.clearInstruments();
}

function clientCmdReceiveInstrumentList(%list) {
  %count = getRecordCount(%list);

  for (%r = 0; %r < %count; %r++) {
    %line = getRecord(%list, %r);
    %instrument = getField(%line, 0);
    %note = getField(%line, 1);

    InstrumentsClient.addNote(%instrument, %note);
  }
}

function clientCmdReceiveInstrumentListDone() {
  InstrumentsClient.populateInstrumentList();
  InstrumentsClient.populateNoteList();
}

function clientCmdReceiveInstrumentAddOnData(%list) {
  %index = getField(%list, 0);
  %addOn = getField(%list, 1);
  %authors = getFields(%list, 2);

  // Done manually so they'll be in order in the GUI

  InstrumentsClient.set("addOn_" @ %index, %addOn);
  InstrumentsClient.set("addOnIndex_" @ %addOn, %index);
  InstrumentsClient.set("addOnAuthor_" @ %addOn, %authors);

  InstrumentsClient.addOnCount++;
}
