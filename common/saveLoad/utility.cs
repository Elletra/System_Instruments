function Instruments::validateFilename(%this, %filename, %client, %showMessage) {
  if (strLen(%filename) > Instruments.const["MAX_FILENAME_LENGTH"]) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Filename", "The filename is too long!", %client);
    }

    return false;
  }

  if (_strEmpty(%filename)) {
    if (%showMessage) {
      Instruments.messageBoxOK("No Filename", "Please enter a filename.", %client);
    }

    return false;
  }

  if (!isValidFileName(%filename) || _cleanFilename(%filename) !$= %filename) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Filename", 
        "Invalid filename.\n\nOnly A-Z, 0-9, spaces, dashes, and underscores are allowed.", %client);
    }

    return false;
  }

  return true;
}

function Instruments::validateFileAuthor(%this, %author, %client, %showMessage) {
  %name = getField(%author, 0);
  %bl_id = getField(%author, 1);

  if (_strEmpty(%name)) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Author", "Missing author name!", %client);
    }

    return false;
  }

  if (_strEmpty(%bl_id)) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Author", "Missing author BL_ID!", %client);
    }

    return false;
  }

  // This is Blockland's name length limit
  if (strLen(%name) > 16) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Author", "Author name is too long!  (Max 16 characters)", %client);
    }

    return false;
  }

  if (strLen(%bl_id) > 8) {
    if (%showMessage) {
      Instruments.messageBoxOK("Invalid Author", "Author BL_ID is too high!", %client);
    }

    return false;
  }

  return true;
}

function Instruments::getFileAuthor(%this, %type, %filename, %localOrServer) {
  if (!Instruments.validateFilename(%filename)) { 
    return; 
  }

  %file = new FileObject();
  %path = Instruments.getFilePath(%type, %filename, %localOrServer);

  %file.openForRead(%path);
  %file.readLine();  // Skip the version line

  %author = %file.readLine();

  %file.close();
  %file.delete();

  return %author;
}

function Instruments::getFilePath(%this, %type, %filename, %localOrServer) {
  if (!Instruments.validateFilename(%filename)) { 
    return; 
  }

  %path = "config/";

  if (%localOrServer $= "server") {
    %path = %path @ "server/";
  }
  else {
    %path = %path @ "client/";
  }

  return %path @ "instruments/" @ %type @ "s/" @ %filename @ ".txt";
}
