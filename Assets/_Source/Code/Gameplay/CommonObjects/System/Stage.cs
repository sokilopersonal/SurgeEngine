using System;
using System.Collections;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.System
{
    public class Stage : MonoBehaviour
    {
        public static Stage Instance { get; private set; }
        [SerializeField] private StageData data;
        [SerializeField] private StudioEventEmitter backgroundMusicEmitter;
        public StageData Data => data;
        public StudioEventEmitter BackgroundMusicEmitter => backgroundMusicEmitter;
        
        public PointMarker CurrentPointMarker { get; private set; }

        [Inject] private CharacterBase _character;

        [Inject]
        private void Initialize(Stage stage)
        {
            Instance = stage;
        }
        
        private void Awake()
        {
            if (data == null)
            {
                data = new StageData();
                return;
            }

            data.RingCount = 0;
            data.State = _character.GetStartData().startType == StartType.None ? StageState.Playing : StageState.Start;
        }

        private void Update()
        {
            if (Data.State == StageState.Playing)
            {
                Data.Time += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectTriggered += OnObjectTriggered;

            _character.StateMachine.OnStateAssign += OnActorState;
            _character.Life.OnDied += OnCharacterDied;
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectTriggered -= OnObjectTriggered;
            
            _character.Life.OnDied -= OnCharacterDied;
        }

        private void OnActorState(FState obj)
        {
            if (_character.StateMachine.PreviousState is FStateStart)
            {
                Data.State = StageState.Playing;
            }
        }

        private void OnCharacterDied(CharacterBase obj)
        {
            StartCoroutine(LoadCurrentPointMarker());
        }

        private IEnumerator LoadCurrentPointMarker()
        {
            if (CurrentPointMarker != null)
            {
                yield return new WaitForSeconds(2f);
                CurrentPointMarker.Load();
                Data.Score = 0;
                Data.RingCount = 0;
            }
            else
            {
                yield return new WaitForSeconds(1.75f);
                SceneLoader.LoadGameScene(SceneManager.GetActiveScene().name);
            }
        }

        private void OnObjectTriggered(ContactBase obj)
        {
            if (obj is PointMarker marker)
            {
                CurrentPointMarker = marker;
            }

            if (obj is GoalRing.GoalRing)
            {
                Data.State = StageState.Goal;
            }
        }
    }
    
    [Serializable]
    public class StageData
    {
        [SerializeField] private int ringCount;
        [SerializeField] private float time;
        [SerializeField] private StageState state;
        [SerializeField] private int score;
        [SerializeField] private StageResult result = new StageResult();

        [SerializeField] private float bonusOptimalTime = 120f;
        [SerializeField] private float bonusZeroTime = 300f;
        [SerializeField] private int bonusMax = 100_000;
        
        public StageResult Result => result;

        public int RingCount
        {
            get => ringCount;
            set => ringCount = value;
        }

        public float Time
        {
            get => time;
            set => time = value;
        }

        public StageState State
        {
            get => state;
            set => state = value;
        }

        public int Score
        {
            get => score;
            set => score = value;
        }

        public void AddScore(int value)
        {
            Score += value;
        }

        public int TimeBonus
        {
            get
            {
                int min = 10000;
                
                if (Time <= bonusOptimalTime) return bonusMax;
                if (Time >= bonusZeroTime) return min;
                var t = (Time - bonusOptimalTime) / (bonusZeroTime - bonusOptimalTime);
                return Mathf.RoundToInt(Mathf.Lerp(bonusMax, min, t));
            }
        }
        
        public int RingBonus => ringCount * 100;

        public int TotalScore => Score + TimeBonus + RingBonus;
    }

    [Serializable]
    public class StageResult
    {
        [SerializeField] private int requiredScore = 150000;
        public int RequiredScore => requiredScore;
        
        public GoalRank GetRank(int score)
        {
            if (score >= RequiredScore)
                return GoalRank.S;
            if (score >= RequiredScore / 2)
                return GoalRank.A;
            if (score >= RequiredScore / 3)
                return GoalRank.B;
            if (score >= RequiredScore / 5)
                return GoalRank.C;
            if (score >= RequiredScore / 6)
                return GoalRank.D;
            return GoalRank.E;
        }
    }

    public enum GoalRank
    {
        S, A, B, C, D, E
    }

    [Serializable]
    public enum StageState
    {
        Start,
        Playing,
        Goal
    }
}