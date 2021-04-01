using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Cloudburst.Cores.States.Wyatt
{
    class YeahDudeIBetterBeOrYouCanFuckinKissMyAssHumanCentipede : BaseSkillState
    {
        public static float baseDuration = 0.7f;
        private List<CharacterBody> negros;

        public override void OnEnter()
        {
            base.OnEnter();
            GatherNiggas();
        }

        public void GatherNiggas()
        {
            negros = new List<CharacterBody>();

            List<GameObject> list = new List<GameObject>();
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = base.gameObject.transform.position;
            bullseyeSearch.maxDistanceFilter = 25;
            bullseyeSearch.teamMaskFilter = TeamMask.GetUnprotectedTeams(base.teamComponent.teamIndex);
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.searchDirection = Vector3.up;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            List<HurtBox> list2 = bullseyeSearch.GetResults().ToList<HurtBox>();
            for (int i = 0; i < list2.Count; i++)
            {
                GameObject gameObject = list2[i].healthComponent.gameObject;
                if (gameObject)
                {
                    list.Add(gameObject);
                }
            }
            
            HahaNiggaHowHardIsItToMoveLikeJustMoveNigga(list2);
            
        }

        public void HahaNiggaHowHardIsItToMoveLikeJustMoveNigga(List<HurtBox> niggas)  {
            foreach (HurtBox nigga in niggas) {

                if (nigga && nigga.healthComponent && nigga.healthComponent.body)
                {
                    var niggaBody = nigga.healthComponent.body;
                    if (NetworkServer.active)
                    {
                        niggaBody.AddTimedBuff(BuffCore.instance.wyattSuspension, 15);
                    }
                    negros.Add(niggaBody);
                }
            }   
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.rootMotion += Vector3.up * (this.moveSpeedStat * 5 * Time.fixedDeltaTime);
            base.characterMotor.velocity.y = 0f;
            if (base.isAuthority)
            {
                foreach (CharacterBody niggaBody in negros)
                {
                    CharacterMotor niggaMotor = niggaBody.characterMotor;
                    if (niggaBody && niggaMotor)
                    {
                        niggaMotor.ApplyForce(new Vector3(0, niggaMotor.mass, 0), true, true);
                    }
                }
            }
            if (fixedAge >= baseDuration)
            {
                foreach (CharacterBody niggaBody in negros)
                {
                    CharacterMotor niggaMotor = niggaBody.characterMotor;
                    if (niggaBody && niggaMotor)
                    {
                        //FUCK YOU CHARACTER MOTOR AND FUCK YOU HOPOO
                        //I ALREADY USED THIS SHITTY PHYSICS SHIT IN MY GAME
                        //I KNOW THE TRICKS
                        //I WILL FUCKING JAPE YOU
                        var a = niggaMotor.Motor.GetState();
                        a.BaseVelocity = Vector3.zero;
                        niggaMotor.Motor.ApplyState(a);
                        if (NetworkServer.active)
                        {
                            niggaBody.AddTimedBuff(BuffCore.instance.antiGrav, 5);
                        }
                        niggaMotor.velocity = Vector3.zero;
                    }
                }
                if (isAuthority)
                {
                    outer.SetNextStateToMain();
                };
            }
        }
    }
}