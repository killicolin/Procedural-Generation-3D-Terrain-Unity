using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    private static Vector2[] gradients2D = {
        new Vector2( 1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2( 0f, 1f),
        new Vector2( 0f,-1f),
        new Vector2( 1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2( 1f,-1f).normalized,
        new Vector2(-1f,-1f).normalized
    };
    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };
    private const int hashMask = 255;

    private const int gradientsMask2D = 7;

    // Use this for initialization
    public static float[,,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offeset)
    {
        float[,] WhitenoiseMap = Noise.WhiteNoise(mapWidth, mapHeight, seed);
        int[,] gradientIndex = Noise.gradientBase(mapWidth, mapHeight, seed);
        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float halfWidth = mapWidth / 2;
        float halfheight = mapHeight / 2;
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(mapWidth, mapWidth + 10000) + offeset.x;
            float offsetY = rng.Next(mapHeight, mapHeight + 10000) + offeset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        float[,,] noiseMap = new float[3, mapWidth, mapHeight];
        if (scale <= 0)
            scale = 0.001f;
        float sampleX;
        float sampleY;
        float amplitude;
        float frequency;
        float noiseValueHeight;
        float noisePerlinHeight;
        float noiseSimplexHeight;
        float minNoisePerlinHeight = float.MaxValue;
        float maxNoisePerlinHeight = float.MinValue;
        float minNoiseValueHeight = float.MaxValue;
        float maxNoiseValueHeight = float.MinValue;
        float minNoiseSimplexHeight = float.MaxValue;
        float maxNoiseSimplexHeight = float.MinValue;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                noiseValueHeight = 0;
                noisePerlinHeight = 0;
                noiseSimplexHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    sampleY = (y - halfheight) / scale * frequency + octaveOffsets[i].y;
                    float valueValue = ValueNoise(sampleX, sampleY, WhitenoiseMap, mapWidth, mapHeight) * 2 - 1;
                    float perlinValue = PerlinNoise(sampleX, sampleY, gradientIndex, WhitenoiseMap, mapWidth, mapHeight) * 2 - 1;
                    float simplexNoise = SimplexNoise(sampleX, sampleY) * 2 - 1;
                    noisePerlinHeight += perlinValue * amplitude;
                    noiseValueHeight += valueValue * amplitude;
                    noiseSimplexHeight += simplexNoise * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noisePerlinHeight > maxNoisePerlinHeight)
                    maxNoisePerlinHeight = noisePerlinHeight;
                else if (noisePerlinHeight < minNoisePerlinHeight)
                    minNoisePerlinHeight = noisePerlinHeight;
                if (noiseValueHeight > maxNoiseValueHeight)
                    maxNoiseValueHeight = noiseValueHeight;
                else if (noiseValueHeight < minNoiseValueHeight)
                    minNoiseValueHeight = noiseValueHeight;
                if (noiseSimplexHeight > maxNoiseSimplexHeight)
                    maxNoiseSimplexHeight = noiseSimplexHeight;
                else if (noiseSimplexHeight < minNoiseSimplexHeight)
                    minNoiseSimplexHeight = noiseSimplexHeight;
                noiseMap[0, x, y] = noiseValueHeight;
                noiseMap[1, x, y] = noisePerlinHeight;
                noiseMap[2, x, y] = noiseSimplexHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //normalise le resultat entre 0 et 1
                noiseMap[0, x, y] = Mathf.InverseLerp(minNoiseValueHeight, maxNoiseValueHeight, noiseMap[0, x, y]);
                noiseMap[1, x, y] = Mathf.InverseLerp(minNoisePerlinHeight, maxNoisePerlinHeight, noiseMap[1, x, y]);
                noiseMap[2, x, y] = Mathf.InverseLerp(minNoiseSimplexHeight, maxNoiseSimplexHeight, noiseMap[2, x, y]);

            }
        }
        return noiseMap;
    }

    public static float[,] WhiteNoise(int mapWidth, int mapHeight, int seed)
    {
        System.Random rng = new System.Random(seed);
        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = (float)rng.Next(0, 10000) / 10000;
            }
        }
        return noiseMap;
    }

    public static int[,] gradientBase(int mapWidth, int mapHeight, int seed)
    {
        System.Random rng = new System.Random(seed);
        int[,] gradIndex = new int[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                gradIndex[x, y] = rng.Next(0, 8);
            }
        }
        return gradIndex;
    }

    public static float ValueNoise(float sampleX, float sampleY, float[,] noiseMap, int mapWidth, int mapHeight)
    {
        sampleX = (mapWidth + (sampleX) % mapWidth) % mapWidth;
        sampleY = (mapHeight + (sampleY) % mapHeight) % mapHeight;
        int x0 = Mathf.FloorToInt(sampleX);
        int x1 = (x0 + 1) % mapWidth;
        int y0 = Mathf.FloorToInt(sampleY);
        int y1 = (y0 + 1) % mapHeight;
        float xtmp = sampleX - x0;
        float ytmp = sampleY - y0;
        return Mathf.Lerp(Mathf.Lerp(noiseMap[x0, y0], noiseMap[x1, y0], xtmp), Mathf.Lerp(noiseMap[x0, y1], noiseMap[x1, y1], xtmp), ytmp);
    }

    private static float Dot(Vector2 g, float x, float y)
    {
        return g.x * x + g.y * y;
    }

    public static float PerlinNoise(float sampleX, float sampleY, int[,] indexGrad, float[,] noiseMap, int mapWidth, int mapHeight)
    {
        sampleX = (mapWidth + (sampleX) % mapWidth) % mapWidth;
        sampleY = (mapHeight + (sampleY) % mapHeight) % mapHeight;
        int x0 = Mathf.FloorToInt(sampleX);
        int x1 = (x0 + 1) % mapWidth;
        int y0 = Mathf.FloorToInt(sampleY);
        int y1 = (y0 + 1) % mapHeight;
        float xtmp = sampleX - x0;
        float ytmp = sampleY - y0;
        Vector2 g00 = gradients2D[indexGrad[x0, y0]];
        Vector2 g10 = gradients2D[indexGrad[x1, y0]];
        Vector2 g01 = gradients2D[indexGrad[x0, y1]];
        Vector2 g11 = gradients2D[indexGrad[x1, y1]];
        float v00 = Dot(g00, x0, y0);
        float v10 = Dot(g10, x1, y0);
        float v01 = Dot(g01, x0, y1);
        float v11 = Dot(g11, x1, y1);
        return Mathf.Lerp(Mathf.Lerp(v00, v10, xtmp), Mathf.Lerp(v01, v11, xtmp), ytmp);
    }

    private static float trianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;

    private static float squaresToTriangles = trianglesToSquares / (1 + 2 * trianglesToSquares);

    public static float SimplexNoise(float sampleX, float sampleY)
    {
        float skew = (sampleX + sampleY) * trianglesToSquares;
        float sx = sampleX + skew;
        float sy = sampleY + skew;
        int x = Mathf.FloorToInt(sx);
        int y = Mathf.FloorToInt(sy);
        float unskew = (x + y) * squaresToTriangles;
        float x0 = x - unskew;
        float y0 = y - unskew;
        x0 = sampleX - x0;
        y0 = sampleY - y0;
        int i1 = 0;
        int j1 = 0;
        if (x0 > y0)
        {
            i1 = 1;
        }
        else
        {
            j1 = 1;
        }
        float x1 = x0 - i1 + squaresToTriangles;
        float y1 = y0 - j1 + squaresToTriangles;
        float x2 = x0 - 1 + 2 * squaresToTriangles;
        float y2 = y0 - 1 + 2 * squaresToTriangles;
        float d0 = 0.5f - x0 * x0 - y0 * y0;
        float d1 = 0.5f - x1 * x1 - y1 * y1;
        float d2 = 0.5f - x2 * x2 - y2 * y2;
        float res = 0;
        int ii = x & hashMask;
        int jj = y & hashMask;
        int gi0 = hash[ii + hash[jj]] & gradientsMask2D;
        int gi1 = hash[ii + i1 + hash[jj + j1]] & gradientsMask2D;
        int gi2 = hash[ii + 1 + hash[jj + 1]] & gradientsMask2D;
        
        if (d0 > 0)
        {
            d0 *= d0;
            res += d0 * d0 * Dot(gradients2D[gi0], x0, y0);
        }
        if (d1 > 0)
        {
            d1 *= d1;
            res += d1 * d1 * Dot(gradients2D[gi1], x1, y1);
        }
        if (d2 > 0)
        {
            d2 *= d2;
            res += d2 * d2 * Dot(gradients2D[gi2], x2, y2);
        }
        return res;
    }
}