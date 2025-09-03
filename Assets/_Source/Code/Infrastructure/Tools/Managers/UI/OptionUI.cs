using SurgeEngine._Source.Code.UI.OptionBars;
using UnityEngine;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers.UI
{
    public abstract class OptionUI : MonoBehaviour
    {
        public bool IsDirty { get; set; }

        protected void Awake()
        {
            IsDirty = false;
            
            foreach (var bar in GetComponentsInChildren<OptionBar>(false))
            {
                bar.OnChanged += _ =>
                {
                    MarkDirty();
                };
            }
        }

        protected void Start()
        {
            Setup();

            MarkClean();
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

        private void MarkDirty()
        {
            IsDirty = true;
        }

        private void MarkClean()
        {
            IsDirty = false;
        }
    }
}