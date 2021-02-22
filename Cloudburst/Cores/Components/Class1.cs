using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.Components
{
    public class MAIDManager2 : MonoBehaviour
    {
        private GameObject maid;

        private SkillLocator skillLocator;
        private CharacterBody body;
        private CharacterMotor characterMotor;
        private CharacterDirection characterDirection;
        public void Awake()
        {
            skillLocator = GetComponent<SkillLocator>();
            characterMotor = GetComponent<CharacterMotor>();
            body = GetComponent<CharacterBody>();
            characterDirection = GetComponent<CharacterDirection>();
            //click and she's gone!
        }

        public void GetMAID(GameObject obj) {
            if (maid) {
                Destroy(maid);
                LogCore.LogW("Duplicate MAID!");
            }
            maid = obj;
        }
    }
}