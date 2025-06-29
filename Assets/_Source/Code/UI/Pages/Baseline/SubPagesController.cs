using System;
using UnityEngine;

namespace SurgeEngine.Code.UI.Pages.Baseline
{
    public class SubPagesController : MonoBehaviour
    {
        [SerializeField] private Page[] subPages;

        public void Push(Page page)
        {
            foreach (var subPage in subPages)
            {
                subPage.Exit();
            }
            
            page.Enter();
        }
    }
}