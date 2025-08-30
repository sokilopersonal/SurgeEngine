using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace SurgeEngine._Source.Code.Core.Actor.HUD
{
    [RequireComponent(typeof(PlayableDirector))]
    public class StageEnterHUD : MonoBehaviour
    {
        [SerializeField] private PlayableAsset standingGraph;
        [SerializeField] private PlayableAsset prepareGraph;
        
        private PlayableDirector _director;

        [Inject] private CharacterBase _character;

        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            _director.playOnAwake = false;

            var startType = _character.GetStartData().startType;
            if (startType != StartType.None)
            {
                _director.Play(startType == StartType.Standing
                    ? standingGraph
                    : prepareGraph);
            }
        }
    }
}