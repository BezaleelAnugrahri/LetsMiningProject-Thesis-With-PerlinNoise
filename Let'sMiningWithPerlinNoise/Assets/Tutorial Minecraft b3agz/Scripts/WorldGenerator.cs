using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace MinecraftTutorial
{
    public class WorldGenerator : MonoBehaviour
    {
        [Header("World Generation Values")] 
        public BiomeAttributes[] biomes;

        [Header("Lighting")]
        [Range(0f, 1f)]
        public float globalLightLevel;
        public Color day;
        public Color night;

        [Header("Performance")]
        Thread chunkUpdateThread;
        public object chunkUpdateThreadLock = new object();
        public object chunkListThreadLock = new object();

        [Header("Players")]
        public Player players;
        public bool enableAnimation;
        public ChunkCoord playersChunkCoord;
        ChunkCoord playersLastChunkCoord;

        [Header("World Generator")]
        public Material chunkMat;
        public Material chunkTransparentMat;
        //public BlockType[] blockTypesEasy;
        public Item[] blockTypes;

        [Header("Clouds Generator")]
        public CloudGenerator clouds;

        ChunkGenerator[,] chunks = new ChunkGenerator[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];
        List<ChunkCoord> activedChunk = new List<ChunkCoord>();



        List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
        public List<ChunkGenerator> chunksToUpdate = new List<ChunkGenerator>();

        public Queue<ChunkGenerator> chunksToDraw = new Queue<ChunkGenerator>();

        bool applyingModification = false;
        public int voxelModificationPerFrame = 200;

        Queue<Queue<VoxelMod>> modification = new Queue<Queue<VoxelMod>>(); //modification of basic structure of chunk
        //Queue is FIFO (First In First Out) scenario
        //but list is like Table or array so we can choose line at bla...bla....bla

        private static WorldGenerator _instance;
        public static WorldGenerator Instance
        {
            get
            {
                return _instance;
            }
        }

        public WorldData worldData;

        public string appPath;

        private void Awake()
        {
            /*  if instance value is not null and not "this", we've somehow ended up with more than one WorldGenerator component.
             *  Since another one has already been assigned, delete this one.
             */
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }

            //Else Set this to instance.
            else
            {
                _instance = this;
            }

            //send the enableAnimation value to Chunk Generator
            enableAnimation = players.settings.enableChunkLoadAnimation;

            //provide us a location to save a data where there is persistent
            appPath = Application.persistentDataPath;

        }

        private void Start()
        {
            worldData.worldName = VoxelData.worldName;
            worldData.seed = VoxelData.seed;

            worldData = SaveSystem.LoadWorld(worldData.worldName, worldData.seed);

            Debug.Log("Generating New World With Seed = " + VoxelData.seed);

            Random.InitState(VoxelData.seed);

            Shader.SetGlobalFloat("minGlobalLightLevel", VoxelData.minLightLevel);
            Shader.SetGlobalFloat("maxGlobalLightLevel", VoxelData.maxLightLevel);

            if (players.settings.enableThreading)
            {

                //thread system we need to execute it before we trying to start generate chunks
                chunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
                chunkUpdateThread.Start();

            }
            
            SetGlobalLightValue(globalLightLevel, day, night);

            players.spawnPosition = new Vector3(VoxelData.WorldCenter, VoxelData.chunkHeight + players.playerHeight, VoxelData.WorldCenter);
            
            //LoadWorld();
            
            //ApplyingModifications();

            GenerateWorld();
            playersLastChunkCoord = GetChunkCoordFromVector3(players.transform.position);
            
        }

        //just an example of day and night time
        void SetGlobalLightValue(float globalLightLevel, Color day, Color night)
        {

            Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);
            Camera.main.backgroundColor = Color.Lerp(night, day, globalLightLevel);

        }

        private void Update()
        {
            playersChunkCoord = GetChunkCoordFromVector3(players.transform.position);

            //Only update the chunk if the player has moved from the chunk they were previously on.
            if (!playersChunkCoord.Equals(playersLastChunkCoord))
            {
                CheckViewDistance();
            }
            
            if (chunksToCreate.Count > 0)
            {
                CreateChunk();
            }

            if (chunksToDraw.Count > 0)
            {

                chunksToDraw.Dequeue().CreateMesh();

            }

            if (!players.settings.enableThreading)
            {
            
                if (!applyingModification)
                {
                    ApplyingModifications();
                }


                if (chunksToUpdate.Count > 0)
                {
                    UpdatingChunk();
                }
            
            }

        }


        void LoadWorld()
        {
            //doing full world size chunk
            for (int x = (VoxelData.worldSizeInChunks / 2) - players.settings.loadDistance; x < (VoxelData.worldSizeInChunks / 2) + players.settings.loadDistance; x++)
            {
                for (int z = (VoxelData.worldSizeInChunks / 2) - players.settings.loadDistance; z < (VoxelData.worldSizeInChunks / 2) + players.settings.loadDistance; z++)
                {

                    worldData.LoadChunk(new Vector2Int(x, z));

                }
            }

        }


        void GenerateWorld()
        {
            //doing full world size chunk
            for (int x = (VoxelData.worldSizeInChunks / 2) - players.settings.viewDistance; x < (VoxelData.worldSizeInChunks / 2) + players.settings.viewDistance; x++)
            {
                for (int z = (VoxelData.worldSizeInChunks / 2) - players.settings.viewDistance; z < (VoxelData.worldSizeInChunks / 2) + players.settings.viewDistance; z++)
                {
                    ChunkCoord newChunk = new ChunkCoord(x, z);
                    chunks[x, z] = new ChunkGenerator(newChunk);
                    chunksToCreate.Add(newChunk);
                
                }
            }

            players.transform.position = players.spawnPosition;
            CheckViewDistance();

        }

        void CreateChunk()
        {

            ChunkCoord c = chunksToCreate[0];
            chunksToCreate.RemoveAt(0);
            chunks[c.x, c.z].Init();

        }

        void UpdatingChunk()
        {

            lock(chunkUpdateThreadLock){
                
                chunksToUpdate[0].UpdateChunk();

                if (!activedChunk.Contains(chunksToUpdate[0].coord))
                {
                    activedChunk.Add(chunksToUpdate[0].coord);
                }
                        
                chunksToUpdate.RemoveAt(0);

            }

        }
    

        void ThreadedUpdate()
        {

            while (true)
            {

                if (!applyingModification)
                {
                    ApplyingModifications();
                }


                if (chunksToUpdate.Count > 0)
                {
                    UpdatingChunk();
                }

            }

        }

        private void OnDisable()
        {

            if (players.settings.enableThreading)
            {
                chunkUpdateThread.Abort();
            }

        }


        void ApplyingModifications()
        {

            applyingModification = true;

            while (modification.Count > 0)
            {

                Queue<VoxelMod> queue = modification.Dequeue();

                while (queue.Count > 0)
                {

                    VoxelMod v = queue.Dequeue();

                    worldData.SetVoxel(v.position, v.id);
                    
                }

            }

            applyingModification = false;

        }


        ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

            return new ChunkCoord(x, z);
        }

        public ChunkGenerator GetChunkFromVector3(Vector3 pos)
        {
            
            int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

            return chunks[x, z];
            
        }

        void CheckViewDistance()
        {

            clouds.UpdateClouds();

            //get player's position, so we can figure out whitch chunk that player on
            ChunkCoord coord = GetChunkCoordFromVector3(players.transform.position);
            playersLastChunkCoord = playersChunkCoord;

            List<ChunkCoord> previouslyActiveChunk = new List<ChunkCoord>(activedChunk);

            activedChunk.Clear();
            
            //loop through all chunks currently within view distance of the player
            for (int x = coord.x - players.settings.viewDistance; x < coord.x + players.settings.viewDistance; x++)
            {
                for (int z = coord.z - players.settings.viewDistance; z < coord.z + players.settings.viewDistance; z++)
                {

                    ChunkCoord thisChunkCoord = new ChunkCoord(x, z);

                    //if current chunk is in the world 
                    //place chunk at the list
                    if (IsChunkInWorld(thisChunkCoord))
                    {
                        //Check if it active, if not, activated it.
                        if (chunks[x, z] == null)
                        {
                            chunks[x, z] = new ChunkGenerator(thisChunkCoord);
                            chunksToCreate.Add(thisChunkCoord);
                        }
                        else if (!chunks[x, z].IsActive)
                        {
                            chunks[x, z].IsActive = true;
                        }
                        activedChunk.Add(thisChunkCoord);
                    }

                    //Check through previously active chunks to see if the chunk is there. if it is, remove it from the list. 
                    //remove chunk from the list
                    for (int i = 0; i < previouslyActiveChunk.Count; i++)
                    {
                        if (previouslyActiveChunk[i].Equals(thisChunkCoord))
                        {
                            previouslyActiveChunk.RemoveAt(i);
                        }
                    }

                }
            }

            // Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
            foreach(ChunkCoord c in previouslyActiveChunk)
            {
                chunks[c.x, c.z].IsActive = false;
            }

        }

        public bool CheckForVoxel(Vector3 pos)
        {

            VoxelState voxel = worldData.GetVoxel(pos);

            if (blockTypes[voxel.id].isSolid)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public VoxelState GetVoxelState(Vector3 pos)
        {

            return worldData.GetVoxel(pos);

        }

        public byte GetVoxel(Vector3 pos)
        {
            int yPos = Mathf.FloorToInt(pos.y);

            /*IMMUTABLE PASS
                means anythings down here is absolute
             */

            //if voxel outside world, return Airblock
            if (!IsVoxelInWorld(pos))
            {
                return 0;//this is Air block
            }

            //if bottom block of chunk, return stone
            if (yPos == 0)
            {
                return 5;//stone
            }

            /*BIOMES SELECTION PASS*/

            int solidGroundHeight = 42;
            float sumOfHeights = 0f;
            int count = 0;//count is ammount of how many values height that we add
            float strongestWeight = 0f;
            int strongestBiomeIndex = 0;

            for (int i = 0; i < biomes.Length; i++)
            {

                float weight = NoiseGenerator.Get2DPerlin(new Vector2(pos.x, pos.z), biomes[i].offset, biomes[i].scale);

                //keep track of which weight is strongest
                if (weight > strongestWeight)
                {

                    strongestWeight = weight;
                    strongestBiomeIndex = i;

                }

                //Get the height of the terrain (for current biome) and multiply it by it's weight.
                float height = biomes[i].terrainHeight * NoiseGenerator.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biomes[i].terrainScale) * weight;

                //If height value is greater than 0 add it to the sum of heights.
                if (height > 0)
                {

                    sumOfHeights += height;
                    count++;

                }

            }

            //Set biome to the one with the strongest weight 
            BiomeAttributes biome = biomes[strongestBiomeIndex];

            //Get the average of the height 
            sumOfHeights /= count;

            int terrainHeight = Mathf.FloorToInt(sumOfHeights + solidGroundHeight);

            /* BASIC TERRAIN PASS*/

            byte voxelValue = 0;

            if (yPos == terrainHeight)
            {               
                voxelValue = biome.surfaceBlock;//grass is 3
            }
            else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            {
                voxelValue = biome.subSurfaceBlock;//rock is 1
            }
            else if(yPos > terrainHeight)
            {
                return 0;//air
            }
            else
            {
                voxelValue = 2;//dirt
            }

            /*SECOND PASS (Cave system can be put in here too)*/

            //if voxelValue is equal to dirt, then we're gonna loop throught each of our LODE
            if (voxelValue == 2)
            {

                foreach (LODE lodes in biome.lodes)
                {
                    
                    if (yPos > lodes.minHeight && yPos < lodes.maxHeight)
                    {

                        if (NoiseGenerator.Get3DPerlin(pos, lodes.noiseOffset, lodes.scale, lodes.threshold))
                        {
                            voxelValue = lodes.blockID;
                        }

                    }

                }

            }

            /*TREES PASS*/

            if (yPos == terrainHeight && biome.placeMajorFlora)
            {

                if (NoiseGenerator.Get2DPerlin(new Vector2(pos.x, pos.z), biome.majorFloraOffset, biome.majorFloraZoneScale) > biome.majorFloraZoneThreshold)
                {

                    //voxelValue = 2;//dirt

                    if (NoiseGenerator.Get2DPerlin(new Vector2(pos.x, pos.z), biome.majorFloraOffset, biome.majorFloraPlacementScale) > biome.majorFloraPlacementThreshold)
                    {

                        //voxelValue = 9;//treeTrunk
                        //now we need to tell the game system to pass out the value one block higher than this, because the tree just not one column of wood, but the leaves spread around
                        modification.Enqueue(Structure.GenerateMajorFlora(biome.majorFloraIndex, pos, biome.minHeight, biome.maxHeight));
                        
                    }

                }

            }

                return voxelValue;

        }

        
        bool IsChunkInWorld(ChunkCoord coord)
        {
            
            bool isInWorld = coord.x > 0 && coord.x < VoxelData.worldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.worldSizeInChunks - 1;

            if (isInWorld)
            {
                return true;
            }
            else
            {
                return false;
            }

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

    }


    /*[System.Serializable]
    public class BlockType
    {
        [Header("Block Description")]
        public string blockName;
        public bool isSolid;
        public bool renderNeighborFaces;
        public bool isLiquidType;
        [Range(0, 1)]
        public float transparency;
        public Sprite icon;
        [TextArea]
        public string blockDescription;
        public int maxStack;
        public int maxCondition;

        //public Material chunkMat;
        [Header("Texture Values")]
        public int frontFaceTexture;
        public int backFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;

        public int getTextureID(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0 :
                    {
                        return frontFaceTexture;
                    }
                case 1:
                    {
                        return backFaceTexture;
                    }
                case 2:
                    {
                        return topFaceTexture;
                    }
                case 3:
                    {
                        return bottomFaceTexture;
                    }
                case 4:
                    {
                        return leftFaceTexture;
                    }
                case 5:
                    {
                        return rightFaceTexture;
                    }
                default :
                    {
                        Debug.Log("Error in GetTextureID; Invalid face Index");
                        return 0;
                    }
                    
            }
        }

    }*/


    public class VoxelMod
    {
        public Vector3 position;
        public byte id;

        public VoxelMod()
        {

            position = new Vector3();
            id = 0;

        }

        public VoxelMod(Vector3 _position, byte _id)
        {

            position = _position;
            id = _id;

        }

    }

}