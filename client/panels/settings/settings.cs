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
  else if (%pref $= "MuteByDefault") {
    // do something
  }
}

function InstrumentsClient::addToPlayerList(%this, %clientName, %clientID, %bl_id) {
  if (InstrumentsDlg_PlayerList.getRowNumById(%clientID) >= 0) {
    return;
  }

  %muted = false;

  if ($Instruments::Client::CanUseMuting) {
    $Instruments::GUI::Muted_[%clientID] = $Pref::Client::Instruments::MuteByDefault;
    %muted = $Instruments::GUI::Muted_[%clientID];
    InstrumentsDlg_PlayerList.addRow(%clientID, (%muted ? "M" : "-") TAB %clientName TAB %bl_id);
    InstrumentsClient.setPlayerMuted(%clientID, %muted, true);
  }
  else {
    InstrumentsDlg_PlayerList.addRow(%clientID, "-" TAB %clientName TAB %bl_id);
  }
}

function InstrumentsClient::removeFromPlayerList(%this, %clientID) {
  deleteVariables("$Instruments::GUI::Muted_" @ %clientID);
  InstrumentsDlg_PlayerList.removeRowById(%clientID);
}

function InstrumentsClient::setPlayerMuted(%this, %clientID, %muted, %useServerCmd) {
  if (InstrumentsDlg_PlayerList.getRowNumById(%clientID) < 0) {
    return;
  }

  $Instruments::GUI::Muted_[%clientID] = %muted;

  %text = InstrumentsDlg_PlayerList.getRowTextById(%clientID);
  InstrumentsDlg_PlayerList.setRowById(%clientID, (%muted ? "M" : "-") TAB getFields(%text, 1));

  if (%useServerCmd) {
    commandToServer('Instruments_setPlayerMuted', %clientID, %muted);
  }
}

function InstrumentsClient::muteSelectedPlayer(%this) {
  %selected = InstrumentsDlg_PlayerList.getSelectedId();

  if (%selected < 0) {
    return;
  }

  InstrumentsClient.setPlayerMuted(%selected, true, true);
}

function InstrumentsClient::unmuteSelectedPlayer(%this) {
  %selected = InstrumentsDlg_PlayerList.getSelectedId();

  if (%selected < 0) {
    return;
  }

  InstrumentsClient.setPlayerMuted(%selected, false, true);
}

function InstrumentsClient::muteAllPlayers(%this, %exceptMe) {
  %count = InstrumentsDlg_PlayerList.rowCount();

  for (%i = 0; %i < %count; %i++) {
    %id = InstrumentsDlg_PlayerList.getRowId(%i);
    %text = InstrumentsDlg_PlayerList.getRowTextById(%id);

    if (%exceptMe && getField(%text, 2) == getNumKeyID()) {
      continue;
    }

    InstrumentsClient.setPlayerMuted(%id, true, false);
  }

  commandToServer('Instruments_muteAllPlayers', %exceptMe);
}

function InstrumentsClient::unmuteAllPlayers(%this, %exceptMe) {
  %count = InstrumentsDlg_PlayerList.rowCount();

  for (%i = 0; %i < %count; %i++) {
    %id = InstrumentsDlg_PlayerList.getRowId(%i);
    %text = InstrumentsDlg_PlayerList.getRowTextById(%id);

    if (%exceptMe && getField(%text, 2) == getNumKeyID()) {
      continue;
    }
    
    InstrumentsClient.setPlayerMuted(%id, false, false);
  }

  commandToServer('Instruments_unmuteAllPlayers', %exceptMe);
}

function InstrumentsClient::sortPlayersBy(%this, %column) {
  if (InstrumentsDlg_PlayerList.sortedBy == %column) {
    InstrumentsDlg_PlayerList.sortedAsc = !InstrumentsDlg_PlayerList.sortedAsc;
  }
  else {
    InstrumentsDlg_PlayerList.sortedAsc = 0;
    InstrumentsDlg_PlayerList.sortedBy = %column;
  }

  InstrumentsDlg_PlayerList.sort(InstrumentsDlg_PlayerList.sortedBy, InstrumentsDlg_PlayerList.sortedAsc);
}
