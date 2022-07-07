using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [RequireComponent(typeof(MapGenerator))]
    public class EndlessTerrain : MonoBehaviour
    {
        private const float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 25f;

        private const float SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE =
            VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;

        [SerializeField] private LODInfo[] _detailLevles;
        [SerializeField] private Material _material;
        [SerializeField] private Transform _viewer;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunks = new();
        private MapGenerator _mapGenerator;
        private Vector2 _lastViewerPosition;

        public static List<TerrainChunk> LastUpdatedChunks { get; } = new();
        public static float MaxViewDistance { get; private set; }
        public static Vector2 ViewerPosition { get; private set; }
        private static int ChunkSize => MapGenerator.MAP_CHUNK_SIZE - 1;
        private int CountForwardVisibleChunks { get; set; }

        private void Start()
        {
            _mapGenerator = GetComponent<MapGenerator>();
            MaxViewDistance = _detailLevles[^1].visibleDistanceThreshold;
            CountForwardVisibleChunks = Mathf.RoundToInt(MaxViewDistance / ChunkSize);
            UpdateVisibleChunks();
        }

        private void Update()
        {
            Vector3 position = _viewer.position;
            ViewerPosition = new Vector2(position.x, position.z);
            if ((_lastViewerPosition - ViewerPosition).sqrMagnitude >= SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE)
            {
                _lastViewerPosition = ViewerPosition;
                UpdateVisibleChunks();
            }
        }

        private void UpdateVisibleChunks()
        {
            LastUpdatedChunks.ForEach(chunk => chunk.SetVisible(false));
            LastUpdatedChunks.Clear();

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
                        _terrainChunks.Add(viewedChunkCoord,
                            new TerrainChunk(viewedChunkCoord,
                                ChunkSize, _detailLevles, transform, _material, _mapGenerator));
                    }
                }
            }
        }
    }
}