using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	//GUI Element
	private GUIText Fuel_text;
	private GUIText Velocity;
	private GUIText Rotation;
	private GUIText Throttle_text;
	private GUIText Health_Points;

	// Game Objects
	public AudioSource bang;
	public AudioSource engine_sound;
	public AudioSource dir_thrust_sound;
	public AudioSource damage_sound;
	public ParticleSystem engine_particle;
	public GameObject bullet;
	private GameObject clone;

	//Variables
	private float pos_neg;
	private float temp_rot;
	public float fire_rate;
	private float last_fire;
	private float fuel_amount;
	private float throttle;
	private Vector3 input_1;
	private Vector3 input_2;
	private Vector3 empty;
	public float multiplier;
	private bool thrust1;
	private bool thrust2;
	private bool thrustC;
	private int HP;


	// Use this for initialization
	void Start () 
	{
		Fuel_text = GameObject.Find ("HUD GUI/Fuel Text").GetComponent<GUIText>();
		Velocity = GameObject.Find ("HUD GUI/Velocity Text").GetComponent<GUIText>();
		Rotation = GameObject.Find ("HUD GUI/Rotation Text").GetComponent<GUIText>();
		Throttle_text = GameObject.Find ("HUD GUI/Throttle Text").GetComponent<GUIText>();
		Health_Points = GameObject.Find ("HUD GUI/HP Text").GetComponent<GUIText>();
		throttle = 0.0f;
		fuel_amount = 100;
		HP = 100;
		empty.Set (0.0f, 0.0f, 0.0f);
		multiplier = 0.005f;
		thrust1 = false;
		thrust2 = false;
		thrustC = false;
		engine_sound.Play ();

		if (!GetComponent<NetworkView>().isMine)
		{	
			GetComponentInChildren<Camera>().enabled = false; // disable the camera of the non-owned Player;
			GetComponentInChildren<ParticleSystem>().emissionRate = 0;
			GetComponentInChildren<AudioListener>().enabled = false; 
		}
	}

	void OnTriggerEnter( Collider other)
	{
		Destroy (other.gameObject);
		HP = HP - 25;
		damage_sound.Play ();
	}
	
	void Update()
	{
		//Set GUI text
		Fuel_text.text = fuel_amount.ToString () + "%";
		Velocity.text = GetComponent<Rigidbody>().velocity.ToString();
		Rotation.text = GetComponent<Rigidbody>().angularVelocity.ToString();
		Throttle_text.text = throttle.ToString();
		Health_Points.text = HP.ToString();

		//Firing!
		if (Input.GetAxis("Fire1") != 0 && HP > 0 && last_fire+fire_rate < Time.time)
		{	
			last_fire = Time.time;
			bang.Play();
			clone = Network.Instantiate(bullet, gameObject.transform.TransformPoint(0,0,0.2f) , gameObject.transform.rotation, 0) as GameObject;
			clone.GetComponent<Rigidbody>().velocity = gameObject.GetComponent<Rigidbody>().velocity;
			clone.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,1000));
		}
	}

	void FixedUpdate () 
	{
		if (fuel_amount > 0 && HP > 0)
		{
			//adds force or torque when appropriate key is pressed
			input_1.Set (Input.GetAxis ("Rotate Vertical") * 0.3f, Input.GetAxis ("Rotate Horizontal") * 0.3f, Input.GetAxis ("Barrel Roll") * 0.3f);
			input_2.Set (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), Input.GetAxis ("Back") * 2f);
			if (input_1 != empty)
			{
				fuel_amount = fuel_amount - ((Mathf.Abs(input_1.y)+Mathf.Abs(input_1.x)+Mathf.Abs(input_1.z))*multiplier);
				GetComponent<Rigidbody>().AddRelativeTorque(input_1);
				thrust1 = true;
			}
			else
			{
				thrust1 = false;
			}
			if (input_2 != empty)
			{
				fuel_amount = fuel_amount - ((Mathf.Abs(input_2.y)+Mathf.Abs(input_2.x)+Mathf.Abs(input_2.z))*multiplier);
				GetComponent<Rigidbody>().AddRelativeForce(input_2);
				thrust2 = true;
			}
			else
			{
				thrust2 = false; 
			}
			// Set throttle value according to input. Limit at max 5 min 0.
			if (Input.GetAxis("Throttle") != 0)
			{
				if (throttle < 5.1f)
				{
					if (throttle >= 0.0f)
					{
						throttle = throttle + Input.GetAxis("Throttle")*0.3f;
					}
					else
					{
						throttle = 0.0f;
					}
				}
				else
				{
					throttle = 5.0f;
				}
			}
			// do throtteling and effects!
			if (throttle > 0)
			{
				GetComponent<Rigidbody>().AddRelativeForce(0,0,throttle);
				fuel_amount = fuel_amount - (throttle * multiplier);
				engine_particle.enableEmission = true;
				engine_particle.startSpeed = throttle;
				engine_sound.mute = false;
				if (throttle > 1)
				{
					engine_sound.pitch = throttle*0.75f;
				}
			}
			else
			{
				engine_particle.enableEmission = false;
				engine_sound.mute = true;
			}

			// Gradually stops rotation when pressing C key
			if (Input.GetKey ("c")) 
			{
				temp_rot = GetComponent<Rigidbody>().angularVelocity[0];
				thrustC = true;
				if ( temp_rot != 0)
				{
					pos_neg = temp_rot < 0 ? 0.3f : -0.3f;
					GetComponent<Rigidbody>().AddTorque((pos_neg),0,0);
					fuel_amount = fuel_amount - Mathf.Abs(pos_neg)*multiplier;
				}
				temp_rot = GetComponent<Rigidbody>().angularVelocity[1];
				if ( temp_rot != 0)
				{
					pos_neg = temp_rot < 0 ? 0.3f : -0.3f;
					GetComponent<Rigidbody>().AddTorque(0,(pos_neg),0);
					fuel_amount = fuel_amount - Mathf.Abs(pos_neg)*multiplier;
				}
				temp_rot = GetComponent<Rigidbody>().angularVelocity[2];
				if ( temp_rot != 0)
				{
					pos_neg = temp_rot < 0 ? 0.3f : -0.3f;
					GetComponent<Rigidbody>().AddTorque(0,0,(pos_neg));
					fuel_amount = fuel_amount - Mathf.Abs(pos_neg)*multiplier;
				}
			}
			else
			{
				thrustC = false;
			}
			if (thrust1 | thrust2 | thrustC)
			{
				dir_thrust_sound.mute = false;
			}
			else
			{
				dir_thrust_sound.mute = true;
			}
		}
		else  //If fuel is out!
		{
			Fuel_text.text = "0%"; 
			engine_sound.mute = true;
			dir_thrust_sound.mute = true;
			engine_particle.enableEmission = false; 
		}
	}	
}
