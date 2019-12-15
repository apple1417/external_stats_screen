using MemTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace external_stats_screen {
  enum GameVersion {
    NONE, TFE, TSE, REVOLUTION
  }

  class GameHook {
    public static MemManager Manager = null;
    public static ProcessModule Engine;
    public static GameVersion CurrentVersion = GameVersion.NONE;

    public static bool IsReady => Manager != null && Manager.IsHooked && Engine != null && CurrentVersion != GameVersion.NONE;

    private static Pointer difficultyPtr;
    private static Pointer currentIGTPtr;
    private static Pointer levelNamePtr;

    public static int Difficulty {
      get {
        if (!IsReady) {
          return 0;
        }
        return Manager.Read<Int32>(difficultyPtr);
      }
    }

    public static TimeSpan IGT {
      get {
        if (!IsReady) {
          return TimeSpan.Zero;
        }
        return TimeSpan.FromSeconds(
          CurrentVersion == GameVersion.REVOLUTION
          ? Manager.Read<Double>(currentIGTPtr)
          : Manager.Read<Single>(currentIGTPtr)
        );
      }
    }

    public static string LevelName {
      get {
        if (!IsReady) {
          return "Unknown Level";
        }
        string name = DeformatCTString(Manager.ReadString(levelNamePtr, Encoding.ASCII));
        if (name.Length == 0) {
          return "Unknown level";
        }
        return name;
      }
    }

    private const int PLAYER_TARGET_SIZE = 0x88;
    public static Player[] AllPlayers;

    public static void TryHookGame() {
      foreach (Process p in Process.GetProcessesByName("SeriousSam")) {
        Manager = new MemManager(p);

        Engine = Manager.HookedProcess.Modules.Cast<ProcessModule>().Where(m => m.ModuleName == "Engine.dll").FirstOrDefault();
        if (Engine == null) {
          continue;
        }

        CurrentVersion = CheckGameVersion();
        if (CurrentVersion != GameVersion.NONE) {
          break;
        }
      }

      if (CurrentVersion == GameVersion.NONE) {
        return;
      }

      IntPtr _pNetwork = Manager.Read<IntPtr>(Manager.Read<IntPtr>(Manager.SigScan(
        Engine.BaseAddress,
        Engine.ModuleMemorySize,
        2,
        "8B 0D ????????",           // mov ecx,[Engine._pNetwork]
        "83 C4 08",                 // add esp,08
        "E8 ????????",              // call Engine.CNetworkLibrary::IsPaused
        "85 C0"                     // test eax,eax
      )));

      if (_pNetwork == IntPtr.Zero) {
        CurrentVersion = GameVersion.NONE;
        return;
      }

      if (CurrentVersion == GameVersion.REVOLUTION) {
        difficultyPtr = new Pointer(_pNetwork, 0x9C);
        currentIGTPtr = new Pointer(_pNetwork, 0x20, 0x58);
        levelNamePtr = new Pointer(_pNetwork, 0x12EC, 0x0);
      } else {
        difficultyPtr = new Pointer(_pNetwork, 0x988);
        currentIGTPtr = new Pointer(_pNetwork, 0x20, 0x38);
        levelNamePtr = new Pointer(_pNetwork, (CurrentVersion == GameVersion.TFE)
                                              ? 0x1284
                                              : 0x1288, 0x0);
      }

      int playerCount = Manager.Read<Int32>(new Pointer(_pNetwork, 0x20, 0x0));
      Pointer firstPlayer = new Pointer(_pNetwork, 0x20, 0x4, 0x0);
      AllPlayers = new Player[playerCount];
      for (int i = 0; i < playerCount; i++) {
        AllPlayers[i] = new Player(firstPlayer);
        firstPlayer = firstPlayer.Adjust(PLAYER_TARGET_SIZE);
      }
    }

    public static GameVersion CheckGameVersion() {
      if (Manager == null || !Manager.IsHooked || Engine == null) {
        return GameVersion.NONE;
      }

      try {
        IntPtr addr = Manager.SigScan(
          Engine.BaseAddress,
          Engine.ModuleMemorySize,
          6,
          "E8 ????????",              // call Engine.CON_GetBufferSize+480
          "6A ??",                    // push 07                          <--- Minor Version
          "68 ????????",              // push 00002710                    <--- Major Version
          "6A 04",                    // push 04
          "68 ????????"               // push Engine._ulEngineBuildMinor+118
        );

        if (addr != IntPtr.Zero) {
          byte minor = Manager.Read(addr, 1)[0];
          int major = Manager.Read<Int32>(IntPtr.Add(addr, 2));
          if (major == 10000 && minor == 5) {
            return GameVersion.TFE;
          } else if (major == 10000 && minor == 7) {
            return GameVersion.TSE;
          }
        }

        addr = Manager.SigScan(
          Engine.BaseAddress,
          Engine.ModuleMemorySize,
          2,
          "FF 35 ????????",           // push [Engine._SE_VER_STRING]
          "8D 85 30FFFFFF"            // lea eax,[ebp-000000D0]
        );

        if (addr == IntPtr.Zero) {
          return GameVersion.NONE;
        }

        string versionStr = Manager.ReadString(Manager.Read<IntPtr>(Manager.Read<IntPtr>(addr)), Encoding.ASCII);

        if (versionStr.StartsWith("AP_3")) {
          return GameVersion.REVOLUTION;
        }
      } catch (Win32Exception) { }

      return GameVersion.NONE;
    }

    public static string DeformatCTString(string input) {
      StringBuilder output = new StringBuilder();
      for (int i = 0; i < input.Length; i++) {
        char c = input[i];
        if (c != '^') {
          output.Append(c);
        } else {
          switch (input[i + 1]) {
            case 'c': i += 7; break;
            case 'a': i += 3; break;
            case 'f': i += 2; break;
            case 'r':
            case 'o':
            case 'b':
            case 'i':
            case 'C':
            case 'A':
            case 'B':
            case 'I':
            case 'F': i++; break;
            default: output.Append(c); break;
          }
        }
      }
      return output.ToString();
    }
  }
}
