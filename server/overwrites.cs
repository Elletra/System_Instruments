// Overwrite old Support_Instruments functions
// We don't want to accidentally overwrite someone else's functions, so we put some checks before doing so

if (!isFunction(serverCmdPlayLoadPhrase)) {
  %playLoadPhrase = "function serverCmdPlayLoadPhrase(%client) {" NL
    "  if (%client.hasInstrumentsClient) {" NL
    "    Instruments.messageBoxOK(\"Use GUI\", \"This command no longer exists.  Please use the instruments GUI.\", %client);" NL
    "    return;" NL
    "  }" NL
    
    "  %download = \"<a:electrk.rocks/ElectrkMods/Blockland/System_Instruments.zip>download</a>\";" NL
    "  %body = \"This command no longer exists.\\n\\nPlease \" @ %download @ \" the instruments mod and use the GUI. " @
    "  \\n\\n(You will need to restart Blockland.)\";" NL
    "  Instruments.messageBoxOK(\"Download the Instruments Mod\", %body, %client);" NL
    "}";

  eval(%playLoadPhrase);

  if (!isFunction(serverCmdPlayLoadSong)) { 
    eval("function serverCmdPlayLoadSong(%client) { serverCmdPlayLoadPhrase(%client); }"); 
  }

  if (!isFunction(serverCmdPLS)) { 
    eval("function serverCmdPLS(%client) { serverCmdPlayLoadPhrase(%client); }"); 
  }

  if (!isFunction(serverCmdPLP)) { 
    eval("function serverCmdPLP(%client) { serverCmdPlayLoadPhrase(%client); }"); 
  }
}
