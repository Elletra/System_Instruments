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

    InstrumentsClient.clearInstruments();
    InstrumentsClient.clearAllKeys();

    InstrumentsClient.clearPhrase();
    InstrumentsClient.clearSong();

    InstrumentsClient.bindToAllKeys("", 0);
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
};
activatePackage(System_Instruments__client);
