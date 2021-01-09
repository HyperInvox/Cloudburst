using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class MAID : MonoBehaviour
    {
        private ProjectileController controller;
        private List<CharacterMotor> characterMotors;
        private List<RigidbodyMotor> rigidBodies;
        private float stopwatch;

        public void Awake()
        {
            characterMotors = new List<CharacterMotor>();
            rigidBodies = new List<RigidbodyMotor>();

            controller = base.gameObject.GetComponent<ProjectileController>();
            //manager = controller.owner.GetComponent<MAIDManager>();
            //LogCore.LogI(controller);
            //LogCore.LogI(controller.___ownerNetId);
            //LogCore.LogI(controller.owner);
            //LogCore.LogI("ok bro. die.");
            //body = controller.owner.GetComponent<CharacterBody>();
            //manager.DeployMAIDAuthority(base.gameObject);
            //else LogCore.LogF("DUUUUUDE WHAT IS WRONG WITH YOU????");
        }

        public void Start() {

            LogCore.LogI(controller);
            LogCore.LogI(controller.___ownerNetId);
            LogCore.LogI(controller.owner);
            
            controller.owner.GetComponent<MAIDManager>().DeployMAIDAuthority(base.gameObject);
        }

        public void OnTriggerEnter(Collider collider)
        {
            var motor = collider.GetComponent<CharacterMotor>();
            var rigidBody = collider.GetComponent<RigidbodyMotor>();
            var rememberNOPROJECTILES = collider.GetComponent<ProjectileController>();

            if (motor)
            {
               // LogCore.LogI(Language.GetString(motor.body.baseNameToken) + " is entering the bubble. Adding to the list.");
                characterMotors.Add(motor);
            }
            if (rigidBody && !rememberNOPROJECTILES)
            {
                //LogCore.LogI(rigidBody.gameObject.name + " is entering the bubble. Adding to the list.");
                rigidBodies.Add(rigidBody);
            }

            if (base.gameObject.name == "WyattMaid(Clone)")
            {
                if (collider.gameObject.name == "WyattWinch(Clone)")
                {
                    LiterallyDieJustForAFunnyVideo();
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var motor = collider.GetComponent<CharacterMotor>();
            var rigidBody = collider.GetComponent<RigidbodyMotor>();
            var rememberNOPROJECTILES = collider.GetComponent<ProjectileController>();

            if (motor)
            {
                //LogCore.LogI(Language.GetString(motor.body.baseNameToken) + " is leaving the bubble. Removing from the list.");
                characterMotors.Remove(motor);
            }
            if (rigidBody && !rememberNOPROJECTILES)
            {
                //LogCore.LogI(rigidBody.gameObject.name + " is leaving the bubble. Removing from the list.");
                rigidBodies.Remove(rigidBody);
            }
        }

        private void FixedUpdate() {
            stopwatch += Time.deltaTime;

            if (stopwatch >= 1.5f) {
                EffectManager.SpawnEffect(EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab, new EffectData
                {
                    origin = transform.position,
                    scale = 2.5f
                }, true);
                for (int i = 0; i < this.characterMotors.Count; i++)
                {
                    var motor = this.characterMotors[i];

                    var body = motor.body;
                    TeamIndex index = TeamIndex.None;
                    if (body && body.teamComponent)
                    {
                        index = body.teamComponent.teamIndex;
                    }

                    if (motor && index == TeamIndex.Monster)
                    {
                        Vector3 normalized = (transform.position - motor.transform.position).normalized;

                        motor.ApplyForce(normalized * 700, true, true);
                        //Why won't you come home?
                        /*EffectManager.SpawnEffect(EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab, new EffectData
                        {
                            origin = motor.transform.position,
                            scale = 5
                        }, true);*/
                    }

                    //wait
                    //else
                    //{
                    //    this.characterMotors.Remove(motor);
                    //}
                }
                for (int i = 0; i < this.rigidBodies.Count; i++)
                {
                    var rigid = this.rigidBodies[i];
                    var body = rigid.gameObject.GetComponent<CharacterBody>();
                    
                    TeamIndex index = TeamIndex.None;
                    if (body && body.teamComponent)
                    {
                        index = body.teamComponent.teamIndex;
                    }

                    if (rigid && index == TeamIndex.Monster && !body.characterMotor)
                    {
                        Vector3 normalized = (transform.position - rigid.transform.position).normalized;

                        rigid.rigid.AddForce(normalized * 700, ForceMode.Impulse);
                        /*EffectManager.SpawnEffect(EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab, new EffectData
                        {
                            origin = rigid.transform.position,
                            scale = 1
                        }, true);*/

                    }   
                    else
                    {
                        this.rigidBodies.Remove(rigid);
                    }
                }
                stopwatch = 0;
            }
        }

            public void LiterallyDieJustForAFunnyVideo() {
            var body = controller.owner.GetComponent<CharacterBody>();

            BlastAttack attack = new BlastAttack()
            {
                attacker = controller.owner,
                attackerFiltering = AttackerFiltering.Default,
                baseDamage = 5  * body.damage,
                baseForce = -3000,
                bonusForce = new Vector3(0, 0, 0),
                crit = body.RollCrit(),
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Shock5s,
                falloffModel = BlastAttack.FalloffModel.None,
                impactEffect = EffectIndex.Invalid,
                inflictor = base.gameObject,
                losType = BlastAttack.LoSType.NearestHit,
                position = base.transform.position,
                procChainMask = default,
                procCoefficient = 0.5f,
                radius = 30,
                teamIndex = TeamIndex.Player,
            };
            attack.Fire();
            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/VagrantNovaExplosion"), new EffectData
            {
                origin = transform.position,
                scale = 30
            }, true);
            Destroy(base.gameObject);
        }
    }
}
