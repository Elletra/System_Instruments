// mod version TAB notation version
// player name TAB bl_id
// phrase (e.g. C3,C4,C5)

// mod version TAB notation version
// player name TAB bl_id
// song (e.g. 0,1,2)
// phrase 0
// phrase 1
// phrase 2
// etc.


function Instruments::saveFile(%this, %type, %filename, %phraseOrSong, %client, %overwrite, %authorToWrite) {
  if (%type !$= "phrase" && %type !$= "song" && %type !$= "bindset") { 
    return; 
  }

  %filename = stripTrailingSpaces(%filename);
  %phraseOrSong = _cleanPhrase(%phraseOrSong);

  if (!Instruments.validateFilename(%filename, %client, 1)) { 
    return; 
  }

  if (%authorToWrite $= "") {
    if (%client $= "") {
      %authorToWrite = $Pref::Player::NetName TAB getNumKeyID();
    }
    else {
      %authorToWrite = %client.getPlayerName() TAB %client.getBLID();
    }
  }

  if (!Instruments.validateFileAuthor(%authorToWrite, %client, 1)) {
    return;
  }

  if (%client $= "") {
    %binds = InstrumentsClient.binds;
  }
  else {
    %binds = %client.instrumentBinds;
  }

  if (%type $= "bindset" && %binds.bindCount < 3) {
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
  %overwritingFile = isFile(%path);

  if (%overwritingFile) {

    // If we're saving to server, we need to check if client has permission on the serverto overwrite files
    // Otherwise, if it were on the client's local system, you don't need to check for permissions (obviously)
    if (%client !$= "") {
      %hasPermission = InstrumentsServer.checkDeletingPermissions(%client, 0);

      if (!%hasPermission) {
        Instruments.messageBoxOK("File Exists", "File already exists!", %client);
        return;
      }

      %author = Instruments.getFileAuthor(%type, %filename, %localOrServer);
      %bl_id = getField(%author, 1);

      if (%bl_id != %client.getBLID() && !%client.isAdmin && !%client.isSuperAdmin && !%client.isHost()) {
        Instruments.messageBoxOK("File Exists", "File already exists!", %client);
        return;
      }
    }

    if (!%overwrite) {

      if (%client $= "") {
        // If we're saving to local
        %yes = "Instruments.saveFile(\"" @ %type @ "\", \"" @ %filename @ "\", \"" @ %phraseOrSong @ 
          "\", \"\", 1, \"" @ %authorToWrite @ "\");";
          
        Instruments.messageBoxYesNo("File Exists", "File already exists!  Overwrite?", %yes);
        return;
      }

      if (%type $= "phrase") {
        %client.instrumentPhrase = %phraseOrSong;
        %yes = 'savePhraseOverwrite';
      }
      
      if (%type $= "song") {
        %client.instrumentSong = %phraseOrSong;
        %yes = 'saveSongOverwrite';
      }
      
      if (%type $= "bindset") {
        %yes = 'saveBindsetOverwrite';
      }

      %client.instrumentFileAuthor = %authorToWrite;
      %client.instrumentFileName = %filename;

      %author = Instruments.getFileAuthor(%type, %filename, %localOrServer);
      %name = getField(%author, 0);
      %bl_id = getField(%author, 1);

      %body = "File already exists!\n\n<color:000000>Author: <color:0000FF>" @ %name @ 
        "\n<color:000000>BL_ID: <color:0000FF>" @ %BL_ID @ "\n\n<color:000000>Overwrite?";
        
      Instruments.messageBoxYesNo("File Exists", %body, %yes, %client);
      return;
    }
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

  %file = new FileObject();
  %file.openForWrite(%path);
  %file.writeLine($Instruments::Version TAB $Instruments::NotationVersion);
  %file.writeLine(%authorToWrite);

  if (%type !$= "bindset") {
    %file.writeLine(%phraseOrSong);
  }

  if (%type $= "song") {

    if (%localOrServer $= "local") {
      %maxSongPhrases = $Instruments::Client::ServerPref::MaxSongPhrases;
    }
    else {
      %maxSongPhrases = $Pref::Server::Instruments::MaxSongPhrases;
    }

    for (%i = 0; %i < %maxSongPhrases; %i++) {
      if (%client $= "") {
        %phrase = getField(InstrumentsDlg_SongPhraseList.getRowText(%i), 1);
      }
      else {
        %phrase = %client.songPhrase[%i];
      }

      %file.writeLine(%phrase);
    }
  }
  else if (%type $= "bindset") {
    %bindCount = %binds.currIndex;

    for (%i = 0; %i < %bindCount; %i++) {
      
      // Hard-coded
      if (%i >= Instruments.const["MAX_BINDS"]) {
        break;
      }

      %bind = %binds._bind[%i];

      if (_strEmpty(%bind)) {
        continue;
      }

      %file.writeLine(%bind);
    }
  }

  %file.close();
  %file.delete();


  if (!%overwritingFile) {
    if (%client $= "") {
      InstrumentsClient.refreshFileLists();
    }
    else if (isFile(%path)) {
      commandToAll('Instruments_onFileAdded', %filename, %type, %client.name TAB %client.getBLID());
    }
  }

  if (isFile(%path)) {
    Instruments.messageBoxOK("File Saved", "File saved successfully.", %client);
  }
  else {
    Instruments.messageBoxOK("Write Error", "Could not save file!", %client);
  }

  if (%client !$= "") {
    %client.lastInstrumentsSaveTime = getSimTime();
  }
}
