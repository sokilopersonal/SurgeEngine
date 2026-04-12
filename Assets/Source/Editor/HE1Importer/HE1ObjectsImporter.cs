using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects;
using SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter;
using SurgeEngine.Source.Code.Tools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace SurgeEngine.Source.Editor.HE1Importer
{
    public static class HE1ObjectsImporter
    {
        static Dictionary<string, GameObject> GetHEObjectsPrefabs()
        {
            return new Dictionary<string, GameObject>
            {
                ["Ring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Ring.prefab"),
                ["SuperRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/SuperRing.prefab"),
                ["DashPanel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/DashPanel.prefab"),
                ["Spring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Spring.prefab"),
                ["SpringFake"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/SpringFake.prefab"),
                ["WideSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/WideSpring.prefab"),
                ["AirSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/AirSpring.prefab"),
                ["ThornSpring"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/ThornSpring.prefab"),
                ["eFighter"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eFighterGun"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/EggFighter.prefab"), // TODO: Implement an actual eFighterGun
                ["eFighterMissile"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/EggFighter.prefab"), // TODO: Implement an actual eFighterMissile
                ["eFighterTutorial"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/EggFighter.prefab"),
                ["eSpinner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/Spinner.prefab"),
                ["eSpanner"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/Spinner.prefab"), // TODO: Implement an actual eSpanner
                ["eAirCannonNormal"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Enemies/AeroCannon.prefab"),
                ["ObjCameraPan"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Camera/ObjCameraPan.prefab"),
                ["ObjCameraParallel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Camera/ObjCameraParallel.prefab"),
                ["ObjCameraPoint"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Camera/ObjCameraPoint.prefab"),
                ["ObjCameraPathTarget"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Camera/ObjCameraPathTarget.prefab"),
                ["JumpBoard"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/JumpPanel_15S.prefab"),
                ["JumpBoard3D"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/JumpPanel_30M.prefab"),
                ["TrickJumper"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/TrickPanel.prefab"),
                ["UpReel"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Upreel.prefab"),
                ["JumpCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/JumpCollision.prefab"),
                ["ChangeVolumeCamera"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Camera/ChangeVolumeCamera.prefab"),
                ["StumbleCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/StumbleCollision.prefab"),
                ["DashRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/DashRing.prefab"),
                ["RainbowRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/RainbowDashRing.prefab"),
                ["SetRigidBody"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/SetRigidBody.prefab"),
                ["PointMarker"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/PointMarker.prefab"),
                ["DirectionalThorn"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/DirectionalThorn.prefab"),
                ["ChangeMode_3DtoForward"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/System/ChangeMode_3DtoForward.prefab"),
                ["ChangeMode_3DtoDash"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/System/ChangeMode_3DtoDash.prefab"),
                ["ChangeMode_3Dto2D"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/System/ChangeMode_2D.prefab"),
                ["AutorunStartCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/AutorunCollision.prefab"),
                ["AutorunFinishCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/AutorunCollision.prefab"),
                ["GoalRing"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/GoalRing/GoalRing.prefab"),
                ["Flame"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/Flame.prefab"),
                ["EventCollision"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/EventCollision.prefab"),
                ["MykonosFloor"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Apotos/MykonosFloor.prefab"),
                ["ReactionPlate"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/ReactionPlate.prefab"),
                ["JumpPole"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/JumpPole.prefab"),
                ["JumpSelector"] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Prefabs/HE1/Common/JumpSelector.prefab"),
            };
        }

        static Dictionary<string, Action<GameObject, XElement>> GetCustomHandlers()
        {
            return new Dictionary<string, Action<GameObject, XElement>>
            {
                ["Ring"] = (go, elem) =>
                {
                    var ring = go.GetComponent<Ring>();
                    HE1Helper.SetBool(ring, "isLightSpeedDashTarget", HE1Helper.GetBool(elem, "IsLightSpeedDashTarget"));
                },
                ["ChangeVolumeCamera"] = (go, elem) =>
                {
                    var volume = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(volume, elem);

                    float easeTimeEnter = HE1Helper.GetFloat(elem, "Ease_Time_Enter");
                    float easeTimeExit = HE1Helper.GetFloat(elem, "Ease_Time_Leave");
                    var camVolume = go.GetComponent<ChangeVolumeCamera>();
                    float priority = HE1Helper.GetFloat(elem, "Priority");
                    HE1Helper.SetFloat(camVolume, "easeTimeEnter", easeTimeEnter);
                    HE1Helper.SetFloat(camVolume, "easeTimeLeave", easeTimeExit);
                    camVolume.GetType().GetField("priority", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(camVolume, (int)priority);
                    var targetField = camVolume.GetType().GetField("target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var volumeConnectedId = elem.Element("Target")?.Element("SetObjectID")?.Value.Trim() ?? "0";
                    foreach (var cameraPan in Object.FindObjectsByType<ObjCameraBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        if (cameraPan.SetID == long.Parse(volumeConnectedId, CultureInfo.InvariantCulture))
                        {
                            targetField?.SetValue(camVolume, cameraPan);
                            RecordStageObject(cameraPan.gameObject);
                            break;
                        }
                    }
                },
                ["DashPanel"] = (go, elem) =>
                {
                    float speed = HE1Helper.GetFloat(elem, "Speed");
                    float speedMin = HE1Helper.GetFloat(elem, "SpeedMin");
                    float speedMax = HE1Helper.GetFloat(elem, "SpeedMax");
                    float outOfControl = HE1Helper.GetFloat(elem, "OutOfControl");
                    bool cameraLag = HE1Helper.GetBool(elem, "IsUseDelayCamera");
                    bool constantVelocity = HE1Helper.GetBool(elem, "IsConstantStartVelocity");
                    var dashPanel = go.GetComponent<DashPanel>();
                    HE1Helper.SetFloat(dashPanel, "speed", speed);
                    HE1Helper.SetFloat(dashPanel, "speedMin", speedMin);
                    HE1Helper.SetFloat(dashPanel, "speedMax", speedMax);
                    HE1Helper.SetFloat(dashPanel, "outOfControl", outOfControl);
                    HE1Helper.SetBool(dashPanel, "isUseDelayCamera", cameraLag);
                    HE1Helper.SetBool(dashPanel, "isConstantStartVelocity", constantVelocity);
                },
                ["JumpBoard"] = (go, elem) =>
                {
                    int angleType = (int)HE1Helper.GetFloat(elem, "AngleType");
                    var jumpPanel = go.GetComponent<JumpPanel>();
                    HE1Helper.SetJumpPanelProperties(jumpPanel, elem);
                    HE1Helper.SetFloat(jumpPanel, "pitch", angleType == 0 ? 15 : angleType == 1 ? 30 : angleType > 1 ? angleType : 0);
                },
                ["JumpBoard3D"] = (go, elem) =>
                {
                    var jumpPanel = go.GetComponent<JumpPanel3D>();
                    HE1Helper.SetJumpPanelProperties(jumpPanel, elem);
                },
                ["TrickJumper"] = (go, elem) =>
                {
                    var trickJumper = go.GetComponent<TrickJumper>();
                    HE1Helper.SetFloat(trickJumper, "firstSpeed", HE1Helper.GetFloat(elem, "FirstSpeed"));
                    HE1Helper.SetFloat(trickJumper, "firstPitch", HE1Helper.GetFloat(elem, "FirstPitch"));
                    HE1Helper.SetFloat(trickJumper, "firstOutOfControl", HE1Helper.GetFloat(elem, "FirstOutOfControl"));
                    HE1Helper.SetFloat(trickJumper, "secondSpeed", HE1Helper.GetFloat(elem, "SecondSpeed"));
                    HE1Helper.SetFloat(trickJumper, "secondPitch", HE1Helper.GetFloat(elem, "SecondPitch"));
                    HE1Helper.SetFloat(trickJumper, "secondOutOfControl", HE1Helper.GetFloat(elem, "SecondOutOfControl"));
                    HE1Helper.SetInt(trickJumper, "trickCount1", HE1Helper.GetInt(elem, "TrickCount1"));
                    HE1Helper.SetInt(trickJumper, "trickCount2", HE1Helper.GetInt(elem, "TrickCount2"));
                    HE1Helper.SetInt(trickJumper, "trickCount3", HE1Helper.GetInt(elem, "TrickCount3"));
                    HE1Helper.SetFloat(trickJumper, "trickTime1", HE1Helper.GetFloat(elem, "TrickTime1"));
                    HE1Helper.SetFloat(trickJumper, "trickTime2", HE1Helper.GetFloat(elem, "TrickTime2"));
                    HE1Helper.SetFloat(trickJumper, "trickTime3", HE1Helper.GetFloat(elem, "TrickTime3"));
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
                    HE1Helper.SetFloat(upReel, "length", HE1Helper.GetFloat(elem, "Length"));
                    HE1Helper.SetFloat(upReel, "outOfControl", HE1Helper.GetFloat(elem, "OutOfControl"));
                    HE1Helper.SetFloat(upReel, "upMaxSpeed", HE1Helper.GetFloat(elem, "UpSpeedMax"));
                    HE1Helper.SetFloat(upReel, "impulseVelocity", HE1Helper.GetFloat(elem, "ImpulseVelocity"));
                },
                ["JumpCollision"] = (go, elem) =>
                {
                    if (go.TryGetComponent<BoxCollider>(out var bc))
                    {
                        float w = HE1Helper.GetFloat(elem, "Collision_Width");
                        float h = HE1Helper.GetFloat(elem, "Collision_Height");
                        bc.size = new Vector3(w, h, bc.size.z);
                    }

                    var jumpCollision = go.GetComponent<JumpCollision>();
                    HE1Helper.SetFloat(jumpCollision, "speedMin", HE1Helper.GetFloat(elem, "SpeedMin") / 2);
                    HE1Helper.SetFloat(jumpCollision, "outOfControl", HE1Helper.GetFloat(elem, "OutOfControl"));
                    HE1Helper.SetFloat(jumpCollision, "impulseOnNormal", HE1Helper.GetFloat(elem, "ImpulseSpeedOnNormal"));
                    HE1Helper.SetFloat(jumpCollision, "impulseOnBoost", HE1Helper.GetFloat(elem, "ImpulseSpeedOnBoost"));
                    HE1Helper.SetFloat(jumpCollision, "pitch", HE1Helper.GetFloat(elem, "Pitch"));
                    HE1Helper.SetFloat(jumpCollision, "terrainIgnoreTime", HE1Helper.GetFloat(elem, "TerrainIgnoreTime", 0.25f));
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

                    float pitch = HE1Helper.GetFloat(elem, "Pitch");
                    float yaw = HE1Helper.GetFloat(elem, "Yaw");
                    
                    var euler = Quaternion.Euler(pitch, yaw - 180, 0);
                    comp.transform.rotation = HE1Helper.ToEulerYXZ(euler);
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
                    
                    float offsetOnEyePath = HE1Helper.GetFloat(elem, "OffsetOnEyePath");
                    HE1Helper.SetFloat(dataObj, "offsetOnEye", offsetOnEyePath);
                },
                ["StumbleCollision"] = (go, elem) =>
                {
                    var stumble = go.GetComponent<StumbleCollision>();
                    var box = go.GetComponent<BoxCollider>();
                    
                    HE1Helper.SetBoxColliderSize(box, elem);
                    box.size = new Vector3(box.size.x * 1.75f, box.size.y, box.size.z);

                    HE1Helper.SetFloat(stumble, "noControlTime", HE1Helper.GetFloat(elem, "NoControlTime"));
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
                    float width = HE1Helper.GetFloat(elem, "Width");
                    float height = HE1Helper.GetFloat(elem, "Height");
                    float length = HE1Helper.GetFloat(elem, "Length");
                    box.size = new Vector3(width, height, length);

                    bool defaultOn = bool.Parse(elem.Element("DefaultON").Value.Trim());
                    var setRb = go.GetComponent<SetRigidBody>();
                    setRb.GetType().GetField("defaultOn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(setRb, defaultOn);
                },
                ["PointMarker"] = (go, elem) =>
                {
                    float width = HE1Helper.GetFloat(elem, "Width");
                    float height = HE1Helper.GetFloat(elem, "Height");
                    
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
                    HE1Helper.SetFloat(dir, "moveTime", HE1Helper.GetFloat(elem, "MoveTime"));
                    HE1Helper.SetFloat(dir, "onTime", HE1Helper.GetFloat(elem, "OnTime"));
                    HE1Helper.SetFloat(dir, "offTime", HE1Helper.GetFloat(elem, "OffTime"));
                    HE1Helper.SetInt(dir, "phase", HE1Helper.GetInt(elem, "Phase"));
                },
                ["ChangeMode_3DtoForward"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, box.size.z);

                    var mode = go.GetComponent<ChangeMode3D>();
                    HE1Helper.SetChangeMode3DProperties(mode, elem);
                },
                ["ChangeMode_3DtoDash"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, box.size.z);

                    var mode = go.GetComponent<ChangeMode3D>();
                    HE1Helper.SetChangeMode3DProperties(mode, elem);
                },
                ["ChangeMode_3Dto2D"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem, box.size.z);

                    var mode = go.GetComponent<ChangeMode2D>();
                    HE1Helper.SetChangeMode2DProperties(mode, elem);
                },
                ["AutorunStartCollision"] = (go, elem) =>
                {
                    var autorun = go.GetComponent<AutorunCollision>();
                    
                    HE1Helper.SetBoxColliderSize(autorun.GetComponent<BoxCollider>(), elem);
                    HE1Helper.SetFloat(autorun, "speed", HE1Helper.GetFloat(elem, "Speed"));
                    HE1Helper.SetFloat(autorun, "easeTime", HE1Helper.GetFloat(elem, "EaseTime"));
                    HE1Helper.SetFloat(autorun, "keepTime", HE1Helper.GetFloat(elem, "KeepTime"));
                    HE1Helper.SetFloat(autorun, "pathEaseTime", HE1Helper.GetFloat(elem, "ToPathEaseTime"));
                    HE1Helper.SetBool(autorun, "isFinish", false);
                },
                ["AutorunFinishCollision"] = (go, elem) =>
                {
                    var autorun = go.GetComponent<AutorunCollision>();
                    
                    HE1Helper.SetBoxColliderSize(autorun.GetComponent<BoxCollider>(), elem);
                    HE1Helper.SetBool(autorun, "isFinish", true);
                },
                ["Flame"] = (go, elem) =>
                {
                    var flame = go.GetComponent<Flame>();
                    HE1Helper.SetFloat(flame, "appearTime", HE1Helper.GetFloat(elem, "AppearTime"));
                    HE1Helper.SetFloat(flame, "onTime", HE1Helper.GetFloat(elem, "OnTime"));
                    HE1Helper.SetFloat(flame, "offTime", HE1Helper.GetFloat(elem, "OffTime"));
                    HE1Helper.SetFloat(flame, "length", HE1Helper.GetFloat(elem, "Length"));
                    HE1Helper.SetInt(flame, "type", HE1Helper.GetInt(elem, "Type"));
                    HE1Helper.SetInt(flame, "phase", HE1Helper.GetInt(elem, "Phase"));

                    flame.FillMultiSet(elem, "multiSetParam");
                },
                ["EventCollision"] = (go, elem) =>
                {
                    var box = go.GetComponent<BoxCollider>();
                    HE1Helper.SetBoxColliderSize(box, elem);

                    var eventCollision = go.GetComponent<EventCollision>();
                    HE1Helper.SetInt(eventCollision, "defaultStatus", HE1Helper.GetInt(elem, "DefaultStatus"));
                    HE1Helper.SetInt(eventCollision, "durability", HE1Helper.GetInt(elem, "Durability"));

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
                    HE1Helper.SetFloat(floor, "amplitude", HE1Helper.GetFloat(elem, "Amplitude"));
                    HE1Helper.SetFloat(floor, "cycle", HE1Helper.GetFloat(elem, "Cycle"));
                    HE1Helper.SetFloat(floor, "phase", HE1Helper.GetFloat(elem, "Phase"));
                    
                    var type = floor.GetType();
                    var typeField = type.GetField("moveType", BindingFlags.Instance | BindingFlags.NonPublic);
                    var moveType = (MykonosFloorType)HE1Helper.GetInt(elem, "MoveType");
                    typeField?.SetValue(floor, moveType);
                    
                    HE1Helper.SetFloat(floor, "onGroundTime", HE1Helper.GetFloat(elem, "OnGroundTime"));
                    HE1Helper.SetFloat(floor, "resetTime", HE1Helper.GetFloat(elem, "ResetTime"));
                    HE1Helper.SetFloat(floor, "gravity", HE1Helper.GetFloat(elem, "Gravity"));
                },
                ["ThornSpring"] = (go, elem) =>
                {
                    var thornSpring = go.GetComponent<ThornSpring>();
                    HE1Helper.SetSpringProperties(thornSpring, elem);
                    HE1Helper.SetFloat(thornSpring, "upThornTime", HE1Helper.GetFloat(elem, "UpThornTime"));
                    HE1Helper.SetFloat(thornSpring, "downThornTime", HE1Helper.GetFloat(elem, "DownThornTime"));
                    HE1Helper.SetBool(thornSpring, "cancelBoost", HE1Helper.GetBool(elem, "m_IsStopBoost"));
                },
                ["ReactionPlate"] = (go, elem) =>
                {
                    var plate = go.GetComponent<ReactionPlate>();
                    
                    var type = HE1Helper.GetInt(elem, "Type");
                    switch (type)
                    {
                        case 0:
                            HE1Helper.SetInt(plate, "type", (int)ReactionPlateType.Spring);
                            break;
                        case 1 | 2 | 3 | 4 | 6:
                            HE1Helper.SetInt(plate, "type", (int)ReactionPlateType.Plate);
                            break;
                        case 5:
                            HE1Helper.SetInt(plate, "type", (int)ReactionPlateType.End);
                            break;
                    }
                    
                    if (type == 1)
                        HE1Helper.SetInt(plate, "buttonType", (int)ReactionPlateButton.B);
                    else if (type == 2)
                        HE1Helper.SetInt(plate, "buttonType", (int)ReactionPlateButton.A);
                    else if (type == 3)
                        HE1Helper.SetInt(plate, "buttonType", (int)ReactionPlateButton.X);
                    else if (type == 4)
                        HE1Helper.SetInt(plate, "buttonType", (int)ReactionPlateButton.Y);
                    else if (type == 6)
                        HE1Helper.SetInt(plate, "buttonType", (int)ReactionPlateButton.Random);
                    
                    long targetId = long.Parse(elem.Element("Target").Element("SetObjectID").Value, CultureInfo.InvariantCulture);
                    if (targetId != 0)
                    {
                        foreach (var stageObject in Object.FindObjectsByType<StageObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                        {
                            if (stageObject.SetID == targetId)
                            {
                                plate.GetType().GetField("target", BindingFlags.Instance | BindingFlags.Public)?.SetValue(plate, stageObject.transform);
                                break;
                            }
                        }
                    }
                    else
                    {
                        plate.GetType().GetField("target", BindingFlags.Instance | BindingFlags.Public)?.SetValue(plate, null);
                    }
                },
                ["eFighterTutorial"] = (go, elem) =>
                {
                    var eg = go.GetComponent<EggFighter>();
                    
                    HE1Helper.SetInt(eg, "type", (int)EggFighterType.Tutorial);
                }
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
                        var parent = TryInstantiate(prefab, elem, HE1Helper.GetObjectID(elem));
                        applyQueue.Add((name, parent, elem));
                        HE1Helper.SetObjectID(parent, HE1Helper.GetObjectID(elem));

                        int i = 0;
                        foreach (var child in ms.Elements("Element"))
                        {
                            long childId = HE1Helper.GetMultiSetObjectID(elem, i++);
                            var msGO = TryInstantiate(prefab, child, childId);
                            applyQueue.Add((name, msGO, child));
                            HE1Helper.SetObjectID(msGO, childId);
                        }
                    }
                    else
                    {
                        var parent = TryInstantiate(prefab, elem, HE1Helper.GetObjectID(elem));
                        applyQueue.Add((name, parent, elem));
                        HE1Helper.SetObjectID(parent, HE1Helper.GetObjectID(elem));
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
            
            var sceneObjects = Object.FindObjectsByType<StageObject>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(s => s.SetID != 0)
                .GroupBy(s => s.SetID)
                .ToDictionary(g => g.Key, g => g.First());
            
            foreach (var (_, go, elem) in applyQueue)
            {
                foreach (var resolvable in go.GetComponents<IHE1TargetResolvable>())
                {
                    resolvable.ResolveTarget(elem, sceneObjects);
                    PrefabUtility.RecordPrefabInstancePropertyModifications((Object)resolvable);
                }
            }

            Undo.CollapseUndoOperations(group);
            EditorSceneManager.MarkAllScenesDirty();
        }

        private static GameObject CreateObjectPhysics(XElement elem)
        {
            string type = HE1Helper.GetValue(elem, "Type", "None");
            string path = "";

            switch (type)
            {
                case "ThornCylinder2M":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/ThornSC.prefab";
                    break;
                case "ThornCylinder3M":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/Thorns/ThornSCB.prefab";
                    break;
                case "IronBox2":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/IronBox.prefab";
                    break;
                case "myk_obj_hh_potplant_mixDS":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/PotPlantMixDS.prefab";
                    break;
                case "myk_obj_hh_potplant_mixEF":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/PotPlantMixEF.prefab";
                    break;
                case "myk_obj_ky_blue_woodbox":
                    path = "Assets/Source/Prefabs/HE1/Common/ObjectPhysics/BlueWoodBox.prefab";
                    break;
            }
            
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) return null;

            var go = TryInstantiate(asset, elem, HE1Helper.GetObjectID(elem));
            HE1Helper.SetObjectID(go, HE1Helper.GetObjectID(elem));

            var ms = elem.Element("MultiSetParam");
            if (ms != null)
            {
                int i = 0;
                foreach (var child in ms.Elements("Element"))
                {
                    long childId = HE1Helper.GetMultiSetObjectID(elem, i++);
                    var msGO = TryInstantiate(asset, child, childId);
                    HE1Helper.SetObjectID(msGO, childId);
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
            
            var targetRot = HE1Helper.ToEulerYXZ(q);
            
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
            if (parent == null)
            {
                parent = new GameObject("SetData").transform;
                parent.tag = "SetData";
            }

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

        static void ApplyCustom(string name, GameObject go, XElement elem)
        {
            foreach (var importable in go.GetComponents<IHE1Importable>())
            {
                importable.ImportSetData(name, elem);
                Undo.RecordObject((Object)importable, "Import HE1 Objects");
                PrefabUtility.RecordPrefabInstancePropertyModifications((Object)importable);
            }
        }

        private static void RecordStageObject(GameObject go)
        {
            if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                foreach (var component in go.GetComponents<StageObject>())
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(component);
                    
                    component.OnImport();
                }
            }
        }
    }
}
