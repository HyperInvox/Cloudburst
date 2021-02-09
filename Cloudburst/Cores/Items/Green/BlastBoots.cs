using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace Cloudburst.Cores.Items.Green
{

    public class BlastBoots : ItemBuilder
    {
        public override string ItemName => "Blast Boots";

        public override string ItemLangTokenName => "BLASTBOOTS";
        public override string ItemPickupDesc => "Using your secondary in midair propels you upwards and<style=cIsDamage> spawns an explosion around you and beneath you that both deal 250% <style=cStack>(250% per stack)</style> damage, and inflict burn</style>";

        public override string ItemFullDescription => "On use of your secondary, spawn an explosion beneath you and around if in midair.";

        public override string ItemLore => "";

        public override ItemTag[] ItemTags => new ItemTag[2]
{
    ItemTag.AIBlacklist,
    ItemTag.Utility
    };

    public override ItemTier Tier => ItemTier.Tier2;

        public override string ItemModelPath => "@Cloudburst:Assets/Cloudburst/Items/CarePackageRequester/IMDLCarePackageRequester.prefab";

        public override string ItemIconPath => "@Cloudburst:Assets/Cloudburst/Items/CarePackageRequester/icon.png";


        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override void Initialization()
        {

        }

        public override void Hooks()
        {
            GlobalHooks.onInventoryChanged += GlobalHooks_onInventoryChanged;
        }

        private void GlobalHooks_onInventoryChanged(CharacterBody body)
        {
            body.AddItemBehavior<BlastBootBehavior>(GetCount(body));
        }
    }
    public class BlastBootBehavior : CharacterBody.ItemBehavior
    {
        public void Start()
        {
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            orig(self);
            if (body.skillLocator.secondary == self && body.characterMotor && !body.characterMotor.isGrounded)
            {
                float multiplier = 2.5f * stack;

                var beastieBoys = new BlastAttack
                {
                    //position = raycastHit.point,
                    //baseForce = 3000,
                    attacker = null,
                    inflictor = null,
                    teamIndex = body.teamComponent.teamIndex,
                    baseDamage = body.damage * multiplier,
                    attackerFiltering = default,
                    //bonusForce = new Vector3(0, -3000, 0),
                    damageType = DamageType.PercentIgniteOnHit, //| DamageTypeCore.spiked,
                    crit = body.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = 0,
                    radius = 10
                };
                beastieBoys.position = base.transform.position;
                beastieBoys.Fire();

                RaycastHit raycastHit;
                //check my style,
                if (Physics.Raycast(body.transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
                {
                    body.characterMotor.velocity.y = body.jumpPower * 1.5f;
                    body.characterMotor.Motor.ForceUnground();
                    //stops infinite loops!
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX"), new EffectData
                    {
                        scale = 10,
                        rotation = Quaternion.identity,
                        origin = raycastHit.point,
                    }, true);
                    beastieBoys.position = raycastHit.point;
                    beastieBoys.Fire();
                }
            }
        }
    }
}