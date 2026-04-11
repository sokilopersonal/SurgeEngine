using System.Xml.Linq;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class StumbleCollision : StageObject, IHE1Importable
    {
        [Tooltip("Speed required to trigger the stumble")]
        [SerializeField] private float launchVelocity = 14;
        [SerializeField] private float noControlTime = 0.5f;
        [SerializeField] private EventReference stumbleSound;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                if (character.StateMachine.CurrentState is not FStateStumble && character.Kinematics.Speed >= launchVelocity && character.Kinematics.CheckForGround(out _))
                {
                    const float stumbleSpeed = 10;
                
                    character.Rigidbody.linearVelocity = character.transform.forward * stumbleSpeed + 
                                                       character.transform.up * stumbleSpeed;
                
                    if (noControlTime > 0) character.Flags.AddFlag(new Flag(FlagType.OutOfControl, noControlTime));
                    if (character.StateMachine.GetState(out FBoost boost))
                    {
                        boost.Active = false;
                    }
                
                    RuntimeManager.PlayOneShot(stumbleSound, character.transform.position);

                    character.StateMachine.SetState<FStateStumble>().SetNoControlTime(noControlTime);
                }
            }
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            var box = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            float l = HE1Helper.GetFloat(elem, "Collision_Length");
            box.size = new Vector3(w * 1.75f, h, l);
            
            launchVelocity = HE1Helper.GetFloat(elem, "LaunchVelocity");
            noControlTime = HE1Helper.GetFloat(elem, "NoControlTime");
        }
    }
}