using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject asteroid0;
	public GameObject asteroid1;
	public GameObject asteroid2;
	private GameObject[] asteroids = new GameObject[3];

	private int astr_count;

	System.Random rnd = new System.Random();
	public int amount_asteroids;
	private double neg1;
	private double neg2;
	private double neg3;



	public void generateAsteroids() 
	{
			astr_count = 0;
			asteroids [0] = asteroid0;
			asteroids [1] = asteroid1;
			asteroids [2] = asteroid2;
			amount_asteroids = rnd.Next (200, 300);

			for (int i = 0; i < amount_asteroids; i++)
			{
				neg1 = rnd.NextDouble();
				neg2 = rnd.NextDouble();
				neg3 = rnd.NextDouble();
				Network.Instantiate(asteroids[astr_count%3],new Vector3(rnd.Next(0, 200)*(neg1 > 0.5 ? 1 : -1), rnd.Next(0,200)*(neg2 > 0.5 ? 1 : -1), rnd.Next(0, 200)*(neg3 > 0.5 ? 1 : -1)),Quaternion.Euler(rnd.Next(),rnd.Next(), rnd.Next()),0);
				astr_count++;
			}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
