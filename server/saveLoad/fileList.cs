function InstrumentsServer::sendFilenames(%this, %client, %type) {
  if (%type $= "" || %type $= "all") {
    InstrumentsServer.sendFilenames(%client, "phrase");
    InstrumentsServer.sendFilenames(%client, "song");
    InstrumentsServer.sendFilenames(%client, "bindset");
  }
  else if (%type !$= "phrase" && %type !$= "song" && %type !$= "bindset") {
    error("InstrumentsServer::sendFilenames(" @ %client @ ") - Invalid type: " @ %type);
    return;
  }

  commandToClient(%client, 'Instruments_receiveFileListStart', %type);

  %path = "config/server/instruments/" @ strLwr(%type) @ "s/*.txt";
  %client.remainingFiles_[%type] = getFileCount(%path);

  %list = "";
  %count = 0;

  for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {
    %filename = fileBase(%file);
    %author = Instruments.getFileAuthor(%type, %filename, "server");

    if (%count > 0) {
      %list = %list @ "\n";
    }

    // Send the data in chunks so we're not spamming the client with a million clientCmds
    %reachedChunkSize = %count % Instruments.const["FILE_LIST_CHUNK_SIZE"] == 0 && %count > 0;

    // serverCmds and clientCmds have a max length of 250 chars, so let's send it if
    // it's too long already
    %reachedMaxLength = strLen(%list) >= Instruments.const["MAX_PACKET_LENGTH"];

    if (%reachedChunkSize || %reachedMaxLength) {
      if (!_strEmpty(%list)) {
        InstrumentsServer.schedule(1, sendFilePacket, %client, %list, %type);
        %list = "";
        %count = 0;
      }

      %list = %filename TAB %type TAB %author;
    }
    else {
      %list = %list @ %filename TAB %type TAB %author;
    }

    %count++;
  }

  if (!_strEmpty(%list)) {
    InstrumentsServer.schedule(1, sendFilePacket, %client, %list, %type);
  }
}

function InstrumentsServer::updateFilename(%this, %old, %new, %type) {
  commandToClient(%client, 'Instruments_updateFileName', %old, %new, %type);
}

function InstrumentsServer::onFileDeleted(%this, %name, %type) {
  commandToClient(%client, 'Instruments_onFileDeleted', %name, %type);
}

function InstrumentsServer::sendFilePacket(%this, %client, %list, %type) {
  commandToClient(%client, 'Instruments_receiveFileList', %list);
  %client.remainingFiles_[%type] -= getRecordCount(%list);

  if (%client.remainingFiles_[%type] <= 0) {
    commandToClient(%client, 'Instruments_receiveFileListDone', %type);
  }
}

function InstrumentsServer::canDeleteFile(%this, %client, %file, %type) {
  if (!Instruments.validateFilename(%file)) {
    return false;
  }

  if (!InstrumentsServer.checkDeletingPermissions(%client, 1)) {
    return false;
  }

  if (!%client.isAdmin && !%client.isSuperAdmin && !%client.isHost()) {
    %author = InstrumentsServer.getFileAuthor(%type, %file, "server");
    %name = getField(%author, 0);
    %bl_id = getField(%author, 1);

    if (%client.getBLID() != %bl_id) {
      return false;
    }
  }

  return true;
}
