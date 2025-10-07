using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.PhysicsObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
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
                ["SpringFake"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/SpringFake.prefab"),
                ["WideSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/WideSpring.prefab"),
                ["AirSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/AirSpring.prefab"),
                ["ThornSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/ThornSpring.prefab"),
                ["eFighter"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eFighterGun"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"), // TODO: Implement an actual eFighterGun
                ["eFighterMissile"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"), // TODO: Implement an actual eFighterMissile
                ["eFighterTutorial"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eSpinner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/Spinner.prefab"),
                ["eSpanner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/Spinner.prefab"), // TODO: Implement an actual eSpanner
                ["eAirCannonNormal"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Enemies/AeroCannon.prefab"),
                ["ObjCameraPan"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraPan.prefab"),
                ["ObjCameraParallel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraParallel.prefab"),
                ["ObjCameraPoint"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraPoint.prefab"),
                ["ObjCameraPathTarget"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Camera/ObjCameraPathTarget.prefab"),
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
                ["ChangeMode_3DtoForward"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/System/ChangeMode_3DtoForward.prefab"),
                ["ChangeMode_3DtoDash"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/System/ChangeMode_3DtoDash.prefab"),
                ["ChangeMode_3Dto2D"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/System/ChangeMode_2D.prefab"),
                ["AutorunStartCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/AutorunCollision.prefab"),
                ["AutorunFinishCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/AutorunCollision.prefab"),
                ["GoalRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/GoalRing/GoalRing.prefab"),
                ["Flame"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/Flame.prefab"),
                ["EventCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Common/EventCollision.prefab"),
                ["MykonosFloor"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Source/Prefabs/HE1/Apotos/MykonosFloor.prefab"),
            };
        }

        static Dictionary<string, Action<GameObject, XElement>> GetCustomHandlers()
        {
            return new Dictionary<string, Action<GameObject, XElement>>
            {
                ["ChangeVolumeCamera"] = (go, elem) =>
                {
                    var volume = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(volume, elem);

                    float easeTimeEnter = HE1Helper.GetFloatWithMultiSetParam(elem, "Ease_Time_Enter");
                    float easeTimeExit = HE1Helper.GetFloatWithMultiSetParam(elem, "Ease_Time_Leave");

                    var camVolume = go.GetComponent<ChangeCameraVolume>();
                    float priority = HE1Helper.GetFloatWithMultiSetParam(elem, "Priority");
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
                            RecordStageObject(cameraPan.gameObject);
                            break;
                        }
                    }
                },
                ["DashPanel"] = (go, elem) =>
                {
                    float speed = HE1Helper.GetFloatWithMultiSetParam(elem, "Speed");
                    float outOfControl = HE1Helper.GetFloatWithMultiSetParam(elem, "OutOfControl");
                    var dashPanel = go.GetComponent<DashPanel>();
                    HE1Helper.SetFloatReflection(dashPanel, "speed", speed);
                    HE1Helper.SetFloatReflection(dashPanel, "outOfControl", outOfControl);
                },
                ["JumpBoard"] = (go, elem) =>
                {
                    int angleType = (int)HE1Helper.GetFloatWithMultiSetParam(elem, "AngleType");
                    var jumpPanel = go.GetComponent<JumpPanel>();
                    HE1Helper.SetJumpPanelProperties(jumpPanel, elem);
                    HE1Helper.SetFloatReflection(jumpPanel, "pitch", angleType == 0 ? 15 : 30);
                },
                ["JumpBoard3D"] = (go, elem) =>
                {
                    var jumpPanel = go.GetComponent<JumpPanel3D>();
                    HE1Helper.SetJumpPanelProperties(jumpPanel, elem);
                },
                ["TrickJumper"] = (go, elem) =>
                {
                    var trickJumper = go.GetComponent<TrickJumper>();
                    HE1Helper.SetFloatReflection(trickJumper, "initialSpeed", HE1Helper.GetFloatWithMultiSetParam(elem, "FirstSpeed"));
                    HE1Helper.SetFloatReflection(trickJumper, "initialPitch", HE1Helper.GetFloatWithMultiSetParam(elem, "FirstPitch"));
                    HE1Helper.SetFloatReflection(trickJumper, "finalSpeed", HE1Helper.GetFloatWithMultiSetParam(elem, "SecondSpeed"));
                    HE1Helper.SetFloatReflection(trickJumper, "finalPitch", HE1Helper.GetFloatWithMultiSetParam(elem, "SecondPitch"));
                    HE1Helper.SetFloatReflection(trickJumper, "initialOutOfControl", HE1Helper.GetFloatWithMultiSetParam(elem, "FirstOutOfControl"));
                    HE1Helper.SetFloatReflection(trickJumper, "finalOutOfControl", HE1Helper.GetFloatWithMultiSetParam(elem, "SecondOutOfControl"));
                    HE1Helper.SetIntReflection(trickJumper, "trickCount1", HE1Helper.GetIntWithMultiSetParam(elem, "TrickCount1"));
                    HE1Helper.SetIntReflection(trickJumper, "trickCount2", HE1Helper.GetIntWithMultiSetParam(elem, "TrickCount2"));
                    HE1Helper.SetIntReflection(trickJumper, "trickCount3", HE1Helper.GetIntWithMultiSetParam(elem, "TrickCount3"));
                    HE1Helper.SetFloatReflection(trickJumper, "trickTime1", HE1Helper.GetFloatWithMultiSetParam(elem, "TrickTime1"));
                    HE1Helper.SetFloatReflection(trickJumper, "trickTime2", HE1Helper.GetFloatWithMultiSetParam(elem, "TrickTime2"));
                    HE1Helper.SetFloatReflection(trickJumper, "trickTime3", HE1Helper.GetFloatWithMultiSetParam(elem, "TrickTime3"));
                },
                ["Spring"] = (go, elem) =>
                {
                    var spring = go.GetComponent<Spring>();
                    HE1Helper.SetSpringProperties(spring, elem);
                },
                ["SpringFake"] = (go, elem) =>
                {
                    var spring = go.GetComponent<SpringFake>();
                    HE1Helper.SetSpringProperties(spring, elem);
                },
                ["AirSpring"] = (go, elem) =>
                {
                    var spring = go.GetComponent<Spring>();
                    HE1Helper.SetSpringProperties(spring, elem);
                },
                ["WideSpring"] = (go, elem) =>
                {
                    var wideSpring = go.GetComponent<WideSpring>();
                    HE1Helper.SetSpringProperties(wideSpring, elem);
                },
                ["UpReel"] = (go, elem) =>
                {
                    var upReel = go.GetComponent<Upreel>();
                    HE1Helper.SetFloatReflection(upReel, "length", HE1Helper.GetFloatWithMultiSetParam(elem, "Length"));
                    HE1Helper.SetFloatReflection(upReel, "outOfControl", HE1Helper.GetFloatWithMultiSetParam(elem, "OutOfControl"));
                    HE1Helper.SetFloatReflection(upReel, "upMaxSpeed", HE1Helper.GetFloatWithMultiSetParam(elem, "UpMaxSpeed"));
                    HE1Helper.SetFloatReflection(upReel, "impulseVelocity", HE1Helper.GetFloatWithMultiSetParam(elem, "ImpulseVelocity"));
                },
                ["JumpCollision"] = (go, elem) =>
                {
                    if (go.TryGetComponent<BoxCollider>(out var bc))
                    {
                        float w = HE1Helper.GetFloatWithMultiSetParam(elem, "Collision_Width");
                        float h = HE1Helper.GetFloatWithMultiSetParam(elem, "Collision_Height");
                        bc.size = new Vector3(w, h, bc.size.z);
                    }

                    var jumpCollision = go.GetComponent<JumpCollision>();
                    HE1Helper.SetFloatReflection(jumpCollision, "speedMin", HE1Helper.GetFloatWithMultiSetParam(elem, "SpeedMin") / 2);
                    HE1Helper.SetFloatReflection(jumpCollision, "outOfControl", HE1Helper.GetFloatWithMultiSetParam(elem, "OutOfControl"));
                    HE1Helper.SetFloatReflection(jumpCollision, "impulseOnNormal", HE1Helper.GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal"));
                    HE1Helper.SetFloatReflection(jumpCollision, "impulseOnBoost", HE1Helper.GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnBoost"));
                    HE1Helper.SetFloatReflection(jumpCollision, "pitch", HE1Helper.GetFloatWithMultiSetParam(elem, "Pitch"));
                },
                ["ObjCameraPan"] = (go, elem) =>
                {
                    var comp = go.GetComponent<ObjCameraPan>();
                    HE1Helper.SetCameraDataProperties(comp, elem);
                },
                ["ObjCameraParallel"] = (go, elem) =>
                {
                    var comp = go.GetComponent<ObjCameraParallel>();
                    HE1Helper.SetCameraDataProperties(comp, elem, includeDistance: true);

                    float pitch = HE1Helper.GetFloatWithMultiSetParam(elem, "Pitch");
                    float yaw = HE1Helper.GetFloatWithMultiSetParam(elem, "Yaw");
                    
                    var euler = Quaternion.Euler(pitch, yaw - 180, 0);
                    comp.transform.rotation = ToEulerYXZ(euler);
                },
                ["ObjCameraPoint"] = (go, elem) =>
                {
                    var comp = go.GetComponent<ObjCameraPoint>();
                    HE1Helper.SetCameraDataProperties(comp, elem, includeDistance: true);

                    var dataField = comp.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dataObj = dataField?.GetValue(comp);
                    if (dataObj == null) return;

                    long targetId = long.Parse(elem.Element("Target").Element("SetObjectID").Value, CultureInfo.InvariantCulture);
                    if (targetId != 0)
                    {
                        foreach (var stageObject in Object.FindObjectsByType<StageObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                        {
                            if (stageObject.SetID == targetId)
                            {
                                dataObj.GetType().GetField("target", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, stageObject.transform);
                                break;
                            }
                        }
                    }
                    else
                    {
                        dataObj.GetType().GetField("target", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, null);
                    }
                },
                ["ObjCameraPathTarget"] = (go, elem) =>
                {
                    var comp = go.GetComponent<ObjCameraPathTarget>();
                    HE1Helper.SetCameraDataProperties(comp, elem);
                    
                    var dataField = comp.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dataObj = dataField?.GetValue(comp);
                    if (dataObj == null) return;
                    
                    float offsetOnEyePath = HE1Helper.GetFloatWithMultiSetParam(elem, "OffsetOnEyePath");
                    HE1Helper.SetFloatReflection(dataObj, "offsetOnEye", offsetOnEyePath);
                },
                ["StumbleCollision"] = (go, elem) =>
                {
                    var stumble = go.GetComponent<StumbleCollision>();
                    var box = go.GetComponent<BoxCollider>();
                    
                    HE1Helper.SetBoxColliderSize(box, elem);
                    box.size = new Vector3(box.size.x * 1.75f, box.size.y, box.size.z);

                    HE1Helper.SetFloatReflection(stumble, "noControlTime", HE1Helper.GetFloatWithMultiSetParam(elem, "NoControlTime"));
                },
                ["DashRing"] = (go, elem) =>
                {
                    var dashRing = go.GetComponent<DashRing>();
                    HE1Helper.SetDashRingProperties(dashRing, elem);
                },
                ["RainbowRing"] = (go, elem) =>
                {
                    var dashRing = go.GetComponent<DashRing>();
                    HE1Helper.SetDashRingProperties(dashRing, elem);
                },
                ["SetRigidBody"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    float width = HE1Helper.GetFloatWithMultiSetParam(elem, "Width");
                    float height = HE1Helper.GetFloatWithMultiSetParam(elem, "Height");
                    float length = HE1Helper.GetFloatWithMultiSetParam(elem, "Length");
                    box.size = new Vector3(width, height, length);

                    bool defaultOn = bool.Parse(elem.Element("DefaultON").Value.Trim());
                    var setRb = go.GetComponent<SetRigidBody>();
                    setRb.GetType().GetField("defaultOn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(setRb, defaultOn);
                },
                ["PointMarker"] = (go, elem) =>
                {
                    float width = HE1Helper.GetFloatWithMultiSetParam(elem, "Width");
                    float height = HE1Helper.GetFloatWithMultiSetParam(elem, "Height");
                    
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
                    var dir = go.GetComponent<DirectionalThorn>();
                    HE1Helper.SetFloatReflection(dir, "moveTime", HE1Helper.GetFloatWithMultiSetParam(elem, "MoveTime"));
                    HE1Helper.SetFloatReflection(dir, "onTime", HE1Helper.GetFloatWithMultiSetParam(elem, "OnTime"));
                    HE1Helper.SetFloatReflection(dir, "offTime", HE1Helper.GetFloatWithMultiSetParam(elem, "OffTime"));
                    HE1Helper.SetIntReflection(dir, "phase", HE1Helper.GetIntWithMultiSetParam(elem, "Phase"));
                },
                ["ChangeMode_3DtoForward"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, 0.2f);

                    var mode = go.GetComponent<ChangeMode3D>();
                    HE1Helper.SetChangeMode3DProperties(mode, elem);
                },
                ["ChangeMode_3DtoDash"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, 0.2f);

                    var mode = go.GetComponent<ChangeMode3D>();
                    HE1Helper.SetChangeMode3DProperties(mode, elem);
                },
                ["ChangeMode_3Dto2D"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, 0.2f);
                },
                ["AutorunStartCollision"] = (go, elem) =>
                {
                    var autorun = go.GetComponent<AutorunCollision>();
                    
                    HE1Helper.SetBoxColliderSize(autorun.GetComponent<BoxCollider>(), elem);
                    HE1Helper.SetFloatReflection(autorun, "speed", HE1Helper.GetFloatWithMultiSetParam(elem, "Speed"));
                    HE1Helper.SetFloatReflection(autorun, "easeTime", HE1Helper.GetFloatWithMultiSetParam(elem, "EaseTime"));
                    HE1Helper.SetFloatReflection(autorun, "keepTime", HE1Helper.GetFloatWithMultiSetParam(elem, "KeepTime"));
                    HE1Helper.SetBoolReflection(autorun, "isFinish", false);
                },
                ["AutorunFinishCollision"] = (go, elem) =>
                {
                    var autorun = go.GetComponent<AutorunCollision>();
                    
                    HE1Helper.SetBoxColliderSize(autorun.GetComponent<BoxCollider>(), elem);
                    HE1Helper.SetBoolReflection(autorun, "isFinish", true);
                },
                ["Flame"] = (go, elem) =>
                {
                    var flame = go.GetComponent<Flame>();
                    HE1Helper.SetFloatReflection(flame, "appearTime", HE1Helper.GetFloatWithMultiSetParam(elem, "AppearTime"));
                    HE1Helper.SetFloatReflection(flame, "onTime", HE1Helper.GetFloatWithMultiSetParam(elem, "OnTime"));
                    HE1Helper.SetFloatReflection(flame, "offTime", HE1Helper.GetFloatWithMultiSetParam(elem, "OffTime"));
                    HE1Helper.SetFloatReflection(flame, "length", HE1Helper.GetFloatWithMultiSetParam(elem, "Length"));
                    HE1Helper.SetIntReflection(flame, "type", HE1Helper.GetIntWithMultiSetParam(elem, "Type"));
                    HE1Helper.SetIntReflection(flame, "phase", HE1Helper.GetIntWithMultiSetParam(elem, "Phase"));

                    flame.FillMultiSet(elem, "multiSetParam");
                },
                ["EventCollision"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem);

                    var eventCollision = go.GetComponent<EventCollision>();
                    HE1Helper.SetIntReflection(eventCollision, "defaultStatus", HE1Helper.GetIntWithMultiSetParam(elem, "DefaultStatus"));
                    HE1Helper.SetIntReflection(eventCollision, "durability", HE1Helper.GetIntWithMultiSetParam(elem, "Durability"));

                    var uEvent =
                        eventCollision.GetType()
                            .GetField("eventOnContact", BindingFlags.Instance | BindingFlags.NonPublic)
                            ?.GetValue(eventCollision) as UnityEvent;

                    if (uEvent != null && uEvent.GetPersistentEventCount() == 0)
                    {
                        Debug.LogWarning("[HE1 Importer] Due to how events work in SU, the event won't be assigned. You should do it manually.");
                    }
                },
                ["MykonosFloor"] = (go, elem) =>
                {
                    var floor = go.GetComponent<MykonosFloor>();
                    HE1Helper.SetFloatReflection(floor, "amplitude", HE1Helper.GetFloatWithMultiSetParam(elem, "Amplitude"));
                    HE1Helper.SetFloatReflection(floor, "cycle", HE1Helper.GetFloatWithMultiSetParam(elem, "Cycle"));
                    HE1Helper.SetFloatReflection(floor, "phase", HE1Helper.GetFloatWithMultiSetParam(elem, "Phase"));
                    HE1Helper.SetIntReflection(floor, "moveType", HE1Helper.GetIntWithMultiSetParam(elem, "MoveType"));
                    HE1Helper.SetFloatReflection(floor, "onGroundTime", HE1Helper.GetFloatWithMultiSetParam(elem, "OnGroundTime"));
                    HE1Helper.SetFloatReflection(floor, "resetTime", HE1Helper.GetFloatWithMultiSetParam(elem, "ResetTime"));
                    HE1Helper.SetFloatReflection(floor, "gravity", HE1Helper.GetFloatWithMultiSetParam(elem, "Gravity"));
                },
                ["ThornSpring"] = (go, elem) =>
                {
                    var thornSpring = go.GetComponent<ThornSpring>();
                    HE1Helper.SetSpringProperties(thornSpring, elem);
                    HE1Helper.SetFloatReflection(thornSpring, "upThornTime", HE1Helper.GetFloatWithMultiSetParam(elem, "UpThornTime"));
                    HE1Helper.SetFloatReflection(thornSpring, "downThornTime", HE1Helper.GetFloatWithMultiSetParam(elem, "DownThornTime"));
                    HE1Helper.SetBoolReflection(thornSpring, "cancelBoost", HE1Helper.GetBoolWithMultiSetParam(elem, "m_IsStopBoost"));
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
                    if (!prefabs.TryGetValue(name, out var prefab) || prefab == null) continue;

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
            string type = HE1Helper.GetValueWithMultiSetParam(elem, "Type", "None");
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
                case "myk_obj_ky_blue_woodbox":
                    path = "Assets/_Source/Prefabs/HE1/Common/ObjectPhysics/BlueWoodBox.prefab";
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
            
            var targetRot = ToEulerYXZ(q);
            
            foreach (var stageObject in Object.FindObjectsByType<StageObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (stageObject.SetID == setId && stageObject.SetID != 0)
                {
                    stageObject.transform.position = p;
                    stageObject.transform.rotation = targetRot;
                    return stageObject.gameObject;
                }
            }
            
            var parent = GameObject.FindWithTag("SetData").transform;

            GameObject go;
            if (PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab)
            {
                go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            else
            {
                go = prefab;
            }
            
            Undo.RegisterCreatedObjectUndo(go, "Import HE1 Objects");
            go.transform.position = p;
            go.transform.rotation = targetRot;
            go.transform.SetParent(parent, true);
            return go;
        }

        private static Quaternion ToEulerYXZ(Quaternion q)
        {
            q.Normalize();
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, -euler.y, -euler.z);
        }

        static void SetObjectID(GameObject go, long id)
        {
            if (go.TryGetComponent(out StageObject stageObject))
                stageObject.SetID = id;
        }

        static long GetObjectID(XElement elem) => long.Parse(elem.Element("SetObjectID")?.Value.Trim() ?? "0", CultureInfo.InvariantCulture);

        public static long GetMultiSetObjectID(XElement parentElem, int index)
        {
            long parentId = GetObjectID(parentElem);
            return parentId * 1000 + index;
        }
        
        static void ApplyCustom(string name, GameObject go, XElement elem)
        {
            if (GetCustomHandlers().TryGetValue(name, out var handler))
            {
                handler(go, elem);

                RecordStageObject(go);
            }
        }

        private static void RecordStageObject(GameObject go)
        {
            if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                foreach (var component in go.GetComponents<StageObject>())
                    PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            }
        }
    }

    static class HE1Helper
    {
        public static float GetFloatWithMultiSetParam(XElement elem, string valueName, float defaultValue = 1f)
        {
            var value = GetValueWithMultiSetParam(elem, valueName, defaultValue.ToString(CultureInfo.InvariantCulture));
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static int GetIntWithMultiSetParam(XElement elem, string valueName, int defaultValue = 1)
        {
            var value = GetValueWithMultiSetParam(elem, valueName, defaultValue.ToString(CultureInfo.InvariantCulture));
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        public static bool GetBoolWithMultiSetParam(XElement elem, string valueName, bool defaultValue = false)
        {
            var value = GetValueWithMultiSetParam(elem, valueName, defaultValue.ToString());
            return bool.Parse(value);
        }

        public static string GetValueWithMultiSetParam(XElement elem, string valueName, string defaultValue = "1")
        {
            if (elem.Name == "Element" && elem.Parent?.Name == "MultiSetParam")
            {
                var parentElem = elem.Parent.Parent;
                return parentElem.Element(valueName)?.Value.Trim() ?? defaultValue;
            }

            return elem.Element(valueName)?.Value.Trim() ?? defaultValue;
        }

        public static void SetFloatReflection(object obj, string name, float value)
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

        public static void SetIntReflection(object obj, string name, int value)
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

        public static void SetBoolReflection(object obj, string name, bool value)
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

        public static void SetBoxColliderSize(BoxCollider box, XElement elem, float? depth = null)
        {
            float width = GetFloatWithMultiSetParam(elem, "Collision_Width");
            float height = GetFloatWithMultiSetParam(elem, "Collision_Height");

            if (depth.HasValue)
            {
                box.size = new Vector3(width, height, depth.Value);
            }
            else
            {
                float length = GetFloatWithMultiSetParam(elem, "Collision_Length");
                box.size = new Vector3(width, height, length);
            }
        }

        public static void SetSpringProperties(object spring, XElement elem)
        {
            float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
            float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
            float keepVelocity = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
            SetFloatReflection(spring, "speed", speed);
            SetFloatReflection(spring, "outOfControl", outOfControl);
            SetFloatReflection(spring, "keepVelocityDistance", keepVelocity);
        }

        public static void SetJumpPanelProperties(object jumpPanel, XElement elem)
        {
            float impulseNormal = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnNormal");
            float impulseBoost = GetFloatWithMultiSetParam(elem, "ImpulseSpeedOnBoost");
            float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
            SetFloatReflection(jumpPanel, "impulseOnNormal", impulseNormal);
            SetFloatReflection(jumpPanel, "impulseOnBoost", impulseBoost);
            SetFloatReflection(jumpPanel, "outOfControl", outOfControl);
        }

        public static void SetDashRingProperties(object dashRing, XElement elem)
        {
            float speed = GetFloatWithMultiSetParam(elem, "FirstSpeed");
            float outOfControl = GetFloatWithMultiSetParam(elem, "OutOfControl");
            float keepDistance = GetFloatWithMultiSetParam(elem, "KeepVelocityDistance");
            SetFloatReflection(dashRing, "speed", speed);
            SetFloatReflection(dashRing, "outOfControl", outOfControl);
            SetFloatReflection(dashRing, "keepVelocityDistance", keepDistance);
        }

        public static void SetCameraDataProperties(object cameraComponent, XElement elem, bool includeDistance = false)
        {
            var dataField = cameraComponent.GetType().GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);
            if (dataField == null) return;

            var dataObj = dataField.GetValue(cameraComponent);

            float fovy = GetFloatWithMultiSetParam(elem, "Fovy");
            bool isControllable = GetBoolWithMultiSetParam(elem, "IsControllable");
            bool isCollision = GetBoolWithMultiSetParam(elem, "IsCollision");

            dataObj.GetType().GetField("fov", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, fovy);
            dataObj.GetType().GetField("allowRotation", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, isControllable);
            dataObj.GetType().GetField("isCollision", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, isCollision);

            if (includeDistance)
            {
                float distance = GetFloatWithMultiSetParam(elem, "Distance");
                dataObj.GetType().GetField("distance", BindingFlags.Instance | BindingFlags.Public)?.SetValue(dataObj, distance);
            }
        }

        public static void SetChangeMode3DProperties(object mode, XElement elem)
        {
            bool isChangeCamera = GetBoolWithMultiSetParam(elem, "m_IsChangeCamera");
            bool isEnabledFront = GetBoolWithMultiSetParam(elem, "m_IsEnableFromFront");
            bool isEnabledBack = GetBoolWithMultiSetParam(elem, "m_IsEnableFromBack");
            bool isLimitEdge = GetBoolWithMultiSetParam(elem, "m_IsLimitEdge");
            float pathCorrectionForce = GetFloatWithMultiSetParam(elem, "m_PathCorrectionForce");

            SetBoolReflection(mode, "isChangeCamera", isChangeCamera);
            SetBoolReflection(mode, "isEnabledFromFront", isEnabledFront);
            SetBoolReflection(mode, "isEnabledFromBack", isEnabledBack);
            SetBoolReflection(mode, "isLimitEdge", isLimitEdge);
            SetFloatReflection(mode, "pathCorrectionForce", pathCorrectionForce);
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
            foreach (var child in ms.Elements("Element"))
            {
                long childId = HE1ObjectsImporter.GetMultiSetObjectID(elem, i++);
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
    }
}
