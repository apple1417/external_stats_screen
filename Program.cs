using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace external_stats_screen
{
  static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      /*Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());*/

      MemoryManager.HookGame();
      if (MemoryManager.IsGameHooked())
      {
        ProcessModule engine = null;
        foreach (ProcessModule m in MemoryManager.Game.Modules)
        {
          if (m.ModuleName == "Engine.dll")
          {
            engine = m;
            break;
          }
        }
        if (engine == null)
        {
          return;
        }

        IntPtr _pNetwork = MemoryManager.ReadPtr(SigScanner.Scan(
          engine.BaseAddress,
          engine.ModuleMemorySize,
          2,
          "8B 0D ????????",           // mov ecx,[Engine._pNetwork]
          "83 C4 08",                 // add esp,08
          "E8 ????????",              // call Engine.CNetworkLibrary::IsPaused
          "85 C0"                     // test eax,eax
        ));

        Pointer secrets = new Pointer(_pNetwork, 0x20, 0x4, 0x4, 0x2BB0);

        Debug.WriteLine(secrets.ReadInt());
      }
    }
  }
}
