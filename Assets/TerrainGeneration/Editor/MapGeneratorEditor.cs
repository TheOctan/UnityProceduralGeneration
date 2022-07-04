using System;
using Octan.TerrainGeneration.Scripts;
using UnityEditor;
using UnityEngine;

namespace Octan.TerrainGeneration.Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _autoGenerate;
        private MapGenerator _mapGenerator;

        private void OnEnable()
        {
            _mapGenerator = target as MapGenerator;
            _autoGenerate = serializedObject.FindProperty(nameof(_autoGenerate));
        }

        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector() && _autoGenerate.boolValue)
            {
                _mapGenerator.GenerateMap();
            }

            if (GUILayout.Button("Generate"))
            {
                _mapGenerator.GenerateMap();
            }
        }
    }
}
