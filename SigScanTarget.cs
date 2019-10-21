using System;
using System.Linq;

namespace external_stats_screen
{
  class SigScanTarget
  {
    public int Offset { get; set; }
    public byte[] Pattern { get; private set; }
    public bool[] Mask { get; private set; }
    public int LastWildCard { get; private set; };

    public void SetPattern(byte[] pattern, bool[] mask)
    {
      if (pattern.Length != mask.Length)
      {
        throw new ArgumentException("Pattern and mask must be the same length!");
      }
      Pattern = pattern;
      Mask = mask;

      LastWildCard = -1;
      for (int i = Mask.Length; i >= 0; i--)
      {
        if (!Mask[i])
        {
          LastWildCard = i;
          break;
        }
      }
    }

    public SigScanTarget(int offset, byte[] pattern, bool[] mask)
    {
      Offset = offset;
      SetPattern(pattern, mask);
    }

    public SigScanTarget(int offset, params string[] pattern)
    {
      Offset = offset;
      // Join all strings and remove whitespace
      string joinedPattern = new string(string.Join("", pattern)
                                              .Where(c => !char.IsWhiteSpace(c))
                                              .ToArray());

      Pattern = new byte[joinedPattern.Length / 2];
      Mask = new bool[joinedPattern.Length / 2];
      LastWildCard = -1;

      for (int i = 0; i < joinedPattern.Length/2; i++)
      {
        try
        {
          Pattern[i] = byte.Parse(joinedPattern.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber); ;
          Mask[i] = true;
        } catch (FormatException) {
          Pattern[i] = 0;
          Mask[i] = false;
          LastWildCard = i;
        }
      }
    }
  }
}
