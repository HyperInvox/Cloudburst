﻿using Cloudburst.Cores;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
    public class FliteredNoGravZone : MonoBehaviour
    {

        public void OnTriggerEnter(Collider other)
        {
            var characterBody = other.GetComponentInParent<CharacterBody>();
            if (characterBody)
            {
                var team = characterBody.teamComponent;
                if (team)
                {
                    if (team.teamIndex == TeamIndex.Player)
                    {


                        /*ICharacterGravityParameterProvider component = other.GetComponent<ICharacterGravityParameterProvider>();
                        if (component != null)
                        {
                            CharacterGravityParameters gravityParameters = component.gravityParameters;
                            gravityParameters.environmentalAntiGravityGranterCount++;
                            //LogCore.LogI("GRAVITY PARAMS: " + gravityParameters.channeledAntiGravityGranterCount);
                            component.gravityParameters = gravityParameters;
                        }
                        ICharacterFlightParameterProvider component2 = other.GetComponent<ICharacterFlightParameterProvider>();
                        if (component2 != null)
                        {
                            CharacterFlightParameters flightParameters = component2.flightParameters;
                            flightParameters.channeledFlightGranterCount++;
                            //LogCore.LogI(flightParameters.channeledFlightGranterCount);
                            component2.flightParameters = flightParameters;
                        }*/
                    }
                }
            }
        }
        public void OnTriggerExit(Collider other)
        {
            var characterBody = other.GetComponentInParent<CharacterBody>();
            if (characterBody)
            {
                var team = characterBody.teamComponent;
                if (team)
                {
                    if (team.teamIndex == TeamIndex.Player)
                    {
                        //is this not getting called?
                        //clients can't get the buff
                        //but the owner of the bubble can

                        /*ICharacterFlightParameterProvider component = other.GetComponent<ICharacterFlightParameterProvider>();
                        if (component != null)
                        {
                            CharacterFlightParameters flightParameters = component.flightParameters;
                            flightParameters.channeledFlightGranterCount--;
                            //LogCore.LogI("FLIGHT PARAMS: " + flightParameters.channeledFlightGranterCount);
                            component.flightParameters = flightParameters;
                        }
                        ICharacterGravityParameterProvider component2 = other.GetComponent<ICharacterGravityParameterProvider>();
                        if (component2 != null)
                        {
                            CharacterGravityParameters gravityParameters = component2.gravityParameters;
                            gravityParameters.environmentalAntiGravityGranterCount--;
                            //LogCore.LogI(gravityParameters.environmentalAntiGravityGranterCount);
                            component2.gravityParameters = gravityParameters;
                        }*/
                    }
                }
            }
        }
        public void OnDestroy()
        {
            foreach (var other in Physics.OverlapSphere(transform.position, 22.5f))
            {
                var characterBody = other.GetComponentInParent<CharacterBody>();
                if (characterBody)
                {
                    var team = characterBody.teamComponent;
                    if (team)
                    {
                        if (team.teamIndex == TeamIndex.Player)
                        {
                            /*ICharacterFlightParameterProvider component = other.GetComponent<ICharacterFlightParameterProvider>();
                            if (component != null)
                            {

                                CharacterFlightParameters flightParameters = component.flightParameters;
                                flightParameters.channeledFlightGranterCount--;
                                //LogCore.LogI("FLIGHT PARAMS: " + flightParameters.channeledFlightGranterCount);
                                component.flightParameters = flightParameters;
                            }
                            ICharacterGravityParameterProvider component2 = other.GetComponent<ICharacterGravityParameterProvider>();
                            if (component2 != null)
                            {
                                CharacterGravityParameters gravityParameters = component2.gravityParameters;
                                gravityParameters.environmentalAntiGravityGranterCount--;
                                //LogCore.LogI(gravityParameters.environmentalAntiGravityGranterCount);
                                component2.gravityParameters = gravityParameters;
                            }*/
                        }
                    }
                }
            }
        }
    }
}