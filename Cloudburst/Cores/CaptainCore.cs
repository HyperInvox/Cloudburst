using Cloudburst.Cores.States.Captain;
using EnigmaticThunder.Modules;
using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace Cloudburst.Cores
{
    class CaptainCore
    {
        public GameObject captainObject;
        public CaptainCore() => AddOrbitalLaser();

        public void CreateLaser()
        {
            var obj = Resources.Load<GameObject>("prefabs/networkedobjects/OrbitalLaser");
            var laserController = obj.GetComponent<OrbitalLaserController>();
            laserController.hitEffectPrefab = null;
            laserController.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerCaptainDefenseMatrix");
        }

        public void AddOrbitalLaser() {
            CreateLaser();

            captainObject = Resources.Load<GameObject>("prefabs/characterbodies/CaptainBody");

            SkillDef origDef = Resources.Load<SkillDef>("skilldefs/captainbody/CaptainTazer");
            var newDef = ScriptableObject.CreateInstance<SkillDef>();

            CloudUtils.CopySkillDefSettings(origDef, newDef);

            EnigmaticThunder.Modules.Loadouts.RegisterEntityState(typeof(CallOrbitalLaser));
            EnigmaticThunder.Modules.Loadouts.RegisterSkillDef(newDef);

            Languages.Add("CAPTAIN_UTILITY_ALT2_NAME", "Yeah!");
            Languages.Add("CAPTAIN_UTILITY_ALT2_DESCRIPTION", "Awesome!");

            newDef.baseRechargeInterval = 15;
            newDef.activationState = new SerializableEntityStateType(typeof(CallOrbitalLaser));
            newDef.baseMaxStock = 4;
            newDef.skillName = "YEAH";
            newDef.icon = AssetsCore.engiFlameTurrets;
            newDef.skillNameToken = "CAPTAIN_UTILITY_ALT2_NAME";
            newDef.skillDescriptionToken = "CAPTAIN_UTILITY_ALT2_DESCRIPTION";
            newDef.icon = AssetsCore.engiFlameTurrets;

            var locator = captainObject.GetComponent<SkillLocator>();

            var family = locator.utility.skillFamily;

            SkillFamily.Variant variant = new SkillFamily.Variant();

            variant.skillDef = newDef;

            int prevLength = family.variants.Length;
            Array.Resize<SkillFamily.Variant>(ref family.variants, prevLength + 1);
            family.variants[prevLength] = variant;

        }
    }
}
