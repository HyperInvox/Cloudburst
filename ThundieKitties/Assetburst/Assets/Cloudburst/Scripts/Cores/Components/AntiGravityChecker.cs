using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class AntiGravityChecker : NetworkBehaviour
    {
        [SyncVar]
        public int channeledFlightGranterCount;

        [SyncVar]
        public int environmentalAntiGravityGranterCount;

        public void ActivateAntiGravityAuthority()
        {
            channeledFlightGranterCount++;
            environmentalAntiGravityGranterCount++;
        }

        public void DeactivateAntiGravityAuthority()
        {
            channeledFlightGranterCount--;
            environmentalAntiGravityGranterCount--;
        }
    }
}