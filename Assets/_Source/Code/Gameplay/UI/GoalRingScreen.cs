using System;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine._Source.Code.Gameplay.UI
{
    public class GoalRingScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeBonusText;
        [SerializeField] private TMP_Text ringBonusText;
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private Slider rankProgressBar;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private GameObject objectToSelect;
        [SerializeField] private RawImage playerRenderImage;

        [Inject] private Stage _stage;
        [Inject] private CharacterBase _character;

        private Action OnResultLook;

        private bool _isAnimatingTimeText;
        private float _timeAnimationElapsed;
        private bool _isAnimatingScoreText;
        private float _scoreAnimationElapsed;
        private bool _isAnimatingRank;
        private float _rankAnimationElapsed;
        public event Action OnFlashEnd;

        private void Start()
        {
            rankText.text = "E";
            rankProgressBar.value = 0f;
        }

        private void Update()
        {
            float duration = 0.75f;
            if (_isAnimatingTimeText)
            {
                float time = Mathf.Lerp(0, _stage.Data.Time, _timeAnimationElapsed);
                timeText.text = GetTimeInString(time);
                
                float timeBonus = Mathf.Lerp(0, _stage.Data.TimeBonus, _timeAnimationElapsed);
                timeBonusText.text = timeBonus.ToString("000000");
                
                _timeAnimationElapsed += Time.deltaTime / duration;
            }   
            
            if (_isAnimatingScoreText)
            {
                float score = Mathf.Lerp(0, _stage.Data.Score, _scoreAnimationElapsed);
                scoreText.text = score.ToString("000000");
                
                float ring = Mathf.Lerp(0, _stage.Data.RingBonus, _scoreAnimationElapsed);
                ringBonusText.text = ring.ToString("000000");
                
                _scoreAnimationElapsed += Time.deltaTime / duration;
            }
            
            if (_isAnimatingRank)
            {
                int currentScore = _stage.Data.TotalScore;
                int requiredScore = _stage.Data.Result.RequiredScore;
                float rank = Mathf.Lerp(0, currentScore / (float)requiredScore, _rankAnimationElapsed);
                rankProgressBar.value = rank;
                rankText.text = _stage.Data.Result.GetRank(Mathf.RoundToInt(rank * requiredScore)).ToString();
                
                float total = Mathf.Lerp(0, _stage.Data.TotalScore, _rankAnimationElapsed);
                totalScoreText.text = total.ToString("000000");
                
                _rankAnimationElapsed += Time.deltaTime / 0.65f;
            }
        }

        public void StartAnimatingTime()
        {
            _isAnimatingTimeText = true;
            _timeAnimationElapsed = 0f;
        }
        
        public void StartAnimatingScore()
        {
            _isAnimatingScoreText = true;
            _scoreAnimationElapsed = 0f;
        }

        public void StartAnimatingRank()
        {
            rankProgressBar.value = 0f;
            
            _isAnimatingRank = true;
            _rankAnimationElapsed = 0f;
        }

        public void OnFlashEndEvent()
        {
            OnFlashEnd?.Invoke();
        }
        
        public void Unlock()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void SetRenderTexture(RenderTexture rt)
        {
            playerRenderImage.texture = rt;
        }
        
        public void ResultLook()
        {
            _character.StateMachine.GetState<FStateGoal>().LookAnimation(_stage.Data.Result.GetRank(_stage.Data.TotalScore));
        }

        public void SelectOnEnd()
        {
            EventSystem.current.SetSelectedGameObject(objectToSelect);
        }

        public void LoadToMenu()
        {
            SceneLoader.LoadGameScene("MainMenu");
        }

        public void Restart()
        {
            SceneLoader.LoadGameScene(SceneManager.GetActiveScene().name);
        }

        private static string GetTimeInString(float time)
        {
            int milliseconds = Mathf.FloorToInt(time * 100f) % 100;
            int seconds = Mathf.FloorToInt(time % 60);
            int minutes = Mathf.FloorToInt(time / 60);
            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }
}