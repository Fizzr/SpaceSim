using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour 
{

	private string gemnem = "Fizzr_fighting";
	private bool refreshing;
	private bool isHostData;
	private HostData[] hostData;

	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;

	public GameObject playerPrefab;
	public GameObject GC;

	void Start()
	{
		btnX = Screen.width * 0.05f;
		btnY = Screen.width * 0.05f;
		btnW = Screen.width * 0.1f;
		btnH = Screen.width * 0.1f;
		refreshing = false;
		isHostData = false;

	}

	void startServer()
	{
		Network.InitializeServer (2, 25001, !Network.HavePublicAddress());
		MasterServer.RegisterHost (gemnem, "Test", "Getting shit to work");
		
	}

	void spawnPlayer()
	{
		Network.Instantiate (playerPrefab, new Vector3(0,0,0), Quaternion.identity, 0);
	}

	void OnServerInitialized ()
	{
		Debug.Log ("Server Initialized");
		spawnPlayer();
		GC.GetComponent<GameController> ().generateAsteroids ();
	}

	void OnConnectedToServer()
	{
		spawnPlayer ();
	}

	void OnMasterServerEvent (MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registered Server!");
		}
	}

	void refreshHostList()
	{
		MasterServer.RequestHostList(gemnem);
		refreshing = true;
		MasterServer.PollHostList();
	}

	void Update()
	{
		if (refreshing)
		{
			if (MasterServer.PollHostList().Length > 0)
			{
				refreshing = false;
				Debug.Log(MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
				isHostData = true;
			}
		}
	}

	void OnGUI()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button (new Rect (btnX, btnY, btnW, btnH), "Start Server"))
			{
				Debug.Log ("Starting server");
				startServer();
			}

			if(GUI.Button (new Rect (btnX, btnY*1.2f + btnH, btnW, btnH), "Refresh Hosts"))
			{
				Debug.Log ("Refreshing");
				refreshHostList();
			}
			if(isHostData)
			{
				for (int i = 0; i < hostData.Length; i++)
				{
					if(GUI.Button(new Rect(btnX*1.5f+btnW, btnY+(btnH*1.2f*i),btnW*3f, btnH*0.5f), hostData[i].gameName))
					{
						Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
}