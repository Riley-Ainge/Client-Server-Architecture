using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerManager))]
public class Setup_Local_Player : NetworkBehaviour {
	public static string ID;
	[SerializeField]
	Behaviour[] DisableComponents;
	GameObject thecamera;

	[SerializeField]
	GameObject playerUIPrefab;
	private GameObject playerUIinstance;
	[SerializeField]
	string remoteLayerName = "RemotePlayer"; 
	float[] distances = new float[32];
	// Use this for initialization
	void Start () {
		thecamera =  GameObject.Find("Camera");
//		DontDestroyOnLoad (GameObject.Find("SpawnManager"));

		if (!isLocalPlayer) {
			AssignRemoteLayer ();
			DisableComponent ();


		} else 
		{
			

			Instantiate (playerUIPrefab);
			playerUIinstance = Instantiate (playerUIPrefab);
			playerUIinstance.name = playerUIPrefab.name;
			distances [11] = 70;
			foreach(Transform child in gameObject.transform)
			{
				child.GetComponent<Camera> ().layerCullDistances = distances;
			}
			}
		GetComponent<PlayerManager> ().Setup ();
	}

	public override void OnStartClient()
	{
		base.OnStartClient ();

		string netID = GetComponent<NetworkIdentity>().netId.ToString();
		PlayerManager player = GetComponent<PlayerManager> ();
		GameManager.RegisterPlayer (netID, player);
		StartCoroutine (waittostartteampick ());
	}
	public IEnumerator waittostartteampick()
	{
		yield return new WaitForSeconds (0.1f);
		this.gameObject.GetComponent<TeamPick> ().enabled = true;
	}
	// Update is called once per frame
	void Update () {
		
		}
	void OnDisable ()
	{
		Destroy (playerUIinstance);
		thecamera.gameObject.SetActive (true);
		GameManager.UnRegisterPlayer (transform.name);
	}
	void AssignRemoteLayer()
	{
		this.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
		this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		this.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		this.gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}
	void DisableComponent ()
	{
		for (int i = 0; i < DisableComponents.Length; i++) {

			DisableComponents [i].enabled = false;

		}


	}
		
}
