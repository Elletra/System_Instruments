// This file contains code specific to Blockland Rebuilt, which has the ability to play sounds at a
// specific pitch.

function Instruments_Play3D(%source, %sound, %position, %pitch) {
  %sourceClient = %source.client;

  if (%sourceClient $= "") {
    %sourceClient = 0;
  }

  %count = ClientGroup.getCount();

  for (%i = 0; %i < %count; %i++) {
    %client = ClientGroup.getObject(%i);

    if (!$Instruments::Server::Muted_[%client, %sourceClient]) {
      %client.playPitched3D(%sound, %position, %pitch);
    }
  }
}

function InstrumentsServer::playPitchedSound(%this, %obj, %sound, %pitch, %position) {
  if (%position $= "") {
    %obj.playPitched2D(%sound, %pitch);
  }
  else {
    Instruments_Play3D(%obj, %sound, %position, %pitch);
  }
}
