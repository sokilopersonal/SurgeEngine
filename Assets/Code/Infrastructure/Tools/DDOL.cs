using UnityEngine;

namespace SurgeEngine.Code.Tools
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