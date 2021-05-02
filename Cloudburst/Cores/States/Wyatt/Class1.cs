using Cloudburst.Cores.Components.Wyatt;
using EntityStates;
using EntityStates.ParentMonster;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Wyatt
{
    class YeahDudeIBetterBeOrYouCanFuckinKissMyAssHumanCentipede : BaseSkillState
    {
        public static float baseDuration = 0.1f;
        private List<CharacterBody> negros;
        private WyattWalkmanBehavior walkman;
        public override void OnEnter()
        {
            base.OnEnter();



            walkman = GetComponent<WyattWalkmanBehavior>();
            if (walkman.flowing == false)
            {
                walkman.ActivateFlowAuthority();
            }
            //EffectManager.SpawnEffect(EffectCore.willIsStillTotallyNotPoggers/*Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfx")*/, new EffectData()
            /*{
                origin = characterBody.footPosition,
                scale = 10,
                rotation = Quaternion.identity,
            }, false);
            characterMotor.velocity = Vector3.up * (this.moveSpeedStat * 5);// * 100;
            characterMotor.Motor.ForceUnground();
            GatherNiggas();
            //base.characterMotor.Motor.ForceUnground();
            //base.characterMotor.ApplyForce(, true, false);*/
        }

        public void ShockEnemies()
        {
            LightningOrb lightningOrb2 = new LightningOrb();
            lightningOrb2.origin = base.transform.position;
            lightningOrb2.damageValue = 1f * damageStat;
            lightningOrb2.isCrit = RollCrit();
            lightningOrb2.bouncesRemaining = 1;
            lightningOrb2.teamIndex = GetTeam();
            lightningOrb2.attacker = gameObject;
            lightningOrb2.bouncedObjects = new List<HealthComponent>
                            {
                                base.healthComponent//.GetComponent<HealthComponent>()
                            };
            lightningOrb2.procChainMask = default;
            lightningOrb2.procCoefficient = 1;
            lightningOrb2.lightningType = LightningOrb.LightningType.MageLightning;
            lightningOrb2.damageColorIndex = DamageColorIndex.Default;
            lightningOrb2.range += 10;
            lightningOrb2.damageType = (DamageTypeCore.pullEnemies | DamageType.Stun1s);
            HurtBox hurtBox2 = lightningOrb2.PickNextTarget(transform.position);
            if (hurtBox2)
            {
                lightningOrb2.target = hurtBox2;
                OrbManager.instance.AddOrb(lightningOrb2);
            }
        }

        private void CreateBlinkEffect(Vector3 origin, Vector3 direction)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(direction);
            effectData.origin = origin;
            EffectManager.SpawnEffect(LoomingPresence.blinkPrefab, effectData, true);
        }

        public void GatherNiggas()
        {
            negros = new List<CharacterBody>();

            List<GameObject> list = new List<GameObject>();
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = base.gameObject.transform.position;
            bullseyeSearch.maxDistanceFilter = 25;
            bullseyeSearch.teamMaskFilter = TeamMask.GetUnprotectedTeams(base.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            List<HurtBox> list2 = bullseyeSearch.GetResults().ToList<HurtBox>();
            for (int i = 0; i < list2.Count; i++)
            {
                GameObject gameObject = list2[i].healthComponent.gameObject;
                if (gameObject)
                {
                    list.Add(gameObject);
                }
            }
            
            HahaNiggaHowHardIsItToMoveLikeJustMoveNigga(list2);
            
        }

        public void HahaNiggaHowHardIsItToMoveLikeJustMoveNigga(List<HurtBox> niggas)  {
            foreach (HurtBox nigga in niggas) {

                if (nigga && nigga.healthComponent && nigga.healthComponent.body)
                {
                    var niggaBody = nigga.healthComponent.body;
                    if (NetworkServer.active)
                    {
                        niggaBody.AddTimedBuff(BuffCore.instance.wyattSuspension, 15);
                    }
                    negros.Add(niggaBody);
                }
            }   
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                var a = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                //base.inputBank.aimDirection
                base.characterMotor.rootMotion += base.inputBank.aimDirection * (3 * 35 * Time.fixedDeltaTime);

            }

            if (NetworkServer.active)
            {
               // ShockEnemies();
            }

            if (fixedAge >= baseDuration)
            {
                /*foreach (CharacterBody niggaBody in negros)
                {
                    CharacterMotor niggaMotor = niggaBody.characterMotor;
                    if (niggaBody && niggaMotor)
                    {
                        //FUCK YOU CHARACTER MOTOR AND FUCK YOU HOPOO
                        //I ALREADY USED THIS SHITTY PHYSICS SHIT IN MY GAME
                        //I KNOW THE TRICKS
                        //I WILL FUCKING JAPE YOU
                        var a = niggaMotor.Motor.GetState();
                        a.BaseVelocity = Vector3.zero;
                        niggaMotor.Motor.ApplyState(a);
 
                        niggaMotor.velocity = Vector3.zero;
                    }
                }*/
                if (isAuthority)
                {
                    outer.SetNextStateToMain();
                };
            }
        }
    }
}