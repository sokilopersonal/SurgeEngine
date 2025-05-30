using System;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine.Code.Core.Actor.HUD
{
    public class BoostBarController : MonoBehaviour
    {
        [Header("Boost Bar")] 
        [SerializeField] private Image boostBar;
        [SerializeField] private Image boostFill;
        [SerializeField] private Image boostFill2;
        [SerializeField, Range(0, 100)] private float energy; 
        [SerializeField] private float energyDivider; 
        [SerializeField] private float boostBarYAspect;
        [SerializeField] private BoostBarSize minBoostBarSize;
        [SerializeField] private BoostBarSize maxBoostBarSize;

        [Inject] private ActorBase _actor;

        public void UpdateBoostBar()
        {
            FBoost boost = _actor.StateMachine.GetSubState<FBoost>();
            float amount = boost.BoostEnergy / boost.MaxBoostEnergy;;
            SetBoostBarFill(amount, energyDivider);
            
            float boostBarWidth = Mathf.Lerp(minBoostBarSize.width, maxBoostBarSize.width, boost.MaxBoostEnergy / 100);
            boostBar.rectTransform.sizeDelta = new Vector2(boostBarWidth, boostBarYAspect);
        }

        public void SetBoostBarFill(float energyAmount, float divider)
        {
            boostFill.materialForRendering.SetFloat("_BoostAmount", energyAmount);
            boostFill.materialForRendering.SetFloat("_SplitAmount", divider);
            boostFill2.fillAmount = Mathf.Approximately(energyAmount, 0f) ? 0 : energyAmount + 0.01f;
        }
    }

    [Serializable]
    public struct BoostBarSize
    {
        public float width;
    }
}
