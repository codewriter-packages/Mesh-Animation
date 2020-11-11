namespace CodeWriter.MeshAnimation
{
    using JetBrains.Annotations;
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class MeshAnimator : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private MeshRenderer meshRenderer = default;

        [Required]
        [SerializeField]
        private MeshAnimationAsset meshAnimation = default;

        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        [PublicAPI]
        public void Play(string animationName, bool loop = true, float speed = 1f, float normalizedTime = 0f)
        {
            meshAnimation.Play(_propertyBlock, animationName, loop, speed, normalizedTime);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}