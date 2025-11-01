using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Tools
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