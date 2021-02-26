using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    /// <summary>
    /// representate Chunk in Data.... like World, Biomes and Voxel 
    /// </summary>
    [System.Serializable]
    public class ChunkData
    {
        
        /*note :
         *The Global position of the chunk. ie, (16, 16) Not (1, 1). we want to be able to
         *access it as a Vector2Int, but Vector2Int are not serializable so, we won't be able 
         *to save them. so we'll store them as int
         */
        int x, y;

        public Vector2Int position
        {

            get
            {

                return new Vector2Int(x, y);

            }
            set
            {

                x = value.x;
                y = value.y;

            }

        }

        public ChunkData(Vector2Int pos)
        {
            position = pos;
        }

        public ChunkData(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        [HideInInspector]//Displaying lots of data n inspector slows it down even more. so, hide this one.
        public VoxelState[, ,] map = new VoxelState[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

        public void Populate()
        {

            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    for (int z = 0; z < VoxelData.chunkWidth; z++)
                    {

                        map[x, y, z] = new VoxelState(WorldGenerator.Instance.GetVoxel(new Vector3(x + position.x , y, z + position.y)));

                    }
                }
            }

            WorldGenerator.Instance.worldData.AddToModifiedChunkList(this);

        }

    }


}