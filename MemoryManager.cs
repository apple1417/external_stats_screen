using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace external_stats_screen
{
  class MemoryManager
  {
    const int PROCESS_WM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    private static extern int GetLastError();

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    public static Process Game;
    private static IntPtr gameHandle;
    public static void HookGame()
    {
      bool found = false;
      foreach (Process p in Process.GetProcessesByName("SeriousSam"))
      {
        // Set these two vars so that we can use other functions here on whatever process this is
        Game = p;
        gameHandle = OpenProcess(PROCESS_WM_READ, false, Game.Id);

        if (gameHandle == IntPtr.Zero)
        {
          continue;
        }

        // Verify we have the right process
        foreach (ProcessModule m in Game.Modules)
        {
          if (m.ModuleName == "Engine.dll")
          {
            IntPtr addr;
            try
            {
              addr = SigScanner.Scan(
                m.BaseAddress,
                m.ModuleMemorySize,
                2,
                "FF 35 ????????",           // push [Engine._SE_VER_STRING]
                "8D 85 30FFFFFF"            // lea eax,[ebp-000000D0]
              );
            } catch (Win32Exception)
            {
              break;
            }

            if (addr == IntPtr.Zero)
            {
              break;
            }

            try
            {
              string versionStr = ReadAscii(ReadPtr(ReadPtr(addr)));
              
              if (versionStr == "AP_3381")
              {
                found = true;
              }
            } catch (Win32Exception) {}

            break;
          }
        }

        if (found) {
          break;
        }
      }
      if (!found) {
        Game = null;
      }
    }

    public static bool IsGameHooked() {
      if (Game == null)
      {
        return false;
      }
      Game.Refresh();
      if (Game.HasExited)
      {
        Game = null;
        return false;
      }
      return true;
    }
    
    public static bool IsGameRunning()
    {
      if (IsGameHooked())
      {
        return true;
      }
      HookGame();
      return IsGameHooked();
    }

    public static byte[] Read(IntPtr addr, int len)
    {
      if (!IsGameHooked())
      {
        throw new Win32Exception(6, "Game is not hooked!");
      }

      int lenRead = 0;
      byte[] buf = new byte[len];
      bool result = ReadProcessMemory(gameHandle.ToInt32(), addr.ToInt32(), buf, len, ref lenRead);

      if (!result)
      {
        throw new Win32Exception(GetLastError(), "Error reading memory");
      }

      if (lenRead == len)
      {
        return buf;
      }

      byte[] newBuf = new byte[lenRead];
      for (int i = 0; i < lenRead; i++)
      {
        newBuf[i] = buf[i];
      }
      return newBuf;
    }

    public static int ReadInt(IntPtr addr) => BitConverter.ToInt32(Read(addr, 4), 0);
    public static IntPtr ReadPtr(IntPtr addr) => new IntPtr(ReadInt(addr));
    public static float ReadFloat(IntPtr addr) => BitConverter.ToSingle(Read(addr, 4), 0);
    public static double ReadDouble(IntPtr addr) => BitConverter.ToDouble(Read(addr, 8), 0);

    private const int ASCII_BUF_SIZE = 256;
    public static string ReadAscii(IntPtr addr)
    {
      byte[] buf = Read(addr, ASCII_BUF_SIZE);
      StringBuilder sb = new StringBuilder();

      int i = 0;
      while (buf[i] != 0)
      {
        sb.Append((char) buf[i]);

        i++;
        if (i == buf.Length)
        {
          addr = IntPtr.Add(addr, ASCII_BUF_SIZE);
          buf = Read(addr, ASCII_BUF_SIZE);

          i = 0;
        } 
      }
      return sb.ToString();
    }
  }
}
