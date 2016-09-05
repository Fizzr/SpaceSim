using UnityEngine;
using System.Collections;

public class PHYSICS_MAEN : MonoBehaviour {

	// Use this for initialization

	private float scale;
	private float mass;

	void Start () 
	{
		System.Random rnd = new System.Random ();

		scale = (float)(rnd.Next (1,20) + rnd.NextDouble());

		mass = scale * 0.5f;
		transform.localScale = new Vector3(scale, scale, scale);
		GetComponent<Rigidbody>().mass = mass;
		GetComponent<Rigidbody>().AddTorque (rnd.Next(1,20)*mass, rnd.Next(1,20)*mass, rnd.Next(1,20)*mass);
	}
}