using BepInEx.Configuration;
using Cloudburst.Content;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Cloudburst.Cores.Items.Green
{

    public class BlastBoots : ItemBuilder
    {
        public override string ItemName => "Blast Boot";

        public override string ItemLangTokenName => "BLASTBOOT";

        public override string ItemPickupDesc => "Firework-powered double jump upon Secondary Skill activation.";

        public override string ItemFullDescription => "Activating your Secondary skill also blasts you through the air with a flurry of of 4 <style=cStack>(+1)</style> <style=cIsDamage>fireworks that deal 100% <style=cStack>(+50%)</style> damage</style>. This effect has a cooldown of 3 seconds.";

        public override string ItemLore => "";

        public override ItemTag[] ItemTags => new ItemTag[3]
{
    ItemTag.AIBlacklist,
    ItemTag.Utility,
    ItemTag.Damage
    };

        public override ItemTier Tier => ItemTier.Tier1;

        public override string ItemModelPath => "Assets/Cloudburst/Items/CarePackageRequester/IMDLBlastBoot.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/CarePackageRequester/icon.png";

        public static GameObject fireworkPrefab;

        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var MDL = Load();
            //LogCore.LogI(fabDagMDL);
            //fabDagMDL..SetTexture("_NormalTex", texturehere);
            //material.SetFloat("_NormalStrength", normalstrengthhere);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();//new ItemDisplayRule[]
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[] {

    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "CalfL",
localPos = new Vector3(0.02731F, 0.28558F, -0.01864F),
localAngles = new Vector3(7.92373F, 187.8987F, 185.4126F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)

}
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "FootL",
localPos = new Vector3(0.01535F, 0.00952F, -0.11311F),
localAngles = new Vector3(290.3859F, 87.12296F, 267.2033F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
}
            }); rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "Base",
localPos = new Vector3(-0.00199F, 3.38665F, -2.82651F),
localAngles = new Vector3(350.9689F, 354.3693F, 177.4269F),
localScale = new Vector3(1.4F, 1.4F, 1.4F)
    }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "CalfL",
localPos = new Vector3(0.07311F, 0.09037F, -0.00888F),
localAngles = new Vector3(12.57096F, 162.2495F, 178.7365F),
localScale = new Vector3(0.47F, 0.47F, 0.6F) }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "FootL",
localPos = new Vector3(-0.00998F, 0.01119F, -0.06469F),
localAngles = new Vector3(315.5827F, 187.5823F, 189.1592F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)        }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "FootL",
localPos = new Vector3(0.03039F, 0.01253F, -0.11448F),
localAngles = new Vector3(304.5077F, 186.6602F, 173.0096F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
    }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "FootL",
localPos = new Vector3(0.006F, -0.10728F, -0.20047F),
localAngles = new Vector3(318.6605F, 150.0478F, 194.3605F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
    }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
    new ItemDisplayRule
    {
        ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL,
childName = "CalfL",
localPos = new Vector3(0.17916F, 2.85797F, -0.95134F),
localAngles = new Vector3(10.90281F, 181.5084F, 186.1528F),
localScale = new Vector3(3F, 3F, 3F)
    }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "CalfL",
localPos = new Vector3(0.06974F, 0.19848F, -0.15316F),
localAngles = new Vector3(18.68582F, 179.9198F, 188.5853F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
    }
            }); rules.Add("mdlTreebot", new ItemDisplayRule[]
 {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "FootFrontLEnd",
localPos = new Vector3(-0.02173F, 0.0097F, -0.02386F),
localAngles = new Vector3(0.31136F, 150.9228F, 175.0508F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
    }
 }); rules.Add("mdlBandit2", new ItemDisplayRule[]
  {
    new ItemDisplayRule
        {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
        followerPrefab = MDL  ,
childName = "CalfL",
localPos = new Vector3(0.05839F, 0.35845F, -0.08628F),
localAngles = new Vector3(9.67126F, 197.2447F, 199.8692F),
localScale = new Vector3(0.32F, 0.32F, 0.32F)
    }
  });


            return rules;
        }

        protected override void Initialization()
        {


            GameObject firework = Resources.Load<GameObject>("prefabs/projectiles/CrocoSpit").InstantiateClone("EasyFirework");

            GameObject orig = Resources.Load<GameObject>("prefabs/projectiles/FireworkProjectile");

            ProjectileController controler = firework.GetComponent<ProjectileController>();

            controler.ghostPrefab = orig.GetComponent<ProjectileController>().ghostPrefab;

            foreach (var shit in firework.GetComponents<AkEvent>()) {
                CloudburstPlugin.Destroy(shit);
            }

            foreach (var originalsound in orig.GetComponents<AkEvent>()) {
                //var akEvent = firework.AddComponent<AkEvent>();
                CloudUtils.CopyComponent<AkEvent>(originalsound, firework);
            }

            firework.GetComponent<ProjectileImpactExplosion>().impactEffect = orig.GetComponent<ProjectileImpactExplosion>().impactEffect;
            firework.GetComponent<ProjectileDamage>().damageType = DamageType.Stun1s;      /*MissileController control = firework.Ad<MissileController>();

            control.maxSeekDistance = 0;
            control.giveupTimer = 4;
            control.deathTimer =4;
            control.turbulence = 16;*/
            //CloudburstPlugin.Destroy(firework.GetComponent<ProjectileTargetComponent>());
            //CloudburstPlugin.Destroy(firework.GetComponent<QuaternionPID>());

            //ProjectileSimple propeller = firework.AddComponent<ProjectileSimple>();
            //propeller.lifetime = 7;
            //propeller.desiredForwardSpeed = 50;
            fireworkPrefab = firework;
            ContentHandler.Projectiles.RegisterProjectile(fireworkPrefab);
        }

        public override void Hooks()
        {   
            //On.RoR2.Projectile.MissileController.FindTarget += MissileController_FindTarget;
            GlobalHooks.onInventoryChanged += GlobalHooks_onInventoryChanged;
        }

        private Transform MissileController_FindTarget(On.RoR2.Projectile.MissileController.orig_FindTarget orig, MissileController self)
        {
            //this is retarded
            if (self.gameObject.name == "EasyFirework (Clone)") {
                return null;
            }
            return orig(self);
        }

        private void GlobalHooks_onInventoryChanged(CharacterBody body)
        {
            body.AddItemBehavior<BlastBootBehavior>(GetCount(body));
        }
    }
    public class BlastBootBehavior : CharacterBody.ItemBehavior
    {
        public float timer = 3;
        public void Start()
        {
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            orig(self);
            if (body.skillLocator.secondary == self && body.characterMotor && !body.characterMotor.isGrounded && timer >= 3)
            {
                Vector3 aimer = Vector3.down;
                for (int j = 0; j < 3 + (stack * 1); j++)
                {
                    body.characterMotor.velocity.y += body.jumpPower * .3f;
                    body.characterMotor.Motor.ForceUnground(); 
                    
                    float theta = Random.Range(0.0f, 6.28f);
                    float x = Mathf.Cos(theta);
                    float z = Mathf.Sin(theta);
                    float c = j * 0.3777f;
                    c *= (1f / 12f);
                    aimer.x += c * x;
                    aimer.z += c * z;
                    ProjectileManager.instance.FireProjectile(BlastBoots.fireworkPrefab,
                        base.transform.position,
                        Util.QuaternionSafeLookRotation(aimer),
                        body.gameObject,
                        0.5f + (stack * 0.5f) * body.damage, 
                        5000f,
                        body.RollCrit(),
                        DamageColorIndex.Item,
                        null, 
                        -1
                        ) ;
                    aimer = Vector3.down;
                    timer = 0;
                }
            }
        }
    }
}