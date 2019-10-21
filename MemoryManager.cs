using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace external_stats_screen
{
  class MemoryManager
  {
    const int PROCESS_WM_READ = 0x0010;

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
        Game = p;
        gameHandle = OpenProcess(PROCESS_WM_READ, false, Game.Id);

        // TODO: Check game version
        found = true;
        break;
      }
      if (!found) {
        Game = null;
      }
    }

    public static bool IsGameHooked() => Game != null;

    public static bool IsGameRunning()
    {
      HookGame();
      return IsGameHooked();
    }

    public static byte[] Read(IntPtr addr, int len)
    {
      if (!IsGameHooked())
      {
        throw new InvalidOperationException("Game is not hooked!");
      }

      int lenRead = 0;
      byte[] buf = new byte[len];
      ReadProcessMemory((int)gameHandle, (int)addr, buf, 4, ref lenRead);

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
  }
}
