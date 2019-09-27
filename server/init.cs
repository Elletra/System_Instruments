// Server-specific

if (!isObject(InstrumentsServer)) {
  new ScriptObject(InstrumentsServer) {
    class = "InstrumentsNamespace";
  };
}

function InstrumentsServer::init(%this) {
  if ($Pref::Server::Instruments::Enabled $= "") {
    $Pref::Server::Instruments::Enabled = true;
  }

  if ($Pref::Server::Instruments::Permissions $= "") {
    $Pref::Server::Instruments::Permissions = 0;
  }

  if ($Pref::Server::Instruments::SongPermissions $= "") {
    $Pref::Server::Instruments::SongPermissions = 0;
  }

  if ($Pref::Server::Instruments::BindsetPermissions $= "") {
    $Pref::Server::Instruments::BindsetPermissions = 0;
  }

  if ($Pref::Server::Instruments::SavingPermissions $= "") {
    $Pref::Server::Instruments::SavingPermissions = 0;
  }

  if ($Pref::Server::Instruments::LoadingPermissions $= "") {
    $Pref::Server::Instruments::LoadingPermissions = 0;
  }

  if ($Pref::Server::Instruments::DeletingPermissions $= "") {
    $Pref::Server::Instruments::DeletingPermissions = 1;
  }

  if ($Pref::Server::Instruments::SavingTimeout $= "") {
    $Pref::Server::Instruments::SavingTimeout = 10;
  }

  if ($Pref::Server::Instruments::LoadingTimeout $= "") {
    $Pref::Server::Instruments::LoadingTimeout = 5;
  }

  if ($Pref::Server::Instruments::DeletingTimeout $= "") {
    $Pref::Server::Instruments::DeletingTimeout = 1;
  }

  if (isFunction(RTB_registerPref)) {
    %category = "Playable Instruments";
    %addon = "System_Instruments";

    RTB_registerPref("Enabled", %category, "$Pref::Server::Instruments::Enabled", "bool", %addon, 1, 0, 0);
    RTB_registerPref("Restrict instruments", %category, "$Pref::Server::Instruments::Permissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Anyone 0", %addon, 0, 0, 0);

    RTB_registerPref("Restrict playing/making songs", %category, "$Pref::Server::Instruments::SongPermissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Anyone 0", %addon, 0, 0, 0);

    RTB_registerPref("Restrict server-side bindsets", %category, "$Pref::Server::Instruments::BindsetPermissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Anyone 0", %addon, 0, 0, 0);

    RTB_registerPref("Restrict saving files", %category, "$Pref::Server::Instruments::SavingPermissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Anyone 0", %addon, 0, 0, 0);

    RTB_registerPref("Restrict loading files", %category, "$Pref::Server::Instruments::LoadingPermissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Anyone 0", %addon, 0, 0, 0);

    RTB_registerPref("Restrict deleting/overwriting/renaming files", %category, "$Pref::Server::Instruments::DeletingPermissions", 
      "list Host_Only 3 Super_Admin_Only 2 Admin_Only 1 Author 0", %addon, 1, 0, 0);

    RTB_registerPref("File saving timeout", %category, "$Pref::Server::Instruments::SavingTimeout", 
      "int 5 30", %addon, 10, 0, 0);

    RTB_registerPref("File loading timeout", %category, "$Pref::Server::Instruments::LoadingTimeout", 
      "int 2 30", %addon, 5, 0, 0);

    RTB_registerPref("File deleting timeout", %category, "$Pref::Server::Instruments::DeletingTimeout", 
      "int 1 30", %addon, 1, 0, 0);
  }

  InstrumentsServer.loadInstrumentNotes();
}

function InstrumentsServer::loadInstrumentNotes(%this) {
  InstrumentsServer.clearInstruments();
  
  %path = "Add-Ons/Instrument_*/sounds/*.wav";

  for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {

    %addon = strReplace(getSubStr(%file, 8, striPos(%file, "/sounds/") - 8), "-", "DASH");
    %addon = strReplace(%addon, "'", "APOS");

    // Checking to see if instrument is enabled
    %enabled = $AddOn__[%addon];
    
    if (%enabled != 1) { 
      continue; 
    }

    %fileName = fileName(%file);
    %filePath = filePath(%file);

    // Instrument names are retrieved from the filename itself, rather than the add-on name
    // This is to support sound packs, among other things
    %instrument = getSubStr(%fileName, 0, strPos(%fileName, "_"));

    %note = getSubStr(%fileName, strLen(%instrument) + 1, strLen(%fileName));
    %note = getSubStr(%note, 0, striPos(%note, ".wav"));

    if (!isObject(InstrumentsServer._(%instrument))) {
      InstrumentsServer.addInstrument(%instrument);
    }

    InstrumentsServer.addNote(%instrument, %note);
    %noteName = %instrument @ "_" @ %note @ "Sound";

    if (!isObject(%noteName)) {
      datablock AudioProfile(genericInstrumentSound) {
        description = "AudioClosest3D";
        fileName = %file;
        preload = true;
      };

      if (!isObject(%obj = nameToID("genericInstrumentSound"))) { 
        continue; 
      }

      %obj.setName(%noteName);
    }
  }

  // Load credits

  %path = "Add-Ons/Instrument_*/server.cs";

  for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {

    %addon = strReplace(getSubStr(%file, 8, striPos(%file, "/server.cs") - 8), "-", "DASH");
    %addon = strReplace(%addon, "'", "APOS");

    // Checking to see if instrument is enabled
    %enabled = $AddOn__[%addon];
    
    if (%enabled != 1) { 
      continue; 
    }

    if (InstrumentsServer.addOnIndex(%addon) $= "") {
      %creditsPath = "Add-Ons/" @ %addon @ "/credits.txt";
      %author = "";

      if (isFile(%creditsPath)) {
        %creditsFile = new FileObject();
        %creditsFile.openForRead(%creditsPath);

        %author = %creditsFile.readLine();

        while (!%creditsFile.isEoF()) {
          %author = %author TAB %creditsFile.readLine();
        }

        %creditsFile.close();
        %creditsFile.delete();
        
        InstrumentsServer.newAddOn(%addon, %author);
      }
    }
  }
}

InstrumentsServer.init();
