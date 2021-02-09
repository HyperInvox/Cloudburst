using BepInEx.Configuration;
using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Items.Red
{

    public class REDACTED : ItemBuilder
    {
        public override string ItemName => "REDACTED";

        public override string ItemLangTokenName => "REDACTED";

        public override string ItemPickupDesc => "Nearby enemies with low health have a chance to become overwhelmed, slowing them and inflicting with with a Damage-Over-Time";
        //
        public override string ItemFullDescription => "In combat, nearby low health enemies have a 5% chance to become <style=cIsDamage>Overwhelmed, slowing them and inflicting them with a strong non-lethal Damage-Over-Time that does 200% damage every three seconds and becomes more potent the lower their health is</style> for 8 <style=cStack>(+3 seconds</style> .";

        public override string ItemLore => @"""Order: ""Irregularity Classification - [/]-[//]-[//]""
Tracking Number: 89*****
Estimated Delivery: 03/27/2056
Shipping Method:  High Priority/Biological
Shipping Address: [/////////////], District [//], [/////]
Shipping Details:


Following the Incident at branch facility [//]-[///], we have decided to transfer Irregularity [/]-[//]-[//] to our main facility, Facility [/]-[///].

Irregularity [/]-[//]-[//] shall be placed in a Level [TEHOM] containment chamber, and shall be constantly monitored for any changes in behavior. Experiments and Containment upkeep shall be conducted remotely, or by personnel with a Cognitional Endurance grade of no less than Five. 
Under no circumstance should Irregularity [/]-[//]-[//] be removed from its room of containment without proper authorization and obfuscation procedures. Should Irregularity [/]-[//]-[//] be lost in transit and left in a potentially visible location, an Irregularity Recontainment Unit MUST BE DISPATCHED TO RECOVER THE IRREGULARITY IMMEDIATELY.

All research on Irregularity [/]-[//]-[//] shall be focused on preventing it from incubating. Work on improving the anti-perception device implanted on it to alter the perception on more than digital devices will come later.

Hold strong, and keep your heads up.
-[///////]""";

        public override ItemTier Tier => ItemTier.Tier3;

        public override string ItemModelPath => "prefabs/pickupmodels/PickupMystery";

        public override string ItemIconPath => "textures/miscicons/texMysteryIcon";


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
            if (NetworkServer.active)
            {
                body.AddItemBehavior<RedactedBehavior>(GetCount(body));
            }
        }
    }
    public class RedactedBehavior : CharacterBody.ItemBehavior
    {
        private float timer;

        private void YOUVEBEENSTRUCKBY(HurtBox box)
        {
            //A HEART ATTACK
            if (Util.CheckRoll(30, body.master))
            {
                var health = box.healthComponent;
                bool isFullHealth = health.combinedHealthFraction >= 0.8f;
                if (health && !DotController.FindDotController(health.gameObject) && !isFullHealth)
                {
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/HANDheal"), new EffectData()
                    {
                        origin = box.transform.position,
                        scale = 10,
                        color = Color.red,
                    }, true);
                    Util.PlaySound("Play_item_proc_armorReduction_shatter", box.gameObject);
                    DotController.InflictDot(health.gameObject, body.gameObject, DoTCore.redactedIndex, 8 + (stack * 2), 1);
                }
            }
        }

        public void FixedUpdate()
        {
            timer += Time.deltaTime;
            //LogCore.LogI(timer);

            if (timer >= 3)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.searchOrigin = transform.position;
                bullseyeSearch.maxDistanceFilter = 120;
                bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(TeamIndex.Player);
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
                bullseyeSearch.filterByLoS = true;
                bullseyeSearch.searchDirection = Vector3.up;
                bullseyeSearch.RefreshCandidates();
                bullseyeSearch.FilterOutGameObject(base.gameObject);
                var list = bullseyeSearch.GetResults().ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    YOUVEBEENSTRUCKBY(list[i]);
                }
            }
        }
    }
}

