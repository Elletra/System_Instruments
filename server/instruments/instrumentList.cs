function InstrumentsServer::sendInstrumentList(%this, %client) {
  commandToClient(%client, 'receiveInstrumentListStart');

  %count = InstrumentsServer.addOnCount;

  for (%i = 0; %i < %count; %i++) {
    %addOn = InstrumentsServer.addOn(%i);
    commandToClient(%client, 'receiveInstrumentAddOnData', %i TAB %addOn TAB InstrumentsServer.addOnAuthor(%addOn));
  }

  %count = InstrumentsServer.instrumentCount;

  for (%i = 0; %i < %count; %i++) {
    %name = InstrumentsServer.name(%i);
    %instrument = InstrumentsServer._(%name);
    %soundCount = %instrument.length;
    %list = "";

    for (%n = 0; %n < %soundCount; %n++) {
      if (%n > 0) {
        %list = %list @ "\n";
      }

      // Send the data in chunks so we're not spamming the client with a million clientCmds
      if (%n % 8 == 0) {
        if (!_strEmpty(%list)) {
          commandToClient(%client, 'receiveInstrumentList', %list);
        }

        %list = %name TAB %instrument.note(%n);
      }
      else {
        %list = %list @ %name TAB %instrument.note(%n);
      }  
    }

    if (!_strEmpty(%list)) {
      commandToClient(%client, 'receiveInstrumentList', %list);
    }
  }

  commandToClient(%client, 'receiveInstrumentListDone');
}
