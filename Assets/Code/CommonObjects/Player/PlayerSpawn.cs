using System.Collections;
using SurgeEngine.Code.Actor.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SurgeEngine.Code.CommonObjects
{
    [DefaultExecutionOrder(-9100)]
    public class PlayerSpawn : MonoBehaviour
    {
        [SerializeField] private Transform actorPrefab;
        [SerializeField] private Transform stageHud;
        [SerializeField] private StartType startType = StartType.Standing;
        [SerializeField] private float speed;
        [SerializeField] private float time;
        
        private void Awake()
        {
            
        }
    }
}