using System.Xml.Linq;
using FMODUnity;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public enum MykonosFloorType
    {
        Stationary,
        Unknown1,
        PingPong,
        Falling
    }
    
    public class MykonosFloor : StageObject, IPointMarkerLoader, IHE1Importable
    {
        [SerializeField] private float amplitude = 5f;
        [SerializeField] private float cycle = 0.5f;
        [SerializeField] private float phase;
        [SerializeField] private float gravity = 35f;
        [SerializeField] private MykonosFloorType moveType = MykonosFloorType.PingPong;
        [SerializeField] private float onFloorTime = 2f;
        [SerializeField] private float resetTime = 5f;
        [SerializeField] private bool pingPongDebug = true;
        [SerializeField] private Transform model;
        [SerializeField] private EventReference fallStepSound;
        
        private Vector3 _startPos;
        
        private MovingObject _movingObject;
        private Vector3 _velocity;
        private bool _bodyTriggered;
        private bool _triggeredFall;
        private float _fallTimer;
        private float _resetTimer;

        private void Awake()
        {
            _startPos = transform.position;
            _movingObject = GetComponent<MovingObject>();
        }

        private void OnEnable()
        {
            _movingObject.OnBodyAdded += OnBodyAdded;
        }

        private void OnDisable()
        {
            _movingObject.OnBodyAdded -= OnBodyAdded;
        }

        private void FixedUpdate()
        {
            if (moveType == MykonosFloorType.PingPong)
            {
                PingPong();
            }
            else if (moveType == MykonosFloorType.Falling) // Falling platform 
            {
                Fall();
            }
        }

        private void PingPong()
        {
            float offset = Mathf.Sin((Time.time * cycle + phase) * Mathf.PI * 2f) * amplitude;
            transform.position = _startPos + transform.right * offset;
        }

        private void Fall()
        {
            if (_bodyTriggered)
            {
                _fallTimer += Time.deltaTime;

                if (_fallTimer > onFloorTime)
                {
                    if (!_triggeredFall)
                    {
                        _triggeredFall = true;
                        RuntimeManager.PlayOneShot(fallStepSound, transform.position);
                    }
                }
                else
                {
                    const float shakeSpeed = 25f;
                    float y = Mathf.Sin(_fallTimer * shakeSpeed * Mathf.PI) * 0.05f;
                    model.transform.position = _startPos + Vector3.up * y;
                }
            }
            
            if (_triggeredFall)
            {
                _velocity -= Vector3.up * (gravity * Time.deltaTime);
                transform.position += _velocity * Time.deltaTime;
                _resetTimer += Time.deltaTime;
                
                if (_resetTimer > resetTime)
                {
                    Reset();
                }
            }
        }

        private void Reset()
        {
            _triggeredFall = false;
            _bodyTriggered = false;
            _velocity = Vector3.zero;
            _fallTimer = 0;
            _resetTimer = 0;
            transform.position = _startPos;
        }

        private void OnBodyAdded(Rigidbody obj)
        {
            _bodyTriggered = true;
        }

        private void OnDrawGizmos()
        {
            if (moveType == MykonosFloorType.PingPong && pingPongDebug)
            {
                Vector3 startPos = Application.isPlaying ? _startPos : transform.position;
                Vector3 phasePos = startPos + transform.right * Mathf.Sin((Time.time * cycle + phase) * Mathf.PI * 2f) * amplitude;
                Vector3 minPos = startPos - transform.right * amplitude;
                Vector3 maxPos = startPos + transform.right * amplitude;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(minPos, 0.5f);
                Gizmos.DrawWireSphere(maxPos, 0.5f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(minPos, maxPos);
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(startPos, phasePos);
                Gizmos.DrawWireSphere(phasePos, 0.25f);
            }
        }

        public void Load()
        {
            Reset();
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            amplitude = HE1Helper.GetFloat(elem, "Amplitude");
            cycle = HE1Helper.GetFloat(elem, "Cycle");
            phase = HE1Helper.GetFloat(elem, "Phase");
            gravity = HE1Helper.GetFloat(elem, "Gravity");
            moveType = (MykonosFloorType)HE1Helper.GetInt(elem, "MoveType");
            onFloorTime = HE1Helper.GetFloat(elem, "OnFloorTime");
            resetTime = HE1Helper.GetFloat(elem, "ResetTime");
        }
    }
}