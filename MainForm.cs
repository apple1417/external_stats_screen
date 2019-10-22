using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace external_stats_screen
{
  public partial class MainForm : Form
  {
    private static readonly int[] UPDATE_OPTIONS = new int[] {
      1, 5, 17, 50, 100, 500, 1000
    };
    private int UPDATE_MS = UPDATE_OPTIONS[3];
    private Timer UpdateTimer;
    private ContextMenuStrip UpdateCMS;

    //private ContextMenuStrip PollingContextMenu;
    public MainForm()
    {
      InitializeComponent();
      PlayerBox.SelectedIndex = 0;

      UpdateCMS = new ContextMenuStrip();

      UpdateCMS.Items.Add("Polling Rate (ms)");
      ToolStripMenuItem dropDown = (ToolStripMenuItem)UpdateCMS.Items[0];
      foreach (int option in UPDATE_OPTIONS)
      {
        ToolStripMenuItem sub = new ToolStripMenuItem(option.ToString());
        sub.CheckOnClick = true;
        sub.Click += new EventHandler(ChangeUpdateRate);
        if (option == UPDATE_MS)
        {
          sub.Checked = true;
        }
        dropDown.DropDownItems.Add(sub);
      }

      ContextMenuStrip = UpdateCMS;

      SetupPointers();

      UpdateTimer = new Timer();
      UpdateTimer.Interval = UPDATE_MS;
      UpdateTimer.Tick += new EventHandler(Update);
      UpdateTimer.Start();
    }

    private void ChangeUpdateRate(object myObject, EventArgs myEventArgs)
    {
      ToolStripMenuItem dropDown = (ToolStripMenuItem)UpdateCMS.Items[0];
      for (int i = 0; i < dropDown.DropDownItems.Count; i++)
      {
        ToolStripMenuItem item = (ToolStripMenuItem) dropDown.DropDownItems[i];
        if (item.Equals(myObject))
        {
          UPDATE_MS = UPDATE_OPTIONS[i];
        }
        else
        {
          item.Checked = false;
        }
      }
      UpdateTimer.Stop();
      UpdateTimer.Interval = UPDATE_MS;
      UpdateTimer.Start();
    }

    private Pointer Difficulty;
    private Pointer CurrentTime;
    private Pointer LevelNamePtr;

    private const int PLAYER_TARGET_SIZE = 0x88;
    private Player[] AllPlayers;

    public void SetupPointers()
    {
      if (!MemoryManager.IsGameRunning())
      {
        return;
      }

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

      IntPtr _pNetwork = MemoryManager.ReadPtr(MemoryManager.ReadPtr(SigScanner.Scan(
        engine.BaseAddress,
        engine.ModuleMemorySize,
        2,
        "8B 0D ????????",           // mov ecx,[Engine._pNetwork]
        "83 C4 08",                 // add esp,08
        "E8 ????????",              // call Engine.CNetworkLibrary::IsPaused
        "85 C0"                     // test eax,eax
      )));

      Difficulty = new Pointer(_pNetwork, 0x9C);
      CurrentTime = new Pointer(_pNetwork, 0x20, 0x58);
      LevelNamePtr = new Pointer(_pNetwork, 0x12EC, 0x0);

      int playerCount = new Pointer(_pNetwork, 0x20, 0x0).ReadInt();
      Pointer currentPlayer = new Pointer(_pNetwork, 0x20, 0x4, 0x0);

      AllPlayers = new Player[playerCount];
      for (int i = 0; i < playerCount; i++)
      {
        AllPlayers[i] = new Player(currentPlayer);
        currentPlayer = currentPlayer.Adjust(PLAYER_TARGET_SIZE);
      }
    }

    public void Update(object myObject, EventArgs myEventArgs)
    {
      if (!MemoryManager.IsGameHooked())
      {
        SetupPointers();
      }
      if (MemoryManager.IsGameHooked())
      {
        Title.Text = "External Stats Screen - Hooked";
      } else
      {
        Title.Text = "External Stats Screen - Not Hooked";
        return;
      }

      string levelNameStr = DeformatCTString(LevelNamePtr.ReadAscii());
      if (levelNameStr.Length == 0) {
        LevelName.Text = "Unknown Level";
      } else
      {
        LevelName.Text = levelNameStr;
      }

      foreach (Player p in AllPlayers)
      {
        if (p.IsActive() && !PlayerBox.Items.Contains(p))
        {
          PlayerBox.Items.Add(p);
        }
        else if (!p.IsActive() && PlayerBox.Items.Contains(p))
        {
          PlayerBox.Items.Remove(p);
          if (PlayerBox.SelectedItem == p)
          {
            PlayerBox.SelectedIndex = 0;
          }
        }
      }

      int totalScore = 0;
      int startTimeUnix = -1;
      float levelStartTime = -1.0f;
      foreach (Player p in AllPlayers)
      {
        if (p.IsActive())
        {
          totalScore += p.GameStats.Score.ReadInt();

          int playerStartTime = p.StartTimeUnix.ReadInt();
          float playerLevelStartTime = p.LevelStartTime.ReadFloat();
          if (playerStartTime < startTimeUnix || startTimeUnix < 0)
          {
            startTimeUnix = playerStartTime;
          }
          if (playerLevelStartTime < levelStartTime || levelStartTime < 0)
          {
            levelStartTime = playerLevelStartTime;
          }
        }
      }

      DateTime startTime = DateTimeOffset.FromUnixTimeSeconds(startTimeUnix).LocalDateTime;
      TimeSpan realTime = DateTime.Now - startTime;
      TimeSpan gameIGT = TimeSpan.FromSeconds(CurrentTime.ReadDouble());
      TimeSpan levelIGT = gameIGT.Subtract(TimeSpan.FromSeconds(levelStartTime));
      string startTimeStr;
      string realTimeStr;
      string gameIGTStr;
      string levelIGTStr;
      if (startTimeUnix > 0 && levelStartTime > 0)
      {
        startTimeStr = startTime.ToString("ddd yyyy-MM-dd HH:mm");
        realTimeStr = realTime.ToString("hh\\:mm\\:ss");
        gameIGTStr = gameIGT.ToString("hh\\:mm\\:ss");
        levelIGTStr = levelIGT.ToString("hh\\:mm\\:ss");
      } else {
        startTimeStr = "Not Started";
        realTimeStr = "00:00:00";
        gameIGTStr = "00:00:00";
        levelIGTStr = "00:00:00";
      }

      if (PlayerBox.SelectedItem.Equals("Squad"))
      {
        int score = 0, gameScore = 0;
        int deaths = 0, gameDeaths = 0;
        int kills = 0, gameKills = 0;
        int secrets = 0, gameSecrets = 0;
        int totalKills = 0, gameTotalKills = 0;
        int totalSecrets = 0, gameTotalSecrets = 0;

        foreach (Player p in AllPlayers)
        {
          if (p.IsActive())
          {
            score += p.LevelStats.Score.ReadInt();
            gameScore += p.GameStats.Score.ReadInt();
            deaths += p.LevelStats.Deaths.ReadInt();
            gameDeaths += p.GameStats.Deaths.ReadInt();
            kills += p.LevelStats.Kills.ReadInt();
            gameKills += p.GameStats.Kills.ReadInt();
            secrets += p.LevelStats.Secrets.ReadInt();
            gameSecrets += p.GameStats.Secrets.ReadInt();

            int tmp = p.LevelStats.TotalKills.ReadInt();
            if (tmp > totalKills)
            {
              totalKills = tmp;
            }
            tmp = p.GameStats.TotalKills.ReadInt();
            if (tmp > gameTotalKills)
            {
              gameTotalKills = tmp;
            }
            tmp = p.LevelStats.TotalSecrets.ReadInt();
            if (tmp > totalSecrets)
            {
              totalSecrets = tmp;
            }
            tmp = p.GameStats.TotalSecrets.ReadInt();
            if (tmp > gameTotalSecrets)
            {
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
      }
      else
      {
        Player p = (Player)PlayerBox.SelectedItem;
        LevelStats.Text = p.LevelStats.ToString() + levelIGTStr;
        GameStats.Text = p.GameStats.ToString() + gameIGTStr;
      }

      string diffStr;
      switch (Difficulty.ReadInt())
      {
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
    }

    public static string DeformatCTString(string input)
    {
      StringBuilder output = new StringBuilder();
      for (int i = 0; i < input.Length; i++)
      {
        char c = input[i];
        if (c != '^') {
          output.Append(c);
        } else {
          switch (input[i + 1])
          {
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
