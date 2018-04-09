// This add-on uses certain functions that are apparently only defined in allClientScripts.cs
// So we have to define them ourselves if it's dedicated

function isValidFileName(%filename) {
  if (strlen(%filename) <= 0) {
    return false;
  }

  if (strlen(%filename) >= 255) {
    return false;
  }

  %badChars = "\\ / : * ? \" < > |";
  %count = getWordCount(%badChars);

  for (%i = 0; %i < %count; %i++) {
    %word = getWord(%badChars, %i);

    if (strpos(%filename, %word) != -1) {
      return false;
    }
  }

  return true;
}
