using System;
using SurgeEngine.Code.UI.Menus;
using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public abstract class OptionUI : MonoBehaviour
    {
        private UnsavedOptionsPage unsavedChangesPage;
        
        protected bool IsDirty { get; set; }

        protected virtual void Awake()
        {
            unsavedChangesPage = GetComponentInChildren<UnsavedOptionsPage>();

            foreach (var bar in GetComponentsInChildren<OptionBar>())
            {
                bar.OnIndexChanged += _ => MarkAsDirty();
            }
        }

        private void Start()
        {
            UnmarkAsDirty();
        }

        /// <summary>
        /// Saves the current data of the object implementing this method. It is important to call the base.Save() after the data has been saved to properly reset IsDirty state.
        /// </summary>
        public virtual void Save()
        {
            UnmarkAsDirty();
        }

        public abstract void Revert();

        public virtual void Back()
        {
            if (IsDirty)
            {
                unsavedChangesPage.OpenInternal();
            }
            else
            {
                MenusHandler.Instance.OpenMenu<OptionsPage>();
            }
        }

        public void MarkAsDirty() => IsDirty = true;
        public void UnmarkAsDirty() => IsDirty = false;
    }
}