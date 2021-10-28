using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class Blink : MonoBehaviour, IProjectileImpactBehavior
    {
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            LogCore.LogI("hi4");
            GetComponent<ProjectileController>().owner.GetComponent<CharacterMotor>().Motor.SetPosition(base.transform.position); //TeleportHelper.TeleportBody(, base.transform.position);
            Destroy(base.gameObject);
            //GetComponent<ProjectileController>().owner.GetComponent<CharacterMotor>().
        }
    }
}