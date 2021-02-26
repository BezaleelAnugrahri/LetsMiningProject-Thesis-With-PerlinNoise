Shader "Custom/TerrainShader"
{
    Properties
    {
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
		
		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4;

		int layerCount;
		float3 baseColours[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColourStrength[maxLayerCount];
		float baseTextureScales[maxLayerCount];

		float minHeight;
		float maxHeight;

		sampler2D testTexture;
		float testScale;

		UNITY_DECLARE_TEX2DARRAY(baseTextures);

        struct Input
        {
            float3 worldPos;
			float3 worldNormal;
        };

		float inverseLerp(float min, float max, float value)
		{
			return saturate((value-min)/(max-min));
		}

		//function used that called pixel when mesh is visible
		float3 triplanar(float3 worldPos, float scale, float3 blendAxis, int textureIndex)
		{
				//Applying the Texture
				float3 scaledWorldPos = worldPos / scale;

				float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3 (scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxis.x;
				float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3 (scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxis.y;
				float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3 (scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxis.z;
				
				return xProjection + yProjection + zProjection;
        }
        
		void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            float3 blendAxis = abs(IN.worldNormal);
			//brightness setting blend axis
			blendAxis /= blendAxis.x + blendAxis.y + blendAxis.z;

			// Albedo comes from a texture tinted by color
            //loop throught all of the base color

			for(int i = 0; i < layerCount; i++){
				float drawStrengh = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);
				
				float3 baseColour = baseColours[i] * baseColourStrength[i];
				float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxis, i) * (1-baseColourStrength[i]);

				o.Albedo = o.Albedo * (1-drawStrengh) + (baseColour + textureColour) * drawStrengh;
			}
			// Metallic and smoothness come from slider variables

        }
        ENDCG
    }
    FallBack "Diffuse"
}
