using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "CrawlConfig", menuName = "Surge Engine/Configs/Sonic/Crawl", order = 0)]
    public class CrawlConfig : ScriptableObject
    {
        public float topSpeed = 10f;
        public float maxSpeed = 15f;
    }
}