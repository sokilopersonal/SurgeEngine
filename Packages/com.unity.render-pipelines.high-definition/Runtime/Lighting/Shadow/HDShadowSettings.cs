using System;

namespace UnityEngine.Rendering.HighDefinition
{
    /// <summary>
    /// Settings for shadows.
    /// </summary>
    [Serializable, VolumeComponentMenu("Shadowing/Shadows")]
    [SupportedOnRenderPipeline(typeof(HDRenderPipelineAsset))]
    [HDRPHelpURL("Override-Shadows")]
    [DisplayInfo(name = "Shadows")]
    public class HDShadowSettings : VolumeComponent, ISerializationCallbackReceiver
    {
        internal const int k_MaxCascades = 6;

        float[] m_CascadeShadowSplits = new float[k_MaxCascades - 1];
        float[] m_CascadeShadowBorders = new float[k_MaxCascades];

        /// <summary>
        /// Repartition of shadow cascade splits for directional lights.
        /// </summary>
        public float[] cascadeShadowSplits
        {
            get
            {
                m_CascadeShadowSplits[0] = cascadeShadowSplit0.value;
                m_CascadeShadowSplits[1] = cascadeShadowSplit1.value;
                m_CascadeShadowSplits[2] = cascadeShadowSplit2.value;
                if (m_CascadeShadowSplits.Length > 3) m_CascadeShadowSplits[3] = cascadeShadowSplit3.value;
                if (m_CascadeShadowSplits.Length > 4) m_CascadeShadowSplits[4] = cascadeShadowSplit4.value;
                return m_CascadeShadowSplits;
            }
        }

        internal float InterCascadeToSqRangeBorder(float interCascadeBorder, float prevCascadeRelRange, float cascadeRelRange)
        {
            // Compute the border relative to the cascade range from the inter-cascade range
            float rangeBorder = cascadeRelRange >= 0.0f
                ? (cascadeRelRange - prevCascadeRelRange) * interCascadeBorder / cascadeRelRange
                : 0.0f;

            // Range will be applied to squared distance, which makes the effective range at b = 1-sqrt(1-b'), so we need to output b'=1-(1-b)^2
            return 1.0f - (1.0f - rangeBorder) * (1.0f - rangeBorder);
        }
        internal float SqRangeBorderToInterCascade(float sqRangeBorder, float prevCascadeRelRange, float cascadeRelRange)
        {
            // Inverse of InterCascadeToSqRangeBorder
            float interCascadeRange = cascadeRelRange - prevCascadeRelRange;
            return interCascadeRange > 0.0f
                ? Mathf.Clamp01((1.0f - Mathf.Sqrt(1.0f - sqRangeBorder)) * cascadeRelRange / interCascadeRange)
                : 0.0f;
        }

        /// <summary>
        /// Size of the border between each shadow cascades for directional lights.
        /// </summary>
        public float[] cascadeShadowBorders
        {
            get
            {
                var splitCount = cascadeShadowSplitCount.value;
                m_CascadeShadowBorders[0] = InterCascadeToSqRangeBorder(cascadeShadowBorder0.value, 0.0f, splitCount > 1 ? cascadeShadowSplit0.value : 1.0f);
                m_CascadeShadowBorders[1] = InterCascadeToSqRangeBorder(cascadeShadowBorder1.value, cascadeShadowSplit0.value, splitCount > 2 ? cascadeShadowSplit1.value : 1.0f);
                m_CascadeShadowBorders[2] = InterCascadeToSqRangeBorder(cascadeShadowBorder2.value, cascadeShadowSplit1.value, splitCount > 3 ? cascadeShadowSplit2.value : 1.0f);
                m_CascadeShadowBorders[3] = InterCascadeToSqRangeBorder(cascadeShadowBorder3.value,
                    splitCount > 3 ? cascadeShadowSplit2.value : 1.0f,
                    splitCount > 4 ? cascadeShadowSplit3.value : 1.0f);
                m_CascadeShadowBorders[4] = InterCascadeToSqRangeBorder(cascadeShadowBorder4.value,
                    splitCount > 4 ? cascadeShadowSplit3.value : 1.0f,
                    splitCount > 5 ? cascadeShadowSplit4.value : 1.0f);
                m_CascadeShadowBorders[5] = InterCascadeToSqRangeBorder(cascadeShadowBorder5.value,
                    splitCount > 5 ? cascadeShadowSplit4.value : 1.0f, 1.0f);

                // For now we don't use shadow cascade borders but we still want to have the last split fading out.
                if (!HDRenderPipeline.s_UseCascadeBorders)
                {
                    m_CascadeShadowBorders[cascadeShadowSplitCount.value - 1] = 0.2f;
                }
                return m_CascadeShadowBorders;
            }
        }

        [SerializeField] bool interCascadeBorders = false;
        /// <summary>OnBeforeSerialize.</summary>
        public void OnBeforeSerialize()
        {
            // Borders newly serialized are defined against inter-cascade range (as the UI displays)
            // Previously serialized borders were sent directly as shader input where they are considered against the full squared range
            interCascadeBorders = true;
        }

        /// <summary>OnAfterDeserialize.</summary>
        public void OnAfterDeserialize()
        {
            if (!interCascadeBorders)
            {
                // Previously serialized borders were sent directly as shader input where they are considered against the full squared range
                // This converts those ranges back to inter-cascade ranges (as the UI displays) so now both UI and shader output match
                // Note that if a previously defined border would go out of the inter-cascade range, it will now be clamped to it
                var splitCount = cascadeShadowSplitCount.value;
                cascadeShadowBorder0.value = SqRangeBorderToInterCascade(cascadeShadowBorder0.value, 0.0f, splitCount > 1 ? cascadeShadowSplit0.value : 1.0f);
                cascadeShadowBorder1.value = SqRangeBorderToInterCascade(cascadeShadowBorder1.value, cascadeShadowSplit0.value, splitCount > 2 ? cascadeShadowSplit1.value : 1.0f);
                cascadeShadowBorder2.value = SqRangeBorderToInterCascade(cascadeShadowBorder2.value, cascadeShadowSplit1.value, splitCount > 3 ? cascadeShadowSplit2.value : 1.0f);
                cascadeShadowBorder3.value = SqRangeBorderToInterCascade(cascadeShadowBorder3.value,
                    splitCount > 3 ? cascadeShadowSplit2.value : 1.0f,
                    splitCount > 4 ? cascadeShadowSplit3.value : 1.0f);
                cascadeShadowBorder4.value = SqRangeBorderToInterCascade(cascadeShadowBorder4.value,
                    splitCount > 4 ? cascadeShadowSplit3.value : 1.0f,
                    splitCount > 5 ? cascadeShadowSplit4.value : 1.0f);
                cascadeShadowBorder5.value = SqRangeBorderToInterCascade(cascadeShadowBorder5.value,
                    splitCount > 5 ? cascadeShadowSplit4.value : 1.0f, 1.0f);
            }
        }

        /// <summary>Sets the maximum distance HDRP renders shadows for all Light types.</summary>
        [Tooltip("Sets the maximum distance HDRP renders shadows for all Light types.")]
        public NoInterpMinFloatParameter maxShadowDistance = new NoInterpMinFloatParameter(500.0f, 0.0f);

        /// <summary>Multiplier for thick transmission for directional lights.</summary>
        [Tooltip("Multiplier for thick transmission.")]
        public ClampedFloatParameter directionalTransmissionMultiplier = new ClampedFloatParameter(1.0f, 0.0f, 1.0f);

        /// <summary>Number of cascades HDRP uses for cascaded shadow maps.</summary>
        [Tooltip("Controls the number of cascades HDRP uses for cascaded shadow maps.")]
        public NoInterpClampedIntParameter cascadeShadowSplitCount = new NoInterpClampedIntParameter(4, 1, k_MaxCascades);
        /// <summary>Position of the first cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.</summary>
        [Tooltip("Sets the position of the first cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
        public CascadePartitionSplitParameter cascadeShadowSplit0 = new CascadePartitionSplitParameter(0.05f);
        /// <summary>Position of the second cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.</summary>
        [Tooltip("Sets the position of the second cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
        public CascadePartitionSplitParameter cascadeShadowSplit1 = new CascadePartitionSplitParameter(0.15f);
        /// <summary>Sets the position of the third cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.</summary>
        [Tooltip("Sets the position of the third cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
        public CascadePartitionSplitParameter cascadeShadowSplit2 = new CascadePartitionSplitParameter(0.3f);
        /// <summary>Position of the fourth cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.</summary>
        [Tooltip("Sets the position of the fourth cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
        public CascadePartitionSplitParameter cascadeShadowSplit3 = new CascadePartitionSplitParameter(0.5f);
        /// <summary>Position of the fifth cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.</summary>
        [Tooltip("Sets the position of the fifth cascade split as a percentage of Max Distance if the parameter is normalized or as the distance from the camera if it's not normalized.")]
        public CascadePartitionSplitParameter cascadeShadowSplit4 = new CascadePartitionSplitParameter(0.7f);
        /// <summary>Border size between the first and second cascade split.</summary>
        [Tooltip("Sets the border size between the first and second cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder0 = new CascadeEndBorderParameter(0.0f);
        /// <summary>Border size between the second and third cascade split.</summary>
        [Tooltip("Sets the border size between the second and third cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder1 = new CascadeEndBorderParameter(0.0f);
        /// <summary>Border size between the third and fourth cascade split.</summary>
        [Tooltip("Sets the border size between the third and fourth cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder2 = new CascadeEndBorderParameter(0.0f);
        /// <summary>Border size between the fourth and fifth cascade split.</summary>
        [Tooltip("Sets the border size between the fourth and fifth cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder3 = new CascadeEndBorderParameter(0.0f);
        /// <summary>Border size between the fifth and sixth cascade split.</summary>
        [Tooltip("Sets the border size between the fifth and sixth cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder4 = new CascadeEndBorderParameter(0.0f);
        /// <summary>Border size at the end of the last cascade split.</summary>
        [Tooltip("Sets the border size at the end of the last cascade split.")]
        public CascadeEndBorderParameter cascadeShadowBorder5 = new CascadeEndBorderParameter(0.0f);

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            cascadeShadowSplit0.Init(cascadeShadowSplitCount, 2, maxShadowDistance, null, cascadeShadowSplit1);
            cascadeShadowSplit1.Init(cascadeShadowSplitCount, 3, maxShadowDistance, cascadeShadowSplit0, cascadeShadowSplit2);
            cascadeShadowSplit2.Init(cascadeShadowSplitCount, 4, maxShadowDistance, cascadeShadowSplit1, cascadeShadowSplit3);
            cascadeShadowSplit3.Init(cascadeShadowSplitCount, 5, maxShadowDistance, cascadeShadowSplit2, cascadeShadowSplit4);
            cascadeShadowSplit4.Init(cascadeShadowSplitCount, 6, maxShadowDistance, cascadeShadowSplit3, null);
            cascadeShadowBorder0.Init(cascadeShadowSplitCount, 1, maxShadowDistance, null, cascadeShadowSplit0);
            cascadeShadowBorder1.Init(cascadeShadowSplitCount, 2, maxShadowDistance, cascadeShadowSplit0, cascadeShadowSplit1);
            cascadeShadowBorder2.Init(cascadeShadowSplitCount, 3, maxShadowDistance, cascadeShadowSplit1, cascadeShadowSplit2);
            cascadeShadowBorder3.Init(cascadeShadowSplitCount, 4, maxShadowDistance, cascadeShadowSplit2, cascadeShadowSplit3);
            cascadeShadowBorder4.Init(cascadeShadowSplitCount, 5, maxShadowDistance, cascadeShadowSplit3, cascadeShadowSplit4);
            cascadeShadowBorder5.Init(cascadeShadowSplitCount, 6, maxShadowDistance, cascadeShadowSplit4, null);
        }

        internal void InitNormalized(bool normalized)
        {
            cascadeShadowSplit0.normalized = normalized;
            cascadeShadowSplit1.normalized = normalized;
            cascadeShadowSplit2.normalized = normalized;
            cascadeShadowSplit3.normalized = normalized;
            cascadeShadowSplit4.normalized = normalized;
            cascadeShadowBorder0.normalized = normalized;
            cascadeShadowBorder1.normalized = normalized;
            cascadeShadowBorder2.normalized = normalized;
            cascadeShadowBorder3.normalized = normalized;
            cascadeShadowBorder4.normalized = normalized;
            cascadeShadowBorder5.normalized = normalized;
        }
    }

    /// <summary>
    /// Cascade Partition split parameter.
    /// </summary>
    [Serializable]
    public class CascadePartitionSplitParameter : VolumeParameter<float>
    {
        [NonSerialized]
        NoInterpMinFloatParameter maxDistance;
        internal bool normalized;
        [NonSerialized]
        CascadePartitionSplitParameter previous;
        [NonSerialized]
        CascadePartitionSplitParameter next;
        [NonSerialized]
        NoInterpClampedIntParameter cascadeCounts;
        int minCascadeToAppears;

        internal float min => previous?.value ?? 0f;
        internal float max => (cascadeCounts.value > minCascadeToAppears && next != null) ? next.value : 1f;

        internal float representationDistance => maxDistance.value;

        /// <summary>
        /// Size of the split.
        /// </summary>
        public override float value
        {
            get => m_Value;
            set => m_Value = Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Cascade Partition split parameter constructor.
        /// </summary>
        /// <param name="value">Initial value.</param>
        /// <param name="normalized">Partition is normalized.</param>
        /// <param name="overrideState">Initial override state.</param>
        public CascadePartitionSplitParameter(float value, bool normalized = false, bool overrideState = false)
            : base(value, overrideState)
            => this.normalized = normalized;

        internal void Init(NoInterpClampedIntParameter cascadeCounts, int minCascadeToAppears, NoInterpMinFloatParameter maxDistance, CascadePartitionSplitParameter previous, CascadePartitionSplitParameter next)
        {
            this.maxDistance = maxDistance;
            this.previous = previous;
            this.next = next;
            this.cascadeCounts = cascadeCounts;
            this.minCascadeToAppears = minCascadeToAppears;
        }
    }

    /// <summary>
    /// Cascade End Border parameter.
    /// </summary>
    [Serializable]
    public class CascadeEndBorderParameter : VolumeParameter<float>
    {
        internal bool normalized;
        [NonSerialized]
        CascadePartitionSplitParameter min;
        [NonSerialized]
        CascadePartitionSplitParameter max;
        [NonSerialized]
        NoInterpMinFloatParameter maxDistance;
        [NonSerialized]
        NoInterpClampedIntParameter cascadeCounts;
        int minCascadeToAppears;

        internal float representationDistance => (((cascadeCounts.value > minCascadeToAppears && max != null) ? max.value : 1f) - (min?.value ?? 0f)) * maxDistance.value;

        /// <summary>
        /// Size of the border.
        /// </summary>
        public override float value
        {
            get => m_Value;
            set => m_Value = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Cascade End Border parameter constructor.
        /// </summary>
        /// <param name="value">Initial value.</param>
        /// <param name="normalized">Normalized.</param>
        /// <param name="overrideState">Initial override state.</param>
        public CascadeEndBorderParameter(float value, bool normalized = false, bool overrideState = false)
            : base(value, overrideState)
            => this.normalized = normalized;

        internal void Init(NoInterpClampedIntParameter cascadeCounts, int minCascadeToAppears, NoInterpMinFloatParameter maxDistance, CascadePartitionSplitParameter min, CascadePartitionSplitParameter max)
        {
            this.maxDistance = maxDistance;
            this.min = min;
            this.max = max;
            this.cascadeCounts = cascadeCounts;
            this.minCascadeToAppears = minCascadeToAppears;
        }
    }
}
