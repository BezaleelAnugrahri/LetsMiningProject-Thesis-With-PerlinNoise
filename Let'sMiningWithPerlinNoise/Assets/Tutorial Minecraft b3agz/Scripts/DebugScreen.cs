using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftTutorial
{
    public class DebugScreen : MonoBehaviour
    {

        public GameObject debugScreen;
        public WorldGenerator world;

        Text textPlaceholder;
        string debugText;
        float frameRate;
        float timer;
        
        int halfWorldSizeInVoxels;
        int halfWorldSizeInChunks;

        void Start()
        {
            textPlaceholder = debugScreen.GetComponent<Text>();

            halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
            halfWorldSizeInChunks = VoxelData.worldSizeInChunks / 2;
        }

        void Update()
        {

            ShowDebugWindows();

        }

        public void ShowDebugWindows()
        {
            FramePerSecond();

            if (Input.GetKeyDown(KeyCode.F3))
            {
                debugScreen.SetActive(!debugScreen.activeSelf);
            }

            debugText = "Tutorial Minecraft with Perlin Noise by Me \n";
            debugText += frameRate + " fps \n";
            debugText += "X : " + (Mathf.FloorToInt(world.players.transform.position.x) - halfWorldSizeInVoxels) + " Y : " + Mathf.FloorToInt(world.players.transform.position.y) + " Z : " + (Mathf.FloorToInt(world.players.transform.position.z) - halfWorldSizeInVoxels) + "\n";
            debugText += "Chunk : " + (world.playersChunkCoord.x - halfWorldSizeInChunks) + " / " + (world.playersChunkCoord.z - halfWorldSizeInChunks) + "\n";
            debugText += "Time : " + System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + "\n";
            debugText += "Application Version = " + PlayerPrefs.GetString("AppVer") + "\n";
            debugText += "Seed = " + PlayerPrefs.GetInt("Seed") + "\n";
            debugText += "WorldName = " + PlayerPrefs.GetString("WorldName") + "";

            textPlaceholder.text = debugText;
            
        }

        public void FramePerSecond()
        {

            if (timer > 1f)
            {
                frameRate = (int)(1f / Time.unscaledDeltaTime);
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }

        }

    }

}