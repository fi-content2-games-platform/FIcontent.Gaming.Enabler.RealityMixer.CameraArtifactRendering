using UnityEngine;
using System.Collections;

public class DistanceCalculator : MonoBehaviour {
	
	public Camera cam;
	
	public int a;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		
	}
	
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0, 100, 100), (this.transform.position - cam.transform.position).magnitude + " ");
	}
}
