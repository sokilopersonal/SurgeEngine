using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class MenusHandler : MonoBehaviour
    {
        public static MenusHandler Instance { get; private set; }
        
        [SerializeField] private Menu[] menus;
        private readonly Dictionary<Type, Menu> _menusDictionary = new Dictionary<Type, Menu>();
        
        private Menu _currentMenu;

        private void Awake()
        {
            Instance = this;
            
            foreach (var menu in menus)
            {
                _menusDictionary.Add(menu.GetType(), menu);
            }
        }

        public async void OpenMenu<T>() where T : Menu
        {
            var menu = _menusDictionary[typeof(T)];
            
            if (_currentMenu != null)
            {
                await _currentMenu.Close();
            }
            
            _currentMenu = menu;
            
            await menu.Open();
        }
        
        public async void CloseMenu<T>() where T : Menu
        {
            var menu = _menusDictionary[typeof(T)];
            _currentMenu = null;
            await menu.Close();
        }
    }
}