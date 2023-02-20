using System;

namespace LewdableTanks
{
  [Flags]
  public enum DInfo
  {
    NONE = 0,
    General = 1,
    Salvage = 2,
    Sort = 4,
    AutoFix = 8,
    Death = 16, // 0x00000010
    Assembly = 32, // 0x00000020
    Debug = 16777216, // 0x01000000
    ALL = 2147483647, // 0x7FFFFFFF
  }
}
