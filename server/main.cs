exec("./init.cs");

// Utility functions
exec("./utility/noteSounds.cs");
exec("./utility/permissions.cs");

// Main playing functions
exec("./playing/playNote.cs");
exec("./playing/playPhrase.cs");
exec("./playing/setSongPhrase.cs");
exec("./playing/playSong.cs");
exec("./playing/stopPlaying.cs");
exec("./playing/binding.cs");
exec("./playing/muting.cs");

// Saving and loading files
exec("./saveLoad/fileList.cs");
exec("./saveLoad/save.cs");
exec("./saveLoad/load.cs");
exec("./saveLoad/delete.cs");

// Instrument-related functions??  I guess?
exec("./instruments/instrumentList.cs");
exec("./instruments/setInstrument.cs");

// Packages
exec("./packages.cs");

// Overwriting old, unused instruments functions
exec("./overwrites.cs");

// This add-on uses certain functions that are apparently only defined in allClientScripts.cs
// So we have to define them ourselves if it's dedicated
if ($Server::Dedicated) {
  exec("./clientFunctions.cs");
}

// Miscellaneous
exec("./miscellaneous.cs");
