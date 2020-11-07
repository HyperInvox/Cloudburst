using RoR2;
using UnityEngine;

namespace Moonstorm.Cores.Components
{
    public class SproutingProjectileZone : MonoBehaviour
    {
        public void OnTriggerEnter(Collider collider)
        {
            //i will climb the hill
            //and i won't come down!
            var body = collider.gameObject.GetComponentInParent<CharacterBody>();
            if (body)
            {
                if (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player)
                {
                    var motor = body.characterMotor;
                    motor.ApplyForce(new Vector3(0, 9000, 0), true, false);
                    Destroy(base.gameObject);
                }
            }
        }
    }
}
