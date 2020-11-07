using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class RocketJumpOnImpact : MonoBehaviour, IProjectileImpactBehavior
    {
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            var sphere = Physics.OverlapSphere(transform.position, 15);
            foreach (var body in sphere)
            {
                var cb = body.gameObject.GetComponentInParent<CharacterBody>();
                if (cb)
                {
                    if (cb.characterMotor && cb.Equals(gameObject.GetComponent<ProjectileController>().owner.GetComponent<CharacterBody>()))
                    {
                        API.AddExplosionForce(cb.characterMotor, cb.characterMotor.mass * 20, transform.position, 25, 5);

                    }
                    /*if (cb.Equals(controller.owner.GetComponent<CharacterBody>())) {
                        //cb.rigidbody.AddExplosionForce(500000, base.transform.position, 50000000000, 10000, ForceMode.Impulse);
                        //LogCore.LogI("hello");
                        var aim = cb.inputBank.GetAimRay();
                        cb.characterMotor.ApplyForce(-(aim.direction * 1000), true, false);
                        //cb.rigidbody.AddExplosionForce(cb.characterMotor.mass * 60, transform.position, 15);
                        //characterMotor.ApplyForce(new Vector3(0, , 0), true, false);
                        //(hitPoint2.hitPosition - this.position).normalized;
                    }*/
                }
            }
        }
    }
}
