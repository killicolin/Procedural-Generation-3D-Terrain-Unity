using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMAp, ColorMap, Mesh }
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;

	public NoiseData noiseData;
	public TerrainData terrainData;
	public ErosionData erosionData;
	public TextureData textureData;

	public Material terrainMaterial;
    public bool autoUpdate;

    public TerrainType[] regions;
	float [,] fallofMap;
	void Awake(){
		fallofMap = IslandGenerator.GenerateFallofMap (mapChunkSize);
	}

    public void GenerateMap()
    {
        //float[,] noiseMap = Noise.WhiteNoise(mapWidth, mapHeight,seed);
		float[,,] noiseResultMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize,noiseData.seed, noiseData.noiseScale,noiseData.octaves,noiseData.persistance,noiseData.lacunarity,noiseData.offset);
        float[,] noiseMap= new float[mapChunkSize, mapChunkSize];
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) { 
			for (int x = 0; x < mapChunkSize; x++) {
				noiseMap [x, y] = noiseData.valueNoise * noiseResultMap [0, x, y] + noiseData.perlinNoise * noiseResultMap [1, x, y] + noiseData.simplexeNoise * noiseResultMap [2, x, y];
			}
		}
		if(erosionData.thermalcycle>0)
			noiseMap = Erosion.thermalErosion (noiseMap, erosionData.thermalcycle, erosionData.thermalslopeLimit, erosionData.termalStrength, mapChunkSize);
		if(erosionData.fastcycle>0)
			noiseMap = Erosion.fastErosion (noiseMap, erosionData.fastcycle, erosionData.fastslopeLimit, erosionData.fastStrength, mapChunkSize);
		if (erosionData.hydraulicCycle > 0)
			noiseMap = Erosion.hydraulicErosion (noiseMap, erosionData.hydraulicCycle, erosionData.amountRain/10f, erosionData.solubility, erosionData.evaporation, mapChunkSize);
        for (int y = 0; y < mapChunkSize; y++)
        { 
            for (int x = 0; x < mapChunkSize; x++)
            {
				if (terrainData.useIslandFallOff)
					noiseMap [x, y] = Mathf.Clamp01(noiseMap [x, y] - fallofMap [x, y]);
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height){
                        colorMap[x+ y*mapChunkSize] = regions[i].color;
                        break;
                    }

                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMAp)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        else if (drawMode == DrawMode.Mesh)
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
		textureData.UpdateMeshHeights (terrainMaterial, 10*terrainData.minHeight, 10*terrainData.maxHeight);
	}
		
	void OnValuesUpdated(){
		if(!Application.isPlaying){
			GenerateMap ();
		}
	}

	void OnTextureValuesUpdated(){
		textureData.ApplyToMaterial (terrainMaterial);
	}

    private void OnValidate()
    {
		if (terrainData != null) {
			terrainData.OnValuesUpdated -= OnValuesUpdated;
			terrainData.OnValuesUpdated += OnValuesUpdated;
		}

		if (noiseData != null) {
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}

		if (erosionData!= null) {
			erosionData.OnValuesUpdated -= OnValuesUpdated;
			erosionData.OnValuesUpdated += OnValuesUpdated;
		}

		if (textureData != null) {
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}

		if (noiseData.lacunarity < 1){
			noiseData.lacunarity = 1;
        }
		if (noiseData.octaves < 0)
        {
			noiseData.octaves = 0;
        }
		if (fallofMap ==null)
			fallofMap = IslandGenerator.GenerateFallofMap (mapChunkSize);
    }
}

[System.Serializable]
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}
