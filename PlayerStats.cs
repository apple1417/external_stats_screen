﻿namespace external_stats_screen
{
  class PlayerStats
  {
    public Pointer Score;
    public Pointer Kills;
    public Pointer Deaths;
    public Pointer Secrets;
    public Pointer TotalKills;
    public Pointer TotalSecrets;

    public PlayerStats(Pointer basePtr)
    {
      Score = basePtr.Adjust(0x0);
      Kills = basePtr.Adjust(0x8);
      Deaths = basePtr.Adjust(0x10);
      Secrets = basePtr.Adjust(0x20);
      TotalKills = basePtr.Adjust(0x40);
      TotalSecrets = basePtr.Adjust(0x58);
    }

    public override string ToString()
    {
      return $"{Score.ReadInt()}\n" +
             $"{Deaths.ReadInt()}\n" +
             $"{Kills.ReadInt()}/{TotalKills.ReadInt()}\n" +
             $"{Secrets.ReadInt()}/{TotalSecrets.ReadInt()}";
    }
  }
}