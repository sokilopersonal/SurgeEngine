using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    [ExecuteInEditMode, RequireComponent(typeof(RawImage))]
    public class AnimatedTile : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private Vector2 size = Vector3.one * 5;
        [SerializeField] private Vector2 offset;
        [SerializeField] private bool useUnscaledTime = true;

        private void Start()
        {
            image = GetComponent<RawImage>();
            
            if (image != null)
            {
                image.uvRect = new Rect(0, 0, 10, 10);
            }
        }

        private void Update()
        {
            if (image != null)
            {
                float t = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                
                float aspectRatio = Screen.width / Screen.height;
                
                image.uvRect = new Rect(
                    image.uvRect.x + offset.x * t,
                    image.uvRect.y + offset.y * t,
                    size.x * aspectRatio,
                    size.y * aspectRatio);
            }
        }
    }
}