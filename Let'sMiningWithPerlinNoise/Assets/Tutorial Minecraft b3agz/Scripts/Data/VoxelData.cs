using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftTutorial
{
    public static class VoxelData
    { 
        //vertices = corner, vertex = starting point, voxel = 3D space in array
        // this class must be static class because the point at this Data is just want to store data that we need
        // readonly because we dont want to change the shape value for every cube/voxel/part of the game that automatically generated
        
        //now we want to setting the current width and height of how many voxel that we want to produce
        // we put it at here so when the game starts running this variable didn't changes.  
        public static readonly int chunkWidth = 16;
        public static readonly int chunkHeight = 128;
        public static readonly int worldSizeInChunks = 100;

        //Lighting Values
        public static readonly float minLightLevel = 0.1f;
        public static readonly float maxLightLevel = 0.9f;
        public static readonly float lightFallOff = 0.08f;

        public static int seed;
        public static string worldName;

        public static int WorldSizeInVoxels
        {
            get
            {
               return worldSizeInChunks * chunkWidth;
            }
        }

        //Atlas Texture pack dont change it 
        public static readonly int textureAtLastSizeInBlocks = 16;

        //getting the center of the world
        public static int WorldCenter
        {
            get
            {
                return (worldSizeInChunks * chunkWidth) / 2;
            }
        }

        public static float NormalizedBlockTextureSize
        {
            get
            {
                return 1f / (float)textureAtLastSizeInBlocks;
            }
        }
            
        // we know that every cube in this game has 8 vertices
        public static readonly Vector3[] VoxelVerts = new Vector3[8]
        {

		    new Vector3(0.0f, 0.0f, 0.0f),// First Voxel
		    new Vector3(1.0f, 0.0f, 0.0f),
		    new Vector3(1.0f, 1.0f, 0.0f),
		    new Vector3(0.0f, 1.0f, 0.0f),
		    new Vector3(0.0f, 0.0f, 1.0f),
		    new Vector3(1.0f, 0.0f, 1.0f),
		    new Vector3(1.0f, 1.0f, 1.0f),
		    new Vector3(0.0f, 1.0f, 1.0f)
            
        };

        //this variable is for undraw the face of the cube if it doesn't at the open air
        public static readonly Vector3Int[] FaceCheck = new Vector3Int[6]
        {
            //represent a bunch of offset in the same order that we check the faces
            new Vector3Int(0, 0, -1),
		    new Vector3Int(0, 0, 1),
		    new Vector3Int(0, 1, 0),
		    new Vector3Int(0, -1, 0),
		    new Vector3Int(-1, 0, 0),
		    new Vector3Int(1, 0, 0)

        };

        //inside the game triangle(mesh renderer) must be build in clockwise, otherwise we cant see it 
        //every square have 2 triangle that need 3 vertices
        public static readonly int[,] VoxelTriangle = new int[6, 4]
        {
            //the pattern was 0, 1, 2 and it loop back on it self (or mirroring) 2, 1, 3. so it will be 0, 1, 2, 2, 1, 3
            {0, 3, 1, 2}, // Front Face
		    {5, 6, 4, 7}, // Back Face
		    {3, 7, 2, 6}, // Top Face
		    {1, 5, 0, 4}, // Bottom Face
		    {4, 7, 0, 3}, // Left Face
		    {1, 2, 5, 6} // Right Face

            //we will loop through this index 0, 
            //for each one of the triangle we will add coresponding Vector3 from here into the vertex array

        };

        public static readonly Vector2[] VoxelUvs = new Vector2[4] 
        {
            //assigning texture to chunk, basically each one of UV Vector2 is coordinate on actual texture 
            //0,0 is bottom left; 1,1 is top right; 0,1 is top left; and 1,0 is bottom right
		    new Vector2 (0.0f, 0.0f),
		    new Vector2 (0.0f, 1.0f),
		    new Vector2 (1.0f, 0.0f),
		    new Vector2 (1.0f, 1.0f)

	    };


    }

}