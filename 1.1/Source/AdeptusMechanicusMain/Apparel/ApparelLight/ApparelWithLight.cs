﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class ApparelWithLight : Apparel
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002076 File Offset: 0x00000276
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Thing>(ref this.light, "light", false);
			Scribe_Values.Look<bool>(ref this.lightIsOn, "lightIsOn", false, false);
			Scribe_Values.Look<ApparelWithLight.LightMode>(ref this.lightMode, "lightMode", ApparelWithLight.LightMode.Automatic, false);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020B3 File Offset: 0x000002B3
		public override void Tick()
		{
			base.Tick();
			if (this.lightIsOn || Find.TickManager.TicksGame >= this.nextUpdateTick)
			{
				this.nextUpdateTick = Find.TickManager.TicksGame + 60;
				this.RefreshLightState();
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
		public void RefreshLightState()
		{
			if (this.ComputeLightState())
			{
				this.SwitchOnLight();
				return;
			}
			this.SwitchOffLight();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
		public bool ComputeLightState()
		{
			return base.Wearer != null && !base.Wearer.Dead && !base.Wearer.Downed && base.Wearer.Awake() && (this.lightMode == ApparelWithLight.LightMode.ForcedOn || (this.lightMode != ApparelWithLight.LightMode.ForcedOff && (base.Wearer.Map != null && ((base.Wearer.Position.Roofed(base.Wearer.Map) && base.Wearer.Map.glowGrid.PsychGlowAt(base.Wearer.Position) <= PsychGlow.Lit) || (!base.Wearer.Position.Roofed(base.Wearer.Map) && base.Wearer.Map.glowGrid.PsychGlowAt(base.Wearer.Position) < PsychGlow.Overlit)))));
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
		public void SwitchOnLight()
		{
			IntVec3 intVec = base.Wearer.DrawPos.ToIntVec3();
			if (!this.light.DestroyedOrNull() && intVec != this.light.Position)
			{
				this.SwitchOffLight();
			}
			if (this.light.DestroyedOrNull() && intVec.GetFirstThing(base.Wearer.Map, Util_ApparelWithLight.ApparelLightDef) == null)
			{
				this.light = GenSpawn.Spawn(Util_ApparelWithLight.ApparelLightDef, intVec, base.Wearer.Map, WipeMode.Vanish);
			}
			this.lightIsOn = true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
		public void SwitchOffLight()
		{
			if (!this.light.DestroyedOrNull())
			{
				this.light.Destroy(DestroyMode.Vanish);
				this.light = null;
			}
			this.lightIsOn = false;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
		public override IEnumerable<Gizmo> GetGizmos()
		{
			IEnumerable<Gizmo> wornGizmos = this.GetWornGizmos();
			IEnumerable<Gizmo> gizmos = base.GetGizmos();
			IEnumerable<Gizmo> result;
			if (gizmos != null)
			{
				result = gizmos.Concat(wornGizmos);
			}
			else
			{
				result = wornGizmos;
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000022D4 File Offset: 0x000004D4
		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			IList<Gizmo> list = new List<Gizmo>();
			int num = 700000101;
			Command_Action command_Action = new Command_Action();
			switch (this.lightMode)
			{
			case ApparelWithLight.LightMode.Automatic:
				command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeAutomatic", true);
				command_Action.defaultLabel = "Ligth: automatic.";
				break;
			case ApparelWithLight.LightMode.ForcedOn:
				command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
				command_Action.defaultLabel = "Ligth: on.";
				break;
			case ApparelWithLight.LightMode.ForcedOff:
				command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
				command_Action.defaultLabel = "Ligth: off.";
				break;
			}
			command_Action.defaultDesc = "Switch mode.";
			command_Action.activateSound = SoundDef.Named("Click");
			command_Action.action = new Action(this.SwitchLigthMode);
			command_Action.groupKey = num + 1;
			list.Add(command_Action);
			return list;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
		public void SwitchLigthMode()
		{
			switch (this.lightMode)
			{
			case ApparelWithLight.LightMode.Automatic:
				this.lightMode = ApparelWithLight.LightMode.ForcedOn;
				break;
			case ApparelWithLight.LightMode.ForcedOn:
				this.lightMode = ApparelWithLight.LightMode.ForcedOff;
				break;
			case ApparelWithLight.LightMode.ForcedOff:
				this.lightMode = ApparelWithLight.LightMode.Automatic;
				break;
			}
			this.RefreshLightState();
		}

		// Token: 0x04000001 RID: 1
		public const int updatePeriodInTicks = 60;

		// Token: 0x04000002 RID: 2
		public int nextUpdateTick;

		// Token: 0x04000003 RID: 3
		public Thing light;

		// Token: 0x04000004 RID: 4
		public bool lightIsOn;

		// Token: 0x04000005 RID: 5
		public ApparelWithLight.LightMode lightMode;

		// Token: 0x02000004 RID: 4
		public enum LightMode
		{
			// Token: 0x04000007 RID: 7
			Automatic,
			// Token: 0x04000008 RID: 8
			ForcedOn,
			// Token: 0x04000009 RID: 9
			ForcedOff
		}
	}
}
