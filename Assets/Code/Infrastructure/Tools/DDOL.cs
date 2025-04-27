using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Tools
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