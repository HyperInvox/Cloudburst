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
            LanguageAPI.Add("NEWT_ANNOY_0", "Shape will shatter. Stop. ");
            LanguageAPI.Add("NEWT_ANNOY_1", "Stop stop. Bad bad.");
            LanguageAPI.Add("NEWT_ANNOY_2", "Will shatter your shape. Bad.");
            LanguageAPI.Add("NEWT_ANNOY_3", "Bad. Will reshape into forget.");
            LanguageAPI.Add("NEWT_ANNOY_4", "Cease. Do not bother");
            LanguageAPI.Add("NEWT_ANNOY_5", "Reshape. Out of forever.");
            LanguageAPI.Add("NEWT_ANNOY_6", "Again again. Everytime.");
            LanguageAPI.Add("NEWT_KICK", "  Come. When reshape.");

            //    LanguageAPI.Add("NEWT_KICK", "<color=#ff6961>[NEWT] Come. When reshape.</style>");
            LanguageAPI.Add("NEWT_DIALOGUE_FORMAT", "<color=#50b8e7><size=120%>Newt: {0}</color></size>");
            LanguageAPI.Add("NEWT_KICK_DIALOGUE_FORMAT", "<color=#ff6961><size=120%>Newt: {0}</color></size>");

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
            Chat.SendBroadcastChat(new Chat.NpcChatMessage
            {
                baseToken = "NEWT_KICK",
                formatStringToken = "NEWT_KICK_DIALOGUE_FORMAT",
                sender = self.gameObject,
                sound = self.gameObject.GetComponent<SfxLocator>().barkSound
            });
        }

        private void BazaarController_CommentOnAnnoy(On.RoR2.BazaarController.orig_CommentOnAnnoy orig, BazaarController self)
        {
            if (Util.CheckRoll(5, 0f, null))
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    baseToken = "NEWT_ANNOY_" + UnityEngine.Random.Range(0, list.Count),
                    formatStringToken = "NEWT_DIALOGUE_FORMAT",
                    sender = self.gameObject,
                    sound = self.gameObject.GetComponent<SfxLocator>().barkSound
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
