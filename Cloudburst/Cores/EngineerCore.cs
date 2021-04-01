using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudburst.Cores.Engineer;
using Cloudburst.Cores.Engineer.ETStates;
using EnigmaticThunder.Modules;
using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;


using RoR2;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace Cloudburst.Cores
{
    class EngineerCore
    {
        //she stares you down
        //she cares your top
        //you know she'll never love you

        public static EngineerCore instance;

        //TODO
        //Fix skins
        //Tweak primary
        protected GameObject flameTurret;
        public static GameObject flameTurretMaster;
        public static GameObject projectileGameObject;
        protected GameObject engineerObject;

        private FlameTurrets turrets;
        public class FlameTurrets : EnemyBuilder
        {
            protected override string resourceMasterPath => "prefabs/charactermasters/EngiWalkerTurretMaster";

            protected override string resourceBodyPath => "prefabs/characterbodies/EngiWalkerTurretBody";

            protected override string bodyName => "EngiFlameTurret";

            protected override bool registerNetwork => true;

            public override int DirectorCost => 7595;

            public override bool NoElites => true;

            public override bool ForbiddenAsBoss => true;

            public override HullClassification HullClassification => HullClassification.Human;

            public override MapNodeGroup.GraphType GraphType => MapNodeGroup.GraphType.Ground;

            public override void OverrideCharacterbody(CharacterBody body)
            {
                base.OverrideCharacterbody(body);
                body.baseMaxHealth = 70;
                body.levelMaxHealth = 21;
                body.baseDamage = 8;
                body.levelDamage = 1.6f;

            }

            public override void OverrideSkills()
            {
                base.OverrideSkills();
                //KILL
                EnigmaticThunder.Modules.Loadouts.RegisterEntityState(typeof(DeathState));

                var characterDeathBehavior = enemyBody.GetComponent<CharacterDeathBehavior>();
                characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(DeathState));
            }

            public override void CreatePrimary(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreatePrimary(skillLocator, skillFamily);

                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engiturretbody/EngiTurretFireBeam");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
                CloudUtils.CopySkillDefSettings(origDef, def);

                EnigmaticThunder.Modules.Loadouts.RegisterEntityState(typeof(FireFlameThrower));

                def.activationState = new SerializableEntityStateType(typeof(FireFlameThrower));

                EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateSecondary(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateSecondary(skillLocator, skillFamily);

                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engiturretbody/EngiTurretFireBeam");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
                CloudUtils.CopySkillDefSettings(origDef, def);

                def.activationState = new SerializableEntityStateType(typeof(FireFlameThrower));

                EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateUtility(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateUtility(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engiturretbody/EngiTurretFireBeam");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
                CloudUtils.CopySkillDefSettings(origDef, def);

                def.activationState = new SerializableEntityStateType(typeof(FireFlameThrower));

                EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

            public override void CreateSpecial(SkillLocator skillLocator, SkillFamily skillFamily)
            {
                base.CreateSpecial(skillLocator, skillFamily);
                SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engiturretbody/EngiTurretFireBeam");
                SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
                CloudUtils.CopySkillDefSettings(origDef, def);

                def.activationState = new SerializableEntityStateType(typeof(FireFlameThrower));

                EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(def);

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = def,
                    viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false, null)

                };
            }

        }

        public EngineerCore() => EngineerEdits();


        protected void EngineerEdits() {
            LogCore.LogI("Initializing Core: " + base.ToString());
            instance = this;

            engineerObject = Resources.Load<GameObject>("prefabs/characterbodies/EngiBody");
            RegisterNewTurret();
            AddNewSpecial();
            AddNewPrimary();
            CreateAltPrimaryProjectile();
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
            //IL.RoR2.CharacterMaster.GetDeployableSameSlotLimit += CharacterMaster_GetDeployableSameSlotLimit;
        }

        private void CreateAltPrimaryProjectile() {
            projectileGameObject = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile").InstantiateClone("EngiGrenadeImpactProjectile", true);
            CloudUtils.RegisterNewProjectile(projectileGameObject);
            ProjectileImpactExplosion impact = projectileGameObject.GetComponent<ProjectileImpactExplosion>();
            impact.destroyOnWorld = true;
            UnityEngine.Object.Destroy(projectileGameObject.GetComponent<ProjectileStickOnImpact>());
            
        }
        private int CharacterMaster_GetDeployableSameSlotLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            bool engi = slot == DeployableSlot.EngiTurret;
            if (!engi) return orig(self, slot);
            else {
                //Reflection will never fail me!
                GameObject fuckIL = self.bodyInstanceObject;
                if (fuckIL) {
                    if (fuckIL.GetComponent<SkillLocator>().special.skillDef.skillName == "FUCKIL") {
                        return 4;
                    }
                    return 2;
                }
                return 2;
            }
        }

        private void ILCharacterMaster_GetDeployableSameSlotLimit(ILContext il)
        {
            //FUCK THIS I'M GOING THE DESTRUCTIVE ROUTE
            ILCursor c = new ILCursor(il);
            int AAAA = 2;
            int otherAA = 0;
            c.GotoNext(
                //IL_0067: ldc.i4.2
                x => x.MatchLdcI4(0),
                //IL_0068: stloc.0   
                x => x.MatchStloc(out otherAA)
                );
            c.Index += 0;


            c.GotoNext(
                //IL_0067: ldc.i4.2
                x => x.MatchLdcI4(2),
                //IL_0068: stloc.0   
                x => x.MatchStloc(out AAAA)
                //x => x.MatchBrtrue(out ILLabel a)
                //x => x.MatchStloc(0)
                //we don't talk about this one
                //x => x.Next.Operand = (object)4,
                );
            c.Index += 16;
            
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, AAAA);
            c.Emit(OpCodes.Ldloc, otherAA);



            c.EmitDelegate<Func<CharacterMaster, int>>((cm) =>
            {
                return 2;
            });

            /*c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<CharacterMaster, int>>((cm) =>
            {
                return 2;
            });*/

            //LogCore.LogI(c);
            //c.Emit(OpCodes.Stloc_0);

            //c.Next.Operand = 100;
            /*c.GotoNext(
                //BEFORE CALLVIRT
                x => x.MatchLdarg(0),
                x => x.MatchCall<CharacterMaster>("get_bodyInstanceObject"),
                //do later - done at 10:44am 
               x => x.MatchCallvirt<GameObject>("GetComponent"),
                x => x.MatchLdfld<GenericSkill>("secondary"),
                //another callvirt. boy oh boy.
                x => x.MatchCallvirt<GenericSkill>("get_maxStock"),
                x => x.MatchStloc(4)
                );
            c.Index += 4;*/
        }

        protected void RegisterNewTurret() {
            turrets = new FlameTurrets();

            turrets.Create();

            flameTurret = turrets.enemyBody;
            flameTurretMaster = turrets.enemyMaster;
        }
        protected void AddNewSpecial()
        {
            SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engibody/EngiBodyPlaceWalkerTurret");
            var newDef = ScriptableObject.CreateInstance<SkillDef>();

            CloudUtils.CopySkillDefSettings(origDef, newDef);

            EnigmaticThunder.Modules.Loadouts.RegisterEntityState(typeof(PlaceFlameTurret));
            EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(newDef);

            Languages.Add("ENGI_SPECIAL_ALT2_NAME", "TR27 Immolator Turret");
            Languages.Add("ENGI_SPECIAL_ALT2_DESCRIPTION", "Place a <style=cIsUtility>mobile</style> turret that <style=cIsUtility>inherits all your items.</style> Fires a plume of flame for <style=cIsDamage>100% damage per second</style> that <style=cIsDamage>ignites enemies</style>. <style=cIsDamage>Explodes on death for 3x</style> maximum health. Can place up to 4.");

            newDef.baseRechargeInterval = 15;
            newDef.activationState = new SerializableEntityStateType(typeof(PlaceFlameTurret));
            newDef.baseMaxStock = 4;
            newDef.skillName = "FUCKIL";
            newDef.icon = AssetsCore.engiFlameTurrets;
            newDef.skillNameToken = "ENGI_SPECIAL_ALT2_NAME";
            newDef.skillDescriptionToken = "ENGI_SPECIAL_ALT2_DESCRIPTION";
            newDef.icon = AssetsCore.engiFlameTurrets;

            var locator = engineerObject.GetComponent<SkillLocator>();

            var family = locator.special.skillFamily;

            SkillFamily.Variant variant = new SkillFamily.Variant();

            variant.skillDef = newDef;

            int prevLength = family.variants.Length;
            Array.Resize<SkillFamily.Variant>(ref family.variants, prevLength + 1);
            family.variants[prevLength] = variant;
        }

        protected void AddNewPrimary()
        {
            SkillDef origDef = Resources.Load<SkillDef>("skilldefs/engibody/EngiBodyFireGrenade");
            var newDef = ScriptableObject.CreateInstance<SkillDef>();

            CloudUtils.CopySkillDefSettings(origDef, newDef);

            EnigmaticThunder.Modules.Loadouts.RegisterEntityState(typeof(FireVolley));
            EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(newDef);

            Languages.Add("ENGINEER_PRIMARY_ALT_NAME", "Impact Grenades");
            Languages.Add("ENGINEER_PRIMARY_ALT_DESCRIPTION", "Shoot 3 impact grenades from each cannon that deal <style=cIsDamage>100%</style>");

            newDef.activationState = new SerializableEntityStateType(typeof(FireVolley));
            newDef.skillName = "GrenadeVolley";
            newDef.skillNameToken = "ENGINEER_PRIMARY_ALT_NAME";
            newDef.skillDescriptionToken = "ENGINEER_PRIMARY_ALT_DESCRIPTION";
            //newDef.beginSkillCooldownOnSkillEnd = true;
            //newDef.baseRechargeInterval = .1f;
            newDef.icon = AssetsCore.engiImpact;
            newDef.canceledFromSprinting = false;
            //newDef.cancelSprintingOnActivation = false;
            
                
            var locator = engineerObject.GetComponent<SkillLocator>();

            var family = locator.primary.skillFamily;

            SkillFamily.Variant variant = new SkillFamily.Variant();

            variant.skillDef = newDef;

            int prevLength = family.variants.Length;
            Array.Resize<SkillFamily.Variant>(ref family.variants, prevLength + 1);
            family.variants[prevLength] = variant;
        }
    }
}
