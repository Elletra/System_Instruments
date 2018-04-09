function Instruments::renameFile(%this, %type, %filename, %newFilename, %client) {
  if (%type !$= "phrase" && %type !$= "song" && %type !$= "bindset") { 
    return; 
  }

  %filename = stripTrailingSpaces(%filename);
  %newFilename = stripTrailingSpaces(%newFilename);

  if (!Instruments.validateFilename(%filename, %client, 1)) { 
    return; 
  }

  if (!Instruments.validateFilename(%newFilename, %client, 1)) { 
    return; 
  }

  if (%client !$= "") {
    if (!isObject(%client)) {
      return;
    }

    %localOrServer = "server";
  }
  else {
    %localOrServer = "local";
  }

  %path = Instruments.getFilePath(%type, %filename, %localOrServer);
  %newPath = Instruments.getFilePath(%type, %newFilename, %localOrServer);


  if (isFile(%path)) {
    
    if (isFile(%newPath)) {
      Instruments.messageBoxOK("File Name Taken", "A file with this name already exists!", %client);
      return;
    }

    // If we're renaming a server file
    if (%client !$= "") {
      %hasPermission = InstrumentsServer.checkDeletingPermissions(%client, 1);

      if (!%hasPermission) {
        return;
      }

      %author = Instruments.getFileAuthor(%type, %filename, %localOrServer);
      %bl_id = getField(%author, 1);

      if (%bl_id != %client.getBLID() && !%client.isAdmin && !%client.isSuperAdmin && !%client.isHost()) {
        Instruments.messageBoxOK("Not Allowed", "You do not have permission to rename this file.", %client);
        return;
      }
    }
  }
  else {
    Instruments.messageBoxOK("Error", "File does not exist.", %client);
    return;
  }

  if (%client !$= "") {
    if (!isObject(%client)) {
      return;
    }

    %localOrServer = "server";
  }
  else {
    %localOrServer = "local";
  }

  if (fileCopy(%path, %newPath)) {
    Instruments.messageBoxOK("File Renamed", "File renamed successfully.", %client);

    if (!fileDelete(%path)) {
      error("Instruments.renameFile() - ERROR: Could not delete old file!");
    }

    discoverFile(%newPath);

    if (%client $= "") {
      InstrumentsClient.refreshFileLists();
    }
    else {
      commandToAll('Instruments_onFileRenamed', %filename, %newFilename, %type);
    }
  }
  else {
    Instruments.messageBoxOK("Error", "Error renaming file.", %client);
  }

  if (%client !$= "") {
    %client.lastInstrumentsDeleteTime = getSimTime();
  }
}
