using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MinecraftTutorial
{
    [CreateAssetMenu(fileName = "BiomeAttributes", menuName = "MinecraftTutorial/BiomeAttributes")]
    public class BiomeAttributes : ScriptableObject
    {
        [Header("Terrain Settings")]
        public string biomeName;
        public float offset;
        public float scale;

        public int terrainHeight;
        public float terrainScale;
        public byte surfaceBlock;
        public byte subSurfaceBlock;

        [Header("Major Flora")]
        public int majorFloraIndex;
        public float majorFloraZoneScale = 1.3f;
        [Range(0.1f ,1f)]
        public float majorFloraZoneThreshold = 0.6f;
        [Range(0.1f, 1f)]
        public float majorFloraPlacementThreshold = 0.8f;
        public float majorFloraOffset;
        public float majorFloraPlacementScale = 15f;
        public bool placeMajorFlora = true;//option to not having place anything like this

        public int maxHeight = 12;
        public int minHeight = 5;


        [Header("LODES")]
        public LODE[] lodes;

    }
    
    [System.Serializable]
    public class LODE
    {
        
        public string nodeName;
        public byte blockID;
        public int minHeight;
        public int maxHeight;
        public float scale;
        public float threshold;
        public float noiseOffset;

    }

}