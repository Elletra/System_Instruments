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
  %pickingUp = _strEmpty($Instruments::Client::Instrument) && !_strEmpty(%instrument);

  $Instruments::Client::Instrument = %instrument;
  %id = InstrumentsDlg_InstrumentList.findText(%instrument);

  if (%id >= 0) {
    $Instruments::Client::SelectInstrumentServerCmd = false;
    InstrumentsDlg_InstrumentList.setSelected(%id);
  }

  %serverVersion = strReplace($Instruments::Client::ServerVersion, ".", "\t");

  %serverMajorVersion = getField(%serverVersion, 0);
  %serverMinorVersion = getField(%serverVersion, 1);

  // Don't open the GUI if the server version is too old

  if (%serverMajorVersion == 1 && %serverMinorVersion < 2) {
    return;
  }

  if (%pickingUp && $Pref::Client::Instruments::OpenGuiOnEquip && !InstrumentsDlg.isAwake()) {
    InstrumentsDlg_Toggle(true);
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
  %clientMinorVersion = getField(%clientVersion, 1);
  %serverMajorVersion = getField(%serverVersion, 0);
  %serverMinorVersion = getField(%serverVersion, 1);

  if (%clientMajorVersion == %serverMajorVersion) {
    if (%versionCompare == 2 && %clientMinorVersion !$= %serverMinorVersion) {
      %body = "<font:Arial Bold:18><color:FF0000>WARNING!<font:Arial:14><color:000000>" @
                "\n\nThis server is running a newer version (" @ %version @ ") of the instruments mod." @
                "\nThings might not work properly." @
                "\n\n(Your version: " @ $Instruments::Version @ ")";

      schedule(250, 0, MessageBoxOK, "Newer Server Version Detected", %body);
    }
    else if (%versionCompare == 1 && %clientMinorVersion !$= %serverMinorVersion) {
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


  // Disable/change certain features/settings if server version is too old

  if (%serverMajorVersion == 1) {
    if (%serverMinorVersion < 2) {
      InstrumentsDlg_Preference_OpenGuiOnEquip.enabled = false;
      $Instruments::Client::ServerPref::MaxSongPhrases = Instruments.const["OLD_MAX_SONG_PHRASES"];
    }
    else {
      InstrumentsDlg_Preference_OpenGuiOnEquip.enabled = true;
    }

    if (%serverMinorVersion < 1) {
      $Instruments::Client::CanUseMuting = false;
      $Instruments::Client::CanUseCustomAuthor = false;

      InstrumentsDlg_MutePlayer.disable();
      InstrumentsDlg_UnmutePlayer.disable();
      InstrumentsDlg_MuteAllPlayers.disable();
      InstrumentsDlg_UnmuteAllPlayers.disable();

      InstrumentsDlg_SortPlayersByMuted.disable();
      InstrumentsDlg_SortPlayersByName.disable();
      InstrumentsDlg_SortPlayersByBL_ID.disable();

      InstrumentsDlg_MuteByDefault.enabled = false;
      InstrumentsDlg_MuteNotAvailable.visible = true;
      InstrumentsDlg_MuteNotAvailableOverlay.visible = true;

      InstrumentsSaveDlg_Window.extent = "376 96";
    }
    else {
      InstrumentsDlg_MutePlayer.enable();
      InstrumentsDlg_UnmutePlayer.enable();
      InstrumentsDlg_MuteAllPlayers.enable();
      InstrumentsDlg_UnmuteAllPlayers.enable();

      InstrumentsDlg_SortPlayersByMuted.enable();
      InstrumentsDlg_SortPlayersByName.enable();
      InstrumentsDlg_SortPlayersByBL_ID.enable();

      InstrumentsDlg_MuteByDefault.enabled = true;
      InstrumentsDlg_MuteNotAvailable.visible = false;
      InstrumentsDlg_MuteNotAvailableOverlay.visible = false;

      InstrumentsSaveDlg_Window.extent = "376 146";
    }
  }

  commandToServer('Instruments_GetVersion', $Instruments::Version, $Instruments::NotationVersion);

  if ($Pref::Client::Instruments::MuteByDefault) {
    InstrumentsClient.schedule(100, muteAllPlayers, true);
  }
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
  else if (%type $= "muting") {
    $Instruments::Client::CanUseMuting = %canUse;
  }
  else if (%type $= "customAuthor") {
    $Instruments::Client::CanUseCustomAuthor = %canUse;

    if (%canUse) {
      InstrumentsSaveDlg_Window.extent = "376 146";
    }
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

function clientCmdInstruments_UpdatePref(%pref, %newValue) {
  %currentValue = $Instruments::Client::ServerPref["::" @ %pref];

  %serverVersion = strReplace($Instruments::Client::ServerVersion, ".", "\t");
  %serverMinorVersion = getField(%serverVersion, 1);

  if (%serverMinorVersion >= 3) {
    %newValue = Instruments.const["MAX_SONG_PHRASES"];
  }

  if (%pref $= "MaxSongPhrases") {
    if (%currentValue $= "") {
      %currentValue = 0;
    }

    if (%currentValue < %newValue) {

      for (%i = %currentValue; %i < %newValue; %i++) {

        // If the rowCount on the GUI somehow doesn't match the current value of the
        // "MaxSongPhrases" pref, let's not fuck it up any further by adding more rows

        if (InstrumentsDlg_SongPhraseList.rowCount() <= %i) {
          InstrumentsDlg_SongPhraseList.addRow(%i, (%i + 1) @ "." TAB "");
        }
      }
    }
    else if (%currentValue > %newValue) {
      %body = "The maximum number of song phrases has been reduced to " @ %newValue @ "! \n\n" @
              "This may have affected your song.";

      Instruments.messageBoxOK("Warning", %body);
    }

    // Now let's shave off any excess rows

    %rowCount = InstrumentsDlg_SongPhraseList.rowCount();

    while (%rowCount > %newValue) {
      InstrumentsDlg_SongPhraseList.removeRow(%rowCount - 1);
      %rowCount = InstrumentsDlg_SongPhraseList.rowCount();
    }

    InstrumentsClient.updateSongOrderList();
  }

  $Instruments::Client::ServerPref["::" @ %pref] = %newValue;
}

function clientCmdInstruments_Warning(%type, %message) {
  if ($Instruments::Types::Warning[%type] $= "") {
    return;
  }

  $Instruments::Client::Warning[%type] = %message;
}
