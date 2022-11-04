namespace CodeWriter.MeshAnimation
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using TriInspector;

    [DrawWithTriInspector]
    [CreateAssetMenu(menuName = "Mesh Animation")]
    public class MeshAnimationAsset : ScriptableObject
    {
        [InfoBox("$" + nameof(GetValidationMessage), TriMessageType.Error, visibleIf: nameof(IsInvalid))]
        [SerializeField]
        internal GameObject skin = default;

        [Required]
        [SerializeField]
        internal Shader shader = default;

        [SerializeField]
        internal Material materialPreset = default;

        [SerializeField]
        internal bool npotBakedTexture = false;

        [SerializeField]
        internal bool linearColorSpace = false;

        [PropertySpace]
        [Required]
        [SerializeField]
        [ListDrawerSettings(AlwaysExpanded = true)]
        internal AnimationClip[] animationClips = new AnimationClip[0];

        [Required]
        [SerializeField]
        [TableList]
        internal List<ExtraMaterial> extraMaterials = new List<ExtraMaterial>();

        [ReadOnly]
        [SerializeField]
        internal Texture2D bakedTexture = default;

        [ReadOnly]
        [SerializeField]
        internal Material bakedMaterial = default;

        [TableList(HideAddButton = true, HideRemoveButton = true)]
        [ReadOnly]
        [SerializeField]
        internal List<ExtraMaterialData> extraMaterialData = new List<ExtraMaterialData>();

        [TableList(AlwaysExpanded = true, HideAddButton = true, HideRemoveButton = true)]
        [ReadOnly]
        [SerializeField]
        internal List<AnimationData> animationData = new List<AnimationData>();

        [Serializable]
        internal class ExtraMaterial
        {
            [Required]
            public string name;

            public Material preset;
        }

        [Serializable]
        internal class ExtraMaterialData
        {
            public string name;
            public Material material;
        }

        [Serializable]
        internal class AnimationData
        {
            public string name;
            public float startFrame;
            public float lengthFrames;
            public float lengthSeconds;
            public bool looping;
        }

        public bool IsInvalid => GetValidationMessage() != null;

        public string GetValidationMessage()
        {
            if (skin == null) return "Skin is required";

            if (animationClips.Length == 0) return "No animation clips";

            foreach (var clip in animationClips)
            {
                if (clip == null) return "Animation clip is null";
                if (clip.legacy) return "Legacy Animation clips not supported";
            }

            if (shader == null) return "shader is null";
            if (skin == null) return "skin is null";

            var skinnedMeshRenderer = skin.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null) return "skin.GetComponentInChildren<SkinnedMeshRenderer>() == null";

            var skinAnimator = skin.GetComponent<Animator>();
            if (skinAnimator == null) return "skin.GetComponent<Animator>() == null";
            if (skinAnimator.runtimeAnimatorController == null)
                return "skin.GetComponent<Animator>().runtimeAnimatorController == null";

            return null;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            linearColorSpace = UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear;
        }

        [DisableIf(nameof(IsInvalid))]
        [PropertySpace(10)]
        [Button(ButtonSizes.Large, Name = "Bake")]
        private void Bake()
        {
            MeshAnimationBaker.Bake(this);
        }

        [PropertySpace(5)]
        [Button(ButtonSizes.Small, Name = "Clear baked data")]
        private void Clear()
        {
            MeshAnimationBaker.Clear(this);
        }
#endif
    }
}