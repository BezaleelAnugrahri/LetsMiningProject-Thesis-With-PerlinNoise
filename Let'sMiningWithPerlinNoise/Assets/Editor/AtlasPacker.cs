using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR 
using UnityEditor;


//this is from B3agz Tutorial
namespace MinecraftTutorial
{
    public class AtlasPacker : EditorWindow
    {
        int blockSize = 16;//block size in pixel.
        int atlasSizeInBlocks = 16;
        int textureLength = 256;//amount of texture length for array rawTexture.
        int atlasSize;

        string saveSite = "/Tutorial Minecraft b3agz/Resources/Textures/";
        [Tooltip("TextureName + .png/.jpeg/.gif")]
        string fileName = "Packed_Atlas.png";

        Object[] rawTextures;

        List<Texture2D> sortedTextures = new List<Texture2D>();

        Texture2D atlasTexture;//Result of Combine the single atlas

        [MenuItem("Minecraft/Atlas Packer")]

        public static void ShowWindow()
        {

            EditorWindow.GetWindow(typeof(AtlasPacker));
            //look for the window for atlas packer

        }

        public void OnGUI()
        {
            rawTextures = new Object[textureLength];
            atlasSize = blockSize * atlasSizeInBlocks;
            
            GUILayout.Label("Minecraft Cone Texture Atlas Packer", EditorStyles.boldLabel);

            blockSize = EditorGUILayout.IntField("Block Size", blockSize);
            atlasSizeInBlocks = EditorGUILayout.IntField("Atlas Size (In Blocks)", atlasSizeInBlocks);
            textureLength = EditorGUILayout.IntField("Texture Length", textureLength);
            saveSite = EditorGUILayout.TextField("Save to", saveSite);
            fileName = EditorGUILayout.TextField("File Name", fileName);

            GUILayout.Label(atlasTexture);

            if (GUILayout.Button("Load Textures"))
            {

                LoadTextures();
                PackAtlas();
                Debug.Log("Atlas Packer : Textures Loaded.");

            }

            if (GUILayout.Button("Clear Textures"))
            {

                atlasTexture = new Texture2D(atlasSize, atlasSize);
                Debug.Log("Atlas Packer : Textures Cleared.");
                
            }

            if (GUILayout.Button("Save Atlas"))
            {

                byte[] bytes = atlasTexture.EncodeToPNG();

                try
                {
                    //trying to execute something but if it's error, u can decide what to do.
                    File.WriteAllBytes(Application.dataPath + saveSite + fileName, bytes);
                }

                catch
                {
                    Debug.Log("Atlas Packer : Couldn't save atlas packer to file.");
                }

                Debug.Log("Atlas Packer : Textures Saved to /Textures/Packed_Atlas.png");

            }

        }

        void LoadTextures()
        {
            sortedTextures.Clear();

            rawTextures = Resources.LoadAll("2D/Textures/AtlasPacker", typeof(Texture2D));
            //this just for Folder that inside the Assets/Resources 
            int index = 0;

            foreach (Object tex in rawTextures)
            {

                Texture2D t = (Texture2D)tex;

                if (t.width == blockSize && t.height == blockSize)
                {
                    sortedTextures.Add(t);
                }
                else
                {
                    Debug.Log("Asset Packer : " + tex.name + " incorect size, Texture not loaded.");
                }
                index++;

            }

            Debug.Log("Atlas Packer : " + sortedTextures.Count + " Successfully Loaded.");

        }

        void PackAtlas()
        {

            atlasTexture = new Texture2D(atlasSize, atlasSize);
            Color[] pixels = new Color[atlasSize * atlasSize];

            for (int x = 0; x < atlasSize; x++)
            {
                for (int y = 0; y < atlasSize; y++)
                {

                    //Get the current block that we're looking at.
                    int currentBlockX = x / blockSize;
                    int currentBlockY = y / blockSize;

                    int index = currentBlockY * atlasSizeInBlocks + currentBlockX;
                    //currentBlockY is for column and currentBlockX is for row

                    //get pixel in current block
                    int currentPixelX = x - (currentBlockX * blockSize);
                    int currentPixelY = y - (currentBlockY * blockSize);

                    if (index < sortedTextures.Count)
                    {
                        pixels[(atlasSize - y - 1) * atlasSize + x] = sortedTextures[index].GetPixel(x, blockSize - y - 1); 
                        /* basicaly texture starts from bottom left
                         * it will work like currentBlockY * atlasSizeInBlocks + currentBlockX 
                         * but everything will be upside down because (0,0) is from bottom left (like coordinate)
                         * y == blockSize - y - 1 because without it, it will be upside down and -1 because it's array and it's started from 0.
                         */
                    }
                    else
                    {
                        pixels[(atlasSize - y - 1) * atlasSize + x] = new Color(0f, 0f, 0f, 0f);
                    }

                }
            }

            atlasTexture.SetPixels(pixels);
            atlasTexture.Apply();

        }

    }

}
#endif