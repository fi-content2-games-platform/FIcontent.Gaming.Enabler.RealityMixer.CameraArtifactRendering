using UnityEngine;
using System.Collections;

public class ModelRotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(Vector3.forward, 20 * Time.deltaTime);
	}
}
