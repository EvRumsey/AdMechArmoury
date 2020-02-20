﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AbilityUser;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class AM_Pawn_EquipmentTracker_Notify_EquipmentAdded_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {

            if (eq.TryGetComp<CompPowerWeaponActivatableEffect>() != null && eq.TryGetComp<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
            {
                bool flag = compPowerWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compPowerWeapon.TryActivate();
                }
            }
            if (eq.TryGetComp<CompForceWeaponActivatableEffect>() != null && eq.TryGetComp<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
            {
                bool flag = compForceWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compForceWeapon.TryActivate();
                }
            }
            if (eq.TryGetComp<CompAbilityItem>() != null && eq.TryGetComp<CompAbilityItem>() is CompAbilityItem abilityItem)
            {
                Log.Message("is abilityItem");
                if (!abilityItem.Props.Abilities.NullOrEmpty())
                {
                    Log.Message("has Abilities");
                    foreach (AbilityDef def in abilityItem.Props.Abilities)
                    {
                        if (!__instance.pawn.abilities.abilities.Any(x=> x.def == def))
                        {
                            Log.Message("add Abilities");
                            __instance.pawn.abilities.GainAbility(def);
                        }
                    }
                }
            }
        }
    }
}
