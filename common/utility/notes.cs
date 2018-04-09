// =============================
//  Utility functions for notes
// =============================


// Checks to see if input letter is part of the musical scale
function Instruments::isNoteLetter(%this, %letter) {
  if (strLen(%letter) != 1) return false;
  return (stripChars(strUpr(%letter), "ABCDEFG") $= "");
}

// Checks to see if input note is part of the musical scale
function Instruments::isMelodicNote(%this, %note) {
  %length = strLen(%note);

  if (%length < 2 || %length > 3) return false;

  %firstChar = getSubStr(%note, 0, 1);

  if (!Instruments.isNoteLetter(%firstChar)) return false;

  // Check for sharps
  %secondChar = getSubStr(%note, 1, 1);

  if (%length == 3 && %secondChar !$= "#" && %secondChar !$= "S") return false;
  if (%length == 3 && !_isInt(getSubStr(%note, 2, 1))) return false;
  if (%length == 2 && !_isInt(%secondChar)) return false;

  return true;
}


function Instruments::getNoteOctave(%this, %note) {
  %length = strLen(%note);

  if (%length == 2) return getSubStr(%note, 1, 1);
  if (%length == 3) return getSubStr(%note, 2, 1);
}


function Instruments::getNoteLetter(%this, %note) {
  %length = strLen(%note);

  if (!Instruments.isMelodicNote(%note)) {
    return %note;
  }

  if (%length == 2) return getSubStr(%note, 0, 1);
  if (%length == 3) return getSubStr(strReplace(%note, "#", "S"), 0, 2);
}


function Instruments::isSharpNote(%this, %note) {
  %length = strLen(%note);

  if (%length != 3) return false;

  %sharp = strUpr(getSubStr(%note, 1, 1));

  return (%sharp $= "S" || %sharp $= "#");
}
