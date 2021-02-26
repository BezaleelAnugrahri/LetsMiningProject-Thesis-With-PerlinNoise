using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


namespace PerlinNoiseTutorial
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPreview mapPreview = (MapPreview)target;

            if (DrawDefaultInspector())
            {
                if (mapPreview.autoUpdate)
                {
                    mapPreview.DrawMapInEditor();
                }
            }
 
            if (GUILayout.Button("Generate Map"))
            {
                mapPreview.DrawMapInEditor();
            }
        }
    }
}
#endif