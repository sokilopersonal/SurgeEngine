using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Code.CommonObjects.System
{
    public class Stage : MonoBehaviour
    {
        private static Stage _instance;
        public static Stage Instance => _instance;

        public StageData data;
        
        public PointMarker CurrentPointMarker { get; private set; }

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
            if (!ActorContext.Context.stateMachine.IsExact<FStateStart>())
            {
                data.Time += Time.deltaTime;
            }

            if (CurrentPointMarker != null)
            {
                // Testing stuff

                if (Keyboard.current.f7Key.wasPressedThisFrame)
                {
                    CurrentPointMarker.Load();
                }
            }
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectCollected += OnObjectCollected;
        }
        
        private void OnDisable()
        {
            ObjectEvents.OnObjectCollected -= OnObjectCollected;
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