using UnityEngine;

namespace SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "CrawlConfig", menuName = "SurgeEngine/Configs/Sonic/Crawl", order = 0)]
    public class CrawlConfig : ScriptableObject
    {
        public float topSpeed = 10f;
        public float maxSpeed = 15f;
    }
}