using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdateableData {
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	[Range(0, 1)]
	public float valueNoise;
	[Range(0, 1)]
	public float perlinNoise;
	[Range(0, 1)]
	public float simplexeNoise;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	protected void OnValidate(){
		float total = perlinNoise + valueNoise + simplexeNoise;
		if (total != 1 && total != 0)
		{
			perlinNoise = perlinNoise / total;
			valueNoise = valueNoise / total;
			simplexeNoise = simplexeNoise / total;
		}
		else if(total == 0)
		{
			valueNoise = 1f;
			perlinNoise = 0f;
			simplexeNoise = 0f;
		}
		base.OnValidate ();
	}

}
