if (!isObject(InstrumentsFileManager)) {
  new ScriptObject(InstrumentsFileManager) {
    _phraseCount = 0;
    _songCount = 0;
    _bindsetCount = 0;
  };
}

function InstrumentsFileManager::addFile(%this, %type, %id, %name, %author) {
  %count = %this._[%type @ "Count"];

  %this._[%type @ "Name" @ %id] = %name;
  %this._[%type @ "ID" @ %name] = %id;
  %this._[%type @ "Author" @ %name] = %author;
  %this._[%type @ "Count"]++;
}

// Adding files

function InstrumentsFileManager::addPhrase(%this, %id, %name, %author) {
  %this.addFile("phrase", %id, %name, %author);
}

function InstrumentsFileManager::addSong(%this, %id, %name, %author) {
  %this.addFile("song", %id, %name, %author);
}

function InstrumentsFileManager::addBindset(%this, %id, %name, %author) {
  %this.addFile("bindset", %id, %name, %author);
}

// Getting file IDs

function InstrumentsFileManager::getPhraseID(%this, %name) {
  return %this._["phraseID" @ %name];
}

function InstrumentsFileManager::getSongID(%this, %name) {
  return %this._["songID" @ %name];
}

function InstrumentsFileManager::getBindsetID(%this, %name) {
  return %this._["bindsetID" @ %name];
}

// Getting file names

function InstrumentsFileManager::getPhraseName(%this, %id) {
  return %this._["phraseName" @ %id];
}

function InstrumentsFileManager::getSongName(%this, %id) {
  return %this._["songName" @ %id];
}

function InstrumentsFileManager::getBindsetName(%this, %id) {
  return %this._["bindsetName" @ %id];
}

// Getting file authors

function InstrumentsFileManager::getPhraseAuthor(%this, %name) {
  return %this._["phraseAuthor" @ %name];
}

function InstrumentsFileManager::getSongAuthor(%this, %name) {
  return %this._["songAuthor" @ %name];
}

function InstrumentsFileManager::getBindsetAuthor(%this, %name) {
  return %this._["bindsetAuthor" @ %name];
}

// Setting file IDs

function InstrumentsFileManager::setPhraseID(%this, %name, %id) {
  %oldID = %this._["phraseID" @ %name];

  %this._["phraseID" @ %name] = %id;
  %this._["phraseName" @ %oldID] = "";
  %this._["phraseName" @ %id] = %name;
}

function InstrumentsFileManager::setSongID(%this, %name, %id) {
  %oldID = %this._["songID" @ %name];

  %this._["songID" @ %name] = %id;
  %this._["songName" @ %oldID] = "";
  %this._["songName" @ %id] = %name;
}

function InstrumentsFileManager::setBindsetID(%this, %name, %id) {
  %oldID = %this._["bindsetID" @ %name];

  %this._["bindsetID" @ %name] = %id;
  %this._["bindsetName" @ %oldID] = "";
  %this._["bindsetName" @ %id] = %name;
}

// Setting file names

function InstrumentsFileManager::setPhraseName(%this, %id, %name) {
  %oldName = %this._["phraseName" @ %id];
  %author = %this._["phraseAuthor" @ %oldName];

  %this._["phraseName" @ %id] = %name;
  %this._["phraseID" @ %oldName] = "";
  %this._["phraseID" @ %name] = %id;

  %this._["phraseAuthor" @ %oldName] = "";
  %this._["phraseAuthor" @ %name] = %author;
}

function InstrumentsFileManager::setSongName(%this, %id, %name) {
  %oldName = %this._["songName" @ %id];
  %author = %this._["songAuthor" @ %oldName];

  %this._["songName" @ %id] = %name;
  %this._["songID" @ %oldName] = "";
  %this._["songID" @ %name] = %id;

  %this._["songAuthor" @ %oldName] = "";
  %this._["songAuthor" @ %name] = %author;
}

function InstrumentsFileManager::setBindsetName(%this, %id, %name) {
  %oldName = %this._["bindsetName" @ %id];
  %author = %this._["bindsetAuthor" @ %oldName];

  %this._["bindsetName" @ %id] = %name;
  %this._["bindsetID" @ %oldName] = "";
  %this._["bindsetID" @ %name] = %id;

  %this._["bindsetAuthor" @ %oldName] = "";
  %this._["bindsetAuthor" @ %name] = %author;
}

// Setting file authors

function InstrumentsFileManager::setPhraseAuthor(%this, %name, %author) {
  %this._["phraseAuthor" @ %name] = %author;
}

function InstrumentsFileManager::setSongAuthor(%this, %name, %author) {
  %this._["songAuthor" @ %name] = %author;
}

function InstrumentsFileManager::setBindsetAuthor(%this, %name, %author) {
  %this._["bindsetAuthor" @ %name] = %author;
}

// Deleting file info

function InstrumentsFileManager::deletePhrase(%this, %name) {
  %id = %this.getPhraseID(%name);

  %this._["phraseName" @ %id] = "";
  %this._["phraseID" @ %name] = "";
  %this._["phraseAuthor" @ %name] = "";
}

function InstrumentsFileManager::deleteSong(%this, %name) {
  %id = %this.getSongID(%name);

  %this._["songName" @ %id] = "";
  %this._["songID" @ %name] = "";
  %this._["songAuthor" @ %name] = "";
}

function InstrumentsFileManager::deleteBindset(%this, %name) {
  %id = %this.getBindsetID(%name);

  %this._["bindsetName" @ %id] = "";
  %this._["bindsetID" @ %name] = "";
  %this._["bindsetAuthor" @ %name] = "";
}

// Clearing file info

function InstrumentsFileManager::clearPhrases(%this) {
  %count = %this._phraseCount;

  for (%i = 0; %i < %count; %i++) {
    %name = %this.getPhraseName(%i);
    %this.deletePhrase(%name);
  }
}

function InstrumentsFileManager::clearSongs(%this) {
  %count = %this._songCount;

  for (%i = 0; %i < %count; %i++) {
    %name = %this.getSongName(%i);
    %this.deleteSong(%name);
  }
}

function InstrumentsFileManager::clearBindsets(%this) {
  %count = %this._bindsetCount;

  for (%i = 0; %i < %count; %i++) {
    %name = %this.getBindsetName(%i);
    %this.deleteBindset(%name);
  }
}

function InstrumentsFileManager::clearFiles(%this) {
  %this.clearPhrases();
  %this.clearSongs();
  %this.clearBindsets();
}
