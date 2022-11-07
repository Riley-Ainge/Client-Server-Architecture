using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
public class PlayerManager : NetworkBehaviour
{
	public GameManager gameManager;
	private MatchSettings matchsettings;
	private WeaponGraphics weapongraphics;
	private Item_Database database;
	private Inventory inventory;
	public GUISkin skin;
	public GameObject lastplayertoshoot;
	public float respawntimeseconds;
	[SyncVar]
	private bool _isDead = false;
	public bool isDead  
	{
		get { return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;
	[SyncVar]
	public int currenthealth;

	[SerializeField]
	public Behaviour[] disableOnDeath;
	private bool[] wasEnabled;
	private bool firstSetup = true;
	public bool isInTrigger;
	[SyncVar]
	public GameObject bedObj;
	GameObject sun;
	public GameObject incomingAttack;
	public GameObject DeathScreen;
	public GameObject SpawnedDeathScreen;
	private GameManager gamemanager;
	public GameObject theNuke;
	private GameObject AmbientController;
	private GameObject Sun;
	public void Setup ()
	{
		Sun = GameObject.Find ("Sun");
		if (this.gameObject.GetComponent<NetworkIdentity> ().isServer) {
			GameObject.FindGameObjectWithTag ("NukeTimer").GetComponent<TheNuke> ()._isServer = true;
		}
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		matchsettings = GameObject.FindGameObjectWithTag ("MatchSettings").GetComponent<MatchSettings> ();
		inventory = this.gameObject.GetComponent<Inventory> ();
		CmdSetDefults ();
		if(isServer) {
			StartCoroutine (loopUpdatePlayerCount());
		}
	}
	IEnumerator loopUpdatePlayerCount()
	{
		while (true) {
			yield return new WaitForSeconds (10f);
			GameObject.Find ("Matchmaker").GetComponent<MatchUp.Matchmaker> ().SetMatchData ("Current Players", GameManager.players.Count.ToString());
		}
	}
	[Command]
	private void CmdSetDefults()
	{
		RpcSetDefuls ();
	}
	[ClientRpc]
	private void RpcSetDefuls()
	{
		if (firstSetup) {
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {
				wasEnabled [i] = disableOnDeath [i].enabled;
			}
			firstSetup = false;
		}
		SetDefults();
	}
	[ClientRpc]
	public void	RpcTakeDamage (int amount, GameObject player)
	{

			if (isDead) {
				return;
			}
		lastplayertoshoot = player;
		currenthealth -= amount;
		Debug.Log (transform.name + " now has " + currenthealth + " health.");
		if (currenthealth < 0) {
			currenthealth = 0;
		}
			if (currenthealth <= 0) {
			player.GetComponent<PlayerShoot> ().killcount += 1;
				Die ();
			print ("died");

		}
	}

	public void Die ()
	{
//		this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		isDead = true;
		if (this.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer) {
			GameObject screen = (GameObject)Instantiate (DeathScreen);
			foreach (Transform child in screen.transform) {
				if (child.name == "RawImage") {
					StartCoroutine (enlargeDeathScreen (child.gameObject));
				}
			}
			GetComponent<Inventory> ().Pickup ();
			if (GetComponent<Map> ().isinmap) {
				GetComponent<Map> ().closeMapAfterDeath ();
			}
		}
		inventory = this.gameObject.GetComponent<Inventory> ();
		for (int i = 0; i < 27; i++) {
			if (inventory.inventory [i].itemName != null) {
				CmdSpawn (inventory.inventory [i].itemID, i, this.gameObject);
				inventory.inventory [i] = new Items ();
			}
		}
		inventory.deequipitemwhennull ();
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;

		}
//		foreach (Transform child in transform) {
//			foreach (Transform child2 in child) {
//				if (child2 != null) {
//					child2.GetComponent<WeaponAnimator> ().enabled = false;
//				}
//			}
//		}
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = false;
		}
		Debug.Log (transform.name + " died");

		if (isInTrigger) {
			
			CmdExitthetrigger (this.gameObject);
		}
		foreach (Transform child in transform) {
			child.GetComponent<Camera> ().fieldOfView = 60;
			foreach(Transform child2 in child)
			{
				if (child2.tag == "PickupHands") {
					Destroy (child2.gameObject);
				}
			}
		}
		GetComponent<Rigidbody> ().isKinematic = false;
		GetComponent<CharacterController>().enabled = false;
		this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x - 10f, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
		if (this.transform.parent != null) {
			if (this.transform.parent.tag == "Seat") {
				this.gameObject.transform.localPosition = this.transform.parent.transform.forward * 2f;
				this.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_UseHeadBob = true;
				this.gameObject.GetComponent<VehicleGetIn> ().vehiclein = null;
				this.transform.parent.parent.GetComponent<VehicleControl> ().isDriving = false;
				this.gameObject.GetComponent<CapsuleCollider> ().enabled = true;
				this.gameObject.GetComponent<VehicleGetIn> ().CmdSetVehicleParent (this.gameObject);
				this.gameObject.GetComponent<VehicleGetIn>().isInThirdPerson = true;
			}
		}
		//CALL RESPAWN Method
		StartCoroutine (Respawn());
	}
	public IEnumerator Respawn ()
	{
		matchsettings = GameObject.FindGameObjectWithTag ("MatchSettings").GetComponent<MatchSettings> ();
		StartCoroutine (CountDownRespawn ());
		for (int i = 0; i < matchsettings.respawnTime; i++) {
			yield return new WaitForSeconds (1f);
		}
		if (isLocalPlayer) {
			matchsettings.respawnTime = 30f;
			Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
			if (bedObj != null) {
				spawnPoint = bedObj.transform;
				spawnPoint.transform.position = new Vector3 (spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z + 1);
			}
			transform.position = spawnPoint.position;
			transform.rotation = spawnPoint.rotation;
		}
		yield return new WaitForSeconds (0.5f);
		CmdSetDefults ();
	}
	public void SetDefults ()
	{
		isDead = false;
		if (GameObject.FindGameObjectWithTag ("CPUI") != null) {
			GameObject.FindGameObjectWithTag ("CPUI").GetComponent<Canvas> ().enabled = true;
		}
		Destroy (SpawnedDeathScreen);	
		currenthealth = maxHealth;
		if (isLocalPlayer) {
			for (int i = 0; i < disableOnDeath.Length; i++) {
				disableOnDeath [i].enabled = wasEnabled [i];	
				foreach (Transform child in transform) {
					foreach (Transform child2 in child) {
						if(child2.tag != "PickupHands")
						child2.GetComponent<WeaponAnimator> ().enabled = false;
					}
				}
			}
			GetComponent<CharacterController> ().enabled = true;
			GetComponent<Rigidbody> ().isKinematic = true;
			Collider _col = GetComponent<Collider> ();
			if (_col != null) {
				_col.enabled = true;
			}
		}
		StartCoroutine (WaitForRedo ());
	}
	[Command]
	void CmdExitthetrigger(GameObject player)
	{
		GameObject.FindGameObjectWithTag ("CP").GetComponent<CP> ().hasExited = true;
		GameObject.FindGameObjectWithTag ("CP").GetComponent<CP> ().ExitTrigger (player.gameObject.GetComponent<Collider> ());
	}
	void Update()
	{
		if (isServer) {
			if (gameManager.roundrestarting == true) {
				CmdKillAllPlayers (this.gameObject);
			}
		}
		if(Input.GetKeyDown(KeyCode.K))
		{
			RpcTakeDamage (20, this.gameObject);
		}
	}
	[Command]
	void CmdKillAllPlayers(GameObject player)
	{
		if (gameManager.roundrestarting == true) {
			CmdGiveClientAuthority (this.gameObject.GetComponent<NetworkIdentity> (), gameManager.gameObject);
			gameManager.CmdCleanupItems ();
		
		}
	}
	[Command]
	public void CmdSpawn(int databaseid, int i, GameObject chest)
	{ 
		database = GameObject.FindGameObjectWithTag ("ItemDatabase").GetComponent<Item_Database> ();
		GameObject droppedobject = Instantiate (database.items[databaseid].worldmodel, new Vector3 (chest.transform.position.x, chest.transform.position.y + 1 + (i * 0.5f), chest.transform.position.z), Quaternion.identity, GameObject.FindGameObjectWithTag("SpawnedItems").transform);
		NetworkServer.Spawn (droppedobject);
	}

	[Command]
	public void CmdGiveClientAuthority(NetworkIdentity playerID, GameObject obj)
	{
		print (this.gameObject);
		obj.GetComponent<NetworkIdentity> ().AssignClientAuthority (playerID.connectionToClient);
	}
	public void KillPlayer(GameObject player, int time)
	{
		matchsettings.respawnTime = time;
		print (this.gameObject);
		CmdKillPlayer(player, time);
	}
	[Command]
	public void CmdKillPlayer(GameObject player, int time)
	{
		print (this.gameObject);
		matchsettings.respawnTime = time;
		player.GetComponent<PlayerManager>().RpcTakeDamage (1000, this.gameObject);	
	}

	[Command]
	public void CmdDamagePlayer(GameObject player, int damage, GameObject attacker)
	{
		player.GetComponent<PlayerManager> ().RpcTakeDamage (damage, attacker);
	}
	IEnumerator enlargeDeathScreen(GameObject obj)
	{
		GameObject.FindGameObjectWithTag ("CPUI").GetComponent<Canvas> ().enabled = false;
		for (int i = 0; i < 5; i++) {
			obj.transform.localScale = new Vector3 (obj.transform.localScale.x+0.03f, obj.transform.localScale.y+0.03f, obj.transform.localScale.z);
			yield return new WaitForSeconds (0.01f);
		}
		foreach (Transform child in obj.transform.parent) {
			if (child.name != "RawImage" && child.name != "BlueWins" && child.name != "RedWins" && child.name != "WinBack") {
				child.gameObject.GetComponent<Text>().enabled = true;
			}
		}
		SpawnedDeathScreen = obj.transform.parent.gameObject;
	}
	public void OnDisconnectPressed()
	{
		CmdDisconnectPlayer ();
		if (isServer) {
			GameObject.Find ("Matchmaker").GetComponent<MatchUp.Matchmaker> ().DestroyMatch ();
			GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ().StopHost ();
		} else {
			GameObject.Find ("Matchmaker").GetComponent<MatchUp.Matchmaker> ().LeaveMatch ();
			GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ().StopClient ();
		}
		CmdDisconnectPlayer ();
	}
	[Command]
	void CmdDisconnectPlayer()
	{
		RpcDisconnectPlayer ();
	}
	[ClientRpc]
	void RpcDisconnectPlayer()
	{
		GameManager.UnRegisterPlayer (this.gameObject.name);
	}
	void OnGUI()
	{
		if (isLocalPlayer) {	
			if (isDead) {
				if (SpawnedDeathScreen != null) {
					foreach (Transform child in SpawnedDeathScreen.transform) {
						if (child.name == "Kills") {
							child.GetComponent<Text> ().text = this.GetComponent<PlayerShoot> ().killcount.ToString ();
						}
						if (child.name == "KilledBy") {
							child.GetComponent<Text> ().text = lastplayertoshoot.GetComponent<TeamPick> ().GetName [lastplayertoshoot.name];
						}
						if (child.name == "RespawnTime") {
							if (theNuke.GetComponent<TheNuke> ().nukeCountdown > 490) {
								child.GetComponent<Text> ().text = theNuke.GetComponent<TheNuke> ().nukeCountdownSeconds10.ToString ();
							} else {
								child.GetComponent<Text> ().text = respawntimeseconds.ToString ();
							}
						}
						if (child.name == "RoundNo") {
							child.GetComponent<Text> ().text = gameManager.round.ToString ();
						}
						if (child.name == "RoundInfo") {
							if (theNuke.GetComponent<TheNuke> ().nukeCountdown > 490) {
								child.GetComponent<Text> ().color = Color.red;
								child.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().endRoundResults;
							} else {
								child.GetComponent<Text> ().color = Color.black;
								if (gameManager.round == 1) {
									child.GetComponent<Text> ().text = "Low Tier Loot Abundance";
								}
								if (gameManager.round == 2) {
									child.GetComponent<Text> ().text = "Medium Tier Loot Abundance";
								}
								if (gameManager.round == 3) {
									child.GetComponent<Text> ().text = "High Tier Loot Abundance";
								}
							}
						}
						if (child.name == "BlueWins") {
							if (theNuke.GetComponent<TheNuke> ().nukeCountdown > 490) {
								child.gameObject.GetComponent<Image>().enabled = true;
								foreach (Transform child2 in child) {
									child2.gameObject.GetComponent<Text>().enabled = true;
									child2.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2RoundsWon.ToString ();
								}
							}
						}
						if (child.name == "RedWins") {
							if (theNuke.GetComponent<TheNuke> ().nukeCountdown > 490) {
								child.gameObject.GetComponent<Image>().enabled = true;
								foreach (Transform child2 in child) {
									child2.gameObject.GetComponent<Text>().enabled = true;
									child2.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1RoundsWon.ToString ();
								}
							}
						}
						if (child.name == "WinBack") {
							if (theNuke.GetComponent<TheNuke> ().nukeCountdown > 490) {
								child.gameObject.GetComponent<Image>().enabled = true;
							}
						}
					}
				}
//				//470/ 960
//				float rectx5 = 0.489583333f * Screen.width;
//				//70 / 540
//				float recty5 = 0.12962963f * Screen.height;
//				//200/540
//				float scalerecty5 = 0.37037037f * Screen.height;
//				//500/960
//				float scalerectx5 = 0.520833333f * Screen.width;
//				GUI.Box (new Rect (rectx5, recty5, scalerectx5, scalerecty5 ), "You Were Killed By: " + lastplayertoshoot.GetComponent<TeamPick>().GetName[lastplayertoshoot.name], skin.GetStyle ("SmallMatchText"));
//
//				//600/ 960
//				float rectx4 = 0.625f * Screen.width;
//				//30 / 540
//				float recty4 = 0.0555555556f * Screen.height;
//				//200/540
//				float scalerecty4 = 0.37037037f * Screen.height;
//				//500/960
//				float scalerectx4 = 0.520833333f * Screen.width;
//				GUI.Box (new Rect (rectx4, recty4, scalerectx4, scalerecty4), "Kills: " + this.GetComponent<PlayerShoot> ().killcount.ToString (), skin.GetStyle ("SmallMatchText"));
//
//				if (matchsettings.respawnTime > 15) {
//					//50 / 960
//					float rectx3 = 0.0520833333f * Screen.width;
//					//75 / 540
//					float recty3 = 0.138888889f * Screen.height;
//					//200/540
//					float scalerecty3 = 0.37037037f * Screen.height;
//					//500/960
//					float scalerectx3 = 0.520833333f * Screen.width;
//
//
//					GUI.Box (new Rect (rectx3, recty3, scalerectx3, scalerecty3), respawntimeseconds.ToString (), skin.GetStyle ("GameCountdown"));
//					//35/ 960
//					float rectx2 = 0.0364583333f * Screen.width;
//					//15 / 540
//					float recty2 = 0.0277777778f * Screen.height;
//					//200/540
//					float scalerecty2 = 0.37037037f * Screen.height;
//					//500/960
//					float scalerectx2 = 0.520833333f * Screen.width;
//					GUI.Box (new Rect (rectx2, recty2, scalerectx2, scalerecty2), "Respawning In: ", skin.GetStyle ("SmallMatchText"));
//				}
			}
			//10/ 960
			float rectx1 = 0.01041666666f * Screen.width;
			//5 / 540
			float recty1 = 0.00925925925f * Screen.height;
			//20/540
			float scalerecty1 = 0.03703703703f * Screen.height;
			//20/960
			float scalerectx1 = 0.04166666666f * Screen.width;
			GUI.Box (new Rect (0, recty1, scalerectx1, scalerecty1), NetworkManager.singleton.client.GetRTT() + "ms", skin.GetStyle ("smallgreen"));
//			//450 / 960
//			float rect7x = 0.9f * Screen.width;
//			// 20 / 540
//			float rect7y = 0f * Screen.height;
//			//25/540
//			float scalerect7y = 0.0462962963f * Screen.height;
//			//50/960
//			float scalerect7x = 0.0520833333f * Screen.width;
//			int i = 0;
//			while (i < Network.connections.Length) {
//				
//				GUI.Box (new Rect (rect7x, rect7y, scalerect7x, scalerect7y), Network.GetAveragePing (Network.connections[i]).ToString (), skin.GetStyle ("SmallMatchText"));
//				print (Network.connections[i]); 
//
//			}
		}
	}
	public IEnumerator CountDownRespawn()
	{
		matchsettings = GameObject.FindGameObjectWithTag ("MatchSettings").GetComponent<MatchSettings> ();
		respawntimeseconds = matchsettings.respawnTime;
		if (matchsettings.respawnTime > 15f) {
			for (int i = 0; i < matchsettings.respawnTime; i++) {
				yield return new WaitForSeconds (1f);
				respawntimeseconds -= 1;
			}

		}
		respawntimeseconds = 0f;

	}
	[Command]
	public void Cmdoncollisison(GameObject crashplane, GameObject orgplane)
	{
		print (crashplane);
		print (orgplane);
		print ("done");
		GameObject crashedplane = GameObject.Instantiate (crashplane, orgplane.gameObject.transform.position, orgplane.gameObject.transform.rotation);
		NetworkServer.Spawn (crashedplane);
		GameObject.FindGameObjectWithTag ("SpawnManager").GetComponent<SpawnManager> ().CmdSpawnItemsForAC130 (crashedplane);
		RpconPlaneCollision (crashedplane);

		NetworkServer.Destroy (orgplane.gameObject);	   
	}
	[ClientRpc]
	public void RpconPlaneCollision(GameObject plane)
	{
		this.GetComponent<showothersposition> ().objectToFollowEvent = plane.transform;
		StartCoroutine(this.GetComponent<showothersposition> ().deleteEvent (15));
	}
	[Command]
	public void CmdSpawnItem(GameObject obj)
	{
		NetworkServer.Spawn(obj);
	}
	IEnumerator WaitForRedo()
	{
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < GameObject.FindGameObjectsWithTag("EnableZone").Length; i++) {
			GameObject.FindGameObjectsWithTag ("EnableZone") [i].GetComponent<Collider> ().enabled = false;
		}
		for (int i = 0; i < GameObject.FindGameObjectsWithTag("EnableZone").Length; i++) {
			GameObject.FindGameObjectsWithTag ("EnableZone") [i].GetComponent<Collider> ().enabled = true;
		}
	}
	[Command]
	public void Cmdteamtakeoradd(bool isTeam1, bool plusorminus, GameObject CP)
	{
		RpcChangeTrigger (plusorminus);
		CP CPComp = CP.GetComponent<CP> ();
		if (plusorminus == true) {
			if (isTeam1 == true) {
				CPComp.team1cappingnumbers = CPComp.team1cappingnumbers + 1;
			}
			if (isTeam1 == false) {
				CPComp.team2cappingnumbers = CPComp.team2cappingnumbers + 1;
			}
		}
		if (plusorminus == false) {
			if (isTeam1 == true) {
				CPComp.team1cappingnumbers = CPComp.team1cappingnumbers - 1;
			}
			if (isTeam1 == false) {	
				CPComp.team2cappingnumbers = CPComp.team2cappingnumbers - 1;
			}
		}
	}
	[ClientRpc]
	void RpcChangeTrigger(bool istrigger)
	{
		print ("changed");
		isInTrigger = istrigger;
	}
	[Command]
	public void CmdRefreshChest(int[] gunstats1, int[] ammostats1, string[] inventarray1, NetworkIdentity playerID, GameObject chest)
	{
		chest.GetComponent<Chest>().RpcRefreshChest (gunstats1, ammostats1, inventarray1, playerID);
	}
	void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		theNuke = GameObject.Find ("NukeCountDown");
		if (isLocalPlayer) {
			AmbientController = GameObject.Find ("AmbientController");
			StartCoroutine (checkCeiling ());
		}
		if (isServer) {
			StartCoroutine (loopUpdateSun ());
			int ran = Random.Range (0, 4);
			if (ran == 0) {
				StartCoroutine (waitForSetFog (0));
				GameObject.Find("Matchmaker").GetComponent<MatchUp.Matchmaker>().SetMatchData("Weather", "Foggy");
			}
			if (ran == 1) {
				GameObject.Find("Matchmaker").GetComponent<MatchUp.Matchmaker>().SetMatchData("Weather", "Very Foggy");
				StartCoroutine (waitForSetFog (1));
			}
			if (ran > 1) {
				GameObject.Find("Matchmaker").GetComponent<MatchUp.Matchmaker>().SetMatchData("Weather", "Clear");
			}
		}

	}
	IEnumerator loopUpdateSun()
	{
		while (true) {
			yield return new WaitForSeconds (0.1f);
			RpcUpdateSun (Sun.transform.rotation);
		}
	}
	IEnumerator waitForSetFog(int num)
	{
		while (true) {
			yield return new WaitForSeconds (1f);
			RpcSetFog (num);
			if (GameObject.Find ("NukeCountDown").GetComponent<TheNuke> ().nukeCountdown < 490) {
				break;
			}
		}
	}
	[ClientRpc]
	void RpcUpdateSun(Quaternion rotation)
	{
		foreach (Transform child in GameObject.Find("SunObj").transform) {
			child.gameObject.SetActive (true);
		}
		Sun.transform.rotation = rotation;
	}
	[ClientRpc]
	void RpcSetFog(int change)
	{
		print (change);
		if(change == 0)
		{
			RenderSettings.fog = true;
		}
		if(change == 1)
		{
			RenderSettings.fog = true;
			RenderSettings.fogDensity = 0.01f;
		}
	}
	IEnumerator checkCeiling()
	{
		bool turnDown = true; 
		float vol = 0;
		int num = 0;
		while (true) {
			yield return new WaitForSeconds (1f);
			num += 1;
			if (num == 20) {
				int ran = Random.Range (-1, 2);
				AmbientController.GetComponents<AudioSource> () [0].pitch = 1+(ran*0.05f/2f);
				AmbientController.GetComponents<AudioSource> ()[1].pitch = 1+(ran*0.05f/2f);
				yield return new WaitForSeconds (0.5f);
				AmbientController.GetComponents<AudioSource> () [0].pitch = 1+(ran*0.05f);
				AmbientController.GetComponents<AudioSource> ()[1].pitch = 1+(ran*0.05f); 
				num = 0;
			}
			RaycastHit hit;
			if (Physics.Raycast (transform.position, this.transform.up, out hit, 50, GetComponent<PlayerShoot> ().mask)) {
				if (hit.collider.gameObject.tag != "Terrain") {
					if (turnDown == true) {
						vol = AmbientController.GetComponents<AudioSource> ()[0].volume;
						for (int i = 0; i < 10; i++) {
							yield return new WaitForSeconds (0.1f);
							AmbientController.GetComponents<AudioSource> ()[0].volume -= (vol/10);
							AmbientController.GetComponents<AudioSource> ()[1].volume += ((vol-0.33f)/10);
						}
						turnDown = false;
					}
				} else {
					if (turnDown == false) {
						for (int i = 0; i < 10; i++) {
							yield return new WaitForSeconds (0.1f);
							AmbientController.GetComponents<AudioSource> ()[0].volume += (vol/10);
							AmbientController.GetComponents<AudioSource> ()[1].volume -= ((vol-0.33f)/10);
						}
						turnDown = true;
					}
				}
			} else {
				if (turnDown == false) {
					for (int i = 0; i < 10; i++) {
						yield return new WaitForSeconds (0.1f);
						AmbientController.GetComponents<AudioSource> ()[0].volume += (vol/10);
						AmbientController.GetComponents<AudioSource> ()[1].volume -= ((vol-0.33f)/10);
					}
					turnDown = true;
				}
			}
		}
	}
	[ClientRpc]
	public void RpcSetChild(GameObject parent, GameObject child)
	{
		child.transform.parent = parent.transform;
	}
	[ClientRpc]
	public void RpcCreateFlash()
	{
		GameObject uiObj = Instantiate (incomingAttack);
		StartCoroutine (FlashIncomingAttack (uiObj));
	}
	IEnumerator FlashIncomingAttack(GameObject obj)
	{
		bool isEnabled = true;
		for (int i = 0; i < 42	; i++) {
			yield return new WaitForSeconds (0.5f);
			obj.SetActive (isEnabled);
			isEnabled = !isEnabled;
		}
		Destroy (obj);
	}
}