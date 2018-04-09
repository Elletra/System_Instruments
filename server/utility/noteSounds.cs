// Gets the sound datablock of the note if it exists.
// If it doesn't, then it checks to see if there's an interval datablock that it can use.

function InstrumentsServer::getNoteSound(%this, %instrument, %note) {
  %noteName = %instrument @ "_" @ %note @ "Sound";

  // If there's already a sound datablock for the note, just play that instead

  if (isObject(%noteName)) {
    return nameToId(%noteName);
  }

  // if not, then let's determine the interval that will be used

  %letter = Instruments.getNoteLetter(%note);
  %octave = Instruments.getNoteOctave(%note);

  %interval = $Instruments::NoteToInterval[%letter];
  %pitch = $Instruments::NoteToPitch[%letter];

  if (!isObject(%instrument @ "_" @ %interval @ %octave @ "Sound")) {
    return -1;
  }

  if (%interval $= "C") {
    // Keep it CX-CX rather than have some notes before and after
    // else it'd be like A#2-D#6 which is just weird
    // Also it would create some weird pitch anomalies (e.g. C6:2 and D6:2 would be the same pitch)
    if (%pitch < 1) {
      %octave++;
    }
    else if (!isObject(%instrument @ "_" @ %interval @ %octave + 1 @ "Sound")) {
      return -1;
    }
  }

  return nameToId(%instrument @ "_" @ %interval @ %octave @ "Sound");
}

function InstrumentsServer::getNotePitch(%this, %note) {
  if (Instruments.isMelodicNote(%note)) {
    return $Instruments::NoteToPitch[Instruments.getNoteLetter(%note)];
  }
  else {
    return 1;
  }
}
