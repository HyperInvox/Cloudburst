using Cloudburst.Cores.HAND.Components;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    public class WyattTracker : HANDDroneTracker
    {
        //GameObject indicator = Resources.Load<GameObject>("prefabs/EngiShieldRetractIndicator");
        public override float GetDistance()
        {
            return maxTrackingDistance + (characterBody.GetBuffCount(BuffCore.instance.wyattCombatIndex) * 10);
        }
    }
}
