using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData  {
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool useIslandFallOff;
	
	public float minHeight {
		get {
			return  meshHeightMultiplier * meshHeightCurve.Evaluate (0);
		}
	}

	public float maxHeight {
		get {
			return  meshHeightMultiplier * meshHeightCurve.Evaluate (1);
		}
	}
}
