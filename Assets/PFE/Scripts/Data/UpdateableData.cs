using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject {
	public event System.Action OnValuesUpdated;
	public bool autoUpdate;

	protected void OnValidate(){
		if (autoUpdate) {
			UnityEditor.EditorApplication.update+=NotifyOfUpdatedValues;
		}
	}

	public void NotifyOfUpdatedValues(){
		UnityEditor.EditorApplication.update-=NotifyOfUpdatedValues;
		if (OnValuesUpdated != null) {
			OnValuesUpdated ();
		}
	}
}
