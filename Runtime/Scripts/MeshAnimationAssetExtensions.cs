namespace CodeWriter.MeshAnimation
{
    using UnityEngine;

    public static class MeshAnimationAssetExtensions
    {
        private static readonly int AnimationTimeProp = Shader.PropertyToID("_AnimTime");
        private static readonly int AnimationLoopProp = Shader.PropertyToID("_AnimLoop");

        public static void Play(this MeshAnimationAsset asset,
            MaterialPropertyBlock block,
            string animationName,
            bool loop = true,
            float speed = 1f,
            float normalizedTime = 0f)
        {
            var data = asset.animationData.Find(d => d.clip.name == animationName);

            var start = data.startFrame;
            var lengthFrames = data.lengthFrames;
            var lengthSeconds = data.clip.length;
            var s = speed / Mathf.Max(lengthSeconds, 0.01f);
            var time = Time.timeSinceLevelLoad + Mathf.Clamp01(normalizedTime) * lengthSeconds;

            block.SetFloat(AnimationLoopProp, loop ? 1 : 0);
            block.SetVector(AnimationTimeProp, new Vector4(start, lengthFrames, s, time));
        }
    }
}