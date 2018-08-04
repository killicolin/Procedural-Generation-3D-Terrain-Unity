Shader "Custom/Terrain" {
	Properties {
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Standard fullforwardshadows

		
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

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float3 triplanar(float3 worldPos, float scale, float3 blend, int textureIndex) {
			float3 newdWorldPos = worldPos / scale;
			float3 ProjectionX = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(newdWorldPos.y, newdWorldPos.z, textureIndex)) * blend.x;
			float3 ProjectionY = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(newdWorldPos.x, newdWorldPos.z, textureIndex)) * blend.y;
			float3 ProjectionZ = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(newdWorldPos.x, newdWorldPos.y, textureIndex)) * blend.z;
			return ProjectionX + ProjectionY + ProjectionZ;
		}

		float invLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
		float3 blend = abs(IN.worldNormal);
			float heightPercent = invLerp(minHeight,maxHeight, IN.worldPos.y);
			blend /= blend.x + blend.y + blend.z;

			for (int i = 0; i < layerCount; i ++) {
				float strength = invLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

				float3 tintColour = baseColours[i] * baseColourStrength[i];
				float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blend, i) * (1-baseColourStrength[i]);

				o.Albedo = o.Albedo * (1-strength) + (tintColour+textureColour) * strength;
			}

		
		}


		ENDCG
	}
	FallBack "Diffuse"
}