function InstrumentsServer::setPlayerMuted(%this, %client, %target, %muted) {
  if (!isObject(%target)) {
    return;
  }

  if (%muted) {
    $Instruments::Server::Muted_[%client, %target] = true;
  }
  else {
    $Instruments::Server::Muted_[%client, %target] = "";
  }
}

function InstrumentsServer::muteAllPlayers(%this, %client, %exceptMe) {
  %count = ClientGroup.getCount();

  for (%i = 0; %i < %count; %i++) {
    %target = ClientGroup.getObject(%i);

    if (%exceptMe && %target == %client) {
      continue;
    }

    InstrumentsServer.setPlayerMuted(%client, %target, true);
  }
}

function InstrumentsServer::unmuteAllPlayers(%this, %client, %exceptMe) {
  %mutedMe = $Instruments::Server::Muted_[%client, %client];
  deleteVariables("$Instruments::Server::Muted_" @ %client @ "_*");

  if (%exceptMe) {
    $Instruments::Server::Muted_[%client, %client] = %mutedMe;
  }
}


function serverCmdInstruments_setPlayerMuted(%client, %target, %muted) {
  if (!%client.hasInstrumentsClient || !$Pref::Server::Instruments::Enabled) {
    return;
  }

  if (getSimTime() - %client.lastInstrumentsMuteTime < 100) {
    Instruments.messageBoxOK("Too Fast", "Woah... slow down there, champ!", %client);
    return;
  }

  if (!isObject(%target) || %target.getClassName() !$= "GameConnection") {
    Instruments.messageBoxOK("Error", "Player not found!", %client);
    return;
  }

  %client.lastInstrumentsMuteTime = getSimTime();
  InstrumentsServer.setPlayerMuted(%client, %target, %muted);
}

function serverCmdInstruments_muteAllPlayers(%client, %exceptMe) {
  if (!%client.hasInstrumentsClient || !$Pref::Server::Instruments::Enabled) {
    return;
  }

  if (getSimTime() - %client.lastInstrumentsMuteTime < 100) {
    Instruments.messageBoxOK("Too Fast", "Woah... slow down there, champ!", %client);
    return;
  }

  %client.lastInstrumentsMuteTime = getSimTime();
  InstrumentsServer.muteAllPlayers(%client, %exceptMe);
}

function serverCmdInstruments_unmuteAllPlayers(%client, %exceptMe) {
  if (!%client.hasInstrumentsClient || !$Pref::Server::Instruments::Enabled) {
    return;
  }

  if (getSimTime() - %client.lastInstrumentsMuteTime < 100) {
    Instruments.messageBoxOK("Too Fast", "Woah... slow down there, champ!", %client);
    return;
  }

  %client.lastInstrumentsMuteTime = getSimTime();
  InstrumentsServer.unmuteAllPlayers(%client, %exceptMe);
}
