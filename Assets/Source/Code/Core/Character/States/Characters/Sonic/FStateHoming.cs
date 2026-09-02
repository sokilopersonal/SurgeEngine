using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects;
using SurgeEngine.Source.Code.Infrastructure.Config;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
	public class FStateHoming : FCharacterState, IStateTimeout
	{
		private HomingTarget _target;
		private float _timer;
		private bool _targetReached;

		private readonly HomingConfig _config;

		public FStateHoming(CharacterBase owner) : base(owner)
		{
			owner.TryGetConfig(out _config);
		}

		public override void OnEnter()
		{
			base.OnEnter();

			if (StateMachine.GetState(out FBoost boost))
				boost.Active = false;

			_timer = 0f;
			_targetReached = false;
			Timeout = _config.delay;

			PhysicsConfig config = Character.Config;
			Model.SetCollisionParam(config.jumpCollisionHeight, config.jumpCollisionCenter, config.jumpCollisionRadius);
		}

		public override void OnExit()
		{
			base.OnExit();

			Model.ResetCollisionToDefault();
			Model.Collision.isTrigger = false;
			_target = null;
		}

		public override void OnFixedTick(float dt)
		{
			base.OnFixedTick(dt);

			if (_target != null && _target.gameObject.activeInHierarchy)
			{
				Vector3 toTarget = _target.transform.position - Rigidbody.position + Rigidbody.transform.up * 0.5f;
				float distance = toTarget.magnitude;
				Vector3 direction = toTarget / distance;
				
				if (_config.speed * dt >= distance)
				{
					if (!_targetReached)
					{
						_targetReached = true;
						_target.OnTargetReached.Invoke(Character);
						return;
					}
					
					Rigidbody.MovePosition(_target.transform.position + Rigidbody.transform.up * 0.5f);
					Rigidbody.linearVelocity = Vector3.zero;
				}
				else
				{
					Rigidbody.linearVelocity = direction * _config.speed;
					Rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
				}

				_timer += dt / _config.maxTime;
				if (_timer >= 1f)
				{
					Debug.Log($"[{GetType().Name}] <color=red>Character stuck.</color> Recovering.");
					Kinematics.ResetVelocity();
					StateMachine.SetState<FStateAir>();
				}
			}
			else
			{
				if (Kinematics.CheckForGround(out var predictHit, CheckGroundType.Predict, 1f))
				{
					if (!predictHit.transform.GetComponent<BreakableObject>())
					{
						StateMachine.SetState<FStateAir>();
						Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, 10f);
						return;
					}
				}

				Vector3 direction = Vector3.Cross(Character.transform.right, Vector3.up);
				Rigidbody.linearVelocity = direction * (_config.jumpDashDistance *
					_config.JumpDashCurve.Evaluate(_timer));
				Rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);

				_timer += dt / _config.jumpDashTime;
				if (_timer >= 1f)
				{
					Debug.Log("Over time");
					StateMachine.SetState<FStateAir>();
				}
			}

			bool foundDamageable = HurtBox.CreateBoxAttached(Character, Character.transform, Vector3.zero, Vector3.one * 0.5f, HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
			if (foundDamageable)
			{
				StateMachine.SetState<FStateAfterHoming>();
			}
		}

		public void SetTarget(HomingTarget target)
		{
			_timer = 0;
			_targetReached = false;
			_target = target;

			if (_target != null)
			{
				Model.Collision.isTrigger = true;
			}
		}

		public float Timeout { get; set; }
	}
}