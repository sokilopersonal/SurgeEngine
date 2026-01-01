using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.AnimationCallback;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States;
using SurgeEngine.Source.Code.Gameplay.Enemy.RagdollPhysics;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using NaughtyAttributes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
{
    public enum EggFighterType
    {
        Normal,
        Tutorial
    }
    public class EggFighter : EnemyBase, IDamageable, IPointMarkerLoader
    {
        [SerializeField] private new EGAnimation animation;
        [SerializeField] private EGEffects effects;
        [SerializeField] private EnemyRagdoll ragdollPrefab;
        public EGAnimation Animation => animation;
        
        public VisionSensor Sensor { get; private set; }
        [field: SerializeField] public AnimationEventCallback PunchAnimationCallback { get; private set; }
        [SerializeField] private EggFighterType type;

        [Header("AI")]
        [SerializeField, Tooltip("Disabling this will disable enemy vision, meaning enemy won't notice you, but still will be functional.")] private bool enableAI = true;
        [SerializeField] private float punchRadius = 2f;
        [SerializeField] private bool followPlayer = true;

        public float PunchRadius => punchRadius;
        public bool FollowPlayer => followPlayer;

        public NavMeshAgent Agent { get; private set; }
        public CharacterBase Character => _character;

        [Inject] private CharacterBase _character;
        private int _ragdollLayer = 69;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

# if UNITY_EDITOR
        const string egMatPath = "Assets/Source/Materials/HE1/Enemies/EggFighter/";
        const string egPiecePath = "Assets/Source/Prefabs/HE1/DestroyedPieces";
        private Material FetchMaterial(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        
        private Material FetchMaterial(string path, string path2)
        {
            return FetchMaterial(path+"/"+path2);
        }
        
        private DestroyedPiece FetchDestroyedPiece(string path)
        {
            return AssetDatabase.LoadAssetAtPath<DestroyedPiece>(path);
        }
        
        private DestroyedPiece FetchDestroyedPiece(string path, string path2)
        {
            return FetchDestroyedPiece(path+"/"+path2);
        }
        
        private void OnValidate()
        {
            SkinnedMeshRenderer meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            if (meshRenderer == null)
                return;

            Material[] mats = meshRenderer.sharedMaterials;

            if (mats.Length < 4)
                return;

            mats[0] = type == EggFighterType.Normal ? FetchMaterial(egMatPath, "em_eday_ef2n_body01.mat") : FetchMaterial(egMatPath, "em_eday_ef2n_body01_T.mat");
            mats[3] = type == EggFighterType.Normal ? FetchMaterial(egMatPath, "em_eday_ef2n_body02.mat") : FetchMaterial(egMatPath, "em_eday_ef2n_body02_T.mat");

            meshRenderer.sharedMaterials = mats;

            View.SetDestroyedPiece(type == EggFighterType.Normal ? FetchDestroyedPiece(egPiecePath, "EggFighterPieces.prefab") : FetchDestroyedPiece(egPiecePath, "EggFighterPieces_T.prefab"));
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            
            animation.Initialize(this);
            effects.Initialize(this);
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;

            _ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            Agent = GetComponent<NavMeshAgent>();
            Agent.updatePosition = false;
            Agent.updateRotation = true;
            
            if (!NavMesh.SamplePosition(Vector3.zero, out var hit, 1000.0f, NavMesh.AllAreas))
            {
                Agent.enabled = false;
            }
            
            Sensor = GetComponentInChildren<VisionSensor>();
            Sensor.enabled = enableAI;
            
            StateMachine.AddState(new EGStateIdle(this));
            StateMachine.AddState(new EGStateChase(this));
            StateMachine.AddState(new EGStatePunch(this));
            StateMachine.AddState(new EGStateDead(this));
            
            StateMachine.SetState<EGStateIdle>();
        }

        public void TakeDamage(Component sender)
        {
            if (!IsDead)
            {
                Vector3 force = sender.GetComponentInChildren<Rigidbody>().linearVelocity;
                
                Vector3 horizontal = Vector3.ProjectOnPlane(force, Vector3.up);
                Vector3 vertical = Vector3.Project(force, Vector3.up);
                vertical = Vector3.ClampMagnitude(vertical, 2f);
                
                Kill(horizontal + vertical);
            
                OnDied?.Invoke();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == _ragdollLayer)
            {
                TakeDamage(other.rigidbody);
            }
        }

        private void Kill(Vector3 force)
        {
            StateMachine.SetState<EGStateDead>(true).ApplyKnockback(force, ragdollPrefab);
        }

        public void Load()
        {
            Agent.Warp(_startPosition);
            transform.rotation = _startRotation;

            IsDead = false;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (StateMachine?.CurrentState != null)
            {
                float maxDistance = 40f;
                if (Camera.current != null)
                {
                    var style = new GUIStyle(EditorStyles.boldLabel);
                    style.fontSize = 24;
                    
                    float distance = Vector3.Distance(Camera.current.transform.position, transform.position);
                    if (distance <= maxDistance)
                    {
                        Handles.Label(transform.position + Vector3.up * 2f, StateMachine.CurrentState.GetType().Name, style);
                    }
                }
            }
#endif
        }
    }
}