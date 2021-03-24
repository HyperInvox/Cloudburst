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
{        /// <summary>
         /// Contains basic info about the owner, such as  their character body, character motor, and their rigidbody. 
         /// </summary>
    public struct BasicOwnerInfo
    {        /// <summary>
             /// The owner's gameobject
             /// </summary>
        public readonly GameObject gameObject;

        /// <summary>
        /// The owner's characterBody
        /// </summary>
        public readonly CharacterBody characterBody;

        /// <summary>
        /// The owner's characterMotor
        /// </summary>
        public readonly CharacterMotor characterMotor;

        /// <summary>
        /// The owner's rigidbody
        /// </summary>
        public readonly Rigidbody rigidbody;

        /// <summary>
        /// The owner's selected statemachine
        /// </summary>
        public readonly EntityStateMachine stateMachine;

        /// <summary>
        /// If owner has authority
        /// </summary>
        public readonly bool hasEffectiveAuthority;

        /// <summary>
        /// The owner's input bank
        /// </summary>
        public readonly InputBankTest inputBank;
        public BasicOwnerInfo(GameObject ownerGameObject, string targetCustomName)
        {
            this = default(BasicOwnerInfo);
            this.gameObject = ownerGameObject;
            if (this.gameObject)
            {
                this.characterBody = this.gameObject.GetComponent<CharacterBody>();
                inputBank = characterBody.inputBank;
                this.characterMotor = this.gameObject.GetComponent<CharacterMotor>();
                this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
                this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.gameObject);
                EntityStateMachine[] components = characterBody.GetComponent<SetStateOnHurt>().idleStateMachine;
                for (int i = 0; i < components.Length; i++)
                {
                    //LogCore.LogI(components[i]);
                    if (components[i].customName == targetCustomName)
                    {
                        this.stateMachine = components[i];
                        return;
                    }
                }
            }
        }


    }

    class WyattWinchManager : MonoBehaviour
    {
        private MAIDManager maidManager;
        private GameObject maid;
        private ProjectileController controller = null;
        private BasicOwnerInfo owner = default;

        public void Awake()
        {
            controller = base.GetComponent<ProjectileController>();
        }

        public void Start()
        {
            owner = new BasicOwnerInfo(controller.owner, "Weapon");
            AssignHookReferenceToBodyStateMachine();
            maidManager = owner.gameObject.GetComponent<MAIDManager>();

        }

        private void AssignHookReferenceToBodyStateMachine()
        {
            TrashOut trashOut; //LogCore.LogI("check 0!");
            if (this.owner.stateMachine && (trashOut = (this.owner.stateMachine.state as TrashOut)) != null)
            {
                trashOut.SetHookReference(base.gameObject);
                //LogCore.LogI("check 1!");
            }
            else {
                maid = owner.gameObject.GetComponent<MAIDManager>().GetWinch(this.gameObject);
            }
        }

        public void FixedUpdate() {
            if (maid)
            {
                base.transform.position = maid.transform.position;
                //     base.transform.localRotation = Quaternion.identity;
            }
        }

        public void OnHit()
        {

            //LogCore.LogI("hi!!!");
            //goodbye, world
            Destroy(gameObject);
        }
    }
}
