function Player::setInstrument(%this, %instrument) {
  %this.instrument = %instrument;

  if (isObject(%client = %this.client)) {
    commandToClient(%client, 'setInstrument', %instrument);
  }
}

function serverCmdOnSelectInstrument(%client, %instrument) {
  // callback
}
