using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudburst.Cores
{
    class NewtCore
    {
        private List<string> list = new List<string>();
        public NewtCore() => Newt();

        private void Newt()
        {
            LanguageAPI.Add("NEWT_ANNOY_0", "<color=#50b8e7>[NEWT] Shape will shatter. Stop. </style>");
            LanguageAPI.Add("NEWT_ANNOY_1", "<color=#50b8e7>[NEWT] Stop stop. Bad bad.</style>");
            LanguageAPI.Add("NEWT_ANNOY_2", "<color=#50b8e7>[NEWT] Will shatter your shape. Bad.</style>");
            LanguageAPI.Add("NEWT_ANNOY_3", "<color=#50b8e7>[NEWT] Bad. Will reshape into forget.</style>");
            LanguageAPI.Add("NEWT_ANNOY_4", "<color=#50b8e7>[NEWT] Cease. Do not bother</style>");
            LanguageAPI.Add("NEWT_ANNOY_5", "<color=#50b8e7>[NEWT] Reshape. Out of forever.</style>");
            LanguageAPI.Add("NEWT_ANNOY_6", "<color=#50b8e7>[NEWT] Again again. Everytime.</style>");
            LanguageAPI.Add("NEWT_KICK", "<color=#ff6961>[NEWT] Come. When reshape.</style>");

            list.Add("NEWT_ANNOY_0");
            list.Add("NEWT_ANNOY_1");
            list.Add("NEWT_ANNOY_2");// 
            list.Add("NEWT_ANNOY_3");
            list.Add("NEWT_ANNOY_4");
            list.Add("NEWT_ANNOY_5");
            list.Add("NEWT_ANNOY_6");

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.EntityStates.NewtMonster.KickFromShop.OnExit += KickFromShop_OnExit;
            On.RoR2.BazaarController.CommentOnAnnoy += BazaarController_CommentOnAnnoy;
        }
        private void KickFromShop_OnExit(On.EntityStates.NewtMonster.KickFromShop.orig_OnExit orig, EntityStates.NewtMonster.KickFromShop self)
        {
            orig(self);
            LogCore.LogI("hi");
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = "NEWT_KICK"
            });
        }

        private void BazaarController_CommentOnAnnoy(On.RoR2.BazaarController.orig_CommentOnAnnoy orig, BazaarController self)
        {
            if (Util.CheckRoll(5, 0f, null))
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "NEWT_ANNOY_" + UnityEngine.Random.Range(0, list.Count),
                });
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self && damageInfo != null)
            {
                var go = self.gameObject;
                if (go && go.name == "ShopkeeperBody(Clone)")
                {
                    BazaarController.instance.CommentOnAnnoy();
                }
            }
            orig(self, damageInfo);
        }
    }
}
