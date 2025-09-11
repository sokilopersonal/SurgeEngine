using System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Model
{
    public class CopyTransform : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Awake()
        {
            transform.SetParent(target, false);
        }
    }
}
