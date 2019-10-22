using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace external_stats_screen {
  enum GameVersion {
    NONE, TFE, TSE, REVOLUTION
  }

  class MemoryManager {
    const int PROCESS_WM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    private static extern int GetLastError();

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    public static Process Game;
    public static GameVersion HookedGameVersion;
    private static IntPtr gameHandle;
    public static void HookGame() {
      GameVersion foundVersion = GameVersion.NONE;
      foreach (Process p in Process.GetProcessesByName("SeriousSam")) {
        // Set these two vars so that we can use other functions here on whatever process this is
        Game = p;
        gameHandle = OpenProcess(PROCESS_WM_READ, false, Game.Id);

        if (gameHandle == IntPtr.Zero) {
          continue;
        }

        // Check what game version we have
        foreach (ProcessModule m in Game.Modules) {
          if (m.ModuleName == "Engine.dll") {
            try {
              IntPtr addr = SigScanner.Scan(
                m.BaseAddress,
                m.ModuleMemorySize,
                6,
                "E8 ????????",              // call Engine.CON_GetBufferSize+480
                "6A ??",                    // push 07                          <--- Minor Version
                "68 ????????",              // push 00002710                    <--- Major Version
                "6A 04",                    // push 04
                "68 ????????"               // push Engine._ulEngineBuildMinor+118
              );

              if (addr != IntPtr.Zero) {
                byte minor = Read(addr, 1)[0];
                int major = ReadInt(IntPtr.Add(addr, 2));
                if (major == 10000 && minor == 5) {
                  foundVersion = GameVersion.TFE;
                } else if (major == 10000 && minor == 7) {
                  foundVersion = GameVersion.TSE;
                }
              } else {
                addr = SigScanner.Scan(
                  m.BaseAddress,
                  m.ModuleMemorySize,
                  2,
                  "FF 35 ????????",           // push [Engine._SE_VER_STRING]
                  "8D 85 30FFFFFF"            // lea eax,[ebp-000000D0]
                );

                if (addr == IntPtr.Zero) {
                  break;
                }

                string versionStr = ReadAscii(ReadPtr(ReadPtr(addr)));

                if (versionStr.StartsWith("AP_3")) {
                  foundVersion = GameVersion.REVOLUTION;
                }
              }
            } catch (Win32Exception) {}
            // Regardless of if it worked, we found what we wanted, we can stop
            break;
          }
        }

        if (foundVersion != GameVersion.NONE) {
          break;
        }
      }

      if (foundVersion == GameVersion.NONE) {
        Game = null;
      }
      HookedGameVersion = foundVersion;
    }

    public static bool IsGameHooked() {
      if (Game == null) {
        return false;
      }
      Game.Refresh();
      if (Game.HasExited) {
        Game = null;
        HookedGameVersion = GameVersion.NONE;
        return false;
      }
      return true;
    }

    public static bool IsGameRunning() {
      if (IsGameHooked()) {
        return true;
      }
      HookGame();
      return IsGameHooked();
    }

    public static byte[] Read(IntPtr addr, int len) {
      if (!IsGameHooked()) {
        throw new Win32Exception(6, "Game is not hooked!");
      }

      int lenRead = 0;
      byte[] buf = new byte[len];
      bool result = ReadProcessMemory(gameHandle.ToInt32(), addr.ToInt32(), buf, len, ref lenRead);

      if (!result) {
        throw new Win32Exception(GetLastError(), "Error reading memory");
      }

      if (lenRead == len) {
        return buf;
      }

      byte[] newBuf = new byte[lenRead];
      for (int i = 0; i < lenRead; i++) {
        newBuf[i] = buf[i];
      }
      return newBuf;
    }

    public static int ReadInt(IntPtr addr) => BitConverter.ToInt32(Read(addr, 4), 0);
    public static IntPtr ReadPtr(IntPtr addr) => new IntPtr(ReadInt(addr));
    public static float ReadFloat(IntPtr addr) => BitConverter.ToSingle(Read(addr, 4), 0);
    public static double ReadDouble(IntPtr addr) => BitConverter.ToDouble(Read(addr, 8), 0);

    private const int ASCII_BUF_SIZE = 256;
    public static string ReadAscii(IntPtr addr) {
      byte[] buf = Read(addr, ASCII_BUF_SIZE);
      StringBuilder sb = new StringBuilder();

      int i = 0;
      while (buf[i] != 0) {
        sb.Append((char) buf[i]);

        i++;
        if (i == buf.Length) {
          addr = IntPtr.Add(addr, ASCII_BUF_SIZE);
          buf = Read(addr, ASCII_BUF_SIZE);

          i = 0;
        }
      }
      return sb.ToString();
    }
  }
}
