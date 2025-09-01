using System;
using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI.Discord
{
    public sealed class SurgeRPC : IInitializable, ITickable, ILateDisposable
    {
        private static readonly long AppID = Convert.ToInt64("1366779308995248229");

        private global::Discord.Discord _discord;
        
        public void Initialize()
        {
            _discord = new global::Discord.Discord(AppID, (ulong)CreateFlags.NoRequireDiscord);
            
            Debug.Log("[Discord RPC] Initialized");
        }

        public void Tick()
        {
            UpdateStatus();
            
            _discord.RunCallbacks();
        }

        private void UpdateStatus()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            bool isInActionLevel = currentScene.name != "MainMenu";

            var manager = _discord.GetActivityManager();
            var activity = new Activity
            {
                Assets = new ActivityAssets
                {
                    LargeImage = "base_icon_unknown",
                    LargeText = "no cool icon lol",
                },
                State = isInActionLevel ? "Playing" : "Sitting in menu.",
                Details = currentScene.name,
                Timestamps = new ActivityTimestamps
                {
                    Start = 0,
                }
            };
            manager.UpdateActivity(activity, _ =>
            {
                
            });
        }

        public void LateDispose()
        {
            _discord?.Dispose();
        }
    }
}