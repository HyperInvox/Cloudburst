﻿using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
/*
namespace Cloudburst.Cores.Items.Green
{

    public class RiftBubble : ItemBuilder
    {
        public static RiftBubble instance;

        public static bool Enabled;

        public override ItemTag[] ItemTags => new ItemTag[1] {
            ItemTag.Healing,
        };

        public override string ItemName => "Rift Bubble";

        public override string ItemLangTokenName => "SUPERHOT";

        public override string ItemPickupDesc => "Gain a small area around you where the attack and movement speed of enemies is slowed. Also gain reduced knockback.";

        public override string ItemFullDescription => "h";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override string ItemModelPath => "Assets/Cloudburst/Items/RiftBubble/IMDLRiftBubble.prefab";

        public override string ItemIconPath => "Assets/Cloudburst/Items/RiftBubble/icon.png";

        public static GameObject rift;

        public BuffDef slow;

        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override void Initialization()
        {
            slow = new BuffBuilder()
            {
                canStack = false,
                isDebuff = true,
                iconSprite = Resources.Load<Sprite>("textures/bufficons/texbuffslow50icon"),
                buffColor = CloudUtils.HexToColor("#44236b"),
            }.BuildBuff();

            rift = AssetsCore.mainAssetBundle.LoadAsset<GameObject>("RiftIndicator");
            Transform bubble = rift.transform.Find("Visuals/Sphere");
            Material mat = CloudburstPlugin.Instantiate<Material>(AssetsCore.mainAssetBundle.LoadAsset<Material>("matRiftIndicator"));
            mat.shader = Resources.Load<Shader>("shaders/fx/hgintersectioncloudremap");
            bubble.GetComponent<Renderer>().material = mat; //AssetsCore.mainAssetBundle.LoadAsset<Material>("matRiftIndicator").transform.Find("spingfisj/Mball.001").GetComponent<MeshRenderer>().material;


            //make a new parent gameobject
            //have it at 1,1,1
            //have the child ( the actual visuals ) have the transform be 2,2,2
            //figure this out later


            rift.AddComponent<NetworkIdentity>();
            rift.AddComponent<NetworkedBodyAttachment>().shouldParentToAttachedBody = true;
            BuffWard ward = rift.AddComponent<BuffWard>();




            ward.radius = 10;
            ward.interval = 0.2f;
            ward.rangeIndicator = rift.transform.Find("Visuals");
            ward.buffDef = slow;
            ward.buffDuration = 1;
            ward.floorWard = false;
            ward.invertTeamFilter = true;
            ward.expires = false;
            ward.expireDuration = 0;
            ward.animateRadius = false;
            ward.removalTime = 0;
            ward.removalSoundString = ""; 
        
            
        }


        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            GlobalHooks.onInventoryChanged += GlobalHooks_onInventoryChanged;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(slow)) {
                args.attackSpeedMultAdd -= 0.5f ;
                args.moveSpeedMultAdd -= 0.5f ;
            }
        }

        private void GlobalHooks_onInventoryChanged(CharacterBody body)
        {
            body.AddItemBehavior<RiftIndicator>(GetCount(body));
        }
    }
    public class RiftIndicator : CharacterBody.ItemBehavior
    {
        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            bool flag = this.stack > 0;
            if (this.indicator != flag)
            {
                if (flag)
                {
                    this.indicator = UnityEngine.Object.Instantiate<GameObject>(RiftBubble.rift);
                    this.indicator.GetComponent<TeamFilter>().teamIndex = this.body.teamComponent.teamIndex;
                    ward = indicator.GetComponent<BuffWard>();
                    ward.radius = 7 + (stack * 3) + this.body.radius;
                    this.indicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.body.gameObject);
                    return;
                }
                UnityEngine.Object.Destroy(this.indicator);
                this.indicator = null;
            }
            if (indicator) {
                ward.radius = 7 + (stack * 3) + this.body.radius;
            }
        }

        private void OnDisable()
        {
            if (this.indicator)
            {
                UnityEngine.Object.Destroy(this.indicator);
            }
        }
        private BuffWard ward;
        private GameObject indicator;
    }
}
*/