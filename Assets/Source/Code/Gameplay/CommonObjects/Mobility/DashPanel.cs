using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Inputs;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class DashPanel : StageObject, IHE1Importable
    {
        [SerializeField] private float speed = 50f;
        [SerializeField] private float speedMin = 25f;
        [SerializeField] private float speedMax = 60f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool isConstantStartVelocity = true;
        [SerializeField, Tooltip("When enabled, the camera will lag behind after triggering the dash panel.")] private bool isUseDelayCamera = true;
        public bool IsUseDelayCamera => isUseDelayCamera;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            context.StateMachine.SetState<FStateGround>();
            Rigidbody body = context.Kinematics.Rigidbody;
            body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            var bodySpeed = context.Kinematics.Speed;
            body.linearVelocity = transform.forward * GetTargetSpeed(bodySpeed);

            if (outOfControl > 0) context.Flags.AddFlag(new Flag(FlagType.OutOfControl, outOfControl));

            Rumble.Vibrate(0.7f, 0.9f, 0.5f);
        }
        
        private float GetTargetSpeed(float bodySpeed)
        {
            if (isConstantStartVelocity)
                return bodySpeed < speed ? speed : bodySpeed;

            return Mathf.Clamp(bodySpeed, speedMin, speedMax);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            speed = HE1Helper.GetFloat(elem, "Speed");
            speedMin = HE1Helper.GetFloat(elem, "SpeedMin");
            speedMax = HE1Helper.GetFloat(elem, "SpeedMax");
            outOfControl = HE1Helper.GetFloat(elem, "OutOfControl");
            isConstantStartVelocity = HE1Helper.GetBool(elem, "IsConstantStartVelocity");
            isUseDelayCamera = HE1Helper.GetBool(elem, "IsUseDelayCamera");
        }
    }
}