if (!isObject(InstrumentsStatsTCP)) {
  new TCPObject(InstrumentsStatsTCP) {
    hasSentStats = false;
  };
}

if (!isObject(InstrumentsStatsSO)) {
  new ScriptObject(InstrumentsStatsSO);
}

InstrumentsStatsTCP.scriptObject = InstrumentsStatsSO;
InstrumentsStatsSO.tcpObject = InstrumentsStatsTCP;

function InstrumentsStatsTCP::onLine(%this, %line) {
  // To prevent buffer overrun
  if (strLen(%line) >= 4076) {
    %line = getSubStr(%line, 0, 4060) @ "...  (truncated)";
  }

  echo("[Instruments Stats] " @ %line);
}

function InstrumentsStatsTCP::sendStats(%this) {
  if (%this.hasSentStats) {
    return;
  }

  echo("Sending instruments stats...");
  
  %stats = "{";

  %stats = %stats NL _keyValuePair("name", $pref::Player::NetName, ",");
  %stats = %stats NL _keyValuePair("instrumentsVersion", $Instruments::Version, ",");
  %stats = %stats NL _keyValuePair("notationVersion", $Instruments::NotationVersion, ",");
  %stats = %stats NL _keyValuePair("hasInstrumentsEvents", isFile("Add-Ons/Event_Instruments/server.cs"), ",");

  %path = "Add-Ons/Instrument_*/server.cs";
  %instrumentCount = 0;

  for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {
    %instrumentCount++;
  }

  %stats = %stats NL _keyValuePair("instrumentCount", %instrumentCount, ",");

  %stats = %stats NL _keyValuePair("clientPref_changeKeyLabels", $Pref::Client::Instruments::ChangeKeyLabels, ",");
  %stats = %stats NL _keyValuePair("clientPref_coloredKeys", $Pref::Client::Instruments::ColoredKeys, ",");
  %stats = %stats NL _keyValuePair("clientPref_disableMoveMap", $Pref::Client::Instruments::DisableMoveMap, ",");
  %stats = %stats NL _keyValuePair("clientPref_muteByDefault", $Pref::Client::Instruments::MuteByDefault ? "1" : "0", ",");

  %stats = %stats NL _keyValuePair("serverPref_enabled", $Pref::Server::Instruments::Enabled, ",");
  %stats = %stats NL _keyValuePair("serverPref_permissions", $Pref::Server::Instruments::Permissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_songPermissions", $Pref::Server::Instruments::SongPermissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_bindsetPermissions", $Pref::Server::Instruments::BindsetPermissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_savingPermissions", $Pref::Server::Instruments::SavingPermissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_loadingPermissions", $Pref::Server::Instruments::LoadingPermissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_deletingPermissions", $Pref::Server::Instruments::DeletingPermissions, ",");
  %stats = %stats NL _keyValuePair("serverPref_savingTimeout", $Pref::Server::Instruments::SavingTimeout, ",");
  %stats = %stats NL _keyValuePair("serverPref_loadingTimeout", $Pref::Server::Instruments::LoadingTimeout, ",");
  %stats = %stats NL _keyValuePair("serverPref_deletingTimeout", $Pref::Server::Instruments::DeletingTimeout);

  %stats = %stats NL "}";
  %this.send(%stats);
  %this.hasSentStats = true;
  %this.schedule(1000, disconnect);
}

function InstrumentsStatsTCP::onConnected(%this) {
  echo("Instrument stats connected.");
  %this.sendStats();
}

function InstrumentsStatsTCP::onConnectFailed(%this) {
  echo("Instrument stats failed to connect!");
}

function InstrumentsStatsTCP::onDisconnect(%this) {
  echo("Instrument stats disconnected.");
}

InstrumentsStatsTCP.schedule(1000, connect, "electrk.rocks:8585");
