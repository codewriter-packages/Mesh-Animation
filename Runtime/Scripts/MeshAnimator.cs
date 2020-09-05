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
        public void Play(string animationName, float speed = 1f)
        {
            meshAnimation.Play(_propertyBlock, animationName, speed);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}