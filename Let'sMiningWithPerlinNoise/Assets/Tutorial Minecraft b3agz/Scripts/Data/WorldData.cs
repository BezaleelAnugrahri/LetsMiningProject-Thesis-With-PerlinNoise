using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftTutorial
{
    /// <summary>
    /// representate World in Data.... like World, Biomes and Voxel 
    /// </summary>
    [System.Serializable]
    public class WorldData
    {
        [Header("Variables")]
        public string worldName;
        public int seed;

        [Header("Dictionary, List, and Queue")]
        [System.NonSerialized]
        public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();
        [System.NonSerialized]
        public List<ChunkData> modifiedChunks = new List<ChunkData>();

        public void AddToModifiedChunkList(ChunkData chunk)
        {

            if (!modifiedChunks.Contains(chunk))
            {
                modifiedChunks.Add(chunk);
            }

        }

        public WorldData(string _worldName, int _seed)
        {

            worldName = _worldName;
            seed = _seed;

        }

        public WorldData(WorldData wD)
        {

            worldName = wD.worldName;
            seed = wD.seed;

        }

        //function that requesting a chunk from dictionary
        public ChunkData RequestChunk(Vector2Int coord, bool create)
        {

            ChunkData c;

            lock (WorldGenerator.Instance.chunkListThreadLock)
            {

                if (chunks.ContainsKey(coord))
                {

                    c = chunks[coord];

                }
                else if (!create)
                {
                    c = null;
                }
                else
                {
                    LoadChunk(coord);
                    c = chunks[coord];
                }

            }

            return c;

        }

        public void LoadChunk(Vector2Int coord)
        {

            if (chunks.ContainsKey(coord))
            {
                return;
            }

            //Load chunk from file
            ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
            if (chunk != null)
            {

                chunks.Add(coord, chunk);
                return;

            }


            chunks.Add(coord, new ChunkData(coord));
            chunks[coord].Populate();

        }

        bool IsVoxelInWorld(Vector3 pos)
        {
            bool isVoxelTrue = pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels;

            if (isVoxelTrue)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void SetVoxel(Vector3 pos, byte value)
        {
            //If the voxel is outside of the world, we don't need to do anything with it 
            if (!IsVoxelInWorld(pos))
            {
                return;
            }

            //find out the ChunkCoord value of our voxel's chunk.
            int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

            //Then reverse that to get the position of the chunk.
            x *= VoxelData.chunkWidth;
            z *= VoxelData.chunkWidth;

            //check if the chunk exists. If not, create it 
            ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

            //then create a Vector3Int with the position of our voxel "within" the chunk.
            Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

            //then set our voxel in our chunk 
            chunk.map[voxel.x, voxel.y, voxel.z].id = value;

            AddToModifiedChunkList(chunk);

        }

        public VoxelState GetVoxel(Vector3 pos)
        {

            //If the voxel is outside of the world, we don't need to do anything with it 
            if (!IsVoxelInWorld(pos))
            {
                return null;
            }

            //find out the ChunkCoord value of our voxel's chunk.
            int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

            //Then reverse that to get the position of the chunk.
            x *= VoxelData.chunkWidth;
            z *= VoxelData.chunkWidth;

            //check if the chunk exists. If not, create it 
            ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

            //then create a Vector3Int with the position of our voxel "within" the chunk.
            Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

            //then set our voxel in our chunk 
            return chunk.map[voxel.x, voxel.y, voxel.z];

        }

    }


}