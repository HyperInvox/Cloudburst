using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.MegaMushrum
{
    public class Planted : BaseState
    {
        public static float healFraction = EntityStates.MiniMushroom.Plant.healFraction;
        public static float baseMaxDuration = EntityStates.MiniMushroom.Plant.baseMaxDuration;
        public static float baseMinDuration = EntityStates.MiniMushroom.Plant.baseMinDuration;
        public static float mushroomRadius = 60;
        public static string healSoundLoop = EntityStates.MiniMushroom.Plant.healSoundLoop;
        public static string healSoundStop = EntityStates.MiniMushroom.Plant.healSoundStop;
        private float maxDuration;

        //Don't forget to fetch these values from the .cctor!

        private float castTimer;

        private float sunTime;

        private float meleeCharacterSuck;

        public static float baseDuration = 15f;
        private float duration;

        public static float explosionDelay = 2f;

        public static int explosionCount = 60;

        public static GameObject delayPrefab = PrefabCore.pillar;

        public static float damageCoefficient = 5;
        public static float randomRadius = 16;
        public static float radius = 4;

        private float minDuration;
        private GameObject mushroomWard;
        private uint soundID;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Plant", "PlantLoop");
            this.maxDuration = baseMaxDuration / this.attackSpeedStat;
            this.minDuration = baseMinDuration / this.attackSpeedStat;
            this.soundID = Util.PlaySound(healSoundLoop, base.characterBody.modelLocator.modelTransform.gameObject);
            duration = baseDuration / attackSpeedStat;
            sunTime = duration / (float)explosionCount / attackSpeedStat;
            if (!NetworkServer.active)
            {
                return;
            }

            if (this.mushroomWard == null)
            {
                this.mushroomWard = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MiniMushroomWard"), base.characterBody.footPosition, Quaternion.identity);
                this.mushroomWard.GetComponent<TeamFilter>().teamIndex = base.teamComponent.teamIndex;
                if (this.mushroomWard)
                {
                    HealingWard component = this.mushroomWard.GetComponent<HealingWard>();
                    component.healFraction = healFraction;
                    component.healPoints = 0f;
                    component.radius = mushroomRadius;
                }
                NetworkServer.Spawn(this.mushroomWard);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            castTimer += Time.fixedDeltaTime;
            meleeCharacterSuck += Time.fixedDeltaTime;

            if (castTimer >= sunTime)
            {
                PlacePillar();
                castTimer -= sunTime;
            }
            if (meleeCharacterSuck >= 2) {
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
                meleeCharacterSuck = 0;
            }

            if (base.isAuthority)
            {
                if (base.fixedAge > this.maxDuration || (base.fixedAge > this.minDuration) || base.inputBank.moveVector.sqrMagnitude > 0.1f)
                {
                    this.outer.SetNextState(new Unplant());
                }
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
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

                    Vector3 hitPoint = raycastHit.point;

                    //rotation

                    Quaternion rotation = Util.QuaternionSafeLookRotation(raycastHit.normal);

                    //ProjectileManager.instance.FireProjectile(SporeGrenades.projectilePrefab, gameObject.transform.position, rotation, base.gameObject, this.damageStat * SporeGrenades.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null);


                    var blastObject = Object.Instantiate<GameObject>(delayPrefab, hitPoint, rotation);
                    var delayBlast = blastObject.GetComponent<DelayBlast>();

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
                    delayBlast.position = hitPoint;
                    delayBlast.procCoefficient = 1;
                    delayBlast.radius = radius;

                    blastObject.GetComponent<TeamFilter>().teamIndex = GetTeam();
                    blastObject.transform.localScale = new Vector3(radius, radius, 1f);

                    var particles = blastObject.GetComponent<ScaleParticleSystemDuration>();
                    if (particles)
                    {
                        particles.newDuration = explosionDelay;
                    }
                }
            }
            //Make sure that our aimray isn't aiming at nothing
            /*Vector3 vector = Vector3.zero;
            Ray aimRay = GetAimRay();
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

        public override void OnExit()
        {
            base.PlayAnimation("Plant", "Empty");
            AkSoundEngine.StopPlayingID(this.soundID);
            Util.PlaySound(healSoundStop, base.gameObject);
            if (this.mushroomWard)
            {
                EntityState.Destroy(this.mushroomWard);
            }
            base.OnExit();
        }

    }
}