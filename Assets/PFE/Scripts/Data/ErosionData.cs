using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ErosionData : UpdateableData {
	public int thermalcycle;
	[Range(0,1)]
	public float thermalslopeLimit;
	[Range(0,1)]
	public float termalStrength;

	public int fastcycle;
	[Range(0,1)]
	public float fastslopeLimit;
	[Range(0,1)]
	public float fastStrength;

	public int hydraulicCycle;
	[Range(0.001f,1)]
	public float amountRain;
	[Range(0,1)]
	public float solubility;
	[Range(0,1)]
	public float evaporation;
}
