namespace CodeWriter.MeshAnimation
{
    using JetBrains.Annotations;
    using UnityEngine;
    using TriInspector;

    [DrawWithTriInspector]
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

            MeshCache.GenerateSecondaryUv(this.meshRenderer.GetComponent<MeshFilter>().sharedMesh);
        }

        [PublicAPI]
        public void Play(string animationName, float speed = 1f, float? normalizedTime = 0f)
        {
            meshRenderer.GetPropertyBlock(_propertyBlock);
            meshAnimation.Play(_propertyBlock, animationName, speed, normalizedTime);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}