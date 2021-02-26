using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerlinNoiseTutorial;


[CreateAssetMenu()]
public class MeshSettings : AutoUpdateData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatShadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };//replacement for mapChunk size...
    //public static readonly int[] supportedFlatShadedChunkSizes = { 48, 72, 96 };

    public float meshScale = 2.5f;//everytime scale of player change, the map will not meshup / error

    public bool useFlatShading;
    public bool useFalloffMap;
    
    public float evaluateA = 4.46f;
    public float evaluateB = 2.2f;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numSupportedFlatShadedChunkSizes - 1)]
    public int flatShadedChunkSizeIndex;


    //num vertices per line of mesh rendered at LOD = 0. Includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals
    //adding support for multiple mesh chunk resolution (LOD started)
    //public const int mapChunkSize = 95; //because the map is always square
    //we need to pick something lower because it has been used by flatshading. so, it will be using extra vertices.
    //because unity provide maximum vertises per mesh about 255squere 
    //width x height = mapChunkSize 65025 vertises per mesh
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 3) * meshScale;
            //3 vertices and width of 2 because width is the space between each vertices
            //for the first time we need to -1, because of this ^ explanation... 
            //but we need to -2 because we dont want count vertices that aren't included in mesh so we need to -3.
        }
    }

}