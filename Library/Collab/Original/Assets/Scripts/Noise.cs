using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise{

	// Use this for initialization
	public static float[,] GenerateNoiseMap(int mapWidth,int mapHeight,int seed,float scale,int octaves,float persistance,float lacunarity,Vector2 offeset)
    {
        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-10000, 10000)+ offeset.x;
            float offsetY = rng.Next(-10000, 10000)+ offeset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        float[,] noiseMap = new float[mapWidth, mapHeight];
        if (scale <= 0)
            scale = 0.001f;
        float sampleX;
        float sampleY;
        float amplitude;
        float frequency;
        float noiseHeight;
        float minNoiseHeight=float.MaxValue;
        float maxNoiseHeight=float.MinValue;
        float halfWidth = mapWidth / 2;
        float halfheight = mapHeight / 2;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    sampleX =(x-halfWidth) / scale *frequency+ octaveOffsets[i].x;
                    sampleY =(y-halfheight) / scale *frequency+ octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //rng.Next(-10000, 10000)/ 10000f;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if(noiseHeight< minNoiseHeight)
                    minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //normalise le resultat entre 0 et 1
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                Debug.Log(x / 1.1f + " " + y / 1.1f + " " + noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
	
}
