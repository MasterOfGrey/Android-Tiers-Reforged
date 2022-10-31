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
    internal class Corpse_Patch
    {

        // No one is bothered by seeing a destroyed mechanical chassis. It doesn't rot, decay, or deteriorate significantly. It may not even have had an intelligence when destroyed.
        [HarmonyPatch(typeof(Corpse), "GiveObservedThought")]
        public class GiveObservedThought_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref Thought_Memory __result)
            {
                if (Utils.IsConsideredMechanical(__instance.InnerPawn))
                {
                    __result = null;
                }
            }
        }
    }
}