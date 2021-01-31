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
                damageCoefficient = 2,
                interval = 2,
            };

            var lmao = new DotAPI.CustomDotBehaviour(CleanBehaviour);
            var lol = new DotAPI.CustomDotVisual(CleanVisuals);
            //clean = RegisterDotDef(1, 0.5f, DamageColorIndex.WeakPoint, BuffCore.instance.cleanIndex, lmao, lol);
        }

        void CleanBehaviour(DotController controller, DotController.DotStack stacks) {
            stacks.damageType = DamageType.NonLethal;
            var body = controller.victimBody;
            if (body) {
                var healthComponent = body.healthComponent;
                if (healthComponent) {

                    bool isFullHealth = body.healthComponent.combinedHealthFraction >= 1;
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
