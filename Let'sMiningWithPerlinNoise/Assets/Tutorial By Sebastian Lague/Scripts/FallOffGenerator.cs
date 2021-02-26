using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerlinNoiseTutorial
{ // for generate island map
    public static class FallOffGenerator
    {
        public static float[,] GenerateFalloffMap(int size, float evaluateA, float evaluateB)
        {
            float[,] map = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //coordinate i,j is point side inside our square map, so we need to take that's coordinate make them in rage -1 to 1
                    float x = i / (float)size * 2 - 1;
                    float y = j / (float)size * 2 - 1;

                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)); //to find out which one is closes to the edge of the square (max value)
                    map[i, j] = Evaluate(value, evaluateA, evaluateB);
                }
            }
            return map; 
        }

        //for evaluate/control how strong transition between black and white in Fall off Map Generator. using function f(x) = x^a / x^a + (b-bx)^a ; 
        //where a(between 1 to 10) = for getting curves between white and black, and b(between 1 to 10) = shifting curve along x axis which can control the region of black
        static float Evaluate(float value, float a, float b)
        {
            float total = Mathf.Pow(value,a)/(Mathf.Pow(value,a) + Mathf.Pow((b - b*value),a));
            return total;
        }
    }
}