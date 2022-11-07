using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour {
	public static GameManager instance;
	[SyncVar]
	public int round = 1;
	public MatchSettings matchSettings;
	[SyncVar]
	public bool roundrestarting = false;
	public GameObject spawnManager;
	public static List<string> maps = new List<string>() {"Scene_Map_2"};
	void Awake ()
	{
		matchSettings = GameObject.FindGameObjectWithTag ("MatchSettings").GetComponent<MatchSettings> ();
		if (instance != null) {
//			Debug.LogError ("More than one GameManager in scene");
		} else {
			instance = this;
		}
		}







	private const string PLAYER_ID_PREFIX = "Player ";
	public static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();
	void Start ()
	{
		
		NetworkServer.SpawnObjects ();
	}
	public static void RegisterPlayer(string netID, PlayerManager player)
	{
		
		string playerID = PLAYER_ID_PREFIX + netID;
		players.Add (playerID, player);
		player.transform.name = playerID;
	}
	public static void UnRegisterPlayer (string playerID)
	{
		players.Remove (playerID);
	}
	public static PlayerManager GetPlayer (string playerID)
	{
		return players [playerID];
	}
	void OnGUI ()
	{
		GUILayout.BeginArea (new Rect (200, 200, 200, 500));
		GUILayout.BeginVertical ();

		foreach (string playerID in players.Keys) {
			GUILayout.Label (players[playerID].transform.name);
		}
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
}
	[Command]
	public void CmdCleanupItems()
	{
		roundrestarting = false;
		StartCoroutine (waitasecond ());

	}
	public IEnumerator waitasecond()
	{
		yield return new WaitForSeconds (15f);
		foreach (Transform child in GameObject.FindGameObjectWithTag("SpawnedItems").transform) {
			NetworkServer.Destroy (child.gameObject);
			Destroy (child.gameObject);


		}
		spawnManager.GetComponent<SpawnManager> ().Cmdspawnstuff ();
	}
	public void LoadNextMap(int mapNum) {
			NetworkManager.singleton.ServerChangeScene (maps[mapNum]);
            GameObject.Find("Matchmaker").GetComponent<MatchUp.Matchmaker>().SetMatchData("Visable", "True");
    }
}
