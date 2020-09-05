#if UNITY_EDITOR
namespace CodeWriter.MeshAnimation
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static class MeshAnimationBaker
    {
        private static readonly int AnimTextureProp = Shader.PropertyToID("_AnimTex");
        private static readonly int AnimationMulProp = Shader.PropertyToID("_AnimMul");
        private static readonly int AnimationAddProp = Shader.PropertyToID("_AnimAdd");

        public static void Clear([NotNull] MeshAnimationAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            DestroyObject(ref asset.bakedMaterial);
            DestroyObject(ref asset.bakedTexture);
            SaveAsset(asset);
        }

        public static void Bake([NotNull] MeshAnimationAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            var error = asset.GetValidationMessage();
            if (error != null)
            {
                throw new InvalidOperationException(error);
            }

            try
            {
                AssetDatabase.DisallowAutoRefresh();
                EditorUtility.DisplayProgressBar("Mesh Animator", "Baking", 0f);

                DestroyObject(ref asset.bakedTexture);
                CreateTexture(asset);
                CreateMaterial(asset);
                BakeAnimations(asset);
                SaveAsset(asset);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.AllowAutoRefresh();
            }
        }

        private static void DestroyObject<T>(ref T field) where T : Object
        {
            if (field != null)
            {
                Object.DestroyImmediate(field, true);
                field = null;
            }
        }

        private static void CreateMaterial(MeshAnimationAsset asset)
        {
            if (asset.bakedMaterial == null)
            {
                var material = new Material(asset.shader) {name = asset.name + " Material"};
                asset.bakedMaterial = material;
                AssetDatabase.AddObjectToAsset(material, asset);
            }
            else
            {
                asset.bakedMaterial.shader = asset.shader;
            }
        }

        private static void CreateTexture(MeshAnimationAsset asset)
        {
            if (asset.bakedTexture == null)
            {
                var mesh = asset.skin.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
                var vertexCount = mesh.vertexCount;
                var framesCount = asset.animationClips.Sum(clip => clip.GetFramesCount() + 1);

                var texWidth = Mathf.NextPowerOfTwo(vertexCount);
                var textHeight = Mathf.NextPowerOfTwo(framesCount);

                var texture = new Texture2D(texWidth, textHeight, TextureFormat.RGB24, false)
                {
                    name = asset.name + " Texture",
                    hideFlags = HideFlags.NotEditable,
                    wrapMode = TextureWrapMode.Repeat,
                };

                AssetDatabase.AddObjectToAsset(texture, asset);
                asset.bakedTexture = texture;
            }
        }

        private static void BakeAnimations(MeshAnimationAsset asset)
        {
            var bakeObject = Object.Instantiate(asset.skin.gameObject);
            var bakeMesh = new Mesh();
            var skin = bakeObject.GetComponentInChildren<SkinnedMeshRenderer>();

            var bakeTransform = bakeObject.transform;
            bakeTransform.localPosition = Vector3.zero;
            bakeTransform.localRotation = Quaternion.identity;
            bakeTransform.localScale = Vector3.one;

            var boundMin = Vector3.zero;
            var boundMax = Vector3.zero;

            var animator = bakeObject.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController == null)
            {
                Debug.LogError("animator != null && animator.runtimeAnimatorController == null");
                return;
            }

            asset.animationData.Clear();

            AnimationMode.StartAnimationMode();
            AnimationMode.BeginSampling();

            try
            {
                foreach (var clip in asset.animationClips)
                {
                    for (int frame = 0, framesCount = clip.GetFramesCount(); frame < framesCount; frame++)
                    {
                        EditorUtility.DisplayProgressBar("Mesh Animator", clip.name, 1f * frame / framesCount);

                        AnimationMode.SampleAnimationClip(bakeObject, clip, frame / clip.frameRate);
                        skin.BakeMesh(bakeMesh);

                        var vertices = bakeMesh.vertices;
                        foreach (var vertex in vertices)
                        {
                            boundMin = Vector3.Min(boundMin, vertex);
                            boundMax = Vector3.Max(boundMax, vertex);
                        }
                    }
                }

                int globalFrame = 0;
                foreach (var clip in asset.animationClips)
                {
                    var framesCount = clip.GetFramesCount();

                    asset.animationData.Add(new MeshAnimationAsset.AnimationData
                    {
                        clip = clip,
                        startFrame = globalFrame,
                        lengthFrames = framesCount,
                    });

                    for (int frame = 0; frame < framesCount; frame++)
                    {
                        EditorUtility.DisplayProgressBar("Mesh Animator", clip.name, 1f * frame / framesCount);

                        AnimationMode.SampleAnimationClip(bakeObject, clip, frame / clip.frameRate);
                        skin.BakeMesh(bakeMesh);

                        CaptureMeshToTexture(asset.bakedTexture, bakeMesh, boundMin, boundMax, globalFrame);
                        if (frame == 0)
                        {
                            CaptureMeshToTexture(asset.bakedTexture, bakeMesh, boundMin, boundMax,
                                globalFrame + framesCount);
                        }

                        ++globalFrame;
                    }

                    ++globalFrame;
                }

                while (globalFrame < asset.bakedTexture.height)
                {
                    CaptureMeshToTexture(asset.bakedTexture, bakeMesh, boundMin, boundMax, globalFrame);
                    ++globalFrame;
                }
            }
            finally
            {
                AnimationMode.EndSampling();
                AnimationMode.StartAnimationMode();
            }

            Object.DestroyImmediate(bakeObject);
            Object.DestroyImmediate(bakeMesh);

            asset.bakedMaterial.SetTexture(AnimTextureProp, asset.bakedTexture);
            asset.bakedMaterial.SetVector(AnimationMulProp, boundMax - boundMin);
            asset.bakedMaterial.SetVector(AnimationAddProp, boundMin);
        }

        private static void CaptureMeshToTexture(Texture2D texture, Mesh mesh, Vector3 min, Vector3 max, int line)
        {
            var vertices = mesh.vertices;
            for (var vertexId = 0; vertexId < vertices.Length; vertexId++)
            {
                var vertex = vertices[vertexId];
                var color = new Color(
                    Mathf.InverseLerp(min.x, max.x, vertex.x),
                    Mathf.InverseLerp(min.y, max.y, vertex.y),
                    Mathf.InverseLerp(min.z, max.z, vertex.z)
                );

                texture.SetPixel(vertexId, line, color);
            }
        }

        private static void SaveAsset(MeshAnimationAsset asset)
        {
            EditorUtility.SetDirty(asset);

            var assetPath = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.ImportAsset(assetPath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static int GetFramesCount(this AnimationClip clip)
        {
            return Mathf.CeilToInt(clip.length * clip.frameRate);
        }
    }
}
#endif