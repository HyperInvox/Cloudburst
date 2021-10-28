﻿

using R2API;
using RoR2;
using System;
using UnityEngine;
using static RoR2.GivePickupsOnStart;

namespace Cloudburst.Cores
{
    public sealed class SipSipCore
    {
        public static SipSipCore instance;



        private GameObject sipSipBody;
        private GameObject sipSipMaster;
        public static int sipSipIndex;
        public SipSipCore() => BuildSipsip();
   
        private void BuildSipsip() {
            instance = this;
            LogCore.LogI("Initializing Core: " + base.ToString());

            sipSipBody = Resources.Load<GameObject>("prefabs/characterbodies/ScavLunar1Body").InstantiateClone("ScavLunar5Body", true);
            sipSipMaster = Resources.Load<GameObject>("prefabs/charactermasters/ScavLunar1Master").InstantiateClone("ScavLunar5Master", true);

            CloudUtils.RegisterNewBody(sipSipBody);
            CloudUtils.RegisterNewMaster(sipSipMaster);

            sipSipMaster.GetComponent<CharacterMaster>().bodyPrefab = sipSipBody;

            AddGreedyItems();
            SetNameToken();
            CreateSpawnCard();

            //DOSTUFF();
            //LogCore.LogM("SipSip is greedy!");
        }
        private void AddGreedyItems() {

            /*            ItemInfo ghor = new ItemInfo();
            ghor.count =    10;
            ghor.itemString = "BonusGoldPackOnKill";

            ItemInfo teddie = new ItemInfo();
            teddie.count = 2;
            teddie.itemString = "Bear";
                
            ItemInfo brittle = new ItemInfo();
            brittle.count = 8;
            brittle.itemString = "GoldOnHit";

            ItemInfo backup = new ItemInfo();
            backup.count = 13;
            backup.itemString = "SecondarySkillMagazine";

            ItemInfo medkit = new ItemInfo();
            medkit.count = 4;
            medkit.itemString = "Medkit";

            ItemInfo bloodTester = new ItemInfo();
            bloodTester.count = 2;
            bloodTester.itemString = "RegenOnKill";

            ItemInfo magic = new ItemInfo();
            magic.count = 1;
            magic.itemString = "ExtendedEnemyBuffDuration";

            ItemInfo[] array = new ItemInfo[] {
                ghor,
                teddie,
                brittle,
                backup,
                medkit,
                bloodTester,
                magic
            };

            foreach (GivePickupsOnStart component in sipSipMaster.GetComponents<GivePickupsOnStart>()) {
                component.enabled = false;
            } 

            GivePickupsOnStart pickupComp = sipSipMaster.AddComponent<GivePickupsOnStart>();
            pickupComp.equipmentString = "GoldGat";
            pickupComp.itemInfos = array;
            */
            ItemInfo ghor = new ItemInfo();
            ghor.count = 5;
            ghor.itemString = "BonusGoldPackOnKill";

            ItemInfo teddie = new ItemInfo();
            teddie.count = 5;
            teddie.itemString = "Bear";
                
            ItemInfo brittle = new ItemInfo();
            brittle.count = 4;
            brittle.itemString = "GoldOnHit";

            ItemInfo backup = new ItemInfo();
            backup.count = 10;
            backup.itemString = "SecondarySkillMagazine";

            ItemInfo medkit = new ItemInfo();
            medkit.count = 8;
            medkit.itemString = "Medkit";

            ItemInfo bloodTester = new ItemInfo();
            bloodTester.count = 2;
            bloodTester.itemString = "RegenOnKill";

            ItemInfo[] array = new ItemInfo[] {
                ghor,
                teddie,
                brittle,
                backup,
                medkit,
                bloodTester
            };

            foreach (GivePickupsOnStart component in sipSipMaster.GetComponents<GivePickupsOnStart>()) {
                component.enabled = false;
            } 

            GivePickupsOnStart pickupComp = sipSipMaster.AddComponent<GivePickupsOnStart>();
            pickupComp.equipmentString = "GoldGat";
            pickupComp.itemInfos = array;
        }

        private void SetNameToken() {
            R2API.LanguageAPI.Add("SCAVLUNAR5_BODY_NAME", "Sipsip The Greedy");
            CharacterBody body = sipSipBody.GetComponent<CharacterBody>();
            body.baseNameToken = "SCAVLUNAR5_BODY_NAME";
        }
        private void CreateSpawnCard() {
            MultiCharacterSpawnCard spawnCard = Resources.Load<MultiCharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscScavLunar");
            
            int AAAAAAAAAAAAA = spawnCard.masterPrefabs.Length;

            Array.Resize<GameObject>(ref spawnCard.masterPrefabs, AAAAAAAAAAAAA + 1);
            spawnCard.masterPrefabs[AAAAAAAAAAAAA] = sipSipMaster;
        }
    }
}