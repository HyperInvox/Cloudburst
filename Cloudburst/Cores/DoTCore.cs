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

        public DoTCore() => RegisterDoTs();

        protected internal void RegisterDoTs()
        {
            instance = this;

            LogCore.LogI("Initializing Core: " + base.ToString());

            var lmao = new DotAPI.CustomDotBehaviour(CleanBehaviour);
            var lol = new DotAPI.CustomDotVisual(CleanVisuals);
            //clean = RegisterDotDef(1, 0.5f, DamageColorIndex.WeakPoint, BuffCore.instance.cleanIndex, lmao, lol);
        }

        void CleanBehaviour(DotController controller, DotController.DotStack stacks) {

        }

        void CleanVisuals(DotController controller) {

        }

        protected internal DotIndex RegisterDoT(float interval, float damageCoeff, DamageColorIndex colorIndex, BuffIndex assocatedBuff)
        {
            return RegisterDotDef(interval, damageCoeff, colorIndex, assocatedBuff);
        }
    }
}
