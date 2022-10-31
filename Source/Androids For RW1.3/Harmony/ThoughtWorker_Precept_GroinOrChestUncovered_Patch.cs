﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ATReforged
{
    internal class ThoughtWorker_Precept_GroinOrChestUncovered_Patch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_GroinOrChestUncovered), "HasUncoveredGroinOrChest")]
        public class TW_Precept_GroinUncovered_HasUncoveredGroinOrChest
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref bool __result)
            {
                //Already disabled => no more processing required
                if (!__result)
                    return;

                if (Utils.IsConsideredMechanical(p) || Utils.IsSurrogate(p))
                {
                    __result = false;
                }
            }
        }
    }
}