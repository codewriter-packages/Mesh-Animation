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
            float speed = 1f,
            float normalizedTime = 0f)
        {
            var data = asset.animationData.Find(d => d.name == animationName);

            var start = data.startFrame;
            var length = data.lengthFrames;
            var s = speed / Mathf.Max(data.lengthSeconds, 0.01f);
            var time = Time.timeSinceLevelLoad + Mathf.Clamp01(normalizedTime) * data.lengthSeconds;

            block.SetFloat(AnimationLoopProp, data.looping ? 1 : 0);
            block.SetVector(AnimationTimeProp, new Vector4(start, length, s, time));
        }
    }
}