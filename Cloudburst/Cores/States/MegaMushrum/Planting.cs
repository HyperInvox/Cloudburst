using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.States.MegaMushrum
{
    class Planting : BaseState
    {
        public static float explosionDelay = 2f;

        public static int explosionCount = 15;

        public static GameObject delayPrefab = PrefabCore.pillar;

        public static float damageCoefficient = 5;
        public static float randomRadius = 16;
        public static float radius = 4;
        public static float baseDuration = 8f;

        private float castTimer;
        private float sunTime;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            sunTime = duration / (float)explosionCount;

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            castTimer += Time.fixedDeltaTime;

            if (castTimer >= sunTime)
            {
                PlacePillar();
                castTimer -= sunTime;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void PlacePillar()
        {
            Vector3 hit = Vector3.zero;

            var aimRay = base.GetAimRay();

            aimRay.origin += Random.insideUnitSphere * randomRadius;

            RaycastHit raycastHit;

            if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
            {
                hit = raycastHit.point;
            }
            if (hit != Vector3.zero)
            {
                var enemyTeam = FindEnemyTeam(GetTeam());

                Transform origEnemyPosition = FindOppositeTeamMemberClosest(enemyTeam, hit);

                Vector3 enemyPosition = hit;

                if (origEnemyPosition)
                {
                    enemyPosition = origEnemyPosition.position;
                }

                enemyPosition += Random.insideUnitSphere * randomRadius;

                if (Physics.Raycast(new Ray
                {
                    origin = enemyPosition + Vector3.up * randomRadius,
                    direction = Vector3.down
                }, out raycastHit, 500f, LayerIndex.world.mask))
                {

                    //rotation
                    Vector3 hitPoint = raycastHit.point;

                    Quaternion rotation = Util.QuaternionSafeLookRotation(raycastHit.normal);


                    ProjectileManager.instance.FireProjectile(ProjectileCore.mushrumDelaySproutingMushroom, hitPoint, Util.QuaternionSafeLookRotation(Vector3.up * 20), base.gameObject, this.damageStat * SporeGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null);

                }
            }
            //Make sure that our aimray isn't aiming at nothing
            /*Vector3 vector = Vector3.zero;
            Ray ai  mRay = GetAimRay();
            aimRay.origin += Random.insideUnitSphere * randomRadius;
            RaycastHit raycastHit;
            if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
            {
                vector = raycastHit.point;
            }
            if (vector != Vector3.zero)
            {
            var vector = base.GetAimRay().direction;

                //Get our team
                var currentTeam = GetTeam();
                var enemyTeam = FindEnemyTeam(currentTeam);

                //Get our enemy's transform
                Transform enemyTransform = FindOppositeTeamMemberClosest(enemyTeam, vector);

                //I don't even know man
                var enemyPos = vector;

                if (enemyTransform) {
                    //Our enemy position exists!
                    enemyPos = enemyTransform.position;
                }

                //Randomize the position of our pillar 
                enemyPos += Random.insideUnitSphere * randomRadius;

                var raycastHit = default(RaycastHit);
                if (Physics.Raycast(new Ray
                {
                    origin = enemyPos + Vector3.up * randomRadius,
                    direction = Vector3.down
                }, out raycastHit, 500f, LayerIndex.world.mask)) {

                    var point = raycastHit.point;

                    var rotation = Util.QuaternionSafeLookRotation(enemyPos);
                    var effect = Object.Instantiate<GameObject>(delayPrefab, enemyPos, rotation);

                    var delayBlast = effect.GetComponent<DelayBlast>();

                    delayBlast.attacker = gameObject;
                    delayBlast.baseDamage = damageCoefficient * damageStat;
                    delayBlast.baseForce = 1000;
                    delayBlast.bonusForce = Vector3.up * 1000;
                    delayBlast.crit = RollCrit();
                    delayBlast.damageColorIndex = DamageColorIndex.CritHeal;
                    delayBlast.damageType = DamageType.SlowOnHit;
                    delayBlast.falloffModel = BlastAttack.FalloffModel.None;
                    delayBlast.inflictor = gameObject;
                    delayBlast.maxTimer = explosionDelay;
                    delayBlast.position = enemyPos;
                    delayBlast.procCoefficient = 1;
                    delayBlast.radius = radius;

                    effect.GetComponent<TeamFilter>().teamIndex = GetTeam();

                    effect.transform.localScale = new Vector3(3, 3, 1);

                    var particles = gameObject.GetComponent<ScaleParticleSystemDuration>();
                    if (particles)
                    {
                        particles.newDuration = explosionDelay;
                    }

                //}
            }*/
        }

        private TeamIndex FindEnemyTeam(TeamIndex objectTeam)
        {
            if (objectTeam != TeamIndex.Player)
            {
                if (objectTeam == TeamIndex.Monster)
                {
                    return TeamIndex.Player;
                }
                else
                {
                    return TeamIndex.Neutral;
                }
            }
            else
            {
                return TeamIndex.Monster;
            }
        }
        private Transform FindOppositeTeamMemberClosest(TeamIndex enemyTeam, Vector3 position)
        {
            var teamMembers = TeamComponent.GetTeamMembers(enemyTeam);
            float maxDistance = 99999f;
            Transform result = null;
            for (int i = 0; i < teamMembers.Count; i++)
            {
                float spaceInBetween = Vector3.SqrMagnitude(teamMembers[i].transform.position - position);
                if (spaceInBetween < maxDistance)
                {
                    maxDistance = spaceInBetween;
                    result = teamMembers[i].transform;
                }
            }
            return result;
        }

    }
}
