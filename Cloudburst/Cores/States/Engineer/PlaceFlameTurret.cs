using EntityStates.Engi.EngiWeapon;
using System;
using UnityEngine;

namespace Cloudburst.Cores.Engineer
{
    public class PlaceFlameTurret : PlaceTurret
    {
        public PlaceFlameTurret()
        {
            this.blueprintPrefab = Resources.Load<GameObject>("prefabs/EngiTurretBlueprints");
            this.wristDisplayPrefab = Resources.Load<GameObject>("prefabs/EngiTurretWristDisplay");
            this.turretMasterPrefab = EngineerCore.flameTurretMaster;
        }
    }
}
