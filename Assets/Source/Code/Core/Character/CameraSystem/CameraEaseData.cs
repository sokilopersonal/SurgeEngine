using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public struct CameraEaseData
    {
        public float EnterTime;
        public float LeaveTime;
        
        public CameraEaseData(float enterTime, float leaveTime) => (EnterTime, LeaveTime) = (enterTime, leaveTime);

        public static CameraEaseData FromVolume(ChangeVolumeCamera volume) => new()
        {
            EnterTime = volume.EaseTimeEnter,
            LeaveTime = volume.EaseTimeLeave,
        };

        public static CameraEaseData FromPan(PanData pan) => new()
        {
            EnterTime = pan.easeTimeEnter,
            LeaveTime = pan.easeTimeLeave,
        };
    }
}