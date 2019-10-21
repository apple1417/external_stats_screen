using System;
using System.ComponentModel;
using System.Linq;

namespace external_stats_screen
{
  class Pointer
  {
    public IntPtr BaseAddress { get; }
    public int[] Offsets { get; }
    public Pointer(IntPtr baseAddress, params int[] offsets)
    {
      BaseAddress = baseAddress;
      Offsets = offsets;
    }

    public IntPtr GetFinalAddress()
    {
      try
      {
        IntPtr ptr = MemoryManager.ReadPtr(BaseAddress);
        // Don't read the final offset, we just want the address
        for (int i = 0; i < Offsets.Length - 1; i++)
        {
          ptr = MemoryManager.ReadPtr(IntPtr.Add(ptr, Offsets[i]));
        }
        return IntPtr.Add(ptr, Offsets.Last());
      } catch (Win32Exception) {
        return IntPtr.Zero;
      }
    }

    public bool IsPointerValid() => GetFinalAddress() == IntPtr.Zero;

    public byte[] Read(int len)
    {
      IntPtr addr = GetFinalAddress();
      if (addr == IntPtr.Zero)
      {
        return new byte[0];
      }
      return MemoryManager.Read(addr, len);
    }
    public int ReadInt()
    {
      IntPtr addr = GetFinalAddress();
      if (addr == IntPtr.Zero)
      {
        return 0;
      }
      return MemoryManager.ReadInt(addr);
    }
    public IntPtr ReadPtr()
    {
      IntPtr addr = GetFinalAddress();
      if (addr == IntPtr.Zero)
      {
        return IntPtr.Zero;
      }
      return MemoryManager.ReadPtr(addr);
    }
    public string ReadAscii()
    {
      IntPtr addr = GetFinalAddress();
      if (addr == IntPtr.Zero)
      {
        return "";
      }
      return MemoryManager.ReadAscii(addr);
    }
  }
}
