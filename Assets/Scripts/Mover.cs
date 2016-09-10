using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

    public float speed;
    
	// Use this for initialization
	void Start () {
        //transform.forward:The blue axis of the transform in world space.
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
	
	// Update is called once per frame
	void Update () {
	     
	}
}
