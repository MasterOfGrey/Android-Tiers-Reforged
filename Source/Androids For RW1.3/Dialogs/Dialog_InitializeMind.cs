﻿using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace ATReforged
{
    public class Dialog_InitializeMind : Dialog_MessageBox
    {
        private const float TitleHeight = 42f;
        private const float DialogWidth = 500f;
        private const float DialogHeight = 300f;

        private Vector2 scrollPosition = Vector2.zero;

        private float creationRealTime = -1f;

        private float TimeUntilInteractive => interactionDelay - (Time.realtimeSinceStartup - creationRealTime);

        private bool InteractionDelayExpired => TimeUntilInteractive <= 0f;

        public override Vector2 InitialSize
        {
            get
            {
                float height = DialogHeight;
                if (title != null) height += TitleHeight;
                return new Vector2(DialogWidth, height);
            }
        }

        // Dialog options for initializing the mind of a mechanical unit that just had its autonomous core initialized.
        public Dialog_InitializeMind(Pawn newIntelligence) : base("ATR_InitializeMindDesc".Translate(), "ATR_SkyMindInitialization".Translate(), null, "ATR_AutomaticInitialization".Translate(), null, "ATR_InitializeMindTitle".Translate(), false)
        {
            // If there is any intelligence in the SkyMind, then the new intelligence may copy it. It does not matter if the pawn is in a mind operation or is a controller - this is an immediate and sync-safe operation.
            if (Utils.gameComp.GetCloudPawns().Count() > 0)
            {
                buttonAAction = delegate ()
                {
                    List<FloatMenuOption> opts = new List<FloatMenuOption>();

                    foreach (Pawn pawn in Utils.gameComp.GetCloudPawns())
                    {
                        opts.Add(new FloatMenuOption(pawn.LabelShortCap, delegate ()
                        {
                            Utils.Duplicate(pawn, newIntelligence, false, false);
                        }));
                    }
                    opts.SortBy((x) => x.Label);

                    if (opts.Count == 0)
                        Log.Error("[ATR] Initializing a mind via SkyMind attempted but no viable cloud pawns were found! Unknown behavior may occur.");
                    Find.WindowStack.Add(new FloatMenu(opts, ""));
                };
            }
            // Players may choose to allow the new pawn to initialize itself and choose its own personality and intelligence.
            buttonBAction = delegate ()
            {
                PawnGenerationRequest request = new PawnGenerationRequest(newIntelligence.kindDef, Faction.OfPlayer, forceGenerateNewPawn: true, newborn: true, canGeneratePawnRelations: false, colonistRelationChanceFactor: 0, fixedGender: Utils.GenerateGender(newIntelligence.kindDef));
                Pawn newPawn = PawnGenerator.GeneratePawn(request);
                BackstoryDatabase.TryGetWithIdentifier("AndroidBackstory", out newPawn.story.childhood);
                Utils.Duplicate(newPawn, newIntelligence, false, false);
            };
            closeOnCancel = false;
            closeOnAccept = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float num = inRect.y;
            if (!title.NullOrEmpty())
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, num, inRect.width, 42f), title);
                num += 42f;
            }

            Text.Font = GameFont.Small;
            Rect outRect = new Rect(inRect.x, num, inRect.width, inRect.height - 35f - 5f - num);
            float width = outRect.width - 16f;
            Rect viewRect = new Rect(0f, 0f, width, Text.CalcHeight(text, width));
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Widgets.Label(new Rect(0f, 0f, viewRect.width, viewRect.height), text);
            Widgets.EndScrollView();
            int num4 = (buttonCText.NullOrEmpty() ? 2 : 3);
            float num5 = inRect.width / (float)num4;
            float width2 = num5 - 10f;

            string label = (InteractionDelayExpired ? buttonAText : (buttonAText + "(" + Mathf.Ceil(TimeUntilInteractive).ToString("F0") + ")"));
            if (Widgets.ButtonText(new Rect(num5 * (float)(num4 - 1) + 10f, inRect.height - 35f, width2, 35f), label) && InteractionDelayExpired)
            {
                if (buttonAAction != null)
                {
                    buttonAAction();
                    Close();
                }
                else
                {
                    Messages.Message("ATR_NoAvailableTarget".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }

            GUI.color = Color.white;
            if (buttonBText != null && Widgets.ButtonText(new Rect(0f, inRect.height - 35f, width2, 35f), buttonBText))
            {
                if (buttonBAction != null)
                {
                    buttonBAction();
                }

                Close();
            }
        }
    }
}