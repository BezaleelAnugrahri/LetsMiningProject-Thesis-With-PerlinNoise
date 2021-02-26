using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseTutorial
{
    public class MapPreview : MonoBehaviour
    {
        public GameObject textureRendererGameObject;
        public GameObject meshFilterAndRenderer;

        public enum DrawMode
        {
            //determined which things that want to draw
            NoiseMap,
            Mesh,
            FalloffMap
        };//drop down option in unity
        public DrawMode drawMode;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureDatas textureDatas;

        public Material terrainMaterial;

        //public float estimatedDiv = 1.5f;

        [Range(0, MeshSettings.numSupportedLODs - 1)]
        public int editorPreviewLOD;//for increasing level of detail so we can optimize the graphic of vertises in one mesh
        public bool autoUpdate;



        void Start()
        {
            textureRendererGameObject.gameObject.SetActive(false);
        }
        
        public void DrawMapInEditor()
        {
            textureDatas.ApplyToMaterial(terrainMaterial);
            textureDatas.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            HeightMap heightMap = HeightMapGenerator.GeneratingHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

            if (drawMode == DrawMode.NoiseMap)
            {
                DrawTexture(TextureGenerator.TextureFromHightMap(heightMap));
            }

            else if (drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
            }
            else if (drawMode == DrawMode.FalloffMap)
            {
                DrawTexture(TextureGenerator.TextureFromHightMap(new HeightMap(FallOffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine, meshSettings.evaluateA, meshSettings.evaluateB), 0, 1)));
            }

        }

        

        public void DrawTexture(Texture2D texture)
        {            
            textureRendererGameObject.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
            textureRendererGameObject.gameObject.GetComponent<Renderer>().transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            textureRendererGameObject.gameObject.SetActive(true);
            meshFilterAndRenderer.gameObject.SetActive(false);
        }
        

        public void DrawMesh(MeshData meshData)
        {
            meshFilterAndRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
            textureRendererGameObject.gameObject.SetActive(false);
            meshFilterAndRenderer.gameObject.SetActive(true);
        }

        void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }

        void OnTextureValuesUpdated()
        {
            textureDatas.ApplyToMaterial(terrainMaterial);
        }

        void OnValidate()
        {
            if (meshSettings != null)
            {
                meshSettings.OnValuesUpdated -= OnValuesUpdated;
                meshSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (heightMapSettings != null)
            {
                heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (textureDatas != null)
            {
                textureDatas.OnValuesUpdated -= OnTextureValuesUpdated;
                textureDatas.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }

    }

}