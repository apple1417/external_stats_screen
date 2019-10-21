using System;
using System.Linq;

namespace external_stats_screen
{
  public class SigScanner
  {
    public static IntPtr Scan(IntPtr start, int len, int offset, params string[] pattern)
    {
      // Join all strings and remove whitespace
      string joinedPattern = new string(string.Join("", pattern)
                                              .Where(c => !char.IsWhiteSpace(c))
                                              .ToArray());

      // Our string should have 2 hex chars per byte
      byte[] outputPattern = new byte[joinedPattern.Length / 2];
      bool[] outputMask = new bool[joinedPattern.Length / 2];
      int lastWildCard = -1;

      for (int i = 0; i < outputPattern.Length; i++)
      {
        try
        {
          outputPattern[i] = byte.Parse(joinedPattern.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
          outputMask[i] = true;
        }
        catch (FormatException)
        {
          outputPattern[i] = 0;
          outputMask[i] = false;
          lastWildCard = i;
        }
      }

      return Scan(start, len, offset, outputPattern, outputMask);
    }

    public static IntPtr Scan(IntPtr start, int len, int offset, byte[] pattern, bool[] mask)
    {
      if (pattern.Length != mask.Length)
      {
        throw new ArgumentException("Pattern and mask must be the same length!");
      }

      byte[] memory = MemoryManager.Read(start, len);

      // Just using a naive O(nm) search, it's fast enough
      for (int i = 0; i < memory.Length - pattern.Length; i++)
      {
        bool found = true;
        for (int j = 0; j < pattern.Length; j++)
        {
          if (!mask[j])
          {
            continue;
          }
          if (memory[i + j] != pattern[j])
          {
            found = false;
            break;
          }
        }
        if (found)
        {
          return IntPtr.Add(start, i + offset);
        }
      }

      return IntPtr.Zero;
    }
  }
}
