using System;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    public class EndlessTerrain : MonoBehaviour
    {
        public const float MAX_VIEW_DISTANCE = 300;

        [SerializeField] private Transform _viewer;

        public static Vector2 ViewerPosition;
        private static int ChunkSize => MapGenerator.MAP_CHUNK_SIZE;
        private int _chunksVisibleInViewDistance;

        private void Start()
        {
            _chunksVisibleInViewDistance = Mathf.RoundToInt(MAX_VIEW_DISTANCE / ChunkSize);
        }

        private void UpdateVisibleChunks()
        {
            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

            for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
            {
                for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                }
            }
        }
    }
}