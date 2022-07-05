using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class EndlessTerrain : MonoBehaviour
    {
        private class TerrainChunk
        {
            private readonly GameObject _meshObject;
            private Bounds _bounds;
            private readonly Vector2 _position;

            public TerrainChunk(Vector2 coord, int size)
            {
                _position = coord * size;
                var position3 = new Vector3(_position.x, 0, _position.y);
                _bounds = new Bounds(_position, Vector2.one * size);

                _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _meshObject.transform.position = position3;
                _meshObject.transform.localScale = Vector3.one * size / 10f;
                SetVisible(false);
            }

            public void Update()
            {
                float viewerSqrDistanceFromNearestEdge = _bounds.SqrDistance((ViewerPosition));
                bool visible = viewerSqrDistanceFromNearestEdge <= MAX_VIEW_DISTANCE * MAX_VIEW_DISTANCE;
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }
        }

        public const float MAX_VIEW_DISTANCE = 300;

        [SerializeField] private Transform _viewer;

        private Dictionary<Vector2, TerrainChunk> _terrainChunks = new();

        public static Vector2 ViewerPosition { get; set; }
        private static int ChunkSize => MapGenerator.MAP_CHUNK_SIZE - 1;
        private int CountForwardVisibleChunks { get; set; }

        private void Start()
        {
            CountForwardVisibleChunks = Mathf.RoundToInt(MAX_VIEW_DISTANCE / ChunkSize);
        }

        private void Update()
        {
            Vector3 position = _viewer.position;
            ViewerPosition = new Vector2(position.x, position.z);
            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

            for (int yOffset = -CountForwardVisibleChunks; yOffset <= CountForwardVisibleChunks; yOffset++)
            {
                for (int xOffset = -CountForwardVisibleChunks; xOffset <= CountForwardVisibleChunks; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (_terrainChunks.ContainsKey(viewedChunkCoord))
                    {
                        _terrainChunks[viewedChunkCoord].Update();
                    }
                    else
                    {
                        _terrainChunks.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, ChunkSize));
                    }
                }
            }
        }
    }
}