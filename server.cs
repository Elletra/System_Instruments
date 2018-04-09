if (isFile("Add-Ons/Support_Instruments/server.cs") && $AddOn__Support_Instruments == 1) {
  error("ERROR: System_Instruments is NOT compatible with Support_Instruments!" NL
        "\nPlease disable Support_Instruments if you want to use this mod.");

  return;
}

if (!isObject(Instruments)) {
  // Shared between client and server
  exec("./common/main.cs");
}

exec("./server/main.cs");
