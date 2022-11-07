using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class showothersposition : NetworkBehaviour {
	public RectTransform canvasRectT;
	public RectTransform healthBar;
	public Transform objectToFollow;
	public GameObject canvasprefab;
	public GameObject newcanvasprefab;

	public bool isteam1;
	public bool localplayeristeam1;
	public bool teamassigned = false;
	public GameObject localplayer;

	public GameObject eventPrefab;
	public RectTransform canvasRectTEvent;
	public RectTransform healthBarEvent;
	public Transform objectToFollowEvent;

	public GameObject CPPrefab;
	public RectTransform canvasRectTCP;
	public RectTransform healthBarCP;
	public Transform objectToFollowCP;
	private bool hasStarted = false;
	private bool isPlayingSiren = false;
	void Start()
	{
		StartCoroutine (WaitAtStart());
	}
	public IEnumerator WaitAtStart()
	{
		yield return new WaitForSeconds(0.2f);
		if (isLocalPlayer) {
			GameObject CPMarker = Instantiate (CPPrefab);

			CPMarker.transform.position = CPPrefab.transform.position;
			Cmd1 (CPMarker.transform.position);
			canvasRectTCP = CPMarker.GetComponent<RectTransform> ();
			foreach (Transform child in CPMarker.transform) {
				if (child.name == "Text") {
					healthBarCP = child.GetComponent<RectTransform> ();
				}
				objectToFollowCP = GameObject.FindGameObjectWithTag ("CpMarker").transform;
			}
		}
		StartCoroutine (callgetteam());
		foreach (string playerID in GameManager.players.Keys) {
			if (GameManager.players [playerID].isLocalPlayer) {
				localplayer = GameManager.players [playerID].gameObject;
			}
		}
		hasStarted = true;
	}
	[Command]
	void Cmd1(Vector3 pos)
	{
		print (pos);
	}
	void Update()
	{
		
		if (!hasStarted) {
			return;
		}
		if (!isLocalPlayer) {
			if (canvasRectT != null) {
				foreach (Transform child2 in localplayer.transform) {
					Vector3 screenPos = child2.GetComponent<Camera> ().WorldToScreenPoint (objectToFollow.position);
					foreach (Transform child in newcanvasprefab.transform) {
						if (screenPos.z < 0) {
							child.gameObject.GetComponent<Text> ().enabled = false;
						} else {
							child.gameObject.GetComponent<Text> ().enabled = true;
						}
					}

					Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint (child2.GetComponent<Camera> (), objectToFollow.position);
					healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
				}
			}




		} else {
			if (canvasRectTEvent != null) {
				foreach (Transform child2 in localplayer.transform) {
					Vector3 screenPosEvent = child2.GetComponent<Camera> ().WorldToScreenPoint (objectToFollowEvent.position);
					foreach (Transform child in canvasRectTEvent.gameObject.transform) {
						if (screenPosEvent.z < 0) {

							child.gameObject.GetComponent<Text> ().enabled = false;

						} else {

							child.gameObject.GetComponent<Text> ().enabled = true;
						}
					}

					Vector2 screenPointEvent = RectTransformUtility.WorldToScreenPoint (child2.GetComponent<Camera> (), objectToFollowEvent.position);
					healthBarEvent.anchoredPosition = screenPointEvent - canvasRectTEvent.sizeDelta / 2f;
				}
			}
			if (canvasRectTCP != null) {
				foreach (Transform child2 in localplayer.transform) {

					Vector3 screenPosEvent = child2.GetComponent<Camera> ().WorldToScreenPoint (objectToFollowCP.position);
					foreach (Transform child in canvasRectTCP.gameObject.transform) {
						if (child.name == "Text") {
							if (screenPosEvent.z < 0) {

								child.gameObject.GetComponent<Text> ().enabled = false;

							} else {

								child.gameObject.GetComponent<Text> ().enabled = true;
							}
						}

					}

					Vector2 screenPointEvent = RectTransformUtility.WorldToScreenPoint (child2.GetComponent<Camera> (), objectToFollowCP.position);
					healthBarCP.anchoredPosition = screenPointEvent - canvasRectTCP.sizeDelta / 2f;

				}
			}
		}
		if (isLocalPlayer) {
			if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().CPStutus > 0 && GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers > 0 || GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers > 0) {
				if (isteam1) {
					if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1capping) {
						healthBarCP.GetComponent<Text> ().color = Color.red;
					} else {
						healthBarCP.GetComponent<Text> ().color = Color.blue;
					}
					if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers - GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers > 0 && GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers - GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers != 0) {
						if (healthBarCP.GetComponent<Text> ().text != "Friendly Team Capturing Point") {
							StartCoroutine(PlayAlarm ());
						}
						healthBarCP.GetComponent<Text> ().text = "Friendly Team Capturing Point";
					} else {
						if(healthBarCP.GetComponent<Text> ().text != "Enemy Team Capturing Point")
						{
							StartCoroutine(PlayAlarm ());
						}
						healthBarCP.GetComponent<Text> ().text = "Enemy Team Capturing Point";
					}
				}

				if (!isteam1) {
					if (!GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1capping) {
						healthBarCP.GetComponent<Text> ().color = Color.blue;
					} else {
						healthBarCP.GetComponent<Text> ().color = Color.red;
					}
					if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers - GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers > 0 && GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers - GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers != 0) {
						if(healthBarCP.GetComponent<Text> ().text != "Friendly Team Capturing Point")
						{
							StartCoroutine(PlayAlarm ());
						}
						healthBarCP.GetComponent<Text> ().text = "Friendly Team Capturing Point";
					} else {
						if (healthBarCP.GetComponent<Text> ().text != "Enemy Team Capturing Point") {
							StartCoroutine(PlayAlarm ());
						}
						healthBarCP.GetComponent<Text> ().text = "Enemy Team Capturing Point";
					}
				}
			}
			if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers == 0 && GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers == 0) {
				healthBarCP.GetComponent<Text> ().text = "Capture";
			}
			if (GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers == GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team2cappingnumbers && GameObject.FindGameObjectWithTag ("CP").GetComponent<ServerCP> ().team1cappingnumbers > 0) {
				if (healthBarCP.GetComponent<Text> ().text != "Stalemate") {
					PlayAlarm ();
				}
				healthBarCP.GetComponent<Text> ().text = "Stalemate";
			}
		}
		if (GameObject.FindGameObjectWithTag ("NukeTimer").GetComponent<TheNuke> ().nukeCountdown >= 490) {
			healthBarCP.GetComponent<Text> ().enabled = false;
		}
	}
	public IEnumerator PlayAlarm()
	{
		if(!isPlayingSiren)
		{
			isPlayingSiren = true;
		GameObject.FindGameObjectWithTag ("CP").GetComponent<AudioSource> ().Play (0);
		yield return new WaitForSeconds (2f);
		GameObject.FindGameObjectWithTag ("CP").GetComponent<AudioSource> ().Play (0);
			isPlayingSiren = false;
		}
	}
	void GetTeam() {
		if (isLocalPlayer) {
			if ((localplayer.gameObject.GetComponent ("Team1") as Team1) != null) {
				localplayeristeam1 = true;	
			}
			if ((localplayer.gameObject.GetComponent ("Team2") as Team2) != null) {
				localplayeristeam1 = false;
			}
		} else {
			localplayeristeam1 = localplayer.GetComponent<showothersposition> ().isteam1;
		}
		if ((this.gameObject.GetComponent ("Team1") as Team1) != null) {
			isteam1 = true;
		} if((this.gameObject.GetComponent ("Team2") as Team2) != null) {
			isteam1 = false;
		}
	
	}
	public IEnumerator callgetteam()
	{
		while (true) {
			
			yield return new WaitForSeconds (0.01f);
			GetTeam ();
			if (!isLocalPlayer) {
				yield return new WaitForSeconds (2f);
				if (localplayeristeam1 && isteam1 || !localplayeristeam1 && !isteam1) {
					if (teamassigned == false) {
						teamassigned = true;
						newcanvasprefab = GameObject.Instantiate (canvasprefab);
						canvasRectT = newcanvasprefab.GetComponent<RectTransform> ();
						foreach (Transform child in canvasRectT.transform) {
							healthBar = child.GetComponent<RectTransform> ();



						}
					}
					foreach (Transform child in canvasRectT.transform) {
						child.GetComponent<Text> ().text = this.gameObject.GetComponent<TeamPick> ().playerName + "\n" + this.gameObject.GetComponent<PlayerManager>().currenthealth + "/100";
					}
				}
				if (!localplayeristeam1 && isteam1 || localplayeristeam1 && !isteam1) {
					
					if (teamassigned == true) {
						Destroy (canvasRectT.gameObject);

						teamassigned = false;
					}
				}
			}

		}
	}
	public IEnumerator deleteEvent(int seconds)
	{
		print ("1" + seconds);
		yield return new WaitForSeconds (seconds);
		print("2"+seconds);
		GameObject.Destroy (canvasRectTEvent.gameObject);
		canvasRectTEvent = null;
		healthBarEvent = null;
		objectToFollowEvent = null;
	}
}
