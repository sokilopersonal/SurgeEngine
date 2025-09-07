using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
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
                ["SuperRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/SuperRing.prefab"),
                ["DashPanel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/DashPanel.prefab"),
                ["Spring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Spring.prefab"),
                ["WideSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/WideSpring.prefab"),
                ["AirSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/AirSpring.prefab"),
                ["eFighter"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eFighterTutorial"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eSpinner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/Spinner.prefab"),
                ["eSpanner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/Spinner.prefab"),
                ["eAirCannonNormal"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/AeroCannon.prefab"),
                ["ObjCameraPan"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraPan.prefab"),
                ["ObjCameraParallel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraParallel.prefab"),
                ["JumpBoard"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpPanel_15S.prefab"),
                ["JumpBoard3D"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpPanel_30M.prefab"),
                ["TrickJumper"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/TrickPanel.prefab"),
                ["UpReel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Upreel.prefab"),
                ["JumpCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/JumpCollision.prefab"),
                ["ChangeVolumeCamera"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ChangeCameraVolume.prefab"),
                ["StumbleCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/StumbleCollision.prefab"),
                ["DashRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/DashRing.prefab"),
                ["RainbowRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/RainbowDashRing.prefab"),
                ["SetRigidBody"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/SetRigidBody.prefab"),
                ["PointMarker"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/PointMarker.prefab"),
                ["DirectionalThorn"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/DirectionalThorn.prefab"),
            };
        }

        static Dictionary<string, Action<GameObject, XElement>> GetCustomHandlers()
        {
            return new Dictionary<string, Action<GameObject, XElement>>
            {
                ["ChangeVolumeCamera"] = (go, elem) =>
                {
                    float w = GetFloatWithMultiSetParam(elem, "Collision_Width");
                    float h = GetFloatWithMultiSetParam(elem, "Collision_Height");
                    float l = GetFloatWithMultiSetParam(elem, "Collision_Length");
                    var volume = go.GetComponent<BoxCollider>();
                    volume.size = new Vector3(w, h, l);

                    float easeTimeEnter = GetFloatWithMultiSetParam(elem, "Ease_Time_Enter");
                    float easeTimeExit = GetFloatWithMultiSetParam(elem, "Ease_Time_Exit");
                    
                    var camVolume = go.GetComponent<ChangeCameraVolume>();
                    float priority = GetFloatWithMultiSetParam(elem, "Priority");
                    camVolume.GetType().GetField("priority", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(camVolume, (int)priority);
                    var targetField = camVolume.GetType().GetField("target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var volumeConnectedId = elem.Element("Target").Element("SetObjectID")?.Value.Trim() ?? "0";
                    foreach (var cameraPan in Object.FindObjectsByType<ObjCameraBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        if (cameraPan.SetID == long.Parse(volumeConnectedId, CultureInfo.InvariantCulture))
                        {
                            targetField.SetValue(camVolume, cameraPan);
                            
                            var dataField = cameraPan.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
                            var dataObj = dataField.GetValue(cameraPan);
                            dataObj.GetType().GetField("easeTimeEnter", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, easeTimeEnter);
                            dataObj.GetType().GetField("easeTimeExit", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, easeTimeExit);
                            break;
                        }
                    }
                },
                ["DashPanel"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "Speed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var dashPanel = go.GetComponent<DashPanel>();
                    SetFloatReflection(dashPanel, "speed", speed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(dashPanel, "outOfControl", outOfControl);
                },
                ["JumpBoard"] = (go, elem) =>
                {
                    int angleType = (int)GetFloatWithMultiSetParam(elem, "AngleType");
                    float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
                    float impulseBoost = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnBoost");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var jumpPanel = go.GetComponent<JumpPanel>();
                    SetFloatReflection(jumpPanel, "impulseOnNormal", impulseNormal / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpPanel, "impulseOnBoost", impulseBoost / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpPanel, "outOfControl", outOfControl);

                    if (angleType == 0)
                    {
                        SetFloatReflection(jumpPanel, "pitch", 15);
                    }
                    else
                    {
                        SetFloatReflection(jumpPanel, "pitch", 30);
                    }
                },
                ["JumpBoard3D"] = (go, elem) =>
                {
                    float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
                    float impulseBoost = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnBoost");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var jumpPanel = go.GetComponent<JumpPanel3D>();
                    SetFloatReflection(jumpPanel, "impulseOnNormal", impulseNormal / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpPanel, "impulseOnBoost", impulseBoost / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpPanel, "outOfControl", outOfControl);
                },
                ["TrickJumper"] = (go, elem) =>
                {
                    float firstSpeed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float firstPitch = GetFloatWithMultiSetParam(elem, "FirstPitch");
                    float secondSpeed = GetFloatWithMultiSetParam(elem, "SecondSpeed");
                    float secondPitch = GetFloatWithMultiSetParam(elem, "SecondPitch");
                    float firstOutOfControl = GetFloatWithMultiSetParam(elem, "FirstOutOfControl");
                    float secondOutOfControl = GetFloatWithMultiSetParam(elem, "SecondOutOfControl");
                    float trickCount1 = GetFloatWithMultiSetParam(elem, "TrickCount1");
                    float trickCount2 = GetFloatWithMultiSetParam(elem, "TrickCount2");
                    float trickCount3 = GetFloatWithMultiSetParam(elem, "TrickCount3");
                    float trickTime1 = GetFloatWithMultiSetParam(elem, "TrickTime1");
                    float trickTime2 = GetFloatWithMultiSetParam(elem, "TrickTime2");
                    float trickTime3 = GetFloatWithMultiSetParam(elem, "TrickTime3");
                    var trickJumper = go.GetComponent<TrickJumper>();
                    SetFloatReflection(trickJumper, "initialSpeed", firstSpeed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(trickJumper, "initialPitch", firstPitch);
                    SetFloatReflection(trickJumper, "finalSpeed", secondSpeed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(trickJumper, "finalPitch", secondPitch);
                    SetFloatReflection(trickJumper, "initialOutOfControl", firstOutOfControl);
                    SetFloatReflection(trickJumper, "finalOutOfControl", secondOutOfControl);
                    SetIntReflection(trickJumper, "trickCount1", (int)trickCount1);
                    SetIntReflection(trickJumper, "trickCount2", (int)trickCount2);
                    SetIntReflection(trickJumper, "trickCount3", (int)trickCount3);
                    SetFloatReflection(trickJumper, "trickTime1", trickTime1);
                    SetFloatReflection(trickJumper, "trickTime2", trickTime2);
                    SetFloatReflection(trickJumper, "trickTime3", trickTime3);
                },
                ["Spring"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepVelocity = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var spring = go.GetComponent<Spring>();
                    SetFloatReflection(spring, "speed", speed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(spring, "outOfControl", outOfControl);
                    SetFloatReflection(spring, "keepVelocityDistance", keepVelocity);
                },
                ["AirSpring"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepVelocity = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var spring = go.GetComponent<Spring>();
                    SetFloatReflection(spring, "speed", speed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(spring, "outOfControl", outOfControl);
                    SetFloatReflection(spring, "keepVelocityDistance", keepVelocity);
                },
                ["WideSpring"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepVelocity = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var wideSpring = go.GetComponent<WideSpring>();
                    SetFloatReflection(wideSpring, "speed", speed  / HE1Variables.ImpulseDivider);
                    SetFloatReflection(wideSpring, "outOfControl", outOfControl);
                    SetFloatReflection(wideSpring, "keepVelocityDistance", keepVelocity);
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
                    SetFloatReflection(jumpCollision, "speedMin", speedMin / 2);
                    SetFloatReflection(jumpCollision, "outOfControl", outOfControl);
                    SetFloatReflection(jumpCollision, "impulseOnNormal", impulseNormal / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpCollision, "impulseOnBoost", impulseBoost / HE1Variables.ImpulseDivider);
                    SetFloatReflection(jumpCollision, "pitch", pitchMax);
                },
                ["ObjCameraPan"] = (go, elem) =>
                {
                    float fovy = GetFloatWithMultiSetParam(elem, "Fovy");
                    
                    var comp = go.GetComponent<ObjCameraPan>();
                    var dataField = comp.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dataObj = dataField.GetValue(comp);
                    dataObj.GetType().GetField("fov", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, fovy);
                },
                ["ObjCameraParallel"] = (go, elem) =>
                {
                    float fovy = GetFloatWithMultiSetParam(elem, "Fovy");
                    
                    var comp = go.GetComponent<ObjCameraParallel>();
                    
                    var dataField = comp.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dataObj = dataField.GetValue(comp);
                    dataObj.GetType().GetField("fov", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, fovy);
                    
                    float pitch = GetFloatWithMultiSetParam(elem, "Pitch");
                    float yaw = GetFloatWithMultiSetParam(elem, "Yaw");
                    
                    var euler = comp.transform.eulerAngles;
                    euler.x = pitch;
                    euler.y = yaw == 0 ? 180 : yaw;
                    comp.transform.eulerAngles = euler;
                },
                ["StumbleCollision"] = (go, elem) =>
                {
                    var stumble = go.GetComponent<StumbleCollision>();
                    var box = go.GetComponent<BoxCollider>();
                    
                    float width = GetFloatWithMultiSetParam(elem, "Collision_Width");
                    float height = GetFloatWithMultiSetParam(elem, "Collision_Height");
                    float length = GetFloatWithMultiSetParam(elem, "Collision_Length");
                    box.size = new Vector3(width, height, length);
                    
                    float noControlTime = GetFloatWithMultiSetParam(elem, "NoControlTime");
                    SetFloatReflection(stumble, "noControlTime", noControlTime);
                },
                ["DashRing"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepDistance = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var dashRing = go.GetComponent<DashRing>();
                    SetFloatReflection(dashRing, "speed", speed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(dashRing, "outOfControl", outOfControl);
                    SetFloatReflection(dashRing, "keepVelocityDistance", keepDistance);
                },
                ["RainbowRing"] = (go, elem) =>
                {
                    float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
                    float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
                    float keepDistance = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
                    var dashRing = go.GetComponent<DashRing>();
                    SetFloatReflection(dashRing, "speed", speed / HE1Variables.ImpulseDivider);
                    SetFloatReflection(dashRing, "outOfControl", outOfControl);
                    SetFloatReflection(dashRing, "keepVelocityDistance", keepDistance);
                },
                ["SetRigidBody"] = (go, elem) =>
                {
                    float width = GetFloatWithMultiSetParam(elem, "Width");
                    float height = GetFloatWithMultiSetParam(elem, "Height");
                    float length = GetFloatWithMultiSetParam(elem, "Length");
                    var box = go.GetComponent<BoxCollider>();
                    box.size = new Vector3(width, height, length);

                    bool defaultOn = bool.Parse(elem.Element("DefaultON").Value.Trim());
                    var setRb = go.GetComponent<SetRigidBody>();
                    setRb.GetType().GetField("defaultOn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(setRb, defaultOn);
                },
                ["PointMarker"] = (go, elem) =>
                {
                    float width = GetFloatWithMultiSetParam(elem, "Width");
                    float height = GetFloatWithMultiSetParam(elem, "Height");
                    
                    var box = go.GetComponent<BoxCollider>();
                    Vector3 size = box.size;
                    size.x = width;
                    size.y = height;
                    box.size = size;

                    Vector3 center = box.center;
                    center.y = height / 2;
                    box.center = center;
                },
                ["DirectionalThorn"] = (go, elem) =>
                {
                    float moveTime = GetFloatWithMultiSetParam(elem, "MoveTime");
                    float onTime = GetFloatWithMultiSetParam(elem, "OnTime");
                    float offTime = GetFloatWithMultiSetParam(elem, "OffTime");
                    
                    var dir = go.GetComponent<DirectionalThorn>();
                    SetFloatReflection(dir, "moveTime", moveTime);
                    SetFloatReflection(dir, "onTime", onTime);
                    SetFloatReflection(dir, "offTime", offTime);
                },
            };
        }

        public static void ReadObjects(string xmlPath)
        {
            if (!File.Exists(xmlPath)) return;
            var doc = XDocument.Load(xmlPath);

            int group = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Import HE1 Objects");

            var applyQueue = new List<(string name, GameObject go, XElement elem)>();

            foreach (var elem in doc.Root.Elements())
            {
                var name = elem.Name.LocalName;
                var prefabs = GetHEObjectsPrefabs();

                if (prefabs.ContainsKey(name))
                {
                    if (!prefabs.TryGetValue(name, out var prefab) || prefab == null) return;

                    var ms = elem.Element("MultiSetParam");
                    if (ms != null)
                    {
                        var parent = TryInstantiate(prefab, elem, GetObjectID(elem));
                        applyQueue.Add((name, parent, elem));
                        SetObjectID(parent, GetObjectID(elem));

                        int i = 0;
                        foreach (var child in ms.Elements("Element"))
                        {
                            long childId = GetMultiSetObjectID(elem, i++);
                            var msGO = TryInstantiate(prefab, child, childId);
                            applyQueue.Add((name, msGO, child));
                            SetObjectID(msGO, childId);
                        }
                    }
                    else
                    {
                        var parent = TryInstantiate(prefab, elem, GetObjectID(elem));
                        applyQueue.Add((name, parent, elem));
                        SetObjectID(parent, GetObjectID(elem));
                    }
                }

                if (name == "ObjectPhysics")
                {
                    var phys = CreateObjectPhysics(elem);
                    if (phys)
                        applyQueue.Add((name, phys, elem));
                }
            }

            foreach (var (name, go, elem) in applyQueue)
                ApplyCustom(name, go, elem);

            Undo.CollapseUndoOperations(group);
            EditorSceneManager.MarkAllScenesDirty();
        }

        private static GameObject CreateObjectPhysics(XElement elem)
        {
            string type = GetValueWithMultiSetParam(elem, "Type", "None");
            string path = "";

            switch (type)
            {
                case "ThornCylinder2M":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/ThornSC.prefab";
                    break;
                case "ThornCylinder3M":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/ThornSCB.prefab";
                    break;
                case "IronBox2":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/IronBox.prefab";
                    break;
                case "myk_obj_hh_potplant_mixDS":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/PotPlantMixDS.prefab";
                    break;
                case "myk_obj_hh_potplant_mixEF":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/PotPlantMixEF.prefab";
                    break;
            }
            
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) return null;

            var go = TryInstantiate(asset, elem, GetObjectID(elem));
            SetObjectID(go, GetObjectID(elem));
            
            var ms = elem.Element("MultiSetParam");
            if (ms != null)
            {
                int i = 0;
                foreach (var child in ms.Elements("Element"))
                {
                    long childId = GetMultiSetObjectID(elem, i++);
                    var msGO = TryInstantiate(asset, child, childId);
                    SetObjectID(msGO, childId);
                }
            }

            return go;
        }

        static GameObject TryInstantiate(GameObject prefab, XElement elem, long setId)
        {
            foreach (var stageObject in Object.FindObjectsByType<StageObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (stageObject.SetID == setId)
                {
                    return stageObject.gameObject;
                }
            }
            
            var posE = elem.Element("Position");
            var rotE = elem.Element("Rotation");
            
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

            GameObject go;
            if (PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab)
            {
                go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            else
            {
                go = prefab;
                Debug.Log("1231");
            }
            
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

        static void SetObjectID(GameObject go, long id)
        {
            if (go.TryGetComponent(out StageObject stageObject))
                stageObject.SetID = id;
        }

        static float GetFloatWithMultiSetParam(XElement elem, string valueName, float defaultValue = 1f)
        {
            var value = GetValueWithMultiSetParam(elem, valueName, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value, CultureInfo.InvariantCulture);
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

        static long GetObjectID(XElement elem) => long.Parse(elem.Element("SetObjectID")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture);

        static long GetMultiSetObjectID(XElement parentElem, int index)
        {
            long parentId = GetObjectID(parentElem);
            return parentId * 1000 + index;
        }

        static void SetFloatReflection(object obj, string name, float value)
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

        static void SetIntReflection(object obj, string name, int value)
        {
            try
            {
                var field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
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
            {
                handler(go, elem);

                if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                    foreach (var component in go.GetComponents<StageObject>())
                        PrefabUtility.RecordPrefabInstancePropertyModifications(component);
                }
            }
        }
    }
}
