// ===================================
//  Non-instrument-specific functions
// ===================================


function GameConnection::isHost(%this) {
  return %this.bl_id == getNumKeyID() || %this.isLocalConnection();
}

// ---------------------------------------

function strCount(%string, %needle) {
  %count = 0;

  for (%i = 0; %i < strLen(%string); %i++) {
    if (strPos(getSubStr(%string, %i, strLen(%needle)), %needle) != -1) {
      %i += strLen(%needle) - 1;
      %count++;
    }
  }
  return %count;
}

function striCount(%string, %needle) {
  %count = 0;

  for (%i = 0; %i < strLen(%string); %i++) {
    if (striPos(getSubStr(%string, %i, strLen(%needle)), %needle) != -1) {
      %i += strLen(%needle) - 1;
      %count++;
    }
  }
  return %count;
}

function mRound(%num) {
   return mFloatLength(%num, 0);
}

function capitalizeFirstLetter(%string) {
  return strUpr(getSubStr(%string, 0, 1)) @ getSubStr(%string, 1, strLen(%string));
}

// keepChars() - The opposite of stripChars -- you specify which characters you want to keep,
// and it removes the rest

function keepChars(%string, %chars) {
  %exclude = stripChars(%string, %chars);
  return stripChars(%string, %exclude);
}

function stripAsciiControlChars(%string) {
  %chars = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F" @
           "\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F\x7F";

  return stripChars(%string, %chars);
}

function isAddOnEnabled(%addon) {
  return $AddOn__[%addon] == 1 || $AddOnLoaded__[%addon] == 1;
}

// ---------------------------------------

// Just a quick and dirty technique to check if a value is -999999 <= n <= 999999
// I put a _ in front of it because I don't want to fuck up someone else's isInt, which is probably
// better than this one

function _isInt(%value) {
  return strCount(%value, "-") <= 1 && (stripChars(%value, "1234567890-") $= "");
}

function _strEmpty(%string) {
  return stripTrailingSpaces(%string) $= "";
}

function _addQuotes(%str) {
  return "\"" @ %str @ "\"";
}

// ---------------------------------------

// I lied -- it's not 100% non-instrument specific

function _cleanFilename(%filename) {
  %filename = stripAsciiControlChars(%filename);
  return stripChars(%filename, "`~!@#$%^&*()=+:;\"/\\,.<>[]{}");
}

function _cleanPhrase(%phrase) {
  %phrase = stripAsciiControlChars(%phrase);
  return stripChars(%phrase, "*()='\"\\[]{}");
}

function _cleanNote(%note) {
  %note = stripAsciiControlChars(%note);
  return stripChars(%note, "!@$%^&*()-_=\\|/;'`~\",<>[]{}");
}
