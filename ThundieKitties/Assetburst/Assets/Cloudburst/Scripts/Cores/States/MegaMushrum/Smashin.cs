using EntityStates;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.States.MegaMushrum
{
    public class Smashin : BaseState
    {
        private float smashCount = 0;
        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public void SUPERSMASHBROSULTIMATE()
        {
            //LogCore.LogI("uwu");

            timer += Time.deltaTime;

            if (smashCount < 6 && timer > 0.5)
            {
                new BlastAttack
                {
                    attacker = gameObject,
                    attackerFiltering = AttackerFiltering.NeverHit,
                    baseDamage = 5f * characterBody.damage,
                    baseForce = 2500,
                    crit = RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.None,
                    inflictor = base.gameObject,
                    losType = BlastAttack.LoSType.NearestHit,
                    position = transform.position,
                    procChainMask = default,
                    procCoefficient = 1,
                    radius = 20,
                    teamIndex = GetTeam(),
                }.Fire();
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXTreebot"), new EffectData
                {
                    origin = base.transform.position,
                    scale = 20
                }, true);
                smashCount += 1;
                timer = 0;
                LogCore.LogI(smashCount);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            SUPERSMASHBROSULTIMATE();

            if (isAuthority && smashCount == 6)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

    }
}
