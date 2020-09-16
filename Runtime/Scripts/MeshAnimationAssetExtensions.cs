namespace CodeWriter.MeshAnimation
{
    using UnityEngine;

    public static class MeshAnimationAssetExtensions
    {
        private static readonly int AnimationTimeProp = Shader.PropertyToID("_AnimTime");

        public static void Play(this MeshAnimationAsset asset,
            MaterialPropertyBlock block,
            string animationName,
            float speed = 1f,
            float time = 0f)
        {
            var data = asset.animationData.Find(d => d.clip.name == animationName);

            var start = data.startFrame;
            var length = data.lengthFrames;
            speed = Mathf.Max(0.01f, speed);
            speed /= Mathf.Max(data.clip.length, 0.01f);

            block.SetVector(AnimationTimeProp, new Vector4(start, length, speed, time));
        }
    }
}