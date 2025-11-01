using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces
{
    public interface IPlayerContactable
    {
        void OnContact(Collider msg);
    }
}