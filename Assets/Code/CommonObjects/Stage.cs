using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Code.CommonObjects
{
    public class Stage : MonoBehaviour
    {
        private static Stage _instance;
        
        public static Stage Instance => _instance;

        public StageData data;

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
            if (!ActorContext.Context.stateMachine.Is<FStateStart>())
            {
                data.Time += Time.deltaTime;
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