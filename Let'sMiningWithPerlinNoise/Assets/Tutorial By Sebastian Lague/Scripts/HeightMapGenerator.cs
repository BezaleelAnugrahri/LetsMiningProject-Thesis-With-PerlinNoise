using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseTutorial
{
    public static class HeightMapGenerator
    {
        public static HeightMap GeneratingHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
        {
            //Generating Noise Map
            float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

            AnimationCurve heightCurve_threadSafe = new AnimationCurve(settings.heightCurve.keys);

            float minVal = float.MaxValue;
            float maxVal = float.MinValue;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    values[i, j] *= heightCurve_threadSafe.Evaluate(values[i, j]) * settings.heightMultiplier;
                    
                    if (values[i, j] > maxVal)
                    {
                        maxVal = values[i, j];
                    }
                    if (values[i, j] < minVal)
                    {
                        minVal = values[i, j];
                    }

                }
            }

            return new HeightMap(values, minVal, maxVal);            
        }
    }


    public struct HeightMap
    {
        public readonly float[,] values;
        public readonly float minValue;
        public readonly float maxValue;

        public HeightMap(float[,] values, float minValue, float maxValue)
        {
            this.values = values;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}