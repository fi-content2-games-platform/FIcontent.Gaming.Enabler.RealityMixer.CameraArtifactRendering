using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public Vector3 camUp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.camUp = this.transform.up;
        Debug.Log(this.transform.up + " is up.");
	}
}
