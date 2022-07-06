using System;
using System.Collections.Generic;
using OctanGames.TerrainGeneration.Scripts.Data;
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
        private readonly List<TerrainChunk> _lastUpdatedChunks = new();
        private MapGenerator _mapGenerator;
        private Vector2 _lastViewerPosition;

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
            ViewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);
            if ((_lastViewerPosition - ViewerPosition).sqrMagnitude >= SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE)
            {
                _lastViewerPosition = ViewerPosition;
                UpdateVisibleChunks();
                print("Update");
            }
        }

        private void UpdateVisibleChunks()
        {
            _lastUpdatedChunks.ForEach(chunk => chunk.SetVisible(false));
            _lastUpdatedChunks.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

            for (int yOffset = -CountForwardVisibleChunks; yOffset <= CountForwardVisibleChunks; yOffset++)
            {
                for (int xOffset = -CountForwardVisibleChunks; xOffset <= CountForwardVisibleChunks; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (_terrainChunks.ContainsKey(viewedChunkCoord))
                    {
                        TerrainChunk chunk = _terrainChunks[viewedChunkCoord];
                        chunk.Update();

                        if (chunk.IsVisible)
                        {
                            _lastUpdatedChunks.Add(chunk);
                        }
                    }
                    else
                    {
                        _terrainChunks.Add(viewedChunkCoord,
                            new TerrainChunk(viewedChunkCoord,
                                ChunkSize, _detailLevles, transform, _material,
                                _mapGenerator, ViewerPosition, MaxViewDistance));
                    }
                }
            }
        }
    }
}