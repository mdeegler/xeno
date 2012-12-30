using UnityEngine;
using System.Collections;

public class MarineManager : MonoBehaviour {
	public GUIManager guiManager;
	
	private static MarineManager _instance = new MarineManager();

	public static MarineManager Instance {
		get {
		    if (_instance == null) {
		        _instance = (MarineManager)FindObjectOfType(typeof(MarineManager));
		        if (_instance == null)
		            _instance = (new GameObject("MarineManager")).AddComponent<MarineManager>();
		    }
		    return _instance;
		}
	}
	
	public GameObject Marines {get {
			if(_marines == null)
				_marines = GameObject.Find("Marines");
			return _marines;
		}
	}
	
	private GameObject _marines;
	private Transform _debug_source_trans;
	private Vector3 _debug_target;

	// Use this for initialization
	void Start () {
		guiManager = GetComponentInChildren<GUIManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_debug_source_trans != null)
	   		Debug.DrawRay(_debug_source_trans.position, _debug_target, Color.red);
	}
	
	public void StartMarineTurn () {
		ResetMarineData();
		SelectFirstMarine();
	}
	
	// updates marine data so that none have moved.
	void ResetMarineData() {
		foreach (Transform child in Marines.transform) {	
			MarineData aData = child.GetComponent<MarineData>();
			aData.ResetMarineData();		
		}
	}
	
	void SelectFirstMarine() {
		Transform firstMarine = Marines.transform.GetChild(0);	
		PlayerHeroVarsCS data = firstMarine.GetComponent<PlayerHeroVarsCS>();
			
		SelectMarine(firstMarine.gameObject, data);
		GetComponentInChildren<GUIManager>().StatusMessageText = "Airlock Breached. Xenos Detected.";
	}
	
	public void SelectMarine(GameObject marine, PlayerHeroVarsCS marineData) {
		GameObject mgr = GameObject.Find("Manager Scripts");
		TouchManagerCS touch = mgr.GetComponent<TouchManagerCS>();
			
		marineData.Selected=true;
  	  	touch.lastGameObj = marine;
		touch.SetActiveCamera(marineData.unitCamera);
		marineData.gameObject.GetComponent<ThirdPersonMMOCamera>().Reset();
		unselectAllMarines(marine);
		GetComponentInChildren<GUIManager>().StatusMessageText = "";
		touch.showValidMoves(marine, marineData);	
	}
		
	/**
	 * deselects all marines including their active camera and move markers
	 **/
	public void unselectAllMarines (GameObject exceptMe) {	
		foreach (Transform child in Marines.transform) {
	        if(child.gameObject == exceptMe || child.gameObject.active == false)
					continue;
			PlayerHeroVarsCS pcomp = child.gameObject.GetComponentInChildren<PlayerHeroVarsCS>();
            pcomp.Selected = false;
        }
		
		// hid moves while the marine is moving
		GameObject tiles = GameObject.Find("Tiles");
		tiles.GetComponent<TilesManager>().clearSelectedTiles();
	}
	
	public GameObject GetSelectedMarine() {
		foreach (Transform child in Marines.transform) {
	        if(child.gameObject.active == false)
					continue;
			PlayerHeroVarsCS pcomp = child.gameObject.GetComponentInChildren<PlayerHeroVarsCS>();
            if(pcomp.Selected)
				return child.gameObject;
        }		
		return null;
	}
	
	/**
	 * Check to see if their is los to marines target
	 **/
	public bool CanHitTarget(GameObject marineShooter, GameObject target) {
		// exit if marine isn't shooting an alien
		if(marineShooter == null ||  !target.CompareTag("Alien"))
			return false;
		
		// exit if the alien is already dead
		if(target.GetComponent<AlienData>().unitStatus == AlienData.UnitStatus.DEAD)
			return false;
		
		RaycastHit hit;

		Vector3 targetVector = target.transform.position;
		targetVector.y = marineShooter.transform.position.y;
		Vector3 rayDirection = targetVector - marineShooter.transform.position;
		_debug_source_trans = marineShooter.transform;
	    _debug_target = rayDirection;
		

   	 	if (Physics.Raycast (marineShooter.transform.position, rayDirection, out hit)) {

	        if (hit.transform.gameObject.CompareTag("Alien")) {
	            Debug.Log("i see my target!");
				return true;
	        } else {
	            // there is something obstructing the view
				Debug.Log("i CANT see my target!");
				GetComponentInChildren<GUIManager>().StatusMessageText = "No line of sight.";
				return false;
	        }

    	}
		Debug.Log("no ray collision on shot");
        
		return false;
	}
	
	public void CurrentMarineReloadsWeapon() {
		GetSelectedMarine().GetComponent<MarineData>().ReloadWeapon();	
	}
	
	

}
