function Instruments::deleteFile(%this, %type, %filename, %client, %overwrite) {
  if (%type !$= "phrase" && %type !$= "song" && %type !$= "bindset") { 
    return; 
  }

  %filename = stripTrailingSpaces(%filename);

  if (!Instruments.validateFilename(%filename, %client, 1)) { 
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

  if (isFile(%path)) {

    // If we're deleting a server file
    if (%client !$= "") {
      %hasPermission = InstrumentsServer.checkDeletingPermissions(%client, 1);

      if (!%hasPermission) {
        return;
      }

      %author = Instruments.getFileAuthor(%type, %filename, %localOrServer);
      %bl_id = getField(%author, 1);

      if (%bl_id != %client.getBLID() && !%client.isAdmin && !%client.isSuperAdmin && !%client.isHost()) {
        Instruments.messageBoxOK("Not Allowed", "You do not have permission to delete this file.", %client);
        return;
      }
    }

    if (!%overwrite) {

      if (%client $= "") {
        %yes = "Instruments.deleteFile(\"" @ %type @ "\", \"" @ %filename @ "\", \"\", 1);";
        Instruments.messageBoxYesNo("Delete File?", "Are you sure you want to delete this file?", %yes);
        return;
      }

      if (%type $= "phrase") {
        %yes = 'deletePhraseOverwrite';
      }
      
      if (%type $= "song") {
        %yes = 'deleteSongOverwrite';
      }
      
      if (%type $= "bindset") {
        %yes = 'deleteBindsetOverwrite';
      }

      %client.instrumentFileName = %filename;
      Instruments.messageBoxYesNo("Delete File?", "Are you sure you want to delete this file?", %yes, %client);
      return;
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

  if (fileDelete(%path)) {
    Instruments.messageBoxOK("File Deleted", "File deleted successfully.", %client);
    
    if (%client $= "") {
      InstrumentsClient.refreshFileLists();
    }
    else {
      commandToAll('Instruments_onFileDeleted', %filename, %type);
    }
  }
  else {
    Instruments.messageBoxOK("Error", "Error deleting file.", %client);
  }

  if (%client !$= "") {
    %client.lastInstrumentsDeleteTime = getSimTime();
  }
}
