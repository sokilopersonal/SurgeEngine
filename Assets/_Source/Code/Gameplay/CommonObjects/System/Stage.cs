using System;
using System.Collections;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Gameplay.CommonObjects.System
{
    public class Stage : MonoBehaviour
    {
        public static Stage Instance { get; private set; }
        public StageData data;
        
        public PointMarker CurrentPointMarker { get; private set; }

        [Inject] private ActorBase _actor;

        [Inject]
        private void Initialize(Stage stage)
        {
            Instance = stage;
        }
        
        private void Awake()
        {
            data = new StageData
            {
                RingCount = 0,
                State = _actor.GetStartData().startType == StartType.None ? StageState.Playing : StageState.Start
            };
        }

        private void Update()
        {
            if (data.State == StageState.Playing)
            {
                data.Time += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectTriggered += OnObjectTriggered;

            _actor.StateMachine.OnStateAssign += OnActorState;
            _actor.OnDied += OnActorDied;
        }

        private void OnActorState(FState obj)
        {
            if (_actor.StateMachine.PreviousState is FStateStart)
            {
                data.State = StageState.Playing;
            }
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectTriggered -= OnObjectTriggered;
            
            _actor.OnDied -= OnActorDied;
        }

        private void OnActorDied(ActorBase obj)
        {
            StartCoroutine(LoadCurrentPointMarker());
        }

        private IEnumerator LoadCurrentPointMarker()
        {
            if (CurrentPointMarker != null)
            {
                yield return new WaitForSeconds(2f);
                CurrentPointMarker.Load();
                data.Score = 0;
                data.RingCount = 0;
            }
            else
            {
                yield return new WaitForSeconds(1.75f);
                SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void OnObjectTriggered(ContactBase obj)
        {
            if (obj is Ring)
            {
                data.RingCount += 1;
            }

            if (obj is PointMarker marker)
            {
                CurrentPointMarker = marker;
            }

            if (obj is GoalRing.GoalRing)
            {
                data.State = StageState.Goal;
            }
        }
    }
    
    [Serializable]
    public class StageData
    {
        [field: SerializeField] public int RingCount { get; set; }
        [field: SerializeField] public float Time { get; set; }
        [field: SerializeField] public StageState State { get; set; }

        public int Score { get; set; }

        public void AddScore(int value)
        {
            Score += value;
        }
    }

    [Serializable]
    public enum StageState
    {
        Start,
        Playing,
        Goal
    }
}