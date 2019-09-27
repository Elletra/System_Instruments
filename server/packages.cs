package System_Instruments__server {

  function GameConnection::loadMission(%this) {
    Parent::loadMission(%this);
    InstrumentsServer.schedule(1, sendFilenames, %this, "all");
  }

  function GameConnection::autoAdminCheck(%this) {
    %parent = Parent::autoAdminCheck(%this);

    commandToClient(%this, 'Instruments_GetVersion', $Instruments::Version, $Instruments::NotationVersion);
    commandToClient(%this, 'Instruments_UpdatePref', "MaxSongPhrases", Instruments.const["MAX_SONG_PHRASES"]);

    InstrumentsServer.schedule(1, sendInstrumentList, %this);

    return %parent;
  }

  function GameConnection::onDeath(%client, %killerPlayer, %killerClient, %damageType, %damageLocation) {
    if (isObject(%player = %client.player) && %player.isPlayingInstrument) {
      InstrumentsServer.stopPlaying(%player);
    }

    Parent::onDeath(%client, %killerPlayer, %killerClient, %damageType, %damageLocation);
  }

  function GameConnection::onDrop(%client, %reason) {
    if (isObject(%client.instrumentBinds)) {
      %client.instrumentBinds.delete();
    }

    InstrumentsServer.unmuteAllPlayers(%client);
    deleteVariables("$Instruments::Server::Muted_*_" @ %client);

    Parent::onDrop(%client, %reason);
  }

  function WeaponImage::onMount(%this, %obj, %slot) {
    Parent::onMount(%this, %obj, %slot);

    if (%this.instrumentType $= "" && %obj.instrument !$= "") {
      %obj.setInstrument("");
    }
    else if (%this.instrumentType !$= "") {
      %obj.setInstrument(%this.instrumentType);
    }
  }

  function ServerCmdUnUseTool(%client) {
    if (isObject(%player = %client.player)) {
      %player.setInstrument("");
    }

    Parent::ServerCmdUnUseTool(%client);
  }
};
activatePackage(System_Instruments__server);
