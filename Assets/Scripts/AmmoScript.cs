using UnityEngine;
using System.Collections;

public class AmmoScript : MonoBehaviour {

	private float birth;
	private float lifetime;

	void Start () 
	{
		birth = Time.time;
		lifetime = 40f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((Time.time - birth) > lifetime)
		{
			Destroy(gameObject);
		}
	}
}