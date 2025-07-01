using System;
using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public abstract class OptionUI : MonoBehaviour
    {
        protected bool IsDirty { get; set; }

        protected void Awake()
        {
            foreach (var bar in GetComponentsInChildren<OptionBar>(false))
            {
                bar.OnChanged += _ => MarkDirty();
            }
        }

        protected void Start()
        {
            MarkClean();

            Setup();
        }

        protected abstract void Setup();

        /// <summary>
        /// Saves the current data of the object implementing this method. It is important to call the base.Save() after the data has been saved to properly reset IsDirty state.
        /// </summary>
        public virtual void Save()
        {
            MarkClean();
        }

        public virtual void Revert()
        {
            
        }
        
        protected void MarkDirty()
        {
            IsDirty = true;
        }
        
        protected void MarkClean()
        {
            IsDirty = false;
        }
    }
}