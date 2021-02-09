using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static R2API.DotAPI;
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
                associatedBuff = BuffCore.instance.REDACTED,
                damageColorIndex = DamageColorIndex.DeathMark,
                damageCoefficient = 1,
                interval = 1,
            };

            redactedIndex = RegisterDotDef(def, RedactedBehaviour, null);

            //clean = RegisterDotDef(1, 0.5f, DamageColorIndex.WeakPoint, BuffCore.instance.cleanIndex, lmao, lol);
        }

        void RedactedBehaviour(DotController controller, DotController.DotStack stacks) {
            stacks.damageType = DamageType.NonLethal;
            var body = controller.victimBody;
            if (body) {
                var healthComponent = body.healthComponent;
                if (healthComponent) {

                    bool isFullHealth = body.healthComponent.combinedHealthFraction >= 1;
                    if (isFullHealth) {
                        stacks.damage = 0;
                    }
                    switch (healthComponent.combinedHealthFraction) {
                        case 0.9f:
                            stacks.damage += 0.5f;
                            break;
                        case 0.8f:
                            stacks.damage += 1f;
                            break;
                        case 0.7f:
                            stacks.damage += 1.5f;
                            break;
                        case 0.6f:
                            stacks.damage += 2f;
                            break;
                        case 0.5f:
                            stacks.damage += 3f;
                            break;
                        case 0.4f:
                            stacks.damage += 3.5f;
                            break;
                        case 0.3f:
                            stacks.damage += 4f;
                            break;
                        case 0.2f:
                            stacks.damage += 4.5f;
                            break;
                        case 0.1f:
                            stacks.damage += 5f;
                            break;
                    }
                }
            }
        }

        void CleanVisuals(DotController controller) {

        }

        protected internal DotIndex RegisterDoT(float interval, float damageCoeff, DamageColorIndex colorIndex, BuffIndex assocatedBuff)
        {
            return RegisterDotDef(interval, damageCoeff, colorIndex, assocatedBuff);
        }
    }
}
