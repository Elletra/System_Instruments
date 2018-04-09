function Instruments::validateFilename(%this, %filename, %client, %showMessage) {
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
