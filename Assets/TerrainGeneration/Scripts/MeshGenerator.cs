using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(
            float[,] heightMap,
            float heightMultiplier,
            AnimationCurve heightCurve,
            int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) * -0.5f;
            float topLeftZ = (height - 1) * 0.5f;

            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int vertexPerLine = (width - 1) / meshSimplificationIncrement + 1;

            var meshData = new MeshData(vertexPerLine, vertexPerLine);

            var vertexIndex = 0;
            for (var y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < width; x += meshSimplificationIncrement)
                {
                    meshData.Vertices[vertexIndex] =
                        new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,
                            topLeftZ - y);
                    meshData.Uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(
                            vertexIndex,
                            vertexIndex + vertexPerLine + 1,
                            vertexIndex + vertexPerLine);
                        meshData.AddTriangle(
                            vertexIndex + vertexPerLine + 1,
                            vertexIndex,
                            vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }
}