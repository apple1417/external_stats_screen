using System;
using System.Windows.Forms;

namespace external_stats_screen {
  static class Program {
    public const string VERSION = "v1.1";
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }
}
