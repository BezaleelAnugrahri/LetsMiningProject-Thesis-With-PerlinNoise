using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseTutorial
{
    public static class TextureGenerator
    {
        //applying texture from colour map
        public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            
            //for remove blury things in perlin noise color map
            texture.filterMode = FilterMode.Point;
            //for fixing wraping 
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.SetPixels(colourMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHightMap(HeightMap heightMap)
        {
            int width = heightMap.values.GetLength(0);
            int height = heightMap.values.GetLength(1);

            //Texture2D texture = new Texture2D(width, height);

            Color[] colourMap = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
                }
            }
            //texture.SetPixels(colourMap);
            //texture.Apply();

            //because we had that class be4
            return TextureFromColourMap(colourMap, width, height);
        }
    }
}