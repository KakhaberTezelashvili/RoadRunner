namespace TDOC.Common.Utilities
{
    public static class BitUtilities
    {
        public static bool IsBitSet(byte theBit, int val) => (val & 1 << theBit) != 0;  // Delphi: result := (val and(1 shl TheBit)) <> 0;

        public static long BitOn(byte theBit, long val) => val | (long)(1 << theBit);  // Delphi: result := val or (1 shl TheBit);

        public static long BitOff(byte theBit, long val) => val & (1 << theBit ^ 0xFFFFFFFF);  // Delphi: result := val and ((1 shl TheBit) xor $FFFFFFFF);
    }
}