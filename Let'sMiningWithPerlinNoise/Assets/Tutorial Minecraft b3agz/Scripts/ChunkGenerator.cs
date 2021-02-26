using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftTutorial
{
    public class ChunkGenerator
    {
        //using byte for minimum usage memory data 
        //keeping track from current triangle from VoxelData
        public ChunkCoord coord;
        ChunkData chunkData;

        public Player player;
        
        GameObject chunkObject;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<int> transparentTriangles = new List<int>();
        Material[] materials = new Material[2];
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();

        public Vector3 position;

        private bool _isActive;

        public ChunkGenerator(ChunkCoord _coord)
        {

            coord = _coord;

        }

        public void Init()
        {

            chunkObject = new GameObject();
            meshFilter = chunkObject.AddComponent<MeshFilter>();
            meshRenderer = chunkObject.AddComponent<MeshRenderer>();

            materials[0] = WorldGenerator.Instance.chunkMat;
            materials[1] = WorldGenerator.Instance.chunkTransparentMat;
            meshRenderer.materials = materials;
            
            chunkObject.transform.SetParent(WorldGenerator.Instance.transform);
            chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0f, coord.z * VoxelData.chunkWidth);
            chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

            //we can access the position data for this chunk rather than going to chunkObject.tranform.position everytime we need it
            //in Initialize we setting Vector3 position into chunk object position
            position = chunkObject.transform.position;

            chunkData = WorldGenerator.Instance.worldData.RequestChunk(new Vector2Int((int)position.x, (int)position.z), true);

            lock (WorldGenerator.Instance.chunkUpdateThreadLock)
            {
                WorldGenerator.Instance.chunksToUpdate.Add(this);
            }

            ChunkAnimation();

        }


        void ChunkAnimation()
        {
            if (WorldGenerator.Instance.enableAnimation)
            {
                //animation of generating WorldGenerator.Instance
                chunkObject.AddComponent<ChunkLoadAnimation>();
            }
        }


        public void UpdateChunk()
        {

            ClearMeshData();
            CalculateLight();

            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int y = 0; y < VoxelData.chunkHeight; y++)
                {
                    for (int z = 0; z < VoxelData.chunkWidth; z++)
                    {
                        
                        if (WorldGenerator.Instance.blockTypes[chunkData.map[x,y,z].id].isSolid)
                        {
                            UpdateMeshData(new Vector3(x, y, z));
                        }
                        
                    }
                }
            }

            //we dont know which chunk (0,1 or 0,2) will be run at this line of code first so we need to lock it 

            WorldGenerator.Instance.chunksToDraw.Enqueue(this);

            //what's this done : when we get into this line of code it'll check if it's lock, if it's lock it'll wait 
            //if it's not lock it'll lock it and get into this line of code, then unlock it 


        }

        void CalculateLight()
        {
            Queue<Vector3Int> litVoxels = new Queue<Vector3Int>();

            for (int x = 0; x < VoxelData.chunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    float lightRay = 1f;

                    for (int y = VoxelData.chunkHeight - 1 ; y >= 0; y--)
                    {

                        VoxelState thisVoxel = chunkData.map[x, y, z];

                        if (thisVoxel.id > 0 && WorldGenerator.Instance.blockTypes[thisVoxel.id].transparency < lightRay)
                        {
                            lightRay = WorldGenerator.Instance.blockTypes[thisVoxel.id].transparency;
                        }

                        thisVoxel.globalLightPercent = lightRay;

                        chunkData.map[x, y, z] = thisVoxel;

                        if(lightRay > VoxelData.lightFallOff)
                        {
                            litVoxels.Enqueue(new Vector3Int(x, y, z));
                        }

                    }                                   
                }
            }

            while (litVoxels.Count > 0)
            {

                Vector3Int v = litVoxels.Dequeue();

                for (int p = 0; p < 6; p++)
                {

                    Vector3 currentVoxel = v + VoxelData.FaceCheck[p];
                    Vector3Int neighbor = new Vector3Int((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z);

                    if (IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.z))
                    {

                        if (chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent < chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFallOff)
                        {

                            chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent = chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFallOff;

                            if (chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent > VoxelData.lightFallOff)
                            {

                                litVoxels.Enqueue(neighbor);

                            }

                        }

                    }

                }

            }

        }

        void ClearMeshData()
        {
            
            vertexIndex = 0;
            vertices.Clear();
            triangles.Clear();
            transparentTriangles.Clear();
            uvs.Clear();
            colors.Clear();
            normals.Clear();

        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                
                _isActive = value;
                if (chunkObject != null)
                {
                    chunkObject.SetActive(value);
                }

            }
        }

        bool IsVoxelInChunk(int x, int y, int z)
        {

            bool checkingVoxel = x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkHeight - 1 || z < 0 || z > VoxelData.chunkWidth - 1;
            if (checkingVoxel)//because index is from 0 to 6, not 1 to 6
            { 
                return false; 
            }
            else
            { 
                return true; 
            }

        }

        public void EditVoxel(Vector3 pos, byte newID) 
        {

            int xCheck = Mathf.FloorToInt(pos.x);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z);

            xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
            zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

            chunkData.map[xCheck, yCheck, zCheck].id = newID;
            WorldGenerator.Instance.worldData.AddToModifiedChunkList(chunkData);

            lock (WorldGenerator.Instance.chunkUpdateThreadLock)
            {

                WorldGenerator.Instance.chunksToUpdate.Insert(0, this);
                //update surrounding chunks
                UpdateSurroundingVoxel(xCheck, yCheck, zCheck); 
            
            }
            
        }

        void UpdateSurroundingVoxel(int x, int y, int z)
        {
            Vector3 thisVoxel = new Vector3(x, y, z);

            for (int p = 0; p < 6; p++)
            {

                Vector3 currentVoxel = thisVoxel + VoxelData.FaceCheck[p];

                if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
                {

                    WorldGenerator.Instance.chunksToUpdate.Insert(0, WorldGenerator.Instance.GetChunkFromVector3(currentVoxel + position));

                }

            }

        }

        //this function is for undraw the face of the cube if it doesn't at the open air(true or false)
        VoxelState CheckVoxel(Vector3 pos)
        {

            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            if (!IsVoxelInChunk(x,y,z))//because index is from 0 to 6, not 1 to 6
            {
                //return WorldGenerator.Instance.CheckForVoxel(pos + position);
                return WorldGenerator.Instance.GetVoxelState(pos + position);
            }
            
            return chunkData.map[x, y, z];
            
        }

        public VoxelState GetVoxelFromGlobalVector3(Vector3 pos)
        {

            int xCheck = Mathf.FloorToInt(pos.x);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z);

            xCheck -= Mathf.FloorToInt(position.x);
            zCheck -= Mathf.FloorToInt(position.z);

            return chunkData.map[xCheck, yCheck, zCheck];

        }

        void UpdateMeshData(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            byte blockID = chunkData.map[x, y, z].id;
            //bool isTransparent = WorldGenerator.Instance.blockTypes[blockID].renderNeighborFaces;

            for (int p = 0; p < 6; p++)
            {

                VoxelState neigbor = CheckVoxel(pos + VoxelData.FaceCheck[p]);

                if (neigbor != null && WorldGenerator.Instance.blockTypes[neigbor.id].renderNeighborFaces)// delete ! for checking if it's transparent
                {
                    
                    //looping this and add some data (because we need 6 vertices each triangle that makes square, but we just have 4 verts)

                    for (int h = 0; h < 4; h++)
                    {

                        vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTriangle[p, h]]);

                    }

                    for (int j = 0; j < 4; j++)//because 1 box has 4 side
                    {
                        normals.Add(VoxelData.FaceCheck[p]);
                    }

                    AddTexture(WorldGenerator.Instance.blockTypes[blockID].getTextureID(p));

                    float lightLevel = neigbor.globalLightPercent;

                    for (int k = 0; k < 4; k++)
                    {
                        colors.Add(new Color(0, 0, 0, lightLevel));
                    }

                    //we need seperate triangle for shader
                    if (!WorldGenerator.Instance.blockTypes[neigbor.id].renderNeighborFaces)
                    {

                        //explanation at VoxelData.VoxelTriangle
                        //nonTransparent
                        triangles.Add(vertexIndex);
                        triangles.Add(vertexIndex + 1);
                        triangles.Add(vertexIndex + 2);
                        triangles.Add(vertexIndex + 2);
                        triangles.Add(vertexIndex + 1);
                        triangles.Add(vertexIndex + 3);

                    }
                    else
                    {

                        //transparent triangles array
                        transparentTriangles.Add(vertexIndex);
                        transparentTriangles.Add(vertexIndex + 1);
                        transparentTriangles.Add(vertexIndex + 2);
                        transparentTriangles.Add(vertexIndex + 2);
                        transparentTriangles.Add(vertexIndex + 1);
                        transparentTriangles.Add(vertexIndex + 3);

                    }

                    //now we need to increment the verts index not by 6 but by 4 (because we just have 4 vertices)
                    vertexIndex += 4;

                }
                                
            }
            
        }

        public void CreateMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices.ToArray();

            mesh.subMeshCount = 2;
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.SetTriangles(transparentTriangles.ToArray(), 1);
            //mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            //for rendering the light etc accurately
            mesh.colors = colors.ToArray();
            meshFilter.mesh = mesh;

        }

        void AddTexture(int textureID)
        {
            float y = textureID / VoxelData.textureAtLastSizeInBlocks;
            float x = textureID - (y * VoxelData.textureAtLastSizeInBlocks);

            x *= VoxelData.NormalizedBlockTextureSize;
            y *= VoxelData.NormalizedBlockTextureSize;

            y = 1f - y - VoxelData.NormalizedBlockTextureSize;

            uvs.Add(new Vector2(x, y));
            uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
            uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
            uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));

        }

    }

    public class ChunkCoord
    {
        public int x;
        public int z;
        //this is the position of the chunk that we're throwing but not in the WorldGenerator.Instance space
        public ChunkCoord()
        {
            x = 0;
            z = 0;
        }

        public ChunkCoord(int _x, int _z)
        {
            x = _x;
            z = _z;
        }

        public ChunkCoord(Vector3 pos)
        {
            int xCheck = Mathf.FloorToInt(pos.x);
            int zCheck = Mathf.FloorToInt(pos.z);

            x = xCheck / VoxelData.chunkWidth;
            z = zCheck / VoxelData.chunkWidth;
        }

        public bool Equals(ChunkCoord other)
        {
            if (other == null)
            {
                return false;
            }
            else if(other.x == x && other.z == z)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }


    [System.Serializable]
    public class VoxelState
    {

        public byte id;
        public float globalLightPercent;//precentage of whatever the light value is

        public VoxelState()
        {

            id = 0;
            globalLightPercent = 0f;

        }

        public VoxelState(byte _id/*, float _globalLightPercent*/)
        {

            id = _id;
            globalLightPercent = 0;

        }

    }

}