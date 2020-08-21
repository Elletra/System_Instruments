function serverCmdSetSongPhrase(%client, %index, %phrase, %usingGui) {
  if (!%client.hasInstrumentsClient) {
    return;
  }

  if (!InstrumentsServer.checkSongPermissions(%client, 1)) { 
    return; 
  }

  %songPhraseCap = Instruments.const["MAX_SONG_PHRASES"] - 1;

  if (!_isInt(%index)) {
    if (%usingGui) {
      Instruments.messageBoxOK("Developer Error", 
        "Something is wrong with the row index: " @ %index @
        "\n\nPlease send a screenshot of this " @
        "(along with a short explanation of what you did) to Electrk (BL_ID: 12949)", %client);

      return;
    }  

    messageClient(%client, '', "\n\c6Usage:");
    messageClient(%client, '', "\c3/setSongPhrase phraseNumber(0-" @ %songPhraseCap @ ") phrase");
    return;
  }

  if (_strEmpty(%phrase)) {
    if (%usingGui) {
      Instruments.messageBoxOK(%client, "Error", "No phrase to add!", %client);
      return;
    }

    messageClient(%client, '', "\n\c6Usage:");
    messageClient(%client, '', "\c3/setSongPhrase phraseNumber(0-" @ %songPhraseCap @ ") phrase");

    return;
  }

  if (strLen(%phrase) > Instruments.const["MAX_PHRASE_LENGTH"]) {
    messageClient(%client, '', "\c6Maximum phrase length is \c3" @ 
      Instruments.const["MAX_PHRASE_LENGTH"] @ " characters");

    return;
  }
  
  if (%index < 0 || %index > %songPhraseCap) {
    messageClient(%client, '', "\c6Valid phrase numbers: \c30-" @ %songPhraseCap);
    return;
  }

  if (strPos(%index, ".") != -1) {
    messageClient(%client, '', "\c6Valid phrase numbers: \c30-" @ %songPhraseCap);
    messageClient(%client, '', "\c6Whole numbers only (why did you even try this)");
    return;
  } 

  // Believe it or not, this actually worked
  if (striPos(%index, "e") != -1) {
    messageClient(%client, '', "\c6Valid phrase numbers: \c30-" @ %songPhraseCap);
    messageClient(%client, '', "\c6Regular notation only (what is wrong with you)");
    return;
  } 

  %client.songPhrase[%index] = %phrase;

  if (!%usingGui) {
    messageClient(%client, '', "\c6Song phrase \c3" @ %index SPC "\c6set to \c3" @ %phrase);
  }

  commandToClient(%client, 'updateSongPhrase', %index, %phrase);
}

function serverCmdInstruments_clearAllSongPhrases(%client) {
  if (!%client.hasInstrumentsClient) {
    return;
  }
  
  if (!InstrumentsServer.checkSongPermissions(%client, 0)) { 
    return; 
  }

  for (%i = 0; %i < Instruments.const["MAX_SONG_PHRASES"]; %i++) {
    %client.songPhrase[%i] = "";
  }
}
