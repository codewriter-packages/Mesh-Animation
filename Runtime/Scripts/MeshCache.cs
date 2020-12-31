namespace CodeWriter.MeshAnimation {
    using System.Collections.Generic;
    using UnityEngine;

    public static class MeshCache {
        private static readonly HashSet<Mesh> CachedUv = new HashSet<Mesh>();

        public static void GenerateSecondaryUv(Mesh mesh) {
            if (CachedUv.Contains(mesh)) {
                return;
            }

            CachedUv.Add(mesh);

            var uvs = new Vector2[mesh.vertexCount];
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] = new Vector2(1f * i, 0);
            }

            mesh.SetUVs(1, uvs);
        }
    }
}