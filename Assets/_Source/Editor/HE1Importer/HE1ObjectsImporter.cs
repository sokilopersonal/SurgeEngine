using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SurgeEngine._Source.Editor.HE1Importer
{
    public static class HE1ObjectsImporter
    {
        static Dictionary<string, GameObject> GetHEObjectsPrefabs()
        {
            return new Dictionary<string, GameObject>
            {
                ["Ring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Ring.prefab"),
                ["DashPanel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/DashPanel.prefab"),
                ["Spring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Spring.prefab"),
                ["eFighterTutorial"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eAirCannonNormal"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/AeroCannon.prefab"),
                ["ObjCameraPan"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraPan.prefab"),
                ["JumpBoard"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpPanel_15S.prefab"),
                ["JumpBoard3D"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpPanel_30M.prefab"),
                ["UpReel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Upreel.prefab"),
                ["JumpCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpCollision.prefab"),
            };
        }

        static Dictionary<string, Action<GameObject, XElement>> GetCustomHandlers()
        {
            return new Dictionary<string, Action<GameObject, XElement>>
            {
                ["DashPanel"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "Speed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var dashPanel = go.GetComponent<DashPanel>();
                    SetFloatReflection(dashPanel, "speed", speed);
                    SetFloatReflection(dashPanel, "outOfControl", outOfControl);
                },
                ["JumpBoard"] = (go, elem) =>
                {
                    float pitch = GetFloatWithMultiSetParam(elem, "AngleType");
                    float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var jumpPanel = go.GetComponent<JumpPanel>();
                    SetFloatReflection(jumpPanel, "pitch", pitch);
                    SetFloatReflection(jumpPanel, "impulse", impulseNormal);
                    SetFloatReflection(jumpPanel, "outOfControl", outOfControl);
                },
                ["JumpBoard3D"] = (go, elem) =>
                {
                    float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var jumpPanel = go.GetComponent<JumpPanel>();
                    SetFloatReflection(jumpPanel, "impulse", impulseNormal);
                    SetFloatReflection(jumpPanel, "outOfControl", outOfControl);
                },
                ["Spring"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepVelocity = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var spring = go.GetComponent<Spring>();
                    SetFloatReflection(spring, "speed", speed);
                    SetFloatReflection(spring, "outOfControl", outOfControl);
                    SetFloatReflection(spring, "keepVelocityDistance", keepVelocity);
                },
                ["UpReel"] = (go, elem) =>
                {
                    float length = GetFloatWithMultiSetParam(elem, "Length");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var upReel = go.GetComponent<Upreel>();
                    SetFloatReflection(upReel, "length", length);
                    SetFloatReflection(upReel, "outOfControl", outOfControl);
                },
                ["JumpCollision"] = (go, elem) =>
                {
                    float w = GetFloatWithMultiSetParam(elem, "Collision_Width");
                    float h = GetFloatWithMultiSetParam(elem, "Collision_Height");
                    if (go.TryGetComponent<BoxCollider>(out var bc))
                        bc.size = new Vector3(w, h, bc.size.z);
                    
                    var jumpCollision = go.GetComponent<JumpCollision>();
                    float speedMin = GetFloatWithMultiSetParam(elem, "SpeedMin");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
                    float impulseBoost = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnBoost");
                    float pitchMax = GetFloatWithMultiSetParam(elem, "Pitch");
                    SetFloatReflection(jumpCollision, "speedRequired", speedMin / 2);
                    SetFloatReflection(jumpCollision, "outOfControl", outOfControl);
                    SetFloatReflection(jumpCollision, "impulseOnNormal", impulseNormal / 2);
                    SetFloatReflection(jumpCollision, "impulseOnBoost", impulseBoost / 2);
                    SetFloatReflection(jumpCollision, "pitch", pitchMax + 20);
                }
            };
        }
        
        static string GetValueWithMultiSetParam(XElement elem, string valueName, string defaultValue = "1")
        {
            if (elem.Name == "Element" && elem.Parent?.Name == "MultiSetParam")
            {
                var parentElem = elem.Parent.Parent;
                return parentElem.Element(valueName)?.Value.Trim() ?? defaultValue;
            }

            return elem.Element(valueName)?.Value.Trim() ?? defaultValue;
        }

        static float GetFloatWithMultiSetParam(XElement elem, string valueName, float defaultValue = 1f)
        {
            var value = GetValueWithMultiSetParam(elem, valueName, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value, CultureInfo.InvariantCulture);
        }
        
        public static void ReadObjects(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return;
            var doc = XDocument.Load(xmlPath);
            
            int group = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Import HE1 Objects");

            foreach (var elem in doc.Root.Elements())
            {
                var name = elem.Name.LocalName;
                
                var prefabs = GetHEObjectsPrefabs();
                if (prefabs.ContainsKey(name))
                {
                    if (!GetHEObjectsPrefabs().TryGetValue(name, out var prefab) || prefab == null) return;
                    
                    var ms = elem.Element("MultiSetParam");
                    if (ms != null)
                    {
                        var parent = TryInstantiate(prefab, elem.Element("Position"), elem.Element("Rotation"));
                        ApplyCustom(name, parent, elem);
                        foreach (var child in ms.Elements("Element"))
                        {
                            var msGO = TryInstantiate(prefab, child.Element("Position"), child.Element("Rotation"));
                            ApplyCustom(name, msGO, child);
                        }
                    }
                    else
                    {
                        var parent = TryInstantiate(prefab, elem.Element("Position"), elem.Element("Rotation"));
                        ApplyCustom(name, parent, elem);
                    }
                }

                if (name == "ObjectPhysics")
                {
                    CreateObjectPhysics(elem);
                }
            }
            
            Undo.CollapseUndoOperations(group);
        }

        private static void CreateObjectPhysics(XElement elem)
        {
            string type = GetValueWithMultiSetParam(elem, "Type", "None");
            string path = "";

            switch (type)
            {
                case "ThornCylinder2M":
                    path = "Assets/_Source/Prefabs/HE1/Common/Thorns/ThornSC.prefab";
                    break;
            }
            
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) return;

            TryInstantiate(asset, elem.Element("Position"), elem.Element("Rotation"));
        }

        static GameObject TryInstantiate(GameObject prefab, XElement posE, XElement rotE)
        {
            var p = posE == null
                ? Vector3.zero
                : new Vector3(
                    -float.Parse(posE.Element("x")?.Value.Trim()  ?? "0", CultureInfo.InvariantCulture),
                    float.Parse(posE.Element("y")?.Value.Trim()  ?? "0", CultureInfo.InvariantCulture),
                    float.Parse(posE.Element("z")?.Value.Trim()  ?? "0", CultureInfo.InvariantCulture)
                );
            var q = rotE == null
                ? Quaternion.identity
                : new Quaternion(
                    float.Parse(rotE.Element("x")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture),
                    float.Parse(rotE.Element("y")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture),
                    float.Parse(rotE.Element("z")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture),
                    float.Parse(rotE.Element("w")?.Value.Trim() ?? "1", CultureInfo.InvariantCulture)
                );
            q.Normalize();
            var targetRot = ToEulerYXZ(q);
            var parent = GameObject.FindWithTag("SetData").transform;

            foreach (Transform child in parent)
            {
                if (child.gameObject.name != prefab.name) continue;
                if (Vector3.Distance(child.position, p) < 0.01f 
                    && Quaternion.Angle(child.rotation, targetRot) < 1f)
                    return child.gameObject;
            }

            var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(go, "Import HE1 Objects");
            go.transform.position = p;
            go.transform.rotation = targetRot;
            go.transform.SetParent(parent, true);
            return go;
        }

        private static Quaternion ToEulerYXZ(Quaternion q)
        {
            var m = Matrix4x4.Rotate(q);

            float m02 = m.m02, m12 = m.m12, m22 = m.m22;
            float m10 = m.m10, m11 = m.m11;
        
            float pitch = Mathf.Asin(-m12);
            float yaw, roll;
        
            if (Mathf.Abs(m12) < 0.999999f)
            {
                yaw  = Mathf.Atan2(m02, m22);
                roll = Mathf.Atan2(m10, m11);
            }
            else
            {
                yaw  = 0f;
                roll = 0f;
            }

            return Quaternion.Euler(
                pitch * Mathf.Rad2Deg,
                -yaw   * Mathf.Rad2Deg,
                -roll  * Mathf.Rad2Deg
            );
        }

        static void SetFloatReflection(object obj, string name, float value)
        {
            try
            {
                var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                field.SetValue(obj, value);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't set the value to " + name + ": " + e.Message);
            }
        }
        
        static void ApplyCustom(string name, GameObject go, XElement elem)
        {
            if (GetCustomHandlers().TryGetValue(name, out var handler))
                handler(go, elem);
        }
    }
}
