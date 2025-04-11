using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.Interfaces
{
    public interface IPlayerContactable
    {
        void OnContact(Collider msg);
    }
}