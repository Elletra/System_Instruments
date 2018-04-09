function InstrumentsClient::addHelpCategory(%this, %category) {
  %path = "Add-Ons/System_Instruments/client/panels/help/categories/" @ %category @ ".txt";

  if (!isFile(%path)) {
    error("ERROR: InstrumentsClient::addHelpCategory() - Help file does not exist for \"" @ %category @ "\"");
    return;
  }

  %number = getWord(%category, 0);
  %name = getWords(%category, 1);

  if (stripChars(%number, ".0123456789") !$= "") {
    %number = "\t" @ %number;
  }

  %name = "\t" @ %name;
  %index = InstrumentsDlg_HelpCategoriesList.rowCount();

  InstrumentsDlg_HelpCategoriesList.rowFileName[%index] = %category;
  InstrumentsDlg_HelpCategoriesList.addRow(%index, %number TAB %name);
}

function InstrumentsClient::clickHelpCategory(%this) {
  %categoryID = InstrumentsDlg_HelpCategoriesList.getSelectedID();
  %category = InstrumentsDlg_HelpCategoriesList.rowFileName[%categoryID];

  %path = "Add-Ons/System_Instruments/client/panels/help/categories/" @ %category @ ".txt";

  if (!isFile(%path)) {
    error("ERROR: InstrumentsClient::clickHelpCategory() - Help file does not exist for \"" @ %category @ "\"");
    return;
  }

  %helpText = "<lmargin%:5><rmargin%:95><tab:50,75>\n";

  %file = new FileObject();
  %file.openForRead(%path);

  while (!%file.isEoF()) {
    %helpText = %helpText @ %file.readLine() @ "\n";
  }

  %file.close();
  %file.delete();

  if (%category $= "1. About") {
    %count = InstrumentsClient.addOnCount - 1;

    for (%i = %count; %i >= 0; %i--) {
      %addon = InstrumentsClient.addOn(%i);
      %authors = strReplace(InstrumentsClient.addOnAuthor(%addon), "\t", "\n");

      %helpText = %helpText NL "<lmargin%:8>" NL
        "<font:Arial Bold:18>" @ %addon @ "<font:Arial:8>" NL
        "<lmargin%:10>" NL
        "<font:Arial:16>" @ %authors @ "\n";
    }

    %helpText = %helpText NL "<lmargin%:6>" NL
      "<font:Arial Bold:24>Special Thanks To:<font:Arial:12>" NL
      "<lmargin%:8>" NL
      "<font:Arial:17>Plastiware (1118)" NL
      "Emil (47430)" NL
      "Shock (636)" NL
      "Conan (4928)" NL
      "Metario (51892)" NL
      "Skill4Life (4382)" NL
      "Trogtor (23897)" NL
      "Jam Jar (19091)";
  }

  // Handling constants -- it's fucky, but it'll do
  %constCount = strCount(%helpText, "CONST_");

  for (%i = 0; %i < %constCount; %i++) {
    %firstOccurrence = strPos(%helpText, "CONST_");

    if (%firstOccurrence < 0) {
      continue;
    }

    // Get everything at and after the first occurrence of a "CONST_" string
    %substring = getSubStr(%helpText, %firstOccurrence, strLen(%helpText));

    // String with "CONST_" prefix
    %constStr = getWord(%substring, 0); 
    %constStr = keepChars(%constStr, "ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");

    // String without "CONST_" prefix
    %const = getSubStr(%constStr, 6, strLen(%constStr));  

    %helpText = strReplace(%helpText, %constStr, Instruments.const[%const]);
  }

  // I'm mature
  %helpText = strReplace(%helpText, "Vitawrap", "Vitawrap <bitmap:Add-Ons/System_Instruments/client/images/flag_fr.png>");

  InstrumentsDlg_HelpText.setText(collapseEscape(%helpText @ "\n"));
}

function InstrumentsClient::populateHelpCategoriesList(%this) {
  %selected = InstrumentsDlg_HelpCategoriesList.getSelectedID();

  InstrumentsDlg_HelpCategoriesList.clear();
  InstrumentsDlg_HelpText.setText("");

  %categoryCount = 0;
  %path = "Add-Ons/System_Instruments/client/panels/help/categories/*.txt";

  for (%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path)) {
    %category[%categoryCount] = fileBase(%file);
    %categoryCount++;
  }

  for (%i = %categoryCount - 1; %i >= 0; %i--) {
    InstrumentsClient.addHelpCategory(%category[%i]);
  }

  if (%selected < 0) {
    %selected = 0;
  }

  InstrumentsDlg_HelpCategoriesList.setSelectedByID(%selected);
}
