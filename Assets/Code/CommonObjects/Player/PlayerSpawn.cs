using System.Collections;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
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
            var instance = Instantiate(actorPrefab, transform.position, transform.rotation);
            var hud = Instantiate(stageHud);
            Actor actor = instance.GetComponentInChildren<Actor>();

            StartData data = new StartData()
            {
                startType = startType,
                speed = speed,
                time = time
            };
            
            switch (startType)
            {
                case StartType.None:
                    break;
                case StartType.Standing:
                    //actor.animation.TransitionToState("StartS", 0f);
                    break;
                case StartType.Prepare:
                    //actor.animation.TransitionToState("StartP", 0f);
                    break;
                case StartType.Dash:
                    break;
            }
            
            actor.SetStart(data);
            Destroy(gameObject);
        }
    }
}