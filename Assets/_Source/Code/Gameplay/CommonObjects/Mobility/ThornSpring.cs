using UnityEngine;
using DG.Tweening;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class ThornSpring : Spring, IPointMarkerLoader
    {
        [SerializeField] private float downThornTime = 1;
        [SerializeField] private float upThornTime = 1;
        [SerializeField] private Transform model;

        private float _thornTimer;
        private bool _springUp;
        private Tween _rotationTween;

        private void Awake()
        {
            _springUp = false;
        }

        private void OnValidate()
        {
            if (downThornTime < 0.5f || upThornTime < 0.5f)
            {
                Debug.LogWarning("Down/Up Thorn Time must be greater than or equal 0.5.");
            }
            
            downThornTime = Mathf.Max(0.5f, downThornTime);
            upThornTime = Mathf.Max(0.5f, upThornTime);
        }

        private void Update()
        {
            if (_springUp)
            {
                _thornTimer += Time.deltaTime;
                if (_thornTimer >= downThornTime)
                {
                    _springUp = false;
                    _thornTimer = 0;
                    RotateSpring();
                }
            }
            else
            {
                _thornTimer += Time.deltaTime;
                if (_thornTimer >= upThornTime)
                {
                    _springUp = true;
                    _thornTimer = 0;
                    RotateSpring();
                }
            }
        }

        protected override void Launch(CharacterBase context)
        {
            Vector3 dir = (context.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(model.transform.up, dir);
            if (dot > 0.2f && !context.Life.IsDead)
            {
                base.Launch(context);
            }
            else
            {
                context.transform.position = transform.position;
                context.Life.TakeDamage(this);
            }
        }

        private void RotateSpring()
        {
            _rotationTween?.Kill();
            _rotationTween = model.DORotate(new Vector3(0, 0, model.eulerAngles.z + 180f), 0.5f)
                .SetEase(Ease.InOutElastic).SetLink(gameObject);
        }

        public void Load()
        {
            _springUp = false;
            _thornTimer = 0;
            _rotationTween?.Kill(true);
        }
    }
}