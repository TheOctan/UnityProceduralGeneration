using System.Collections.Generic;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Scripts.Preset
{
    [CreateAssetMenu(fileName = "NewColorTerrainPreset", menuName = "Terrain/ColorPreset", order = 0)]
    public class TerrainPreset : ScriptableObject
    {
        public IEnumerable<TerrainType> Regions => _regions;

        [SerializeField] private TerrainType[] _regions;
    }
}