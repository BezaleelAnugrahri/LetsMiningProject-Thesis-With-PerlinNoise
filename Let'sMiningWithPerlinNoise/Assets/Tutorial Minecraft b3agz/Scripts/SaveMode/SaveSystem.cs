using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MinecraftTutorial
{

    public static class SaveSystem
    {

        public static void SaveWorld(WorldData world)
        {
            //set our save location and make sure we have save folder ready to go.
            string savePath = WorldGenerator.Instance.appPath + "/SaveData/" + world.worldName + "/";
            
            if (!Directory.Exists(savePath))
            {

                Directory.CreateDirectory(savePath);

            }

            Debug.Log("Saving " + world.worldName);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath + "world.world", FileMode.Create);

            formatter.Serialize(stream, world);
            stream.Close();

            Thread thread = new Thread(() => SaveChunks(world));
            thread.Start();

        }

        public static void SaveChunks(WorldData world)
        {

            List<ChunkData> chunks = new List<ChunkData>(world.modifiedChunks);
            world.modifiedChunks.Clear();

            //now we need to know how many chunks that saved inside our data folder
            int count = 0;
            foreach (ChunkData chunk in chunks)
            {

                SaveSystem.SaveChunksPosition(chunk, world.worldName);
                count++;

            }
            Debug.Log(count + " Chunks Saved...");

        }

        public static WorldData LoadWorld(string worldName, int seed = 0)
        {

            string loadPath = WorldGenerator.Instance.appPath + "/SaveData/" + worldName + "/";

            if (File.Exists(loadPath + "world.world"))
            {

                Debug.Log(worldName + " found. Loading from Save...");

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(loadPath + "world.world", FileMode.Open);

                WorldData world = formatter.Deserialize(stream) as WorldData;
                stream.Close();

                return new WorldData(world);

            }
            else
            {

                Debug.Log("Sorry " + worldName + " not found. Creating New World...");
                
                WorldData world = new WorldData(worldName, seed);
                SaveWorld(world);

                return world;

            }

        }

        public static void SaveChunksPosition(ChunkData chunk, string worldName)
        {//saving chunk data class to the file 
            string chunkName = chunk.position.x + " - " + chunk.position.y;

            //set our save location and make sure we have save folder ready to go.
            string savePath = WorldGenerator.Instance.appPath + "/SaveData/" + worldName + "/chunks/";

            if (!Directory.Exists(savePath))
            {

                Directory.CreateDirectory(savePath);

            }
            
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(savePath + chunkName + ".chunk", FileMode.Create);

            formatter.Serialize(stream, chunk);
            stream.Close();

        }

        public static ChunkData LoadChunk(string worldName, Vector2Int position)
        {
            string chunkName = position.x + " - " + position.y;

            string loadPath = WorldGenerator.Instance.appPath + "/SaveData/" + worldName + "/chunks/" + chunkName + ".chunk";

            if (File.Exists(loadPath))
            {

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(loadPath, FileMode.Open);

                ChunkData chunkData = formatter.Deserialize(stream) as ChunkData;
                stream.Close();

                return chunkData;

            }

            return null;

        }

    }


}