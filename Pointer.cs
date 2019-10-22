using System;
using System.ComponentModel;
using System.Linq;

namespace external_stats_screen
{
  class Pointer
  {
    public IntPtr BaseAddress { get; private set;  }
    public int[] Offsets { get; private set; }
    public Pointer(IntPtr baseAddress, params int[] offsets)
    {
      BaseAddress = baseAddress;
      Offsets = offsets;
    }

    public Pointer AddOffsets(params int[] offsets) => new Pointer(BaseAddress, Offsets.Concat(offsets).ToArray());
    public Pointer Adjust(int amount)
    {
      int[] newOffsets = (int[]) Offsets.Clone();
      newOffsets[newOffsets.Length - 1] += amount;
      return new Pointer(BaseAddress, newOffsets);
    }

    public IntPtr GetFinalAddress()
    {
      try
      {
        IntPtr ptr = BaseAddress;
        // Don't read the final offset, we just want the address
        for (int i = 0; i < Offsets.Length - 1; i++)
        {
          ptr = IntPtr.Add(ptr, Offsets[i]);
          ptr = MemoryManager.ReadPtr(ptr);
        }
        return IntPtr.Add(ptr, Offsets.Last());
      } catch (Win32Exception) {
        return IntPtr.Zero;
      }
    }

    public byte[] Read(int len)
    {
      try
      {
        return MemoryManager.Read(GetFinalAddress(), len);
      }
      catch (Win32Exception)
      {
        return new byte[0];
      }
      
    }
    public int ReadInt()
    {
      try
      {
        return MemoryManager.ReadInt(GetFinalAddress());
      }
      catch (Win32Exception)
      {
        return 0;
      }
    }
    public IntPtr ReadPtr()
    {
      try
      {
        return MemoryManager.ReadPtr(GetFinalAddress());
      }
      catch (Win32Exception)
      {
        return IntPtr.Zero;
      }
    }
    public float ReadFloat()
    {
      try
      {
        return MemoryManager.ReadFloat(GetFinalAddress());
      }
      catch (Win32Exception)
      {
        return 0.0f;
      }
    }
    public double ReadDouble()
    {
      try
      {
        return MemoryManager.ReadDouble(GetFinalAddress());
      }
      catch (Win32Exception)
      {
        return 0.0d;
      }
    }
    public string ReadAscii()
    {
      try
      {
        return MemoryManager.ReadAscii(GetFinalAddress());
      }
      catch (Win32Exception)
      {
        return "";
      }
    }
  }
}
