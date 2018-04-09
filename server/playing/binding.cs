function serverCmdInstruments_BindToKey(%client, %key, %phraseOrNote) {
  if (!%client.hasInstrumentsClient) {
    return;
  }

  if (!InstrumentsServer.checkBindsetPermissions(%client, 0)) {
    return;
  }

  // if (%client.lastInstrumentsBindTime !$= "" && getSimTime() - %client.lastInstrumentsBindTime < 20) {
  //    return;
  // }

  if (_strEmpty(%key)) {
    return;
  }

  // %client.lastInstrumentsBindTime = getSimTime();

  if (!isObject(%client.instrumentBinds)) {
    %client.instrumentBinds = new ScriptObject() {
      class = "InstrumentsBindset";
      client = %client;
    };
  }

  if (_strEmpty(%phraseOrNote)) {
    %client.instrumentBinds.removeBind(%key);
  }
  else {
    %client.instrumentBinds.addBind(%key, %phraseOrNote);
  }
}

function serverCmdInstruments_clearAllKeys(%client) {
  if (!%client.hasInstrumentsClient) {
    return;
  }

  if (!InstrumentsServer.checkBindsetPermissions(%client, 0)) {
    return;
  }

  if (!isObject(%client.instrumentBinds)) {
    %client.instrumentBinds = new ScriptObject() {
      class = "InstrumentsBindset";
      client = %client;
    };
  }
  else {
    %client.instrumentBinds.clearBinds();
  }
}
