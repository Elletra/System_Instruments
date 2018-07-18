function InstrumentsClient::selectSongOrderPhrase(%this) {
  %selected = InstrumentsDlg_SongOrderList.getSelectedRow();

  if (InstrumentsDlg_SongOrderList.previouslySelected $= %selected && %selected >= 0) {
    InstrumentsDlg_SongOrderList.setSelectedRow(-1);
    InstrumentsDlg_SongOrderList.previouslySelected = "";
  }
  else {
    InstrumentsDlg_SongOrderList.previouslySelected = %selected;    
  }
}

// -------------------------------------
//  Adding and setting phrases to songs
// -------------------------------------

function InstrumentsClient::addPhraseToSong(%this, %phraseID) {
  if (_strEmpty(%phraseID)) {
    %phraseID = InstrumentsDlg_SongPhraseList.getSelectedRow();
  }

  if (%phraseID < 0 || %phraseID >= $Instruments::Client::ServerPref::MaxSongPhrases) {
    return;
  }

  if (InstrumentsDlg_SongOrderList.rowCount() >= Instruments.const["SONG_ORDER_LIMIT"]) {
    InstrumentsDlg_AddToSong.disable();
    return;
  }

  %phraseText = InstrumentsDlg_SongPhraseList.getRowTextById(%phraseID);
  %afterID = InstrumentsDlg_SongOrderList.getSelectedId();

  if (%afterID < 0) {
    %afterID = InstrumentsDlg_SongOrderList.rowCount();
  }

  InstrumentsDlg_SongOrderList.insertAfter(%afterID, %phraseText);
  InstrumentsClient.updateSongOrderList();
}

function InstrumentsClient::setSongOrderPhrase(%this, %songPhraseRow, %songOrderRow) {
  if (%songPhraseRow $= "") {
    %songPhraseRow = InstrumentsDlg_SongPhraseList.getSelectedRow();
  }

  if (%songPhraseRow < 0 || %songPhraseRow >= $Instruments::Client::ServerPref::MaxSongPhrases) {
    return;
  }

  if (%songOrderRow $= "") {
    %songOrderRow = InstrumentsDlg_SongOrderList.getSelectedRow();
  }

  if (%songOrderRow < 0) {
    return;
  }

  %phraseText = InstrumentsDlg_SongPhraseList.getRowText(%songPhraseRow);

  if (_strEmpty(getField(%phraseText, 1))) {
    return;
  }

  InstrumentsDlg_SongOrderList.setRow(%songOrderRow, %phraseText);
}

// ----------------------------
//  Changing song phrase order
// ----------------------------

function InstrumentsClient::clickMoveUp(%this) {
  %selected = InstrumentsDlg_SongOrderList.getSelectedRow();

  if (%selected <= 0) {
    return;
  }

  InstrumentsDlg_SongOrderList.swapRows(%selected, %selected - 1);
}

function InstrumentsClient::clickMoveDown(%this) {
  %selected = InstrumentsDlg_SongOrderList.getSelectedRow();

  if (%selected >= InstrumentsDlg_SongOrderList.rowCount() - 1) {
    return;
  }

  InstrumentsDlg_SongOrderList.swapRows(%selected, %selected + 1);
}

// -----------------------------
//  Removing phrases from songs
// -----------------------------

function InstrumentsClient::clearSong(%this) {
  InstrumentsDlg_SongOrderList.clear();
  InstrumentsClient.updateSongOrderList();
}

function InstrumentsClient::removeSongOrderPhrase(%this) {
  %row = InstrumentsDlg_SongOrderList.getSelectedRow();

  if (%row < 0 || %row >= Instruments.const["SONG_ORDER_LIMIT"]) {
    return;
  }

  InstrumentsDlg_SongOrderList.removeRow(%row);
  InstrumentsClient.updateSongOrderList();
}

// ----------------------------
//  Update the song order list
// ----------------------------

function InstrumentsClient::updateSongOrderList(%this) {
  %count = InstrumentsDlg_SongOrderList.rowCount();
  %text = mClamp(%count, 0, Instruments.const["SONG_ORDER_LIMIT"]) @ 
          "/" @ Instruments.const["SONG_ORDER_LIMIT"];

  InstrumentsDlg_SongOrderLimit.setText(%text);
  InstrumentsClient.updateSaveButtons();

  if (%count <= 0) {
    InstrumentsDlg_PlaySong.disable();
    InstrumentsDlg_PreviewSong.disable();

    return;
  }

  InstrumentsDlg_PreviewSong.enable();

  if (InstrumentsClient.canPlayLive()) {
    InstrumentsDlg_PlaySong.enable();
  }

  %selected = InstrumentsDlg_SongOrderList.getSelectedRow();
  %tempList = new GuiTextListCtrl("" : InstrumentsDlg_SongOrderList);

  for (%i = 0; %i < %count; %i++) {
    %songRow = InstrumentsDlg_SongOrderList.getRowText(%i);

    if (_strEmpty(%songRow)) {
      continue;
    }

    if (%i >= Instruments.const["SONG_ORDER_LIMIT"]) {
      break;
    }

    %phraseNumber = keepChars(getField(%songRow, 0), "0123456789");
    %orderPhrase = getField(%songRow, 1);

    %phrase = InstrumentsDlg_SongPhraseList.getRowText(%phraseNumber - 1);
    %phrase = getField(%phrase, 1);

    if (!_strEmpty(%phrase)) {
      %tempList.addRow(%tempList.rowCount(), (%phraseNumber) @ "." TAB %phrase);
    }
  }

  InstrumentsDlg_SongOrderList.delete();
  %tempList.setName("InstrumentsDlg_SongOrderList");
  %tempList.previouslySelected = "";
  InstrumentsDlg_SongOrderContainer.add(%tempList);
  %tempList.setSelectedRow(%selected);

  if (%tempList.rowCount() >= Instruments.const["SONG_ORDER_LIMIT"]) {
    InstrumentsDlg_AddToSong.disable();
  }
  else {
    InstrumentsDlg_AddToSong.enable();
  }
}

// ----------------------------------
//  Song conversion to and from text
// ----------------------------------

function InstrumentsClient::songToText(%this) {
  %song = "";

  for (%i = 0; %i < Instruments.const["SONG_ORDER_LIMIT"]; %i++) {
    %text = InstrumentsDlg_SongOrderList.getRowText(%i);
    %id = getField(%text, 0);
    %phrase = getField(%text, 1);

    %id = keepChars(%id, "0123456789");

    if (_strEmpty(%text) || _strEmpty(%id) || _strEmpty(%phrase)) {
      break;
    }

    if (!_strEmpty(%song)) {
      %song = %song @ ",";
    }

    %song = %song @ (%id - 1);
  }

  return %song;
}

function InstrumentsClient::textToSong(%this, %text) {
  %song = strReplace(%text, ",", "\t");
  %song = keepChars(%song, "0123456789\t");

  InstrumentsClient.clearSong();

  %count = getFieldCount(%song);

  for (%i = 0; %i < %count; %i++) {
    %index = getField(%song, %i);
    InstrumentsClient.setSongOrderPhrase(%index, %i);
  }
}

function InstrumentsClient::copySongOrder(%this) {
  setClipboard(InstrumentsClient.songToText());

  InstrumentsDlg_CopySongOrder.disable();
  InstrumentsDlg_CopySongOrder.schedule(500, enable);
}

// ------------------------------
//  Playing and previewing songs
// ------------------------------

function InstrumentsClient::playSong(%this, %preview) {
  if (InstrumentsDlg_SongOrderList.rowCount() <= 0) {
    return;
  }

  %song = InstrumentsClient.songToText();
  
  commandToServer('playSong', %song, $Instruments::GUI::Instrument, %preview);
}

function InstrumentsClient::clickPlaySong() {
  if ($Instruments::Client::isPlaying) {
    commandToServer('stopPlayingInstrument');
  }
  else {
    InstrumentsClient.playSong(0);
  }
}

function InstrumentsClient::clickPreviewSong() {
  if ($Instruments::Client::isPlaying) {
    commandToServer('stopPlayingInstrument');
  }
  else {
    InstrumentsClient.playSong(1);
  }
}
