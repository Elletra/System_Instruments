function InstrumentsClient::clickPianoKey(%this, %key, %isChord) {
  if ($Instruments::GUI::AddToPhrase) {
    if (%isChord) {
      InstrumentsClient.addToPhrase(%key @ "+", "chord");
    }
    else {
      InstrumentsClient.addToPhrase(%key, "note");
    }
  }

  %preview = !$Instruments::GUI::PlayLive;
  commandToServer('playNote', %key, $Instruments::GUI::Instrument, %preview);
}

function InstrumentsClient::selectNote(%this, %isChord) {
  %id = InstrumentsDlg_NoteList.getSelectedId();
  $Instruments::GUI::Note = InstrumentsDlg_NoteList.getRowTextById(%id);
  %note = $Instruments::GUI::Note;

  InstrumentsClient.clickPianoKey(%note, %isChord);
}

function InstrumentsClient::addToPhrase(%this, %note, %type) {
  if (%type $= "") {
    %type = "note";
  }

  %phrase = InstrumentsClient.getPhrase();

  if (%type $= "other") {
    %withCommas = true;
  }
  else if (%phrase $= "" || %type $= "rest" || %type $= "chord") {
    %withCommas = false;
  }
  else if (getSubStr(%phrase, strLen(%phrase) - 1, 1) $= "+" && %type $= "note") {
    %withCommas = false;
  }
  else {
    %withCommas = true;
  }

  %length = strLen(%phrase);

  %pos = InstrumentsDlg_Phrase.getCursorPos();
  %firstHalf = getSubStr(%phrase, 0, %pos);
  %secondHalf = getSubStr(%phrase, %pos, %length);

  %offset = strLen(%note);

  if (%withCommas) {
    %firstComma = true;
    %char = getSubStr(%firstHalf, %pos - 1, 1);

    if (%char $= "," || (%char $= "+" && %type $= "note")) {
      %firstComma = false;
    }

    if (strLen(%firstHalf) > 0 && %firstComma) {
      %firstHalf = %firstHalf @ ",";
      %offset++;
    }

    %secondComma = true;
    %char = getSubStr(%secondHalf, 0, 1);

    if (%char $= "," || (%char $= "+" && %type $= "note")) {
      %secondComma = false;
    }

    if (%secondComma) {
      %secondHalf = "," @ %secondHalf;
      %offset++;
    }
  }

  $Instruments::GUI::LastNoteAdded = %note TAB %type;
  InstrumentsClient.setPhrase(%firstHalf @ %note @ %secondHalf, %offset);
}

function InstrumentsClient::addLastNote(%this) {
  %lastNote = $Instruments::GUI::LastNoteAdded;
  %preview = !$Instruments::GUI::PlayLive;

  %note = getField(%lastNote, 0);
  %type = getField(%lastNote, 1);

  InstrumentsClient.addToPhrase(%note, %type);
  commandToServer('playNote', %note, $Instruments::GUI::Instrument, %preview);
}

// Stuff for the actual piano/keyboard

function InstrumentsClient::setIntervalEnabled(%this, %interval, %octave, %enabled) {
  if (strUpr(%interval) $= "C") {
    InstrumentsClient.setPianoKeyEnabled("C" @ %octave, %enabled);
  }
  else if (strUpr(%interval) $= "F") {
    InstrumentsClient.setPianoKeyEnabled("CS" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("D" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("DS" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("E" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("F" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("FS" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("G" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("GS" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("A" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("AS" @ %octave, %enabled);
    InstrumentsClient.setPianoKeyEnabled("B" @ %octave, %enabled);
  }
}

function InstrumentsClient::setPianoKeyEnabled(%this, %note, %enabled) {
  %key = ("InstrumentsDlg_PianoKeyBack_" @ %note);

  if (isObject(%key)) {
    %key.visible = %enabled;
  }

  %label = ("InstrumentsDlg_" @ %note);

  if (isObject(%label)) {
    %label.visible = %enabled;
  }
}
