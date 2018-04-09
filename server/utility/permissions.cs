// 0 for everyone (except for deleting a file, where 0 is author only)
// 1 for admins and up
// 2 for super admins and up
// 3 for host only

function InstrumentsServer::checkPermissions(%this, %client, %pref, %type, %showMessage) {
  if (!$Pref::Server::Instruments::Enabled) {
    if (%showMessage) {
      Instruments.messageBoxOK("Instruments Disabled", "Instruments are disabled on this server.", %client);
    }

    return false;
  }

  if (%pref >= 3 && !%client.isHost()) {
    if (%showMessage) {
      Instruments.messageBoxOK("Host Only", %type SPC "only for the host on this server.", %client);
    }

    return false;
  }

  if (%pref >= 2 && !%client.isSuperAdmin) {
    if (%showMessage) {
      Instruments.messageBoxOK("Super Admins Only", %type SPC "only for super admins on this server.", %client);
    }

    return false;
  }

  if (%pref >= 1 && !%client.isAdmin && !%client.isSuperAdmin) {
    if (%showMessage) {
      Instruments.messageBoxOK("Admins Only", %type SPC "only for admins on this server.", %client);
    }
    
    return false;
  }

  return true;
}


function InstrumentsServer::checkInstrumentPermissions(%this, %client, %showMessage) {
  if (!%client.canUseInstruments) {
    if (%showMessage) {
      Instruments.messageBoxOK("Not Allowed", "You are not allowed to use the instruments mod on this server.", %client);
    }

    return false;
  }

  %pref = $Pref::Server::Instruments::Permissions;
  return InstrumentsServer.checkPermissions(%client, %pref, "Instruments are", %showMessage);
}

function InstrumentsServer::checkPermissionsPref(%this, %client, %pref, %type, %showMessage) {
  if (!InstrumentsServer.checkInstrumentPermissions(%client, %showMessage)) { 
    return false; 
  }

  return InstrumentsServer.checkPermissions(%client, %pref, %type, %showMessage);
}

function InstrumentsServer::checkSongPermissions(%this, %client, %showMessage) {
  %pref = $Pref::Server::Instruments::SongPermissions;
  return InstrumentsServer.checkPermissionsPref(%client, %pref, "Songs are", %showMessage);
}

function InstrumentsServer::checkBindsetPermissions(%this, %client, %showMessage) {
  %pref = $Pref::Server::Instruments::BindsetPermissions;
  return InstrumentsServer.checkPermissionsPref(%client, %pref, "Bindsets are", %showMessage);
}

function InstrumentsServer::checkSavingPermissions(%this, %client, %showMessage) {
  %pref = $Pref::Server::Instruments::SavingPermissions;
  return InstrumentsServer.checkPermissionsPref(%client, %pref, "Saving files is", %showMessage); 
}

function InstrumentsServer::checkLoadingPermissions(%this, %client, %showMessage) {
  %pref = $Pref::Server::Instruments::LoadingPermissions;
  return InstrumentsServer.checkPermissionsPref(%client, %pref, "Loading files is", %showMessage); 
}

function InstrumentsServer::checkDeletingPermissions(%this, %client, %showMessage) {
  %pref = $Pref::Server::Instruments::DeletingPermissions;
  return InstrumentsServer.checkPermissionsPref(%client, %pref, "Deleting, overwriting, and renaming files is", %showMessage); 
}
