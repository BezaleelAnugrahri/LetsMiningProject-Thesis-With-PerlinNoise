using PerlinNoiseTutorial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : AutoUpdateData
    {
        public NoiseSettings noiseSettings;
    
        public float heightMultiplier;//variable of height Multiplier
        public AnimationCurve heightCurve;

        public float minHeight
        {
            get
            {
                return heightMultiplier * heightCurve.Evaluate(0);
            }
        }

        public float maxHeight
        {
            get
            {
                return heightMultiplier * heightCurve.Evaluate(1);
            }
        }

    #if UNITY_EDITOR//it means just usable inside unity editor
        protected override void OnValidate()
        {
            noiseSettings.ValidatedValues();
            base.OnValidate();
        }
    #endif  
}