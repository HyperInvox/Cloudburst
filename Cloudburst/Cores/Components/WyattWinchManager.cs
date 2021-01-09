using Cloudburst.Cores.States.Wyatt;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cloudburst.Cores.Components
{
    class WyattWinchManager : MonoBehaviour
    {
        private MAIDManager maidManager;
        private ProjectileController controller;
        private ProjectileOwnerInfo owner;

        public void Awake() {
            controller = base.GetComponent<ProjectileController>();
        }

        public void Start() {
            owner = new ProjectileOwnerInfo(controller.owner, "Weapon");
            AssignHookReferenceToBodyStateMachine();
            maidManager = owner.gameObject.GetComponent<MAIDManager>();

        }

        private void AssignHookReferenceToBodyStateMachine()
        {
            TrashOut trashOut;
            if (this.owner.stateMachine && (trashOut = (this.owner.stateMachine.state as TrashOut)) != null)
            {
                trashOut.SetHookReference(base.gameObject);
                LogCore.LogI("heyo!");
            }
        }

            public void OnHit()
        {


            //goodbye, world
            Destroy(gameObject);
        }
        public struct ProjectileOwnerInfo
        {
            public ProjectileOwnerInfo(GameObject ownerGameObject, string targetCustomName)
            {
                this = default(ProjectileOwnerInfo);
                this.gameObject = ownerGameObject;
                if (this.gameObject)
                {
                    this.characterBody = this.gameObject.GetComponent<CharacterBody>();
                    this.characterMotor = this.gameObject.GetComponent<CharacterMotor>();
                    this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
                    this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.gameObject);
                    EntityStateMachine[] components = characterBody.GetComponent<SetStateOnHurt>().idleStateMachine;
                    for (int i = 0; i < components.Length; i++)
                    {
                        LogCore.LogI(components[i]);
                        if (components[i].customName == targetCustomName)
                        {
                            this.stateMachine = components[i];
                            return;
                        }
                    }
                }
            }

            public readonly GameObject gameObject;

            public readonly CharacterBody characterBody;

            public readonly CharacterMotor characterMotor;

            public readonly Rigidbody rigidbody;

            public readonly EntityStateMachine stateMachine;

            public readonly bool hasEffectiveAuthority;
        }
    }
}
