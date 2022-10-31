﻿using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace ATReforged
{
    public class Recipe_InstallOrganicSkyMindInterface : Recipe_InstallImplant
    {
        // This recipe is specifically targetting organic brains, so we only need to check if the brain is available (a slight optimization over checking fixed body parts).
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        { 
            BodyPartRecord targetBodyPart = pawn.health.hediffSet.GetBrain();
            if (targetBodyPart != null && !Utils.IsConsideredMechanical(pawn))
            {
                yield return targetBodyPart;
            }
            yield break;
        }

        // Install the part as normal, and then handle which type of chip was installed if it was successful (which can be measured by seeing if it actually got the hediff or not).
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        { 
            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

            // If the pawn doesn't have any non-isolated core hediff, the operation failed. Also failed if they're dead now.
            if (pawn.Dead || !pawn.health.hediffSet.hediffs.Where(hediff => hediff.def == HediffDefOf.ATR_SkyMindReceiver || hediff.def == HediffDefOf.ATR_SkyMindTransceiver).Any())
                return;

            // There are special considerations for adding these implants. Receiver chips kill the current mind. Transceivers allow SkyMind connections and do nothing else in particular.
            if (recipe.addsHediff == HediffDefOf.ATR_SkyMindReceiver)
            {
                Utils.Duplicate(Utils.GetBlank(), pawn);
                pawn.health.AddHediff(HediffDefOf.ATR_NoController);
            }
        }
    }
}

