
using EnigmaticThunder.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using static R2API.DotAPI;
using static RoR2.DotController;

namespace Cloudburst.Cores
{
    class DoTCore
    {
        //1:53.
        public static DoTCore instance;

        public static DotIndex redactedIndex;

        public DoTCore() => RegisterDoTs();

        protected internal void RegisterDoTs()
        {
            instance = this;

            LogCore.LogI("Initializing Core: " + base.ToString());

            DotDef def = new DotDef() {
                associatedBuff = BuffCore.instance.wyattSuspension,
                damageColorIndex = DamageColorIndex.DeathMark,
                damageCoefficient = 1,
                interval = 1,
            };

            redactedIndex = DoT.RegisterDot(def, RedactedBehaviour, null);
            LogCore.LogI("Redacted index is: " + redactedIndex);
            //clean = RegisterDotDef(1, 0.5f, DamageColorIndex.WeakPoint, BuffCore.instance.cleanIndex, lmao, lol);
        }

        void RedactedBehaviour(DotController controller, DotController.DotStack stacks) {
            stacks.damageType = DamageType.NonLethal;
            var body = controller.victimBody;
            LogCore.LogI("gmc");
            if (body) {
                var healthComponent = body.healthComponent;
                if (healthComponent) {

                    bool isFullHealth = body.healthComponent.combinedHealthFraction >= 1;
                    if (isFullHealth) {
                        stacks.damage = 0;
                    }
                    LogCore.LogI("hi0");
                    LogCore.LogI(healthComponent.combinedHealthFraction);
                    switch (healthComponent.combinedHealthFraction) {
                        case 0.9f:
                            LogCore.LogI("hi1");
                            stacks.damage += 0.5f;
                            break;
                        case 0.8f:
                            LogCore.LogI("hi2");

                            stacks.damage += 1f;
                            break;
                        case 0.7f:
                            LogCore.LogI("hi3");

                            stacks.damage += 1.5f;
                            break;
                        case 0.6f:
                            LogCore.LogI("hi4");
                            stacks.damage += 2f;
                            break;
                        case 0.5f:
                            LogCore.LogI("hi5");
                            stacks.damage += 3f;
                            break;
                        case 0.4f:
                            LogCore.LogI("hi6");
                            stacks.damage += 3.5f;
                            break;
                        case 0.3f:
                            LogCore.LogI("hi7");
                            stacks.damage += 4f;
                            break;
                        case 0.2f:
                            LogCore.LogI("hi8");
                            stacks.damage += 4.5f;
                            break;
                        case 0.1f:
                            LogCore.LogI("hi9");
                            stacks.damage += 5f;
                            break;
                    }
                }
            }
            LogCore.LogI("hi!");
        }
    }
}
