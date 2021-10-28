using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    class Glass : MonoBehaviour, IOnDamageDealtServerReceiver
    {
        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (damageReport.attacker = base.gameObject) {

                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/BrittleDeath"), new EffectData() { origin = base.transform.position, scale = 10, rotation = Quaternion.identity}, true);
                damageReport.attackerBody.healthComponent.HealFraction(15, default);
                damageReport.attackerBody.AddBuff(BuffCore.instance.glassMithrix);
            }
        }
    }
}
