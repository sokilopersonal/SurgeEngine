using System;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine._Source.Code.Core.Character.HUD
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

        [Inject] private CharacterBase _character;

        public void UpdateBoostBar()
        {
            FBoost boost = _character.StateMachine.GetSubState<FBoost>();
            float amount = boost.BoostEnergy / boost.MaxBoostEnergy;;
            SetBoostBarFill(amount, energyDivider);
            
            float boostBarWidth = Mathf.Lerp(minBoostBarSize.width, maxBoostBarSize.width, boost.MaxBoostEnergy / 100);
            boostBar.rectTransform.sizeDelta = new Vector2(boostBarWidth, boostBar.rectTransform.sizeDelta.y);
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
