using MemTools;

namespace external_stats_screen {
  class PlayerStats {
    private readonly Pointer scorePtr;
    public int Score {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(scorePtr);
      }
    }

    private readonly Pointer killsPtr;
    public int Kills {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(killsPtr);
      }
    }

    private readonly Pointer deathsPtr;
    public int Deaths {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(deathsPtr);
      }
    }

    private readonly Pointer secretsPtr;
    public int Secrets {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(secretsPtr);
      }
    }


    private readonly Pointer totalKillsPtr;
    public int TotalKills {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(totalKillsPtr);
      }
    }

    private readonly Pointer totalSecretsPtr;
    public int TotalSecrets {
      get {
        if (!GameHook.IsReady) {
          return 0;
        }
        return GameHook.Manager.ReadInt32(totalSecretsPtr);
      }
    }

    public PlayerStats(Pointer basePtr) {
      if (GameHook.CurrentVersion == GameVersion.REVOLUTION) {
        scorePtr =          basePtr.Adjust( 0x0);
        killsPtr =          basePtr.Adjust( 0x8);
        deathsPtr =         basePtr.Adjust(0x10);
        secretsPtr =        basePtr.Adjust(0x20);
        totalKillsPtr =     basePtr.Adjust(0x40);
        totalSecretsPtr =   basePtr.Adjust(0x58);
      } else {
        scorePtr =          basePtr.Adjust( 0x0);
        killsPtr =          basePtr.Adjust( 0x4);
        deathsPtr =         basePtr.Adjust( 0x8);
        secretsPtr =        basePtr.Adjust( 0xC);
        totalKillsPtr =     basePtr.Adjust(0x18);
        totalSecretsPtr =   basePtr.Adjust(0x20);
      }
    }

    public override string ToString() {
      return $"{Score}\n" +
             $"{Deaths}\n" +
             $"{Kills}/{TotalKills}\n" +
             $"{Secrets}/{TotalSecrets}\n";
    }
  }
}
