using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftTutorial
{
    public static class NoiseGenerator
    {

        public static float Get2DPerlin(Vector2 position, float offset, float scale)
        {
            position.x += (offset + VoxelData.seed + 0.1f);
            position.y += (offset + VoxelData.seed + 0.1f);

            float xValue = position.x / VoxelData.chunkWidth * scale;
            float yValue = position.y / VoxelData.chunkWidth * scale;

            return Mathf.PerlinNoise(xValue, yValue);
        }

        public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold)
        {
            // https://www.youtube.com/watch?v=Aga0TBJkchM Carpilot on YouTube

            float x = (position.x + offset + VoxelData.seed + 0.1f) * scale;//the reason of 0.1f because if you don't give offset decimal value it will be comming below decimal number
            float y = (position.y + offset + VoxelData.seed + 0.1f) * scale;
            float z = (position.z + offset + VoxelData.seed + 0.1f) * scale;

            float AB = Mathf.PerlinNoise(x, y);
            float BC = Mathf.PerlinNoise(y, z);
            float AC = Mathf.PerlinNoise(x, z);
            float BA = Mathf.PerlinNoise(y, x);
            float CB = Mathf.PerlinNoise(z, y);
            float CA = Mathf.PerlinNoise(z, x);

            if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



    }

}