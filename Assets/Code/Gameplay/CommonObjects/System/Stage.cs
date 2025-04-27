using System.Collections;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Gameplay.CommonObjects.System
{
    public class Stage : MonoBehaviour
    {
        private static Stage _instance;
        public static Stage Instance => _instance;

        public StageData data;
        
        public PointMarker CurrentPointMarker { get; private set; }

        [Inject] private ActorBase _actor;

        private void Awake()
        {
            _instance = this;
            
            data = new StageData()
            {
                RingCount = 0
            };
        }

        private void Update()
        {
            if (!_actor.stateMachine.IsExact<FStateStart>())
            {
                data.Time += Time.deltaTime;
            }

            // Testing stuff
            if (Keyboard.current.f7Key.wasPressedThisFrame)
            {
                LoadCurrentPointMarker();
            }
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectCollected += OnObjectCollected;
            
            _actor.OnDied += OnActorDied;
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectCollected -= OnObjectCollected;
            
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
            }
            else
            {
                yield return new WaitForSeconds(1.75f);
                SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void OnObjectCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                data.RingCount += 1;
            }

            if (obj is PointMarker marker)
            {
                CurrentPointMarker = marker;
            }
        }
    }
    
    public class StageData
    {
        public string StageName { get; private set; }
        public int RingCount { get; set; }
        public float Time { get; set; }

        public int Score { get; set; }

        public StageData()
        {
            StageName = SceneManager.GetActiveScene().name;
        }
        
        public void AddScore(int value)
        {
            Score += value;
        }
    }
}