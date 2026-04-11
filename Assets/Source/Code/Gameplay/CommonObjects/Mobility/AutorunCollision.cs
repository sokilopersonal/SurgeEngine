using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class AutorunCollision : StageObject, IHE1Importable
    {
        [SerializeField, Tooltip("How long should this trigger keep the autorun?" +
                                 "It's useful in cases where a player somehow passes through a trigger with some weird way. " +
                                 "Value less than 0 means infinity.")] private float keepTime = 5;
        [SerializeField, Tooltip("How fast the player should move.")] private float speed = 40f;
        [SerializeField, Tooltip("How fast the player will reach the speed.")] private float easeTime = 0.5f;
        [SerializeField, Tooltip("How much time it takes for player to get on the 2D Path")] private float pathEaseTime = 0.2f;
        [SerializeField, Tooltip("Should this trigger end the autorun?")] private bool isFinish;
        
        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            var flags = context.Flags;
            
            float dot = Vector3.Dot(context.transform.forward, transform.forward);
            if (dot < 0) return;
            
            if (!isFinish)
            {
                if (!flags.HasFlag(FlagType.Autorun))
                {
                    if (keepTime > 0)
                    {
                        flags.AddFlag(new AutorunFlag(keepTime, speed, easeTime, pathEaseTime));
                        flags.AddFlag(new Flag(FlagType.OutOfControl, keepTime));
                    }
                }
            }
            else
            {
                flags.RemoveFlag(FlagType.Autorun);
                flags.RemoveFlag(FlagType.OutOfControl);
            }
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            var box = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            box.size = new Vector3(w, h, box.size.z);
            
            keepTime = HE1Helper.GetFloat(elem, "KeepTime");
            speed = HE1Helper.GetFloat(elem, "Speed");
            easeTime = HE1Helper.GetFloat(elem, "EaseTime");
            pathEaseTime = HE1Helper.GetFloat(elem, "ToPathEaseTime");
            isFinish = objectName != "AutorunStartCollision";
        }
    }
}