using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseTutorial
{
    public class TerrainGenerator : MonoBehaviour
    {   
        
        //we need some threshold distance before viewer have to move, so we dont need to updating terrain LOD everytime
        const float viewerMoveThresholdForChunkUpdate = 25f;
        const float sqrviewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;//(Mathf.Pow(viewerMoveThresholdForChunkUpdate, 2))
        //const mean value can't change while game runs
        public int colliderLODIndex;
        public LODInfo[] detailLevels;
        float maxViewDst;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureDatas textureSettings;

        public Transform viewer;
        public Material mapMaterial;

        Vector2 viewerPosition;
        Vector2 viewerPositionOld;
        float meshWorldSize;
        int chunkVisibleInViewDst;

        Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

        public GameObject chunkParentDatas;//for parent of many chunk

        void OnTextureValuesUpdated()
        {
            textureSettings.ApplyToMaterial(mapMaterial);
        }

        void Start()
        {
            OnTextureValuesUpdated();
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        
            maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            meshWorldSize = meshSettings.meshWorldSize;
            chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);//how many times it can divide the chunk size into the view distance
            //241 - 1 = 240 (240 * 240 because of the mapChunkSize 240squere)

            UpdateVisibleChunks();//make sure at start viewerPosition didnt get wrong
        }

        void Update()
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            if (viewerPosition != viewerPositionOld)
            {
                foreach (TerrainChunk chunk in visibleTerrainChunks)
                {
                    chunk.UpdateCollisionMesh();
                }
            }

            if ((viewerPositionOld-viewerPosition).sqrMagnitude > sqrviewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                UpdateVisibleChunks();
            }
        }

        void UpdateVisibleChunks()
        {
            HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
            for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coordinate);
                //visibleTerrainChunks[i].SetVisible(false);
                visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

            for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                            /*if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                            {
                                terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                            }*/
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, chunkParentDatas.transform, viewer, mapMaterial);
                            
                            terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanges;
                            newChunk.Load();
                        }
                    }

                }
            }
        }

        void OnTerrainChunkVisibilityChanges(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible)
            {
                visibleTerrainChunks.Add(chunk);
            }
            else
            {
                visibleTerrainChunks.Remove(chunk);
            }
        }

        
    }


    [System.Serializable]//showing it in inspector 
    public struct LODInfo
    {
        [Range(0, MeshSettings.numSupportedLODs - 1)]
        public int lod;
        public float visibleDstThreshold;

        public float sqrVisibleDstThreshold
        {
            get
            {
                return visibleDstThreshold * visibleDstThreshold;
            }
        }
    }
}