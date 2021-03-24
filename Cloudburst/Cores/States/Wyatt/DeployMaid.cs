using EntityStates;
using Cloudburst.Cores.HAND;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using Cloudburst.Cores.Components;

namespace Cloudburst.Cores.States.Wyatt
{
    class DeployMaid : BaseSkillState
    {
        private float theDevilHasSomeHardToReadFinePrint = 0;
        private bool unstable = false;
        private MAIDManager blaseball = null;
        private bool solarEclipse = false;
        public override void OnEnter()
        {
           // base.activatorSkillSlot.skillDef.baseRechargeInterval = 5;

            base.OnEnter();
            blaseball = GetComponent<MAIDManager>();
            blaseball.OnRetrival += Blaseball_OnRetrival;
           // blaseball.sunset += Blaseball_sunset;
            if (base.isAuthority)
            {    
                FireProjectile();
                //outer.SetNextStateToMain();
            }
        }

        private void Blaseball_sunset()
        {   
            solarEclipse = true;
        }

        private void Blaseball_OnRetrival(bool nat, GenericSkill arg2, Vector3 dis)
        {
            solarEclipse = true;
            if (!nat)
            {
                base.activatorSkillSlot.finalRechargeInterval = 5;
            }
            else
            {
                base.activatorSkillSlot.finalRechargeInterval = 10;
                //who cares about underlying issues in my code
                //no one's gonna read it anyways :^]]
                    
                if (!base.gameObject.HasComponent<BasedDepartment>()) { 
                BasedDepartment based = base.gameObject.AddComponent<BasedDepartment>();
                based.interval = 0.5f;
                    LogCore.LogI(dis);
                }
            }
        
            //else
            {//
             //   base.activatorSkillSlot.skillDef.baseRechargeInterval = 5;
            }            
            base.activatorSkillSlot.DeductStock(1);
        }

        public void FireProjectile() {
            var aimRay = base.GetAimRay();
            FireProjectileInfo info = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = 5 * damageStat,
                damageColorIndex = RoR2.DamageColorIndex.Default,
                damageTypeOverride = DamageType.Generic,
                force = 0,
                owner = gameObject,
                position = aimRay.origin,
                procChainMask = default,
                projectilePrefab = ProjectileCore.wyattMaidBubble,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                target = null,
                useFuseOverride = false,
                useSpeedOverride = false
            };
            ProjectileManager.instance.FireProjectile(info);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
      
            theDevilHasSomeHardToReadFinePrint += Time.fixedDeltaTime;
            if (base.isAuthority && base.IsKeyDownAuthority() && solarEclipse == false && unstable == false && theDevilHasSomeHardToReadFinePrint > 0.3f) {
                blaseball.RetrieveMAIDAuthority();
                unstable = true;
            }

            if (solarEclipse && base.isAuthority) {
                outer.SetNextStateToMain();
                //dW5jb21tZW50IHRoaXMgbGluZSBmb3IgZnVubnkK
                /*outer.SetNextState(new DeployMaid() {
                    solarEclipse = true
                });*/
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}