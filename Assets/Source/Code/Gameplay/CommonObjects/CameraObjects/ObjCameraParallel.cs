using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraParallel : ObjCameraBase<ParallelCameraPan, ParallelPanData>
    {
        private void Awake()
        {
            data.position = transform.position;
            data.forward = transform.forward;
        }

        private void Update()
        {
            data.forward = transform.forward;
        }

        public override void ImportSetData(string objectName, XElement elem)
        {
            base.ImportSetData(objectName, elem);
            
            float distance = HE1Helper.GetFloat(elem, "Distance");
            float targetOffsetUp = HE1Helper.GetFloat(elem, "TargetOffset_Up");
            data.distance = distance;
            data.yOffset = targetOffsetUp;
            
            float pitch = HE1Helper.GetFloat(elem, "Pitch");
            float yaw = HE1Helper.GetFloat(elem, "Yaw");
            
            var euler = Quaternion.Euler(pitch, yaw - 180, 0);
            transform.rotation = HE1Helper.ToEulerYXZ(euler);
        }
    }
}