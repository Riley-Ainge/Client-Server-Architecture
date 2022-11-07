using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsScreen : MonoBehaviour {
	public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void OnDisconnectButtonPressed()
	{
		player.GetComponent<PlayerManager> ().OnDisconnectPressed ();
	}
	public void OnOptionsButtonPressed()
	{
		player.GetComponent<Options> ().OnOptionsPressed ();
	}
}
