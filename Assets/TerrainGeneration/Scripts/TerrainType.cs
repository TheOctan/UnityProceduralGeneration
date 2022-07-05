using System;
using JetBrains.Annotations;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts
{
    [Serializable]
    public struct TerrainType
    {
        [UsedImplicitly]
        public string name;
        public float height;
        public Color color;
    }
}