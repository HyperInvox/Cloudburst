using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cloudburst.Cores.States.Wyatt
{
    public class WyattMain : GenericCharacterMain
    {
        public override void Update()
        { 
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    this.outer.SetInterruptState(EntityStateCatalog.InstantiateState(new SerializableEntityStateType(typeof(BaseEmote.CustodianSickness))), InterruptPriority.Any);
                    return;
                }
            }
        }
    }
}