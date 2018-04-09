// yeesh, all this button changing logic is kind of gnarly isn't it


function clientCmdStartPlayingInstrument(%preview) {
  $Instruments::Client::isPlaying = true;

  if (%preview) {
    InstrumentsDlg_Play.disable();
    InstrumentsDlg_PlaySong.disable();
    InstrumentsDlg_Preview.setColor("1.0 0.0 0.0 1.0");
    InstrumentsDlg_Preview.setText("Stop");

    if (InstrumentsDlg_PreviewSong.enabled) {
      InstrumentsDlg_PreviewSong.setColor("1.0 0.0 0.0 1.0");
      InstrumentsDlg_PreviewSong.setText("Stop");
    }
  }
  else {
    InstrumentsDlg_Preview.disable();
    InstrumentsDlg_PreviewSong.disable();
    InstrumentsDlg_Play.setColor("1.0 0.0 0.0 1.0");
    InstrumentsDlg_Play.setText("Stop");
    InstrumentsDlg_PlaySong.setColor("1.0 0.0 0.0 1.0");
    InstrumentsDlg_PlaySong.setText("Stop");
  }
}

function clientCmdStopPlayingInstrument() {
  $Instruments::Client::isPlaying = false;

  InstrumentsDlg_Preview.enable();
  InstrumentsDlg_Preview.setColor("1.0 1.0 0.0 1.0");
  InstrumentsDlg_Preview.setText("Preview");

  %rowCount = InstrumentsDlg_SongOrderList.rowCount();
  
  if (%rowCount > 0) {
    InstrumentsDlg_PreviewSong.enable();
    InstrumentsDlg_PreviewSong.setColor("1.0 1.0 0.0 1.0");
    InstrumentsDlg_PreviewSong.setText("Preview");
  }

  InstrumentsDlg_Play.setText("Play");
  InstrumentsDlg_PlaySong.setText("Play");

  if (InstrumentsClient.canPlayLive()) {
    InstrumentsDlg_Play.enable();

    if (%rowCount > 0) {
      InstrumentsDlg_PlaySong.enable();
    }
  }
}

function clientCmdSetInstrument(%instrument) {
  $Instruments::Client::Instrument = %instrument;

  %id = InstrumentsDlg_InstrumentList.findText(%instrument);

  if (%id >= 0) {
    $Instruments::Client::SelectInstrumentServerCmd = false;
    InstrumentsDlg_InstrumentList.setSelected(%id);
  }
}

function clientCmdInstruments_GetVersion(%version, %notationVersion) {
  if (!_strEmpty($Instruments::Client::ServerVersion) && !_strEmpty($Instruments::Client::ServerNotationVersion)) {
    return;
  }

  $Instruments::Client::ServerVersion = %version;
  $Instruments::Client::ServerNotationVersion = %notationVersion;

  %serverVersion = strReplace(%version, ".", "\t");
  %clientVersion = strReplace($Instruments::Version, ".", "\t");

  if (getFieldCount(%serverVersion) != 3) {
    error("ERROR: clientCmdInstruments_GetVersion() - Malformed server version: " @ %serverVersion);
    return;
  }
  else if (getFieldCount(%clientVersion) != 3) {
    error("ERROR: clientCmdInstruments_GetVersion() - Malformed client version: " @ %clientVersion);
    return;
  }

  %versionCompare = semanticVersionCompare($Instruments::Version, $Instruments::Client::ServerVersion);

  %clientMajorVersion = getField(%clientVersion, 0);
  %serverMajorVersion = getField(%serverVersion, 0);

  if (%clientMajorVersion == %serverMajorVersion) {
    if (%versionCompare == 2) {
      %body = "<font:Arial Bold:18><color:FF0000>WARNING!<font:Arial:14><color:000000>" @
                "\n\nThis server is running a newer version (" @ %version @ ") of the instruments mod." @
                "\nThings might not work properly." @
                "\n\n(Your version: " @ $Instruments::Version @ ")";

      schedule(250, 0, MessageBoxOK, "Newer Server Version Detected", %body);
    }
    else if (%versionCompare == 1) {
      %body = "<font:Arial Bold:18><color:FF0000>WARNING!<font:Arial:14><color:000000>" @
                "\n\nThis server is running an older version (" @ %version @ ") of the instruments mod." @
                "\nThings might not work properly." @
                "\n\n(Your version: " @ $Instruments::Version @ ")";

      schedule(250, 0, MessageBoxOK, "Older Server Version Detected", %body);
    }
  }
  else {
    $Instruments::Client::CanUseInstruments = false;

    %body = "<font:Arial Bold:18><color:FF0000>WARNING!<font:Arial:14><color:000000>" @
              "\n\nThis server is running a version (" @ %version @ ") of the instruments mod that is " @
              "incompatible with yours (" @ $Instruments::Version @ ")" @
              "\n\nYou will not be able to use it on this server.";

    schedule(250, 0, MessageBoxOK, "Incompatible Versions", %body);
  }

  commandToServer('Instruments_GetVersion', $Instruments::Version, $Instruments::NotationVersion);
}

function clientCmdInstruments_CanIUse(%type, %canUse) {
  if (%type $= "" || %type $= "instruments") {
    $Instruments::Client::CanUseInstruments = %canUse;

    if (!%canUse && InstrumentsDlg.isAwake()) {
      Canvas.popDialog(InstrumentsDlg);
    }
  }
  else if (%type $= "songs") {
    $Instruments::Client::CanUseSongs = %canUse;

    if (%canUse) {
      InstrumentsDlg_SongsTab.enable();
    }
    else {
      InstrumentsDlg_SongsTab.disable();

      if ($Instruments::GUI::FileListMode $= "songs") {
        InstrumentsClient.setFileListMode("root");
      }
    }
  }
  else if (%type $= "bindsets") {
    $Instruments::Client::CanUseBindsets = %canUse;

    if (%canUse) {
      InstrumentsDlg_KeybindsTab.enable();
    }
    else {
      InstrumentsDlg_KeybindsTab.disable();

      if ($Instruments::GUI::FileListMode $= "bindsets") {
        InstrumentsClient.setFileListMode("root");
      }
    }
  }
  else if (%type $= "saving") {
    $Instruments::Client::CanUseSaving = %canUse;
    
  }
  else if (%type $= "loading") {
    $Instruments::Client::CanUseLoading = %canUse;
    
  }
  else if (%type $= "deleting") {
    $Instruments::Client::CanUseDeleting = %canUse;
  }

  // This is ugly sry
  if (!$Instruments::Client::CanUseSaving 
  && !$Instruments::Client::CanUseLoading 
  && !$Instruments::Client::CanUseDeleting) {
    InstrumentsDlg_FilesTab.disable();
  }
  else {
    InstrumentsDlg_FilesTab.enable();
  }

  // Refresh list
  if ($Instruments::GUI::FileListMode $= "root") {
    InstrumentsClient.refreshFileLists(%this);
  }
}
