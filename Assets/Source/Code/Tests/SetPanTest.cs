using Alchemy.Inspector;
using JetBrains.Annotations;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Tests
{
    public class SetPanTest : MonoBehaviour
    {
        [SerializeField] private ObjCameraBase target;

        [Inject] private CharacterBase _character;

        [Button, UsedImplicitly]
        public void Set()
        {
            target.SetPan(_character);
        }
    }
}