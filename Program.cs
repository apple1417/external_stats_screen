using System;
using System.Windows.Forms;

namespace external_stats_screen {
  static class Program {
    [STAThread]
    static void Main() {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}
