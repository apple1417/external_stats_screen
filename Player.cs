namespace external_stats_screen {
  class Player {
    public Pointer Active;

    public Pointer Name;

    public Pointer StartTimeUnix;
    public Pointer LevelStartTime;

    public PlayerStats LevelStats;
    public PlayerStats GameStats;

    public Player(Pointer playerTarget) {
      Active = playerTarget.Adjust(0);

      Pointer playerEntity = playerTarget.Adjust(4);
      switch (MemoryManager.HookedGameVersion) {
        case GameVersion.TFE: {
          Name = playerEntity.AddOffsets(0x310, 0x0);

          StartTimeUnix = playerEntity.AddOffsets(0xAC0);
          LevelStartTime = playerEntity.AddOffsets(0xAC8);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x1240));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x1268));
          break;
        }
        case GameVersion.TSE: {
          Name = playerEntity.AddOffsets(0x310, 0x0);

          StartTimeUnix = playerEntity.AddOffsets(0xC38);
          LevelStartTime = playerEntity.AddOffsets(0xC40);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x2584));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x25AC));
          break;
        }
        case GameVersion.REVOLUTION: {
          Name = playerEntity.AddOffsets(0x368, 0x0);

          StartTimeUnix = playerEntity.AddOffsets(0xC2C);
          LevelStartTime = playerEntity.AddOffsets(0xC34);

          LevelStats = new PlayerStats(playerEntity.AddOffsets(0x2B20));
          GameStats = new PlayerStats(playerEntity.AddOffsets(0x2B90));
          break;
        }
      }
    }

    public bool IsActive() => Active.ReadInt() != 0;

    override public string ToString() {
      if (IsActive()) {
        string name = MainForm.DeformatCTString(Name.ReadAscii());
        if (name.Length == 0) {
          return "Unknown Player Name";
        }
        return name;
      }
      return "Inactive Player";
    }
  }
}
