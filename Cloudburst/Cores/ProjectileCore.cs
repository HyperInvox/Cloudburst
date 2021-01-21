using BepInEx;
using Cloudburst.Cores.Components;
using Moonstorm.Cores.Components;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores
{
    class ProjectileCore
    {
        public ProjectileCore instance;

        public static GameObject bombardierBombProjectile;
        public static GameObject bombardierFireBombProjectile;
        public static GameObject bombardierSeekerBombProjectile;

        //public static GameObject mushrumDelaySproutingMushroom;
        public static GameObject mushrumSproutingMushroom;

        public static GameObject stickyProjectile;
        private GameObject stickyGhost;

        public static GameObject indicator;

        private GameObject winchGhost;
        public static GameObject winch;

        public static GameObject orbitalOrb;
        public static GameObject orbitalOrbGhost;

        public static GameObject wyattMaidBubble;

        public static GameObject electricPillar;
        public static GameObject electricPillarGhost;
        public static GameObject eliteElectricProjectile;

        public ProjectileCore() => CreateProjectiles();


        protected void CreateProjectiles()
        {
            instance = this;

            LogCore.LogI("Initializing Core: " + base.ToString());

            CreateBombardierProjectiles();
            CreateWyattMaidBubble();
            CreateOrbitalOrbs();
            CreateWinchGhost();
            CreateWinchProjectile();

            CloudburstPlugin.start += CloudburstPlugin_start;
        }

        private void CloudburstPlugin_start()
        {
            orbitalOrbGhost = Resources.Load<GameObject>("prefabs/projectileghosts/RedAffixMissileGhost").InstantiateClone("VoidOrbGhost", false);
            orbitalOrbGhost.transform.Find("Sphere").GetComponent<Renderer>().materials = new Material[] {
                VoidCore.voidMat
            };

            orbitalOrb.GetComponent<ProjectileController>().ghostPrefab = orbitalOrbGhost;
        }

        #region Misc   
        #endregion
        #region Elite

        protected void CreateElectricPillarGhost() {
            electricPillarGhost = Resources.Load<GameObject>("prefabs/projectileghosts/BrotherFirePillarGhost").InstantiateClone("OverchargedPillarGhost", false);
            //var temptile = Resources.Load<GameObject>("prefabs/projectileghosts/ElectricOrbGhost");
            //var mat = temptile.transform.Find("Helix/Trail").GetComponent<Renderer>().material;
            var mat = AssetsCore.mainAssetBundle.LoadAsset<Material>("Assets/Cloudburst/753network/Crystallize/Materials/Ramp Foil.mat");

            electricPillarGhost.transform.Find("Scale/Glow, Looping").GetComponent<Renderer>().material = mat;
            electricPillarGhost.transform.Find("Scale/Glow, Initial").gameObject.SetActive(false);
            electricPillarGhost.transform.Find("Scale/Fire, Directional").GetComponent<Renderer>().material = mat;
            electricPillarGhost.transform.Find("Scale/Rock Particles, Fast").gameObject.SetActive(false);
        }

        protected void CreateElectricPillar() {
            electricPillar = Resources.Load<GameObject>("prefabs/projectiles/BrotherFirePillar").InstantiateClone("OverchargedPillar", true);
            electricPillar.GetComponent<ProjectileController>().ghostPrefab = electricPillarGhost;
        }

        protected void CreateElectricProjectile() {
            eliteElectricProjectile = Resources.Load<GameObject>("prefabs/projectiles/ElectricOrbProjectile").InstantiateClone("OverchargedProjectile", true);
            var impact = eliteElectricProjectile.GetComponent<ProjectileImpactExplosion>();
            impact.childrenCount = 1;
            impact.childrenProjectilePrefab = electricPillar;
            impact.minAngleOffset = new Vector3(0, 0, 0);
            impact.maxAngleOffset = new Vector3(0, 0, 0);
        }

        #endregion
        #region Mega Mushrum
        protected private void CreateDelaySproutingMushroom()
        {
            //mushrumDelaySproutingMushroom = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile").InstantiateClone("MegaMushrumDelaySproutingMushroom", true);
            //if (CloudUtils.RegisterNewProjectile(MIRVProjectile))
            //{
            //    mushrumDelaySproutingMushroom.GetComponent<ProjectileImpactExplosion>().childrenProjectilePrefab = mushrumSproutingMushroom;
            //}
        }
        protected private void CreateSproutingMushroom()
        {
            mushrumSproutingMushroom = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone").InstantiateClone("MegaMushrumSproutingMushroom", true);
            if (CloudUtils.RegisterNewProjectile(mushrumSproutingMushroom)) //&& API.RegisterNewProjectile(MIRVClusterProjectile))
            {
                var collider = mushrumSproutingMushroom.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                mushrumSproutingMushroom.AddComponent<SproutingProjectileZone>();

                Transform container = mushrumSproutingMushroom.transform.Find("FX/ScaledOnImpact");
                Transform fx = mushrumSproutingMushroom.transform.Find("FX");


                foreach (Transform child in container)
                {
                    if (child.name != "IndicatorSphere")
                    {
                        //child.gameObject.SetActive(false); 
                        CloudburstPlugin.Destroy(child.gameObject);
                    }
                    else
                    {
                        indicator = child.gameObject.InstantiateClone("GenericIndicator", false);
                    }
                }
                CloudburstPlugin.Destroy(container.gameObject);
                CloudburstPlugin.Destroy(fx.gameObject);

                var newChild = CloudburstPlugin.Instantiate<GameObject>(indicator);
                newChild.transform.SetParent(mushrumSproutingMushroom.transform);
            }
        }
        #endregion
        #region Bombardier
        protected private void CreateBombardierProjectiles() {
            CreateBombardierRocketProjectile();
            CreateBombardierBetterRocketProjectile();
            CreateBombardierPayloadProjectile();
            CreateStickyGhost();
            CreateStickyProjectile();
        }

        private protected void CreateStickyGhost() {
            stickyGhost = Resources.Load<GameObject>("prefabs/projectileghosts/EngiMineGhost").InstantiateClone("BombardierStickyGhost", false);
            foreach (Transform child in stickyGhost.transform)
            {
                //destroy the child. 
                BaseUnityPlugin.Destroy(child.gameObject);
            }
            BaseUnityPlugin.Destroy(stickyGhost.GetComponent<EngiMineAnimator>());
            BaseUnityPlugin.Destroy(stickyGhost.GetComponent<EngiMineGhostController>());

            var newGhost = BaseUnityPlugin.Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("mdlSatchel"));
            newGhost.transform.SetParent(stickyGhost.transform);
        }

        private protected void CreateWinchGhost()
        {
            winchGhost = Resources.Load<GameObject>("prefabs/projectileghosts/SolarFlareGhost").InstantiateClone("BombardierStickyGhost", false);
            foreach (Transform child in winchGhost.transform)
            {
                //destroy the child. 
                BaseUnityPlugin.Destroy(child.gameObject);
            }
            //BaseUnityPlugin.Destroy(stickyGhost.GetComponent<EngiMineAnimator>());
            //BaseUnityPlugin.Destroy(stickyGhost.GetComponent<EngiMineGhostController>());

            var newGhost = BaseUnityPlugin.Instantiate<GameObject>(AssetsCore.mainAssetBundle.LoadAsset<GameObject>("WinchGhost"));
            var winchController = newGhost.AddComponent<Components.WinchControl>();
            winchController.tailTransform = newGhost.transform.Find("Model/mdlHANDWinch/WinchArmature/WinchTail");
            newGhost.transform.SetParent(winchGhost.transform);

        }

        protected private void CreateStickyProjectile()
        {
            stickyProjectile = Resources.Load<GameObject>("prefabs/projectiles/EngiMine").InstantiateClone("MIRVEquipmentProjectile", true);
            if (CloudUtils.RegisterNewProjectile(stickyProjectile))
            {
                stickyProjectile.AddComponent<BombardierStickyBombProjectile>();
                stickyProjectile.GetComponent<ProjectileController>().ghostPrefab = stickyGhost;

                var sticky = stickyProjectile.AddComponent<ProjectileStickOnImpact>();
                sticky.ignoreCharacters = false;
                sticky.ignoreWorld = false;
                sticky.alignNormals = false;

                var rigidBody = stickyProjectile.GetComponent<Rigidbody>();
                rigidBody.freezeRotation = false;

                GameObject.Destroy(stickyProjectile.GetComponent<ProjectileDeployToOwner>());
                GameObject.Destroy(stickyProjectile.GetComponent<Deployable>());
                GameObject.Destroy(stickyProjectile.GetComponent<RTPCController>());
                foreach (var comp in stickyProjectile.GetComponents<EntityStateMachine>())
                {
                    GameObject.Destroy(comp);
                }
                GameObject.Destroy(stickyProjectile.GetComponent<NetworkStateMachine>());
                GameObject.Destroy(stickyProjectile.GetComponent<ProjectileSphereTargetFinder>());
                GameObject.Destroy(stickyProjectile.GetComponent<ProjectileTargetComponent>());


                //GameObject.Destroy(stickyProjectile.GetComponent<ProjectileTargetComponent>());
                //GameObject.Destroy(stickyProjectile.GetComponent<ProjectileTargetComponent>());
                //GameObject.Destroy(stickyProjectile.GetComponent<ProjectileTargetComponent>());
            }
            else LogCore.LogF("FATAL ERROR:" + stickyProjectile.name + " failed to register!");
        }

        protected private void CreateOrbitalOrbs() {
            orbitalOrb = Resources.Load<GameObject>("prefabs/projectiles/RedAffixMissileProjectile").InstantiateClone("OrbitalOrb", true);

            //MonoBehaviour.Destroy(orbitalOrb.GetComponent<BoxCollider>());

            //var collider = orbitalOrb.AddComponent<SphereCollider>();
            orbitalOrb.GetComponent<BoxCollider>().isTrigger = false;
            var orb = orbitalOrb.AddComponent<ProjectileProximityBeamController>();
            var explosion = orbitalOrb.AddComponent<ProjectileImpactExplosion>();
            var impact = orbitalOrb.GetComponent<ProjectileSingleTargetImpact>();
            var missile = orbitalOrb.GetComponent<MissileController>();

            missile.giveupTimer = float.MaxValue;
            missile.maxSeekDistance = float.MaxValue;
            missile.deathTimer = float.MaxValue;

            LogCore.LogI(-(float.MaxValue * 2));
            //missile.maxVelocity = float.MaxValue;

            impact.impactEffect = EffectCore.orbitalImpact;

            //MonoBehaviour.Destroy(impact);

            explosion.impactEffect = EffectCore.orbitalImpact;
            explosion.offsetForLifetimeExpiredSound = 0;
            explosion.destroyOnEnemy = true;
            explosion.destroyOnWorld = true;
            explosion.timerAfterImpact = false;
            explosion.falloffModel = BlastAttack.FalloffModel.None;
            explosion.lifetime = float.MaxValue;
            explosion.lifetimeAfterImpact = 0;
            explosion.lifetimeRandomOffset = 0;
            explosion.blastRadius = 8;
            explosion.blastDamageCoefficient = 1;
            explosion.blastProcCoefficient = 1;
            explosion.blastAttackerFiltering = AttackerFiltering.Default;

            explosion.childrenCount = 0;
            explosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            //collider.radius = 0.2f;

            orb.attackFireCount = 1;
            orb.attackInterval = 2;
            orb.listClearInterval = 0.1f;
            orb.attackRange = 20    ;
            orb.minAngleFilter = 0;
            orb.maxAngleFilter = 180;
            orb.procCoefficient = 1f;
            orb.damageCoefficient = 1;
            orb.bounces = 0;
            orb.inheritDamageType = false;
            orb.lightningType = RoR2.Orbs.LightningOrb.LightningType.Tesla;

            CloudUtils.RegisterNewProjectile(orbitalOrb);
            //Resources.Load<GameObject>("prefabs/effects/impacteffects/ParentSlamEffect");
        }

        protected private void CreateBombardierRocketProjectile() {
            bombardierBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("BombardierRocketProjectile", true);
            var registered = CloudUtils.RegisterNewProjectile(bombardierBombProjectile);
            if (registered)
            {
                var impact = bombardierBombProjectile.GetComponent<ProjectileImpactExplosion>();
                impact.blastProcCoefficient = 0.3f;
                impact.blastDamageCoefficient = 1.1f;
                //Object.Destroy(impact);

                //bombardierBombProjectile.AddComponent<Components.HookProjectileImpact>();
                //bombardierBombProjectile.GetComponent<ProjectileController>().ghostPrefab = winchGhost;
            }
            else LogCore.LogF("FATAL ERROR:" + bombardierBombProjectile.name + " failed to register to the projectile catalog!");
        }

        protected private void CreateWinchProjectile()
        {
            winch = Resources.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("WyattWinch", true);
            var registered = CloudUtils.RegisterNewProjectile(winch);
            if (registered)
            {
                var impact = winch.GetComponent<ProjectileImpactExplosion>();
                var target = winch.AddComponent<ProjectileTargetComponent>();
                var missileController = winch.AddComponent<MissileController>();
                var pid = winch.AddComponent<QuaternionPID>();
                var stick = winch.AddComponent<ProjectileStickOnImpact>();

                stick.stickSoundString = string.Empty;
                stick.stickParticleSystem = Array.Empty<ParticleSystem>();
                stick.ignoreCharacters = false;
                stick.ignoreWorld = true;
                stick.alignNormals = true;

                missileController.maxVelocity = 25;
                missileController.rollVelocity = 0;
                missileController.acceleration = 3;
                missileController.delayTimer = 0.1f;
                missileController.giveupTimer = 2;
                missileController.deathTimer = 2;
                missileController.turbulence = 0;
                missileController.maxSeekDistance = 1000;

                pid.PID = new Vector3(10, 0.3f, 0);
                pid.inputQuat = new Quaternion(0, 0, 0, 1);
                pid.targetQuat = new Quaternion(0, 0, 0, 1);
                pid.outputVector = new Vector3(0, 0, 0);
                pid.gain = 10;



                CloudburstPlugin.Destroy(impact);

                winch.AddComponent<WyattWinchManager/*:^))*/>();


                winch.GetComponent<ProjectileController>().ghostPrefab = winchGhost;



            }
            else LogCore.LogF("FATAL ERROR:" + winch.name + " failed to register to the projectile catalog!");
        }

        protected private void CreateBombardierBetterRocketProjectile()
        {
            bombardierFireBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("BombardierFireRocketProjectile", true);
            var registered = CloudUtils.RegisterNewProjectile(bombardierFireBombProjectile);
            if (registered)
            {
                var impact = bombardierFireBombProjectile.GetComponent<ProjectileImpactExplosion>();
                var controller = bombardierFireBombProjectile.GetComponent<ProjectileController>();
                var damage = bombardierFireBombProjectile.GetComponent<ProjectileDamage>();

                bombardierFireBombProjectile.AddComponent<RocketJumpOnImpact>();

                damage.damageType = RoR2.DamageType.Stun1s;

                impact.blastProcCoefficient = 0.3f;
                impact.blastDamageCoefficient = 1.1f;

                controller.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/ArchWispCannonGhost");
                controller.procCoefficient = 0.6f;

                impact.impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXArchWispCannonImpact");
                impact.blastProcCoefficient = 1f;
                impact.blastDamageCoefficient = 1f;
                impact.blastRadius = 25;


            }
            else LogCore.LogF("FATAL ERROR:" + bombardierFireBombProjectile.name + " failed to register to the projectile catalog!");
        }

        private protected void CreateWyattMaidBubble()
        {
            //wyattMaidBubble = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("WyattMaid");

            wyattMaidBubble = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("Attempt4");

            foreach (var thingie in wyattMaidBubble.GetComponent<ChildLocator>().transformPairs) {
                GameObject gameObject = thingie.transform.gameObject;
                switch (gameObject .name) {
                    case "CounterBalance":
                        var locator = gameObject.AddComponent<EntityLocator>();
                        locator.entity = gameObject.transform.parent.gameObject;

                        gameObject.layer = LayerIndex.entityPrecise.intVal;
                        break;
                    case "FakeActorCollider":
                        gameObject.layer = LayerIndex.fakeActor.intVal;

                        break;
                    case "GrappleCollider":
                        gameObject.layer = LayerIndex.fakeActor.intVal;

                        break;
                }

            }

            CloudUtils.CreateValidProjectile(wyattMaidBubble, float.MaxValue, 0, true);

            GameObject ward = Resources.Load<GameObject>("prefabs/networkedobjects/TimeBubbleWard");
            Transform origVisuals = ward.transform.Find("Visuals+Collider/Sphere");
            Renderer origRenderer = origVisuals.GetComponent<Renderer>();


            Transform friendly = wyattMaidBubble.transform.Find("FriendlyBuffWard");
            Transform slowProjectiles = wyattMaidBubble.transform.Find("FUCKHOPOO/ProjectileSlow");

            wyattMaidBubble.AddComponent<MAID>();

            CreateProjectileSlow();
            CreateBuffWard();

            void CreateProjectileSlow()
            {
                SlowEnemiesAndProjectiles();
                SetupSlowProjVisuals();
                void SlowEnemiesAndProjectiles()
                {
                    var slow = slowProjectiles.AddComponent<SlowDownProjectiles>();

                    slowProjectiles.gameObject.layer = LayerIndex.entityPrecise.intVal;

                    slow.teamFilter = wyattMaidBubble.GetComponent < TeamFilter>();
                    slow.maxVelocityMagnitude = 3;
                    slow.antiGravity = 1;
                }
                void SetupSlowProjVisuals()
                {
                    slowProjectiles.GetComponent<MeshRenderer>().materials = origRenderer.materials;
                }
            }
            void CreateBuffWard()
            {
                BuffWard buffWard = wyattMaidBubble.AddComponent<BuffWard>();
                buffWard.radius = 25;
                buffWard.interval = 0.2f;
                buffWard.rangeIndicator = slowProjectiles;
                buffWard.buffType = BuffCore.instance.antiGravIndex;
                buffWard.buffDuration = 0.3f;
                buffWard.floorWard = false;
                buffWard.expires = false;
                buffWard.invertTeamFilter = true;
                buffWard.expireDuration = 15;
                buffWard.animateRadius = false;
                buffWard.radiusCoefficientCurve = new AnimationCurve();
                buffWard.removalTime = 0;

                if (CloudburstPlugin.EnableWyattFreeFlight.Value)
                {
                    BuffWard buffWard2 = wyattMaidBubble.AddComponent<BuffWard>();
                    buffWard2.radius = 25;
                    buffWard2.interval = 0.5f;
                    buffWard2.rangeIndicator = null;
                    buffWard2.buffType = BuffCore.instance.antiGravFriendlyIndex;
                    buffWard2.buffDuration = 0.6f;
                    buffWard2.floorWard = false;
                    buffWard2.expires = false;
                    buffWard2.invertTeamFilter = false;
                    buffWard2.expireDuration = 15;
                    buffWard2.animateRadius = false;
                    buffWard2.radiusCoefficientCurve = new AnimationCurve();
                    buffWard2.removalTime = 0;
                }//buffWard.removalSoundString =
                //buffWard.onRemoval = UnityEngine.Events.UnityEvent UnityEngine.Events.UnityEvent
            }


            PrefabAPI.RegisterNetworkPrefab(wyattMaidBubble);

            CloudUtils.RegisterNewProjectile(wyattMaidBubble);

            #region what the fuck is wrong with me?
            /*var networkIdentity = wyattMaidBubble.AddComponent<NetworkIdentity>();
            var teamFilter = wyattMaidBubble.AddComponent<TeamFilter>();
            var controller = wyattMaidBubble.AddComponent<ProjectileController>();
            var networkTransform = wyattMaidBubble.AddComponent<ProjectileNetworkTransform>();
            var projectileSimple = wyattMaidBubble.AddComponent<ProjectileSimple>();
            var torque = wyattMaidBubble.AddComponent<ApplyTorqueOnStart>();

            SetupMAID();
            EnemyBuffWard();
            SetupProjectileSlow();

            void SetupMAID()
            {
                wyattMaidBubble.layer = LayerIndex.projectile.intVal;

                networkIdentity.localPlayerAuthority = true;

                controller.procCoefficient = 0;

                networkTransform.positionTransmitInterval = 0.03333334f;
                networkTransform.interpolationFactor = 1;
                networkTransform.allowClientsideCollision = false;

                projectileSimple.velocity = 0;
                projectileSimple.lifetime = float.MaxValue;
                projectileSimple.updateAfterFiring = false;
                projectileSimple.enableVelocityOverLifetime = false;
                projectileSimple.oscillate = false;
                projectileSimple.oscillateMagnitude = 20;
                projectileSimple.oscillateSpeed = 0;

                //torque.localTorque = new Vector3(6000, 6000, 6000);
                //torque.randomize = true;

                //removed cause it did nothing useful lmao
                var noGravForce = wyattMaidBubble.AddComponent<AntiGravityForce>();
                noGravForce.antiGravityCoefficient = 1f;
                noGravForce.rb = wyattMaidBubble.GetComponent<Rigidbody>();

                SetupGrapplCollider();
                    SetupCounterBalance();
                    SetupFakeActor();
                void SetupGrapplCollider()
                {
                    var grappleCollider = wyattMaidBubble.transform.Find("GrappleCollider");
                    var locator = grappleCollider.AddComponent<EntityLocator>();

                    grappleCollider.gameObject.layer = LayerIndex.entityPrecise.intVal;
                    locator.entity = wyattMaidBubble;
                }


                void SetupCounterBalance()
                {
                    var counterBalance = wyattMaidBubble.transform.Find("CounterBalance");
                    counterBalance.gameObject.layer = LayerIndex.noCollision.intVal;
                }

                void SetupFakeActor()
                {
                    var fakeActorCollider = wyattMaidBubble.transform.Find("FakeActorCollision");
                    fakeActorCollider.gameObject.layer = LayerIndex.fakeActor.intVal;
                }            }
            void EnemyBuffWard() {
                var buffWardObject = wyattMaidBubble.transform.Find("Buffwards/EnemyBuffward");
                BuffWard();
                void BuffWard()
                {
                    //buffward
                    var buffWard = buffWardObject.AddComponent<BuffWard>();
                    buffWard.radius = 45;
                    buffWard.interval = 0.2f;
                    //buffWard.rangeIndicator = wyattMaidBubble.transform.Find("Buffwards/EnemyBuffward/SlowEnemiesAndProjectiles");
                    buffWard.buffType = BuffCore.instance.antiGravIndex;
                    buffWard.buffDuration = 2;
                    buffWard.floorWard = false;
                    buffWard.expires = false;
                    buffWard.invertTeamFilter = true;
                    buffWard.expireDuration = 15;
                    buffWard.animateRadius = false;
                    buffWard.radiusCoefficientCurve = ward.GetComponent<BuffWard>().radiusCoefficientCurve;
                    buffWard.removalTime = 0;
                    //buffWard.removalSoundString =
                    //buffWard.onRemoval = UnityEngine.Events.UnityEvent UnityEngine.Events.UnityEvent
                }
            }
            void SetupProjectileSlow()
            {
                SlowEnemiesAndProjectiles();
                SetupSlowProjVisuals();

                void SlowEnemiesAndProjectiles()
                {
                    var slowProjectiles = wyattMaidBubble.transform.Find("Buffwards/EnemyBuffward/SlowEnemiesAndProjectiles");
                    var slow = slowProjectiles.AddComponent<SlowDownProjectiles>();

                    slowProjectiles.gameObject.layer = LayerIndex.entityPrecise.intVal;

                    slow.teamFilter = teamFilter;
                    slow.maxVelocityMagnitude = 3;
                    slow.antiGravity = 1;
                }
                void SetupSlowProjVisuals()
                {
                    var slowProjectilesVisuals = wyattMaidBubble.transform.Find("Buffwards/SlowEnemiesAndProjectiles/Visuals");
                    slowProjectilesVisuals.GetComponent<MeshRenderer>().materials = origRenderer.materials;
                }
            }

            CloudUtils.RegisterNewProjectile(wyattMaidBubble);

            var mdl = wyattMaidBubble.transform.Find("mdlWyattMaid");
            var counterBalance = wyattMaidBubble.transform.Find("CounterBalance");
            var fakeActorCollider = wyattMaidBubble.transform.Find("FakeActorCollider");

            counterBalance.gameObject.layer = LayerIndex.noCollision.intVal;
            fakeActorCollider.gameObject.layer = LayerIndex.fakeActor.intVal;

            var antiGravDummy = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/PlayerAntiGrav");
            var buffWardObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard");
            var projectileSlowObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/SlowDownProjectiles");
            var visualsObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/Visuals"); //visuals
            var scaleWithRadius = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius");

            var visualsRenderer = visualsObject.GetComponent<Renderer>();

            SetupBuffward();
            SetupSlowProjectiles();
            SetupVisuals();

            void SetupBuffward() {
                TeamFilter();
                BuffWard();

                void TeamFilter() {
                    var teamFilter = buffWardObject.AddComponent<TeamFilter>();
                    teamFilter.teamIndex = TeamIndex.Player;
                }
                void BuffWard() {
                    //buffward
                    var buffWard = buffWardObject.AddComponent<BuffWard>();
                    buffWard.radius = 45;
                    buffWard.interval = 0.2f;
                    buffWard.rangeIndicator = scaleWithRadius;
                    buffWard.buffType = BuffCore.instance.antiGravIndex;
                    buffWard.buffDuration = 2;
                    buffWard.floorWard = false;
                    buffWard.expires = false;
                    buffWard.invertTeamFilter = true;
                    buffWard.expireDuration = 15;
                    buffWard.animateRadius = false;
                    buffWard.radiusCoefficientCurve = ward.GetComponent<BuffWard>().radiusCoefficientCurve;
                    buffWard.removalTime = 0;
                    //buffWard.removalSoundString =
                    //buffWard.onRemoval = UnityEngine.Events.UnityEvent UnityEngine.Events.UnityEvent
                }
            }
            void SetupSlowProjectiles() {
                projectileSlowObject.gameObject.layer = 12;

                var projectileSlow = projectileSlowObject.AddComponent<SlowDownProjectiles>();
                projectileSlow.maxVelocityMagnitude = 3;
                projectileSlow.antiGravity = -0.5f;
            }
            void SetupVisuals() {
                //you're flesh and blood - but what's underneath?
                visualsRenderer.materials = origRenderer.materials;
                //var rotate = visualsObject.AddComponent<Rewired.ComponentControls.Effects.RotateAroundAxis>
            }*/

            PrefabAPI.RegisterNetworkPrefab(wyattMaidBubble);
            CloudUtils.RegisterNewProjectile(wyattMaidBubble);
            //var registered = API.RegisterNewProjectile(wyattMaidBubble);
            //if (registered)
            //{
            /*var origTimeWard = Resources.Load<GameObject>("prefabs/networkedobjects/TimeBubbleWard");

            var activatedWard = Object.Instantiate<GameObject>(origTimeWard);

            wyattMaidBubble.GetComponent<ProjectileSimple>().lifetime = 30;
            wyattMaidBubble.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 25;

            Object.Destroy(wyattMaidBubble.GetComponent<ProjectileProximityBeamController>());

            wyattMaidBubble.AddComponent<MAID>();
            wyattMaidBubble.GetComponent<SphereCollider>().isTrigger = true;
            wyattMaidBubble.GetComponent<AntiGravityForce>().antiGravityCoefficient = 2;

            activatedWard.GetComponent<BuffWard>().radius = 15;
            activatedWard.GetComponent<BuffWard>().buffType = BuffCore.instance.antiGravIndex;
            activatedWard.transform.SetParent(wyattMaidBubble.transform);
            activatedWard.transform.position = new Vector3(0, 0, 0);
            activatedWard.transform.localScale = new Vector3(1f, 1f, 1f);
            //activatedWard.transform.Find("Visuals+Collider").gameObject.layer = 0;
            //activatedWard.transform.Find("Visuals+Collider").gameObject.AddComponent<BoxCollider>();
            //activatedWard.transform.Find("Visuals+Collider").gameObject.AddComponent<FliteredNoGravZone>();*/
            //}
            //else LogCore.LogF("FATAL ERROR:" + wyattMaidBubble.name + " failed to register to the projectile catalog!");
            #endregion



        }

        private protected void CreateBombardierPayloadProjectile()
        {
            bombardierSeekerBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile").InstantiateClone("BombardierSeekerRocketProjectile", true);
            var registered = CloudUtils.RegisterNewProjectile(bombardierFireBombProjectile);
            if (registered)
            {
                var controller = bombardierSeekerBombProjectile.GetComponent<ProjectileController>();
                var impact = bombardierSeekerBombProjectile.AddOrGetComponent<ProjectileImpactExplosion>();
                var missile = bombardierSeekerBombProjectile.GetComponent<MissileController>();

                missile.deathTimer = 15;
                missile.giveupTimer = 10;
                missile.turbulence = 0;
                missile.maxSeekDistance = 200;

                controller.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/ElectricOrbGhost");
                controller.procCoefficient = 0.6f;

                impact.blastAttackerFiltering = RoR2.AttackerFiltering.Default;
                impact.blastDamageCoefficient = 2.5f;
                impact.blastProcCoefficient = 1;
                impact.blastRadius = 10;
                impact.childrenCount = 0;
                impact.childrenDamageCoefficient = 0;
                impact.childrenProjectilePrefab = null;
                impact.destroyOnEnemy = true;
                impact.destroyOnWorld = true;
                impact.falloffModel = RoR2.BlastAttack.FalloffModel.None;
                impact.fireChildren = false;
                impact.impactEffect = Resources.Load<GameObject>("prefabs/effects/LightningStakeNova");
                impact.lifetime = 10;
                impact.lifetimeAfterImpact = 0;
                impact.lifetimeRandomOffset = 0;
            }
            else LogCore.LogF("FATAL ERROR:" + bombardierSeekerBombProjectile.name + " failed to register to the projectile catalog!");
        }
        #endregion
    }
}
