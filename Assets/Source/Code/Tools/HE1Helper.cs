using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SurgeEngine.Source.Code.Tools
{
    public static class HE1Helper
    {
        public static float GetFloat(XElement elem, string valueName, float defaultValue = 1f)
        {
            var value = GetValue(elem, valueName, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static int GetInt(XElement elem, string valueName, int defaultValue = 1)
        {
            var value = GetValue(
                elem,
                valueName,
                defaultValue.ToString(CultureInfo.InvariantCulture)
            );

            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
        }
        
        public static bool GetBool(XElement elem, string valueName, bool defaultValue = false)
        {
            var value = GetValue(elem, valueName, defaultValue.ToString());
            return bool.Parse(value);
        }

        public static string GetValue(XElement elem, string valueName, string defaultValue = "1")
        {
            if (elem.Name == "Element" && elem.Parent?.Name == "MultiSetParam")
            {
                var parentElem = elem.Parent.Parent;
                return parentElem.Element(valueName)?.Value.Trim() ?? defaultValue;
            }

            return elem.Element(valueName)?.Value.Trim() ?? defaultValue;
        }

        public static void SetFloat(object obj, string name, float value)
        {
            try
            {
                var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                field?.SetValue(obj, value);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't set the value to " + name + ": " + e.Message);
            }
        }

        public static void SetInt(object obj, string name, int value)
        {
            try
            {
                var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (field != null) field.SetValue(obj, value);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't set the value to " + name + ": " + e.Message);
            }
        }

        public static void SetBool(object obj, string name, bool value)
        {
            try
            {
                var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (field != null) field.SetValue(obj, value);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't set the value to " + name + ": " + e.Message);
            }
        }

        public static void SetBoxColliderSize(BoxCollider box, XElement elem, float? depth = null)
        {
            float width = GetFloat(elem, "Collision_Width");
            float height = GetFloat(elem, "Collision_Height");

            if (depth.HasValue)
            {
                box.size = new Vector3(width, height, depth.Value);
            }
            else
            {
                float length = GetFloat(elem, "Collision_Length");
                box.size = new Vector3(width, height, length);
            }
        }

        public static void SetSpringProperties(object spring, XElement elem)
        {
            float speed = GetFloat(elem, "FirstSpeed");
            float outOfControl = GetFloat(elem, "OutOfControl");
            float keepVelocity = GetFloat(elem, "KeepVelocityDistance");
            SetFloat(spring, "speed", speed);
            SetFloat(spring, "outOfControl", outOfControl);
            SetFloat(spring, "keepVelocityDistance", keepVelocity);
        }

        public static void SetJumpPanelProperties(object jumpPanel, XElement elem)
        {
            float impulseNormal = GetFloat(elem, "ImpulseSpeedOnNormal");
            float impulseBoost = GetFloat(elem, "ImpulseSpeedOnBoost");
            float outOfControl = GetFloat(elem, "OutOfControl");
            SetFloat(jumpPanel, "impulseOnNormal", impulseNormal);
            SetFloat(jumpPanel, "impulseOnBoost", impulseBoost);
            SetFloat(jumpPanel, "outOfControl", outOfControl);
        }

        public static void SetDashRingProperties(object dashRing, XElement elem)
        {
            float speed = GetFloat(elem, "FirstSpeed");
            float outOfControl = GetFloat(elem, "OutOfControl");
            float keepDistance = GetFloat(elem, "KeepVelocityDistance");
            SetFloat(dashRing, "speed", speed);
            SetFloat(dashRing, "outOfControl", outOfControl);
            SetFloat(dashRing, "keepVelocityDistance", keepDistance);
        }

        public static void SetCameraDataProperties(object cameraComponent, XElement elem, bool includeDistance = false)
        {
            var dataField = cameraComponent.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
            if (dataField == null) return;

            var dataObj = dataField.GetValue(cameraComponent);

            float easeTimeEnter = GetFloat(elem, "Ease_Time_Enter");
            float easeTimeLeave = GetFloat(elem, "Ease_Time_Leave");
            float fovy = GetFloat(elem, "Fovy");
            bool isControllable = GetBool(elem, "IsControllable");
            bool isCollision = GetBool(elem, "IsCollision");
            
            dataObj.GetType().GetField("easeTimeEnter", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, easeTimeEnter);
            dataObj.GetType().GetField("easeTimeExit", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, easeTimeLeave);
            dataObj.GetType().GetField("fov", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, fovy);
            dataObj.GetType().GetField("allowRotation", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, isControllable);
            dataObj.GetType().GetField("isCollision", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, isCollision);

            if (includeDistance)
            {
                float distance = GetFloat(elem, "Distance");
                dataObj.GetType().GetField("distance", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, distance);
            }
        }

        public static void SetChangeMode3DProperties(object mode, XElement elem)
        {
            bool isChangeCamera = GetBool(elem, "m_IsChangeCamera");
            bool isEnabledFront = GetBool(elem, "m_IsEnableFromFront");
            bool isEnabledBack = GetBool(elem, "m_IsEnableFromBack");
            bool isLimitEdge = GetBool(elem, "m_IsLimitEdge");
            float pathCorrectionForce = GetFloat(elem, "m_PathCorrectionForce");

            SetBool(mode, "isChangeCamera", isChangeCamera);
            SetBool(mode, "isEnabledFromFront", isEnabledFront);
            SetBool(mode, "isEnabledFromBack", isEnabledBack);
            SetBool(mode, "isLimitEdge", isLimitEdge);
            SetFloat(mode, "pathCorrectionForce", pathCorrectionForce);
        }

        public static void SetChangeMode2DProperties(object mode, XElement elem)
        {
            bool isChangeCamera = GetBool(elem, "m_IsChangeCamera");
            bool isEnabledFront = GetBool(elem, "m_IsEnableFromFront");
            bool isEnabledBack = GetBool(elem, "m_IsEnableFromBack");
            float pathEaseTime = GetFloat(elem, "m_PathEaseTime");

            SetBool(mode, "isChangeCamera", isChangeCamera);
            SetBool(mode, "isEnabledFromFront", isEnabledFront);
            SetBool(mode, "isEnabledFromBack", isEnabledBack);
            SetFloat(mode, "pathEaseTime", pathEaseTime);
        }

        public static void SetObjectID(GameObject go, long id)
        {
            if (go.TryGetComponent(out StageObject stageObject))
                stageObject.SetID = id;
        }

        public static long GetObjectID(XElement elem) => long.Parse(elem.Element("SetObjectID")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture);
        public static long GetTargetID(XElement elem) => long.Parse(elem.Element("Target")!.Element("SetObjectID")!.Value, CultureInfo.InvariantCulture);

        public static long GetMultiSetObjectID(XElement parentElem, int index)
        {
            long parentId = GetObjectID(parentElem);
            return parentId * 1000 + index;
        }

        public static void FillMultiSet<T>(this T stageObject, XElement elem, string listFieldName) where T : StageObject
        {
            if (stageObject == null || elem == null) return;

            if (stageObject.GetType().GetField(listFieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(stageObject) is not List<T> list) return;

            list.Clear();

            var ms = elem.Element("MultiSetParam");
            if (ms == null) return;

            int i = 0;
            foreach (var _ in ms.Elements("Element"))
            {
                long childId = GetMultiSetObjectID(elem, i++);
                foreach (var obj in Object.FindObjectsByType<StageObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (obj.SetID == childId && obj is T tObj)
                    {
                        list.Add(tObj);
                        break;
                    }
                }
            }
        }
        
        public static Quaternion ToEulerYXZ(Quaternion q)
        {
            q.Normalize();
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, -euler.y, -euler.z);
        }
    }

    public interface IHE1Importable
    {
        void ImportSetData(string objectName, XElement elem);
    }
    
    public interface IHE1TargetResolvable
    {
        void ResolveTarget(XElement elem, Dictionary<long, StageObject> sceneObjects);
    }
}