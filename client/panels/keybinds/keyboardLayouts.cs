// HAHAHAHAHAHAHAHA NEVERMIND TORQUE DOESN'T SUPPORT NON-QWERTY KEYBOARDS PROPERLY


// function InstrumentsClient::populateKeyboardLayoutList(%this) {
//   InstrumentsDlg_KeyboardLayoutList.clear();

//   %path = "config/client/instruments/keyboardLayouts/*.txt";

//   for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {
//     %count = InstrumentsDlg_KeyboardLayoutList.getCount();
//     InstrumentsDlg_KeyboardLayoutList.add(fileBase(%file), %count);
//   }
// }

// function InstrumentsClient::selectKeyboardLayout(%this) {
//   %name = InstrumentsDlg_KeyboardLayoutList.getSelected();
//   %path = "config/client/instruments/keyboardLayouts/" @ %name @ ".txt";

//   if (!isFile(%path)) {
//     return;
//   }

//   %file = new FileObject();
//   %file.openForRead(%path);

//   while(!%file.isEoF()) {
//     %line = %file.readLine();

//     if (getFieldCount(%line) == 4) {
//       %row = getField(%line, 0);
//       %col = getField(%line, 1);
//       %key = getField(%line, 2);
//       %label = getField(%line, 3);

//       %keyControl = "InstrumentsDlg_Key" @ %row @ "_" @ %col;

//       if (isObject(%keyControl)) {
//         %keyControl.command = "InstrumentsClient.openKeybindDialog(\"" @ %key @ "\");";
//         %keyControl.setText(%label);
//       }
//     }
//   }

//   %file.close();
//   %file.delete();
// }
