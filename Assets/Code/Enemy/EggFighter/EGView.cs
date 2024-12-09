using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EGView : EnemyView
    {
        protected override void ViewTick()
        {
            if (IsAbleExcludePlayer())
            {
                eCollider.excludeLayers = 1 << LayerMask.NameToLayer("Actor");
            }
            else
            {
                eCollider.excludeLayers = 0;
            }
        }
    }
}