using UnityEngine;

namespace SurgeEngine._Source.Code.Infrastructure.Tools
{
    /// <summary>
    /// Simple auto DontDestroyOnLoad
    /// </summary>
    public class DDOL : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}