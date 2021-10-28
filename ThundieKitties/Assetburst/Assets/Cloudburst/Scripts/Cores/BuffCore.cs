using Cloudburst.Cores.HAND.Components;
using Cloudburst.Cores.Items.Green;
using Cloudburst.Cores.Items.White;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Cloudburst.Cores
{
    public class DestroyEffectOnBuffEnd : MonoBehaviour
    {
        public BuffDef buff;
        public CharacterBody body;
        public TemporaryOverlay effect;
        public void Update()
        {
            if (!body || !body.HasBuff(buff))
            {
                Destroy(effect);
                Destroy(this);
            }
        }
    }

    public class DestroyCameraTargetEffectOnBuffEnd : MonoBehaviour
    {
        public BuffDef buff;
        public CharacterBody body;
        public GameObject ppv;
        private float FUCKUNITY = 0;
        public void SetShit(BuffDef def, CharacterBody bbody, GameObject pppv) {
            LogCore.LogI(def);
            LogCore.LogI(bbody);
            LogCore.LogI(pppv);
            buff = def;
            body = bbody;
            ppv = pppv;
            LogCore.LogI(buff);
            LogCore.LogI(body);
            LogCore.LogI(ppv);
        }

        public void Update()
        {
            LogCore.LogI(ppv);
            LogCore.LogI(body);
            LogCore.LogI(buff);
            FUCKUNITY += Time.fixedDeltaTime;
            if (FUCKUNITY >= 2)
            {
                ppv.transform.position = body.transform.position;
                if (!body || !body.HasBuff(buff))
                {
                    //Destroy(ppv);
                    Destroy(this);
                }
            }
        }
    }

    public class BuffCore
    {
        public static BuffCore instance;

        protected internal BuffDef skin;
        protected internal BuffDef charm;
        protected internal BuffDef antiGrav;
        protected internal BuffDef wyattCombatIndex;
        protected internal BuffDef japesCloak;
        protected internal BuffDef engageLunarShell;
        protected internal BuffDef REDACTED;

        protected internal BuffDef magicArmor;
        protected internal BuffDef magicRegen;
        protected internal BuffDef magicAttackSpeed;

        protected internal BuffDef wyattSuspension;
        protected internal BuffDef wyattFlow;

        protected internal BuffDef glassMithrix;

        protected internal BuffDef riverGoo;
        internal bool Loaded { get; private set; } = false;
        public BuffCore() => RegisterBuffs();

        internal class BuffBuilder
        {

            public Sprite iconSprite;

            public Color buffColor = Color.white;

            public bool canStack;

            public EliteDef eliteDef;

            public bool isDebuff;

            public NetworkSoundEventDef startSfx;
            public BuffDef BuildBuff()
            {
                //create buff
                var buff = ScriptableObject.CreateInstance<BuffDef>();
                buff.canStack = canStack;
                buff.isDebuff = isDebuff;
                buff.iconSprite = iconSprite; // AssetsCore.mainAssetBundle.LoadAsset<Sprite>("Charm");
                buff.buffColor = buffColor;
                if (startSfx)
                {
                    buff.startSfx = startSfx;
                }
                if (eliteDef)
                {
                    buff.eliteDef = eliteDef;
                }

                //add again
                Cloudburst.Content.ContentHandler.Buffs.RegisterBuff(buff);
                //add buff
                /*BuffCatalog.modHelper.getAdditionalEntries += ModHelper_getAdditionalEntries;

                void ModHelper_getAdditionalEntries(List<BuffDef> defs) {
                    defs.Add(buff);
                }*/
                return buff;
            }
        }
        protected void RegisterBuffs()
        {
            LogCore.LogI("Initializing Core: " + base.ToString());

            instance = this;

            this.skin = new BuffBuilder()
            {

                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("Charm"),
                //name = "SkinStack",
                buffColor = new Color32(219, 224, 198, byte.MaxValue),
            }.BuildBuff();

            this.antiGrav = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffPulverizeIcon"),
                buffColor = new Color(0.6784314f, 0.6117647f, 0.4117647f)
            }.BuildBuff();

            this.wyattCombatIndex = new BuffBuilder()
            {
                canStack = true,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("WyattVelocity"),
                buffColor = new Color(1f, 0.7882353f, 0.05490196f)
            }.BuildBuff();

            this.wyattFlow = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("WyattFlow"),
                buffColor = CloudUtils.HexToColor("#37323e"),
            }.BuildBuff();

            this.japesCloak = new BuffBuilder()
            {
                canStack = true,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("JapesCloakBuff"),
                buffColor = new Color(1f, 0.7882353f, 0.05490196f)
            }.BuildBuff();

            this.REDACTED = new BuffBuilder()
            {
                canStack = false,
                isDebuff = true,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("Redacted"),
                buffColor = new Color32(219, 224, 198, byte.MaxValue)
            }.BuildBuff();

            this.glassMithrix = new BuffBuilder()
            {
                canStack = true,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("GlassShatter"),
                buffColor = CloudUtils.HexToColor("#50b8e7"),
            }.BuildBuff();

            this.magicRegen = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#3CB043"),
            }.BuildBuff();

            this.magicAttackSpeed = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#FFA500"),
            }.BuildBuff();

            this.magicArmor = new BuffBuilder()
            {
                canStack = false,
                isDebuff = false,
                iconSprite = AssetsCore.mainAssetBundle.LoadAsset<Sprite>("BaseMagicIcon"),
                buffColor = CloudUtils.HexToColor("#4D516D"),
            }.BuildBuff();

            //I LOVE YOU
            //ACTUALLY I DON'T

            this.wyattSuspension = new BuffBuilder()
            {
                canStack = false,
                isDebuff = true,
                iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffPulverizeIcon"),
                buffColor = CloudUtils.HexToColor("#37323e"),
            }.BuildBuff();

            this.riverGoo = new BuffBuilder()
            {
                canStack = false,
                isDebuff = true,
                iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffSlow50Icon"),
                buffColor = CloudUtils.HexToColor("#331717"),
            }.BuildBuff();

            /*EnigmaticThunder.Modules.BuffDefs.Add(antiGrav);
            EnigmaticThunder.Modules.BuffDefs.Add(charm);
            EnigmaticThunder.Modules.BuffDefs.Add(engageLunarShell);
            EnigmaticThunder.Modules.BuffDefs.Add(glassMithrix);
            EnigmaticThunder.Modules.BuffDefs.Add(japesCloak);
            EnigmaticThunder.Modules.BuffDefs.Add(magicArmor);
            EnigmaticThunder.Modules.BuffDefs.Add(magicAttackSpeed);
            EnigmaticThunder.Modules.BuffDefs.Add(magicRegen);
            EnigmaticThunder.Modules.BuffDefs.Add(REDACTED);
            EnigmaticThunder.Modules.BuffDefs.Add(skin);
            EnigmaticThunder.Modules.BuffDefs.Add(wyattCombatIndex);*/

            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            GlobalHooks.onFinalBuffStackLost += GlobalHooks_onFinalBuffStackLost;

            //On.RoR2.CharacterBody.RemoveBuff += CharacterBody_RemoveBuff;
            //On.RoR2.CharacterBody.AddBuff += CharacterBody_AddBuff;
            On.RoR2.CharacterMotor.OnDeathStart += CharacterMotor_OnDeathStart;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats1;
        }

        private void CharacterBody_RecalculateStats1(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);

            CharacterBody charBody = null;

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<CharacterBody>>((cb) => {
                charBody = cb;
            });

            int locBaseSpeedIndex = -1;
            int locSpeedMultIndex = -1;
            int locSpeedDivIndex = -1;
            bool ILFound = c.TryGotoNext(
                x => x.MatchLdfld<CharacterBody>("baseMoveSpeed"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("levelMoveSpeed"))
                && c.TryGotoNext(
                x => x.MatchStloc(out locBaseSpeedIndex))
                && c.TryGotoNext(
                    x => x.MatchLdloc(locBaseSpeedIndex),
                    x => x.MatchLdloc(out locSpeedMultIndex),
                    x => x.MatchLdloc(out locSpeedDivIndex),
                    x => x.MatchDiv(),
                    x => x.MatchMul(),
                    x => x.MatchStloc(locBaseSpeedIndex));

            if (ILFound)
            {
                c.GotoPrev(x => x.MatchLdfld<CharacterBody>("levelMoveSpeed"));
                c.GotoNext(x => x.MatchStloc(locBaseSpeedIndex));
                /*c.EmitDelegate<Func<float, float>>((origBaseMoveSpeed) => {
                    return origBaseMoveSpeed + statMods.baseMoveSpeedAdd;
                });*/
                c.GotoNext(x => x.MatchStloc(locSpeedMultIndex));
                c.EmitDelegate<Func<float, float>>((origMoveSpeedMult) => {
                    return origMoveSpeedMult + (0.3f * charBody.GetBuffCount(wyattCombatIndex));
                });
            }
            else
            {
            }
        }

        private void GlobalHooks_onFinalBuffStackLost(CharacterBody self, BuffDef buffDef)
        {
            if (self)
            {
                if (buffDef == antiGrav || buffDef == wyattSuspension)
                {

                    if (self.characterMotor)
                    {
                        self.characterMotor.useGravity = true;
                        self.characterMotor.velocity = Vector3.zero;
                    }
                }

            }
        }


        private void CharacterMotor_OnDeathStart(On.RoR2.CharacterMotor.orig_OnDeathStart orig, CharacterMotor self)
        {
            self.useGravity = true;
            orig(self);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self)
            {
                //this code is shit but i don't care
                //Reflection.SetPropertyValue<float>(self, "maxHealth", characterBody.maxHealth * 100f);

                var attackSpeed = self.attackSpeed;
                var armor = self.armor;
                var moveSpeed = self.moveSpeed;
                var regen = self.regen;
                var crit = self.crit;

                var inv = self.inventory;

                if (inv) {
                    if (MagiciansEarrings.Enabled) {
                        int magicCount = inv.GetItemCount(MagiciansEarrings.instance.Index);
                        if (self && self.HasBuff(magicArmor))
                        {
                            self.armor += (8f * magicCount);
                        }
                        if (self && self.HasBuff(magicAttackSpeed))
                        {
                            self.attackSpeed += (0.1f * magicCount);
                        }
                        if (self && self.HasBuff(magicRegen))
                        {
                            self.regen += (0.2f * magicCount);
                        }
                    }
                    if (LuckyRabbitFoot.Enabled)
                    {
                        if (inv.GetItemCount(LuckyRabbitFoot.instance.Index) > 0)
                        {
                            self.crit += 5;
                        }
                    }
                    if (BrokenBodyArmor.Enabled) {
                        if (self && self.HasBuff(charm))
                        {
                            var vount = 0;
                            if (self.inventory)
                            {
                                vount = inv.GetItemCount(BrokenBodyArmor.instance.Index);
                                self.armor = armor + (vount * 10);
                            }
                        }
                    }
                }
                if (self && self.HasBuff(REDACTED)) {
                    self.moveSpeed -= (moveSpeed / 2);
                    self.attackSpeed -= (attackSpeed / 2);
                    self.armor -= 20;
                    self.regen -= 20;
                }


                if (self && self.HasBuff(RoR2.RoR2Content.Buffs.LunarShell)) {
                    self.attackSpeed += 3;
                    self.armor += 50;
                    self.moveSpeed -= 3;
                    self.regen += 3;
                    self.damage += 2;
                }

                if (self && self.HasBuff(wyattSuspension)) {
                    //nice air movement, dumbfuck
                    self.moveSpeed = 0f;
                    self.acceleration = 80f;
                    self.characterMotor?.ApplyForce(Vector3.zero, true, true);
                }


                if (self && self.HasBuff(japesCloak)) {
                    var buffCount = self.GetBuffCount(japesCloak);
                    for (int i = 0; i < buffCount; i++)
                    {
                        self.armor = armor + 5;
                        self.regen = regen + 0.1f;
                    }
                }
                if (self && self.HasBuff(antiGrav) || self.HasBuff(wyattSuspension)) 
                {
                    if (self.characterMotor)
                    {
                        self.characterMotor.useGravity = false;
                    }
                    self.attackSpeed = attackSpeed -= (.5f * attackSpeed);
                    self.moveSpeed = moveSpeed -= (.5f * moveSpeed);
                }

                /*if (self && self.HasBuff(wyattCombatIndex))
                {
                    var buffCount = self.GetBuffCount(wyattCombatIndex);
                        
                    LogCore.LogI("SPEED BEFORE INCREASE: " + self.moveSpeed);

                    float increase = buffCount * .6f;
                    LogCore.LogI($"INCREASE: " + increase + $" : AMOUNT OF BUFFS: {buffCount}" );

                    self.moveSpeed = moveSpeed + increase;
                    LogCore.LogI("SPEED AFTER INCREASE: " + self.moveSpeed);
                    //moveSpeed * (1f + (buffCount * 0.17f));
                }*/
                if (self && self.HasBuff(wyattFlow))
                {
                    //ebb and flow
                    SkillLocator locator = self.skillLocator;
                    if (!self.HasBuff(RoR2Content.Buffs.NoCooldowns))
                    {
                        locator.secondary.flatCooldownReduction += 0.3f;
                        locator.special.flatCooldownReduction += 0.3f;
                    }
                    self.maxJumpCount++;
                    //self.moveSpeed = moveSpeed * (1f + (moveSpeed * 0.3f));
                }


                if (self && self.HasBuff(glassMithrix))
                {
                    var buffCount = self.GetBuffCount(glassMithrix);
                    self.skillLocator.secondary.flatCooldownReduction = 3;
                    self.skillLocator.utility.flatCooldownReduction = 2;
                    self.skillLocator.secondary.SetBonusStockFromBody(1 * buffCount);
                    self.skillLocator.utility.SetBonusStockFromBody(1 * buffCount);
                    for (int i = 0; i < buffCount; i++)
                    {
                        self.moveSpeed = moveSpeed += (0.1f * moveSpeed);
                        self.armor = armor += 5 ;   
                    }
                }
            }
        }
    }
}
