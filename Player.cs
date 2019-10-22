namespace external_stats_screen
{
  class Player
  {
    public Pointer Active;

    public Pointer Name;

    public Pointer StartTimeUnix;
    public Pointer LevelStartTime;

    public PlayerStats LevelStats;
    public PlayerStats GameStats;

    public Player(Pointer playerTarget)
    {
      Active = playerTarget.Adjust(0);

      Pointer playerEntity = playerTarget.Adjust(4);
      Name = playerEntity.AddOffsets(0x368, 0x0);

      StartTimeUnix = playerEntity.AddOffsets(0xC2C);
      LevelStartTime = playerEntity.AddOffsets(0xC34);

      LevelStats = new PlayerStats(playerEntity.AddOffsets(0x2B20));
      GameStats = new PlayerStats(playerEntity.AddOffsets(0x2B90));
    }

    public bool IsActive() => Active.ReadInt() != 0;

    override public string ToString()
    {
      if (IsActive())
      {
        return MainForm.DeformatCTString(Name.ReadAscii());
      }
      return "Inactive Player";
    }
  }
}
