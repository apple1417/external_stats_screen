using MemTools;
using System;
using System.ComponentModel;

namespace external_stats_screen {
  class Player {
    private readonly Pointer activePtr;
    public bool IsActive {
      get {
        if (!GameHook.IsReady) {
          return false;
        }
        return GameHook.Manager.ReadInt32(activePtr) != 0;
      }
    }

    private readonly Pointer namePtr;
    public string Name => ToString();

    private readonly Pointer startTimeUnixPtr;
    public DateTime UnixStartTime {
      get {
        if (!GameHook.IsReady) {
          return DateTime.MaxValue;
        }
        int startTime = GameHook.Manager.ReadInt32(startTimeUnixPtr);
        if (startTime > 0) {
          return DateTimeOffset.FromUnixTimeSeconds(startTime).LocalDateTime;
        }
        return DateTime.MaxValue;
      }
    }

    private readonly Pointer levelStartTimePtr;
    public float LevelStartTime {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadFloat(levelStartTimePtr);
      }
    }

    public readonly PlayerStats LevelStats;
    public readonly PlayerStats GameStats;

    public Player(Pointer playerTarget) {
      activePtr = playerTarget.Clone();

      Pointer playerEntity = playerTarget.Adjust(4);
      switch (GameHook.CurrentVersion) {
        case GameVersion.TFE: {
          namePtr = playerEntity.AddOffsets(0x310, 0x0);

          startTimeUnixPtr = playerEntity.AddOffsets(0xAC0);
          levelStartTimePtr = playerEntity.AddOffsets(0xAC8);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x1240));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x1268));
          break;
        }
        case GameVersion.TSE: {
          namePtr = playerEntity.AddOffsets(0x310, 0x0);

          startTimeUnixPtr = playerEntity.AddOffsets(0xC38);
          levelStartTimePtr = playerEntity.AddOffsets(0xC40);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x2584));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x25AC));
          break;
        }
        case GameVersion.REVOLUTION: {
          namePtr = playerEntity.AddOffsets(0x368, 0x0);

          startTimeUnixPtr = playerEntity.AddOffsets(0xC2C);
          levelStartTimePtr = playerEntity.AddOffsets(0xC34);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x2B20));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x2B90));
          break;
        }
      }
    }

    override public string ToString() {
      if (!GameHook.Manager.IsHooked) {
        return "Unhooked Player";
      }


      if (IsActive) {
        try {
          string name = GameHook.DeformatCTString(GameHook.Manager.ReadAscii(namePtr));
          if (name.Length > 0) {
            return name;
          }

          // Even though the stack trace goes through MainForm.UpdateSafe(), which already catches this,
          //  we also have to so that the external code part of forms can properly call this
        } catch (Win32Exception) { }

        return "Unknown Player Name";
      }

      return "Inactive Player";
    }
  }
}
