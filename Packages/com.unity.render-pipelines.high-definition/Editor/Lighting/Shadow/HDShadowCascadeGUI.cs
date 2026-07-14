using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

namespace UnityEditor.Rendering.HighDefinition
{
    // Local copy of UnityEditor.Rendering.ShadowCascadeGUI that supports up to 6 cascades.
    // The core package version hard-codes a 4-color palette (kCascadeColors[4]) and throws
    // IndexOutOfRangeException when called with 5 or 6 cascades.
    internal static class HDShadowCascadeGUI
    {
        private const string kPathToHorizontalGradientTexture = "Packages/com.unity.render-pipelines.core/Editor/Lighting/Icons/HorizontalGradient.png";
        private const string kPathToUpSnatchTexture = "Packages/com.unity.render-pipelines.core/Editor/Lighting/Icons/UpSnatch.png";
        private const string kPathToUpSnatchFocusedTexture = "Packages/com.unity.render-pipelines.core/Editor/Lighting/Icons/UpSnatchFocused.png";
        private const string kPathToDownSnatchTexture = "Packages/com.unity.render-pipelines.core/Editor/Lighting/Icons/DownSnatch.png";
        private const string kPathTDownSnatchFocusedTexture = "Packages/com.unity.render-pipelines.core/Editor/Lighting/Icons/DownSnatchFocused.png";

        private const float kSliderbarMargin = 2.0f;
        private const float kSliderbarHeight = 28.0f;
        private const float kLODSliderRangeModifier = 0.78824f;

        // 6 colors, one per cascade. Keep first 4 identical to core for visual consistency.
        // Mirrors the cascade coloring in Debug.hlsl.
        private static readonly Color[] kCascadeColors =
        {
            new Color(0.5f, 0.5f, 0.7f, 1.0f),
            new Color(0.5f, 0.7f, 0.5f, 1.0f),
            new Color(0.7f, 0.7f, 0.5f, 1.0f),
            new Color(0.7f, 0.5f, 0.5f, 1.0f),
            new Color(0.5f, 0.7f, 0.7f, 1.0f),
            new Color(0.7f, 0.5f, 0.7f, 1.0f),
        };

        private static readonly Color kDisabledColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

        private static Vector2 s_DragLastMousePosition;
        private static readonly int s_CascadeSliderId = "s_CascadeSliderId".GetHashCode();

        private static GUIStyle s_HorizontalGradient;
        private static GUIStyle s_UpSnatch;
        private static GUIStyle s_DownSnatch;
        private static readonly GUIStyle s_CascadeSliderBG = "LODSliderRange";
        private static readonly GUIStyle s_TextCenteredStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        public static void DrawCascades(ref ShadowCascadeGUI.Cascade[] cascades, bool useMetric, float baseMetric)
        {
            if (cascades == null || cascades.Length == 0)
            {
                Debug.LogError("No cascades passed.");
                return;
            }

            float cascadeSizeSum = 0;
            for (int i = 0; i < cascades.Length; ++i)
                cascadeSizeSum += cascades[i].size;

            if (Mathf.Abs(cascadeSizeSum - 1f) > 0.01f)
            {
                Debug.LogError($"Cascade total sum of size must be 1.0 (Currently it is {cascadeSizeSum}).");
                for (int i = 0; i < cascades.Length; ++i)
                {
                    if (cascadeSizeSum > 0)
                        cascades[i].size /= cascadeSizeSum;
                    else
                        cascades[i].size = (1f / cascades.Length);
                }
            }

            for (int i = 0; i < cascades.Length; ++i)
                cascades[i].borderSize = Mathf.Clamp01(cascades[i].borderSize);

            EditorGUILayout.BeginVertical();
            GUILayout.Space(13f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15f);

            var sliderRect = GUILayoutUtility.GetRect(
                GUIContent.none,
                s_CascadeSliderBG,
                GUILayout.Height(kSliderbarMargin + kSliderbarHeight + kSliderbarMargin),
                GUILayout.ExpandWidth(true));
            DrawBackgroundBoxGUI(sliderRect, Color.gray);

            var formatSymbol = useMetric ? 'm' : '%';
            var usableRect = new Rect(sliderRect.x + kSliderbarMargin, sliderRect.y + kSliderbarMargin,
                sliderRect.width - kSliderbarMargin * 2, sliderRect.height - kSliderbarMargin * 2);
            var partitionWidth = 2.0f / EditorGUIUtility.pixelsPerPoint;
            var partitionHalfWidth = partitionWidth * 0.5f;

            float widthForCascades = usableRect.width;
            float[] cascadeWidths = new float[cascades.Length];
            float sumOfCascadeWidthsWithoutLast = 0;
            float startX = 0;
            for (int i = 0; i < cascades.Length - 1; ++i)
            {
                float endX = startX + cascades[i].size * widthForCascades;
                float pixelPerfectStartX = Mathf.Round(startX * EditorGUIUtility.pixelsPerPoint) / EditorGUIUtility.pixelsPerPoint;
                float pixelPerfectEndX = Mathf.Round(endX * EditorGUIUtility.pixelsPerPoint) / EditorGUIUtility.pixelsPerPoint;
                float pixelPerfectCascadeWidth = pixelPerfectEndX - pixelPerfectStartX;
                cascadeWidths[i] = pixelPerfectCascadeWidth;
                sumOfCascadeWidthsWithoutLast += cascadeWidths[i];
                startX = endX;
            }
            cascadeWidths[cascades.Length - 1] = widthForCascades - sumOfCascadeWidthsWithoutLast;

            float currentX = usableRect.x;
            for (int i = 0; i < cascades.Length; ++i)
            {
                ref var cascade = ref cascades[i];
                var cascadeWidth = cascadeWidths[i];
                bool isLastCascade = (i == cascades.Length - 1);

                float borderValue;
                float cascadeValue;
                float borderWidth;
                float cascadeWithoutBorderWidth;
                if (cascade.borderHandleState != ShadowCascadeGUI.HandleState.Hidden)
                {
                    borderValue = cascade.size * cascade.borderSize;
                    cascadeValue = cascade.size - borderValue;
                    cascadeWithoutBorderWidth = Mathf.Round(cascadeWidth * (1 - cascade.borderSize) * EditorGUIUtility.pixelsPerPoint) / EditorGUIUtility.pixelsPerPoint;
                    borderWidth = cascadeWidth - cascadeWithoutBorderWidth;
                }
                else
                {
                    borderValue = 0;
                    cascadeValue = cascade.size;
                    borderWidth = 0;
                    cascadeWithoutBorderWidth = cascadeWidth;
                }

                var cascadeRect = new Rect(currentX, usableRect.y, cascadeWithoutBorderWidth, usableRect.height);
                currentX += DrawBoxGUI(cascadeRect, kCascadeColors[i]);

                float cascadeValueForText = useMetric ? cascadeValue * baseMetric : cascadeValue * 100;
                string cascadeText = $"{i}\n{cascadeValueForText:F1}{formatSymbol}";
                DrawLabelGUI(cascadeRect, cascadeText, Color.black);

                if (cascade.borderHandleState != ShadowCascadeGUI.HandleState.Hidden)
                {
                    if (isLastCascade && cascade.borderSize == 0.0)
                        borderWidth = 0;

                    var borderPartitionHandleRect = new Rect(
                        currentX - 6 - partitionHalfWidth,
                        usableRect.y + usableRect.height - 1,
                        12,
                        18);
                    var enabled = cascade.borderHandleState == ShadowCascadeGUI.HandleState.Enabled;
                    var borderPartitionColor = enabled ? kCascadeColors[i] : kDisabledColor;
                    var delta = DrawSnatchWithHandle(borderPartitionHandleRect, cascadeWidth, borderPartitionColor, GetUpSnatchStyle(), enabled);
                    cascade.borderSize = Mathf.Clamp01(cascade.borderSize - delta);

                    DrawBoxGUI(new Rect(currentX - partitionWidth, usableRect.y, partitionWidth, usableRect.height), Color.black);

                    var borderRect = new Rect(currentX, usableRect.y, borderWidth, usableRect.height);
                    var gradientLeftColor = kCascadeColors[i];
                    var gradientRightColor = isLastCascade ? Color.black : kCascadeColors[i + 1];
                    currentX += DrawGradientBoxGUI(borderRect, gradientLeftColor, gradientRightColor);

                    float borderValueForText = useMetric ? borderValue * baseMetric : borderValue * 100;
                    string borderText;
                    if (isLastCascade)
                    {
                        string fallbackText = (borderWidth < 57) ? "F." : "Fallback";
                        borderText = $"{i}\u2192{fallbackText}\n{borderValueForText:F1}{formatSymbol}";
                    }
                    else
                    {
                        borderText = $"{i}\u2192{i + 1}\n{borderValueForText:F1}{formatSymbol}";
                    }
                    DrawLabelGUI(borderRect, borderText, Color.black);
                }

                if (!isLastCascade)
                {
                    if (cascade.cascadeHandleState != ShadowCascadeGUI.HandleState.Hidden)
                    {
                        var cascadeHandleRect = new Rect(
                            currentX - 6 - partitionHalfWidth,
                            usableRect.y - 19 + 1,
                            12,
                            18);
                        var enabled = cascade.cascadeHandleState == ShadowCascadeGUI.HandleState.Enabled;
                        var cascadePartitionColor = enabled ? kCascadeColors[i + 1] : kDisabledColor;
                        var delta = DrawSnatchWithHandle(cascadeHandleRect, usableRect.width, cascadePartitionColor, GetDownSnatchStyle(), enabled);

                        if (delta != 0)
                        {
                            ref var nextCascade = ref cascades[i + 1];
                            var sliderMinimum = 0f;
                            var sliderMaximum = cascade.size + nextCascade.size;
                            var sliderPosition = cascade.size + delta;
                            var cascadeMinimumSize = 0.001f;
                            var sliderPositionPixelPerfectClamped = Mathf.Clamp(sliderPosition,
                                sliderMinimum + cascadeMinimumSize, sliderMaximum - cascadeMinimumSize);
                            cascade.size = sliderPositionPixelPerfectClamped;
                            nextCascade.size = sliderMaximum - sliderPositionPixelPerfectClamped;
                        }
                    }

                    DrawBoxGUI(new Rect(currentX - partitionWidth, usableRect.y, partitionWidth, usableRect.height), Color.black);
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15f);
            EditorGUILayout.EndVertical();
        }

        private static float DrawBackgroundBoxGUI(Rect rect, Color color)
        {
            var cachedColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, GUIContent.none);
            GUI.backgroundColor = cachedColor;
            return rect.width;
        }

        private static float DrawGradientBoxGUI(Rect rect, Color leftColor, Color rightColor)
        {
            if (s_HorizontalGradient == null)
            {
                var horizontalGradientTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(kPathToHorizontalGradientTexture);
                Debug.Assert(horizontalGradientTexture != null, $"Missing texture {kPathToHorizontalGradientTexture}");
                s_HorizontalGradient = new GUIStyle { normal = { background = horizontalGradientTexture } };
            }

            var cachedColor = GUI.backgroundColor;
            GUI.backgroundColor = rightColor;
            GUI.Box(rect, GUIContent.none, s_CascadeSliderBG);
            GUI.backgroundColor = RGBMultiplied(kLODSliderRangeModifier, leftColor);
            GUI.Box(rect, GUIContent.none, s_HorizontalGradient);
            GUI.backgroundColor = cachedColor;
            return rect.width;
        }

        private static float DrawBoxGUI(Rect rect, Color color)
        {
            var cachedColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, GUIContent.none, s_CascadeSliderBG);
            GUI.backgroundColor = cachedColor;
            return rect.width;
        }

        private static float DrawLabelGUI(Rect rect, string text, Color color)
        {
            var cachedColor = GUI.backgroundColor;
            var oldColor = GUI.color;
            GUI.color = color;
            GUI.Label(rect, text, s_TextCenteredStyle);
            GUI.backgroundColor = cachedColor;
            GUI.color = oldColor;
            return rect.width;
        }

        private static float DrawSnatchWithHandle(Rect rect, float distance, Color color, GUIStyle snatch, bool enabled = true)
        {
            int sliderControlId = GUIUtility.GetControlID(s_CascadeSliderId, FocusType.Keyboard, rect);
            Event currentEvent = Event.current;
            EventType eventType = currentEvent.GetTypeForControl(sliderControlId);

            if (eventType == EventType.Repaint)
            {
                bool isFocused = GUIUtility.keyboardControl == sliderControlId && enabled;
                bool isHovered = rect.Contains(currentEvent.mousePosition) && enabled;
                var cachedColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.white;
                if (isFocused)
                    snatch.Draw(rect, false, false, false, isFocused);
                GUI.backgroundColor = color * (isFocused || isHovered ? 1.4f : 1.0f);
                snatch.Draw(rect, false, false, false, false);
                GUI.backgroundColor = cachedColor;
            }

            float delta = 0;
            if (enabled)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal, sliderControlId);
                switch (eventType)
                {
                    case EventType.KeyDown:
                        if (GUIUtility.keyboardControl != sliderControlId) break;
                        if (currentEvent.keyCode == KeyCode.RightArrow) { delta = 0.01f; GUI.changed = true; currentEvent.Use(); }
                        else if (currentEvent.keyCode == KeyCode.LeftArrow) { delta = -0.01f; GUI.changed = true; currentEvent.Use(); }
                        break;
                    case EventType.MouseDown:
                        if (!rect.Contains(currentEvent.mousePosition)) break;
                        GUIUtility.hotControl = sliderControlId;
                        GUIUtility.keyboardControl = sliderControlId;
                        s_DragLastMousePosition = currentEvent.mousePosition;
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == sliderControlId)
                        {
                            GUIUtility.hotControl = 0;
                            currentEvent.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl != sliderControlId) break;
                        delta = (currentEvent.mousePosition - s_DragLastMousePosition).x / distance;
                        GUI.changed = true;
                        s_DragLastMousePosition = currentEvent.mousePosition;
                        currentEvent.Use();
                        break;
                }
            }
            return delta;
        }

        private static GUIStyle GetDownSnatchStyle()
        {
            if (s_DownSnatch == null)
            {
                var downSnatch = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(kPathToDownSnatchTexture);
                var downSnatchFocused = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(kPathTDownSnatchFocusedTexture);
                Debug.Assert(downSnatch != null, $"Missing texture {kPathToDownSnatchTexture}");
                Debug.Assert(downSnatchFocused != null, $"Missing texture {kPathTDownSnatchFocusedTexture}");
                s_DownSnatch = new GUIStyle();
                s_DownSnatch.normal.background = downSnatch;
                s_DownSnatch.hover.background = downSnatch;
                s_DownSnatch.focused.background = downSnatchFocused;
            }
            return s_DownSnatch;
        }

        private static GUIStyle GetUpSnatchStyle()
        {
            if (s_UpSnatch == null)
            {
                var upSnatch = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(kPathToUpSnatchTexture);
                var upSnatchFocused = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(kPathToUpSnatchFocusedTexture);
                Debug.Assert(upSnatch != null, $"Missing texture {kPathToUpSnatchTexture}");
                Debug.Assert(upSnatchFocused != null, $"Missing texture {kPathToUpSnatchFocusedTexture}");
                s_UpSnatch = new GUIStyle();
                s_UpSnatch.normal.background = upSnatch;
                s_UpSnatch.hover.background = upSnatch;
                s_UpSnatch.focused.background = upSnatchFocused;
            }
            return s_UpSnatch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Color RGBMultiplied(float multiplier, Color color)
        {
            return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
        }
    }
}
