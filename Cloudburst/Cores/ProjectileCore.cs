using BepInEx;
using Cloudburst.Cores.Components;
using Moonstorm.Cores.Components;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores
{
    class ProjectileCore
    {
        public ProjectileCore instance;

        public static GameObject bombardierBombProjectile;
        public static GameObject bombardierFireBombProjectile;
        public static GameObject bombardierSeekerBombProjectile;

        public static GameObject mushrumDelaySproutingMushroom;
        public static GameObject mushrumSproutingMushroom;

        public static GameObject MIRVProjectile;
        public static GameObject MIRVClusterProjectile;

        public static GameObject stickyProjectile;
        private GameObject stickyGhost;

        public static GameObject indicator;

        private GameObject winchGhost;
        public static GameObject winch;

        public static GameObject orbitalOrb;

        public static GameObject wyattMaidBubble;

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
            CreateMiscProjectiles();
            CreateSproutingMushroom();
            CreateDelaySproutingMushroom();     
        }

        #region Misc   
        protected private void CreateMiscProjectiles() {
            CreateMIRVProjectile();
        }

        protected private void CreateMIRVProjectile()
        {
            MIRVProjectile = Resources.Load<GameObject>("prefabs/projectiles/CryoCanisterBombletsProjectile").InstantiateClone("MIRVEquipmentProjectile", true);
            MIRVClusterProjectile = Resources.Load<GameObject>("prefabs/projectiles/CryoCanisterBombletsProjectile").InstantiateClone("MIRVClusterEquipmentProjectile", true);
            if (API.RegisterNewProjectile(MIRVProjectile) && API.RegisterNewProjectile(MIRVClusterProjectile))
            {
            }
            else LogCore.LogF("FATAL ERROR:" + MIRVProjectile.name + " failed to register!");
        }
        #endregion
        #region Mega Mushrum
        protected private void CreateDelaySproutingMushroom()
        {
            mushrumDelaySproutingMushroom = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile").InstantiateClone("MegaMushrumDelaySproutingMushroom", true);
            if (API.RegisterNewProjectile(MIRVProjectile))
            {
                mushrumDelaySproutingMushroom.GetComponent<ProjectileImpactExplosion>().childrenProjectilePrefab = mushrumSproutingMushroom;
            }
        }
        protected private void CreateSproutingMushroom()
        {
            mushrumSproutingMushroom = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone").InstantiateClone("MegaMushrumSproutingMushroom", true);
            if (API.RegisterNewProjectile(mushrumSproutingMushroom)) //&& API.RegisterNewProjectile(MIRVClusterProjectile))
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
                        Object.Destroy(child.gameObject);
                    }
                    else
                    {
                        indicator = child.gameObject.InstantiateClone("GenericIndicator", false);
                    }
                }
                Object.Destroy(container.gameObject);
                Object.Destroy(fx.gameObject);

                var newChild = Object.Instantiate<GameObject>(indicator);
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
            if (API.RegisterNewProjectile(stickyProjectile))
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

            orb.attackFireCount = 6;
            orb.attackInterval = 1;
            orb.listClearInterval = 0.1f;
            orb.attackRange = 25;
            orb.minAngleFilter = 0;
            orb.maxAngleFilter = 180;
            orb.procCoefficient = 0.5f;
            orb.damageCoefficient = 1;
            orb.bounces = 0;
            orb.inheritDamageType = false;
            orb.lightningType = RoR2.Orbs.LightningOrb.LightningType.Tesla;

            API.RegisterNewProjectile(orbitalOrb);
            //Resources.Load<GameObject>("prefabs/effects/impacteffects/ParentSlamEffect");
        }

        protected private void CreateBombardierRocketProjectile() {
            bombardierBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("BombardierRocketProjectile", true);
            var registered = API.RegisterNewProjectile(bombardierBombProjectile);
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
            var registered = API.RegisterNewProjectile(winch);
            if (registered)
            {
                var impact = winch.GetComponent<ProjectileImpactExplosion>();

                Object.Destroy(impact);

                winch.AddComponent<Components.HookProjectileImpact>();
                winch.GetComponent<ProjectileController>().ghostPrefab = winchGhost;
            }
            else LogCore.LogF("FATAL ERROR:" + winch.name + " failed to register to the projectile catalog!");
        }

        protected private void CreateBombardierBetterRocketProjectile()
        {
            bombardierFireBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/PaladinRocket").InstantiateClone("BombardierFireRocketProjectile", true);
            var registered = API.RegisterNewProjectile(bombardierFireBombProjectile);
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
            wyattMaidBubble = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("WyattMaid");

            API.CreateValidProjectile(wyattMaidBubble, float.MaxValue, 0, true);

            //game objects
            var activatedWard = Resources.Load<GameObject>("prefabs/networkedobjects/TimeBubbleWard");

            var origVisuals = activatedWard.transform.Find("Visuals+Collider/Sphere");
            var origRenderer = origVisuals.GetComponent<Renderer>();

            var mdl = wyattMaidBubble.transform.Find("mdlWyattMaid");
            var antiGravDummy = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/PlayerAntiGrav");
            var buffWardObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard");
            var projectileSlowObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/SlowDownProjectiles");
            var visualsObject = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius/Visuals"); //visuals
            var scaleWithRadius = wyattMaidBubble.transform.Find("TimeBubble/BuffWard/ScaleWithRadius");

            var visualsRenderer = visualsObject.GetComponent<Renderer>();

            wyattMaidBubble.AddComponent<MAID>();

            var speen = mdl.AddComponent<Spinner>();

            //removed cause it did nothing useful lmao
            //var noGravForce = wyattMaidBubble.AddComponent<AntiGravityForce>();
            //noGravForce.antiGravityCoefficient = 0.1f;
            //noGravForce.rb = wyattMaidBubble.GetComponent<Rigidbody>();

            SetupBuffward();
            SetupSlowProjectiles();
            SetupVisuals();

            //projectileSlowObject.gameObject.SetActive(false);

            var noGravZone = antiGravDummy.AddComponent<FliteredNoGravZone>();
            
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
                    buffWard.radiusCoefficientCurve = activatedWard.GetComponent<BuffWard>().radiusCoefficientCurve;
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
            }

            PrefabAPI.RegisterNetworkPrefab(wyattMaidBubble);
            API.RegisterNewProjectile(wyattMaidBubble);
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
        }

        private protected void CreateBombardierPayloadProjectile()
        {
            bombardierSeekerBombProjectile = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile").InstantiateClone("BombardierSeekerRocketProjectile", true);
            var registered = API.RegisterNewProjectile(bombardierFireBombProjectile);
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
