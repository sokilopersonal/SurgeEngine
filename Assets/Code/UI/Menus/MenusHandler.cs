using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SurgeEngine.Code.UI.Menus;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class MenusHandler : MonoBehaviour
    {
        public static MenusHandler Instance { get; private set; }
        
        [SerializeField] private Page[] menus;
        private readonly Dictionary<Type, Page> _menusDictionary = new Dictionary<Type, Page>();
        private readonly Dictionary<string, Page> _menusDictionaryString = new Dictionary<string, Page>();
        
        private Page _currentPage;

        private void Awake()
        {
            Instance = this;
            
            foreach (var menu in menus)
            {
                _menusDictionary.Add(menu.GetType(), menu);
                _menusDictionaryString.Add(menu.GetType().Name, menu);
            }
        }

        public async void OpenMenu<T>(bool closePrevious = true) where T : Page
        {
            var menu = _menusDictionary[typeof(T)];

            if (closePrevious)
            {
                foreach (var page in _menusDictionary.Values)
                {
                    page.Group.alpha = 0;
                    page.Group.interactable = false;
                    page.Group.blocksRaycasts = false;
                }
            }
            
            if (_currentPage != null)
            {
                await _currentPage.Close();
            }
            
            _currentPage = menu;
            
            await menu.Open();
        }
        
        public async void CloseMenu<T>() where T : Page
        {
            var menu = _menusDictionary[typeof(T)];
            _currentPage = null;
            await menu.Close();
        }

        /// <summary>
        /// Opens a page by name. Will close previous page and wait for it.
        /// </summary>
        /// <param name="page">Page name (type name)</param>
        public async void OpenMenu(string page)
        {
            var menu = _menusDictionaryString[page];
            
            if (_currentPage != null)
            {
                await _currentPage.Close();
            }
            
            _currentPage = menu;
            await menu.Open();
        }
        
        /// <summary>
        /// Opens a page. Will close previous page and wait for it.
        /// </summary>
        /// <param name="page">Page reference to open</param>
        public async void OpenMenu(Page page)
        {
            if (_currentPage != null)
            {
                await _currentPage.Close();
            }
            
            _currentPage = page;
            await page.Open();
        }

        /// <summary>
        /// Opens a page. Will close previous page and wait for waitTime before opening the new page
        /// </summary>
        /// <param name="page">Page reference to open</param>
        /// <param name="waitTime">Time to wait before opening</param>
        public async void OpenMenu(Page page, float waitTime)
        {
            if (_currentPage != null)
            {
                await _currentPage.Close(waitTime);
            }
            
            _currentPage = page;
            await page.Open(waitTime);
        }

        public bool IsAnyMenuOpened()
        {
            return _currentPage != null;
        }
        
        public bool IsOpened<T>() where T : Page
        {
            if (_currentPage)
            {
                return _currentPage.GetType() == typeof(T);
            }
            
            return false;
        }
    }
}