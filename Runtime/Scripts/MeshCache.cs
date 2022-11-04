namespace CodeWriter.MeshAnimation
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public static class MeshCache
    {
        private static readonly HashSet<Mesh> CachedUv = new HashSet<Mesh>();

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredEditMode)
                {
                    CachedUv.Clear();
                }
            };
        }
#endif

        public static void GenerateSecondaryUv(Mesh mesh)
        {
            if (CachedUv.Contains(mesh))
            {
                return;
            }

            CachedUv.Add(mesh);

            var uvs = new Vector2[mesh.vertexCount];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(1f * i, 0);
            }

            mesh.SetUVs(1, uvs);
        }
    }
}