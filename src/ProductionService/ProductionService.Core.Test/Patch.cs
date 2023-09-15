using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ProductionService.Core.Test
{
    [Harmony]
    internal class Patch
    {
        // Patches DateTime.Now to return "2021.1.1" everytime.
        private static MethodBase TargetMethod() => AccessTools.Property(typeof(DateTime), nameof(DateTime.Now)).GetMethod;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            return new[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch), nameof(TestDateTime))),
                new CodeInstruction(OpCodes.Ret)
            };
        }

        public static DateTime TestDateTime() => new DateTime(2021, 1, 1);
    }
}
