using System;
using UnityEngine;

namespace SurgeEngine.Code.UI.Pages.Baseline
{
    public class SubPagesController : MonoBehaviour
    {
        [SerializeField] private Page[] subPages;

        private Page _current;
        
        public void Push(Page page)
        {
            if (_current == page)
                return;
            
            foreach (var subPage in subPages)
            {
                subPage.Exit();
            }
            
            page.Enter();
            _current = page;
        }
    }
}