using MemTools;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace external_stats_screen {
  public partial class MainForm : Form {
    private static readonly int[] UPDATE_OPTIONS = new int[] {
      1, 5, 17, 50, 100, 500, 1000
    };
    private int UPDATE_MS = UPDATE_OPTIONS[3];
    private readonly Timer UpdateTimer;
    private readonly ContextMenuStrip UpdateCMS;

    public MainForm() {
      InitializeComponent();
      PlayerBox.SelectedIndex = 0;

      UpdateCMS = new ContextMenuStrip();

      UpdateCMS.Items.Add("Polling Rate (ms)");
      ToolStripMenuItem dropDown = (ToolStripMenuItem) UpdateCMS.Items[0];
      foreach (int option in UPDATE_OPTIONS) {
        ToolStripMenuItem sub = new ToolStripMenuItem(option.ToString()) {
          CheckOnClick = true
        };
        sub.Click += new EventHandler(ChangeUpdateRate);
        if (option == UPDATE_MS) {
          sub.Checked = true;
        }
        dropDown.DropDownItems.Add(sub);
      }

      ContextMenuStrip = UpdateCMS;

      GameHook.TryHookGame();

      UpdateTimer = new Timer {
        Interval = UPDATE_MS
      };
      UpdateTimer.Tick += new EventHandler(UpdateSafe);
      UpdateTimer.Start();
    }

    private void ChangeUpdateRate(object myObject, EventArgs myEventArgs) {
      ToolStripMenuItem dropDown = (ToolStripMenuItem) UpdateCMS.Items[0];
      for (int i = 0; i < dropDown.DropDownItems.Count; i++) {
        ToolStripMenuItem item = (ToolStripMenuItem) dropDown.DropDownItems[i];
        if (item.Equals(myObject)) {
          UPDATE_MS = UPDATE_OPTIONS[i];
        } else {
          item.Checked = false;
        }
      }
      UpdateTimer.Stop();
      UpdateTimer.Interval = UPDATE_MS;
      UpdateTimer.Start();
    }

    public void UpdateSafe(object myObject, EventArgs myEventArgs) {
      try {
        UpdateForm();
      } catch (Win32Exception e) {
        // Allow ERROR_PARTIAL_COPY incase the game stopped midway through
        // "Only part of a ReadProcessMemory or WriteProcessMemory request was completed."
        if (e.NativeErrorCode != MemManager.ERROR_PARTIAL_COPY) {
          throw e;
        }
      }
    }

    public void UpdateForm() {
      if (!GameHook.IsReady) {
        GameHook.TryHookGame();

        if (!GameHook.IsReady) {
          Title.Text = "External Stats Screen - Not Hooked";
          return;
        }
      }

      Title.Text = "External Stats Screen - Hooked";
      LevelName.Text = GameHook.LevelName;

      // Refresh the player list
      foreach (Player p in GameHook.AllPlayers) {
        if (p.IsActive) {
          if (PlayerBox.Items.Contains(p)) {
            // Replace the player so that the name updates
            PlayerBox.Items[PlayerBox.Items.IndexOf(p)] = p;
          } else {
            PlayerBox.Items.Add(p);
          }
        } else if (!p.IsActive && PlayerBox.Items.Contains(p)) {
          if (PlayerBox.SelectedItem == p) {
            PlayerBox.SelectedItem = PlayerBox.Items[0];
          }
          PlayerBox.Items.Remove(p);
        }
      }

      // The top part of the gui
      int totalScore = 0;
      DateTime startTimeUnix = DateTime.MaxValue;
      float levelStartTime = -1.0f;
      foreach (Player p in GameHook.AllPlayers) {
        if (p.IsActive) {
          totalScore += p.GameStats.Score;

          DateTime playerStartTime = p.UnixStartTime;
          float playerLevelStartTime = p.LevelStartTime;
          if (playerStartTime < startTimeUnix) {
            startTimeUnix = playerStartTime;
          }
          if (playerLevelStartTime < levelStartTime || levelStartTime < 0) {
            levelStartTime = playerLevelStartTime;
          }
        }
      }

      TimeSpan realTime = DateTime.Now - startTimeUnix;
      TimeSpan gameIGT, levelIGT;
      try {
        gameIGT = GameHook.IGT;
        levelIGT = gameIGT.Subtract(TimeSpan.FromSeconds(levelStartTime));
      // While the game starts/stops these can have invalid values - NaNs or values overflowing the timespan
      } catch (Exception e) when (e is ArgumentException || e is OverflowException) {
        gameIGT = TimeSpan.Zero;
        levelIGT = TimeSpan.Zero;
      }

      string startTimeStr;
      string realTimeStr;
      string gameIGTStr;
      string levelIGTStr;
      if (startTimeUnix != DateTime.MaxValue && levelStartTime > 0) {
        startTimeStr = startTimeUnix.ToString("ddd yyyy-MM-dd HH:mm");
        realTimeStr = realTime.ToString("hh\\:mm\\:ss");
        gameIGTStr = gameIGT.ToString("hh\\:mm\\:ss");
        levelIGTStr = levelIGT.ToString("hh\\:mm\\:ss");
      } else {
        startTimeStr = "Not Started";
        realTimeStr = "00:00:00";
        gameIGTStr = "00:00:00";
        levelIGTStr = "00:00:00";
      }

      string diffStr;
      switch (GameHook.Difficulty) {
        case -1: diffStr = "Tourist"; break;
        case 0: diffStr = "Easy"; break;
        default:
        case 1: diffStr = "Normal"; break;
        case 2: diffStr = "Hard"; break;
        case 3: diffStr = "Serious"; break;
        case 4: diffStr = "Mental"; break;
      }

      GeneralStats.Text = $"{totalScore}\n" +
                          $"{diffStr}\n" +
                          $"{startTimeStr}\n" +
                          $"{realTimeStr}\n";


      // Per player/squad stats
      if (PlayerBox.SelectedItem.Equals("Squad")) {
        int score = 0, gameScore = 0;
        int deaths = 0, gameDeaths = 0;
        int kills = 0, gameKills = 0;
        int secrets = 0, gameSecrets = 0;
        int totalKills = 0, gameTotalKills = 0;
        int totalSecrets = 0, gameTotalSecrets = 0;

        foreach (Player p in GameHook.AllPlayers) {
          if (p.IsActive) {
            score += p.LevelStats.Score;
            gameScore += p.GameStats.Score;
            deaths += p.LevelStats.Deaths;
            gameDeaths += p.GameStats.Deaths;
            kills += p.LevelStats.Kills;
            gameKills += p.GameStats.Kills;
            secrets += p.LevelStats.Secrets;
            gameSecrets += p.GameStats.Secrets;

            int tmp = p.LevelStats.TotalKills;
            if (tmp > totalKills) {
              totalKills = tmp;
            }
            tmp = p.GameStats.TotalKills;
            if (tmp > gameTotalKills) {
              gameTotalKills = tmp;
            }
            tmp = p.LevelStats.TotalSecrets;
            if (tmp > totalSecrets) {
              totalSecrets = tmp;
            }
            tmp = p.GameStats.TotalSecrets;
            if (tmp > gameTotalSecrets) {
              gameTotalSecrets = tmp;
            }
          }
        }

        LevelStats.Text = $"{score}\n" +
                          $"{deaths}\n" +
                          $"{kills}/{totalKills}\n" +
                          $"{secrets}/{totalSecrets}\n" +
                          $"{levelIGTStr}";
        GameStats.Text = $"{gameScore}\n" +
                         $"{gameDeaths}\n" +
                         $"{gameKills}/{gameTotalKills}\n" +
                         $"{gameSecrets}/{gameTotalSecrets}\n" +
                         $"{gameIGTStr}";
      } else {
        Player p = (Player) PlayerBox.SelectedItem;
        LevelStats.Text = p.LevelStats.ToString() + levelIGTStr;
        GameStats.Text = p.GameStats.ToString() + gameIGTStr;
      }
    }
  }
}
