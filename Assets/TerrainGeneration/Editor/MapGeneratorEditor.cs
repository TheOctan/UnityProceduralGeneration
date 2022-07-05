using OctanGames.TerrainGeneration.Scripts;
using UnityEditor;
using UnityEngine;

namespace OctanGames.TerrainGeneration.Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        private bool _autoGenerate = true;
        private MapGenerator _mapGenerator;

        private void OnEnable()
        {
            _mapGenerator = target as MapGenerator;
        }

        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector() && _autoGenerate)
            {
                _mapGenerator.GenerateMap();
            }

            _autoGenerate = EditorGUILayout.Toggle("Auto Generate", _autoGenerate);

            if (GUILayout.Button("Generate"))
            {
                _mapGenerator.GenerateMap();
            }
        }
    }
}
