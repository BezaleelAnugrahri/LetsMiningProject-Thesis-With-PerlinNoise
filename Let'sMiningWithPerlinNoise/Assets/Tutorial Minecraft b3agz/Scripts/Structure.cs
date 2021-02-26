using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftTutorial
{
    
    public static class Structure
    {
        public static Queue<VoxelMod> GenerateMajorFlora(int index, Vector3 position, int minTrunkHeight, int maxTrunkHeight)
        {

            switch (index)
            {
                case 0: 
                    {
                        return MakeTree(position, minTrunkHeight, maxTrunkHeight);
                    }

                case 1:
                    {
                        return MakeCactus(position, minTrunkHeight, maxTrunkHeight);
                    }
                                    
            }

            return new Queue<VoxelMod>();

        }

        public static Queue<VoxelMod> MakeTree(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
        {
            Queue<VoxelMod> queue = new Queue<VoxelMod>();

            int height = (int)(maxTrunkHeight * NoiseGenerator.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));
            
            if (height < minTrunkHeight)
            {
                height = minTrunkHeight;
            }

            for (int i = 1; i < height; i++)
            {

                queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 9/*tree trunk*/));

            }

            //leaves shape random generator

            for (int x = -3; x < 4; x++)
            {

                for (int y = 0; y < 7; y++)
                {

                    for (int z = -3; z < 4; z++)
                    {

                        queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + y, position.z + z), 11/*tree leaves*/));

                    }

                }

            }

            return queue;

        }

        public static Queue<VoxelMod> MakeCactus(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
        {
            Queue<VoxelMod> queue = new Queue<VoxelMod>();

            int height = (int)(maxTrunkHeight * NoiseGenerator.Get2DPerlin(new Vector2(position.x, position.z), 23425f, 2f));

            if (height < minTrunkHeight)
            {
                height = minTrunkHeight;
            }

            for (int i = 1; i < height; i++)
            {

                queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 12/*Cactus trunk*/));

            }

            

            return queue;

        }

    }

}