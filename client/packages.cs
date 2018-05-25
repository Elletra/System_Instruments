package System_Instruments__client {
  function InstrumentsClient::clearInstruments(%this) {
    Parent::clearInstruments(%this);

    InstrumentsDlg_InstrumentList.clear();
    InstrumentsDlg_NoteList.clear();
  }

  function disconnectedCleanup(%doReconnect) {
    Parent::disconnectedCleanup(%doReconnect);

    $Instruments::Client::ServerVersion = "";
    $Instruments::Client::ServerNotationVersion = "";

    InstrumentsClient.schedule(100, clearInstruments);
    InstrumentsClient.schedule(100, clearAllKeys);

    InstrumentsClient.schedule(100, clearPhrase);
    InstrumentsClient.schedule(100, clearAllSongPhrases, 0);

    InstrumentsClient.schedule(100, bindToAllKeys, "", 0);

    InstrumentsDlg_PlayerList.clear();

    $Instruments::Client::CanUseMuting = false;
    HUD_InstrumentsNoteIcon.visible = false;
  }

  function PlayGui::createToolHUD(%this) {
    Parent::createToolHUD(%this);

    %resX = getWord(getRes(), 0);
    %resY = getWord(getRes(), 1);


    if (%resX >= 1024) {
      %w = 80;
      %h = 80;
      %x = getWord(HUD_InstrumentsNoteIcon.getPosition(), 0);
      %y = %resY - %h;
      HUD_InstrumentsNoteIcon.resize(%resX * 0.84, %y - (%w / 2), %w, %h);
    }
    else {
      %w = 64;
      %h = 64;
      %x = getWord(HUD_InstrumentsNoteIcon.getPosition(), 0);
      %y = %resY - (%w * 1.5);
      HUD_InstrumentsNoteIcon.resize(%resX * 0.01, %y, %w, %h);
    }
  }

  function secureClientCmd_ClientJoin(%clientName, %clientID, %bl_id, %score, %isAI, %isAdmin, %isSuperAdmin, %trust, %inYourMiniGame) {
    Parent::secureClientCmd_ClientJoin(%clientName, %clientID, %bl_id, %score, %isAI, %isAdmin, %isSuperAdmin, %trust, %inYourMiniGame);
    InstrumentsClient.addToPlayerList(%clientName, %clientID, %bl_id);
  }

  function secureClientCmd_ClientDrop(%clientName, %clientID) {
    Parent::secureClientCmd_ClientDrop(%clientName, %clientID);
    InstrumentsClient.removeFromPlayerList(%clientID);
  }
};
activatePackage(System_Instruments__client);
