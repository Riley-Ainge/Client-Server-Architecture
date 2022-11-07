using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
public class Options : MonoBehaviour {
	public bool LocalPlayer;
	public int OcclusionCulling;
	public int AmbientOcclusion;
	public int ColourGrading;
	public int Bloom;
	public int MaxTrees;
	public int MotionBlur;
	public int EyeAdaption;
	public int Vignetting;
	public int DetailLevel;
	public int ShadowQuality;
	public bool OpenSettings;
	public bool HasSaved = true;
	public PostProcessingProfile prof;
	private bool CanClick = true;
	public Terrain Terrain;
	public GameObject EscScreen;
	public GameObject spawnedEscScreen;
	// Use this for initialization
	public void OnOptionsPressed()
	{
		OpenSettings = true;
		OcclusionCulling = PlayerPrefs.GetInt ("OcclusionCulling");
		AmbientOcclusion = PlayerPrefs.GetInt ("AmbientOcclusion");
		ColourGrading = PlayerPrefs.GetInt ("ColourGrading");
		Bloom = PlayerPrefs.GetInt ("Bloom");
		MaxTrees = PlayerPrefs.GetInt ("MaxTrees");
		MotionBlur = PlayerPrefs.GetInt ("MotionBlur");
		EyeAdaption = PlayerPrefs.GetInt ("EyeAdaption");
		Vignetting = PlayerPrefs.GetInt ("Vignetting");
		DetailLevel = PlayerPrefs.GetInt ("DetailLevel");
		ShadowQuality = PlayerPrefs.GetInt ("ShadowQuality");
		HasSaved = false;
	}
	void Start () {
		OcclusionCulling = PlayerPrefs.GetInt ("OcclusionCulling");
		AmbientOcclusion = PlayerPrefs.GetInt ("AmbientOcclusion");
		ColourGrading = PlayerPrefs.GetInt ("ColourGrading");
		Bloom = PlayerPrefs.GetInt ("Bloom");
		MaxTrees = PlayerPrefs.GetInt ("MaxTrees");
		MotionBlur = PlayerPrefs.GetInt ("MotionBlur");
		EyeAdaption = PlayerPrefs.GetInt ("EyeAdaption");
		Vignetting = PlayerPrefs.GetInt ("Vignetting");
		DetailLevel = PlayerPrefs.GetInt ("DetailLevel");
		prof.depthOfField.enabled = false;
		if (QualitySettings.shadowResolution == ShadowResolution.Low) {
			ShadowQuality = 1;
		}
		if (QualitySettings.shadowResolution == ShadowResolution.Medium) {
			ShadowQuality = 2;
		}
		if (QualitySettings.shadowResolution == ShadowResolution.High) {
			ShadowQuality = 3;
		}
		if (QualitySettings.shadowResolution == ShadowResolution.VeryHigh) {
			ShadowQuality = 4;
		}
		Terrain = GameObject.FindGameObjectWithTag ("Terrain").GetComponent<Terrain> ();
	}
		
	void OnGUI () {
		if (this.gameObject.GetComponent<Inventory> ().inesc) {
			if (spawnedEscScreen == null) {
				GameObject screen = GameObject.Instantiate(EscScreen);
				screen.GetComponent<OptionsScreen> ().player = this.gameObject;
				spawnedEscScreen = screen;

			}
			//60 / 960
//			float slotrectx = 0.0625f * Screen.width;
//			// 110 / 540
//			float slotrecty = 0.2037037037f * Screen.height;
//			// 120/960
//			float slotrectplus1 = 0.125f * Screen.width;
//			// 40 / 540
//			float slotrectplus2 = 0.07407407407f * Screen.height;
//			Rect OptionRect = new Rect (slotrectx, slotrecty, slotrectplus1, slotrectplus2);
//			GUI.Box (OptionRect, "Options");
//			if (OptionRect.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
				
//load
				
//			}
		} else {
			if (!HasSaved) {
//save
				PlayerPrefs.SetInt ("OcclusionCulling", OcclusionCulling);
				PlayerPrefs.SetInt ("AmbientOcclusion", AmbientOcclusion);
				PlayerPrefs.SetInt ("ColourGrading", ColourGrading);
				PlayerPrefs.SetInt ("Bloom", Bloom);
				PlayerPrefs.SetInt ("MaxTrees", MaxTrees);
				PlayerPrefs.SetInt ("MotionBlur", MotionBlur);
				PlayerPrefs.SetInt ("EyeAdaption", EyeAdaption);
				PlayerPrefs.SetInt ("Vignetting", Vignetting);
				PlayerPrefs.SetInt ("DetailLevel", DetailLevel);
				HasSaved = true;
			}
			OpenSettings = false;
			if (spawnedEscScreen != null) {
				Destroy (spawnedEscScreen);
			}
		}
		if (OpenSettings) {
//open options			
			print ("opened");
			//210 / 960
			float slotrectx = 0.21875f * Screen.width;
			// 70 / 540
			float slotrecty = 0.12962962963f * Screen.height;
			// 140/960
			float slotrectplus1 = 0.14583333333f * Screen.width;
			// 40 / 540
			float slotrectplus2 = 0.07407407407f * Screen.height;
			Rect OcclusionCullingR = new Rect (slotrectx, slotrecty * 1f, slotrectplus1, slotrectplus2);
			Rect AmbientOcclusionR = new Rect (slotrectx, slotrecty * 1.6f, slotrectplus1, slotrectplus2);
			Rect ColourGradingR = new Rect (slotrectx, slotrecty * 2.2f, slotrectplus1, slotrectplus2);
			Rect BloomR = new Rect (slotrectx, slotrecty * 2.8f, slotrectplus1, slotrectplus2);
			Rect MaxTreesR = new Rect (slotrectx, slotrecty * 3.4f, slotrectplus1, slotrectplus2);
			Rect MotionBlurR = new Rect (slotrectx, slotrecty * 4f, slotrectplus1, slotrectplus2);
			Rect EyeAdaptionR = new Rect (slotrectx, slotrecty * 4.6f, slotrectplus1, slotrectplus2);
			Rect VignettingR = new Rect (slotrectx, slotrecty * 5.2f, slotrectplus1, slotrectplus2);
			Rect DetailLevelR = new Rect (slotrectx, slotrecty * 5.8f, slotrectplus1, slotrectplus2);
			Rect ShadowQualityR = new Rect (slotrectx, slotrecty * 6.4f, slotrectplus1, slotrectplus2);
			GUI.Box (OcclusionCullingR, "OcclusionCulling: " + ReturnBool(OcclusionCulling));
			GUI.Box (AmbientOcclusionR, "AmbientOcclusion: " + ReturnBool(AmbientOcclusion));
			GUI.Box (ColourGradingR, "ColourGrading: " + ReturnBool(ColourGrading));
			GUI.Box (BloomR, "Bloom: " + ReturnBool(Bloom));
			GUI.Box (MaxTreesR, "MaxTrees: " + MaxTrees);
			GUI.Box (MotionBlurR, "MotionBlur: " + ReturnBool(MotionBlur));
			GUI.Box (EyeAdaptionR, "EyeAdaption: " + ReturnBool(EyeAdaption));
			GUI.Box (VignettingR, "Vignetting: " + ReturnBool(Vignetting));
			GUI.Box (DetailLevelR, "DetailLevel: " + ((float)DetailLevel/10f).ToString());
			GUI.Box (ShadowQualityR, "ShadowQuality: " + QualitySettings.shadowResolution.ToString());
// change option when clicked
			if (CanClick) {
				if (OcclusionCullingR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (OcclusionCulling > 0) {
						OcclusionCulling = 0;
						GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().useOcclusionCulling = false;
						return;
					}
					if (OcclusionCulling < 1) {
						OcclusionCulling = 1;
						GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().useOcclusionCulling = true;
						return;
					}
				}

				if (AmbientOcclusionR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (AmbientOcclusion > 0) {
						AmbientOcclusion = 0;
						prof.ambientOcclusion.enabled = false;
						return;
					}
					if (AmbientOcclusion < 1) {
						AmbientOcclusion = 1;
						prof.ambientOcclusion.enabled = true;
						return;
					}
				}

				if (ColourGradingR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (ColourGrading > 0) {
						ColourGrading = 0;
						prof.colorGrading.enabled = false;
						return;
					}
					if (ColourGrading < 1) {
						ColourGrading = 1;
						prof.colorGrading.enabled = true;
						return;
					}
				}

				if (BloomR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (Bloom > 0) {
						Bloom = 0;
						prof.bloom.enabled = false;
						return;
					}
					if (Bloom < 1) {
						Bloom = 1;
						prof.bloom.enabled = true;
						return;
					}
				}

				if (MaxTreesR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (MaxTrees == 0) {
						MaxTrees = 0;
						prof.ambientOcclusion.enabled = false;
						return;
					}
					if (MaxTrees == 1) {
						MaxTrees = 1;
						prof.ambientOcclusion.enabled = true;
						return;
					}
				}

				if (MotionBlurR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (MotionBlur > 0) {
						MotionBlur = 0;
						prof.motionBlur.enabled = false;
						return;
					}
					if (MotionBlur < 1) {
						MotionBlur = 1;
						prof.motionBlur.enabled = true;
						return;
					}
				}

				if (EyeAdaptionR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (EyeAdaption > 0) {
						EyeAdaption = 0;
						prof.eyeAdaptation.enabled = false;
						return;
					}
					if (EyeAdaption < 1) {
						EyeAdaption = 1;
						prof.eyeAdaptation.enabled = true;
						return;
					}
				}

				if (VignettingR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (Vignetting > 0) {
						Vignetting = 0;
						prof.vignette.enabled = false;
						return;
					}
					if (Vignetting < 1) {
						Vignetting = 1;
						prof.vignette.enabled = true;
						return;
					}
				}

				if (DetailLevelR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (DetailLevel == 0) {
						DetailLevel = 2;
						Terrain.detailObjectDensity = DetailLevel / 10;
						return;
					}
					if (DetailLevel == 2) {
						DetailLevel = 4;
						Terrain.detailObjectDensity = (float)DetailLevel / 10;
						return;
					}
					if (DetailLevel == 4) {
						DetailLevel = 6;
						Terrain.detailObjectDensity = (float)DetailLevel / 10;
						return;
					}
					if (DetailLevel == 6) {
						DetailLevel = 8;
						Terrain.detailObjectDensity = (float)DetailLevel / 10;
						return;
					}
					if (DetailLevel == 8) {
						DetailLevel = 10;
						Terrain.detailObjectDensity = (float)DetailLevel / 10;
						return;
					}
					if (DetailLevel == 10) {
						DetailLevel = 0;
						Terrain.detailObjectDensity = (float)DetailLevel / 10;
						return;
					}
				}

				if (ShadowQualityR.Contains (Event.current.mousePosition) && Input.GetMouseButtonDown (0)) {
					StartCoroutine (WaitAfterClick ());
					if (ShadowQuality == 1) {
						ShadowQuality = 2;
						QualitySettings.shadowResolution = ShadowResolution.Low;
						return;
					}
					if (ShadowQuality == 2) {
						ShadowQuality = 3;
						QualitySettings.shadowResolution = ShadowResolution.Medium;
						return;
					}
					if (ShadowQuality == 3) {
						ShadowQuality = 4;
						QualitySettings.shadowResolution = ShadowResolution.High;
						return;
					}
					if (ShadowQuality == 4) {
						ShadowQuality = 1;
						QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
						return;
					}

				}
//		if (Input.GetKeyDown (KeyCode.L)) {
//			OcclusionCulling = PlayerPrefs.GetInt ("OcclusionCulling");
//		}
//		if (Input.GetKeyDown (KeyCode.S)) {
//			PlayerPrefs.SetInt ("OcclusionCulling", OcclusionCulling);
//		}
			}
		}
	}
	IEnumerator WaitAfterClick()
	{
		CanClick = false;
		yield return new WaitForSeconds (0.2f);
		CanClick = true;
	}
	bool ReturnBool(int num)
	{
		if (num > 0) {
			return true;
		}
		else {
			return false;
		}
	}
}
