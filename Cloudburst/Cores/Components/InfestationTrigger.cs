using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;


namespace Cloudburst.Cores.Components
{
    class InfestationTrigger : MonoBehaviour, IOnKilledServerReceiver
    {
        public static event Action<Vector3> OnKilled;
        public void OnKilledServer(DamageReport damageReport)
        {

            EffectData data = new EffectData()
            {
                rotation = Quaternion.Euler(base.transform.forward),
                scale = 15,
                origin = transform.position,
            };

            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/NullifierExplosion"), data, true);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = "<style=cWorldEvent>[WARNING] MASSIVE BREACH DETECTED IN SPACE AND TIME. SIGNALS INDICATE CLOUDBURST EVENT #" + UnityEngine.Random.Range(0, 9000) + ". SEEK YOUR PEACE.</style>"
            });
            OnKilled(transform.position);
            Destroy(base.gameObject);
        }
    }
}
