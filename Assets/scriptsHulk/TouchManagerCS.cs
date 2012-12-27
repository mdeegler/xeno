using UnityEngine;
using System.Collections;
//using Pathfinding;

/**
 * This manages users input at the scene level. 
 **/
public class TouchManagerCS : MonoBehaviour {
	
	public GameObject lastGameObj; //this is the selected marine
	private GameObject lastAlienTarget; // mouse currently hovering over this guy
	public Vector3 lastMousePosition;
	public Vector3 lastMouseMove;
	public Camera masterCamera;
	
	// Use this for initialization
	void Start () {
		////SetActiveCamera(Camera.main);
		
		// find the first marine
		GameObject marines = GameObject.Find("Marines");
		PlayerHeroVarsCS marineData;
		foreach (Transform child in marines.transform) {
			marineData = child.gameObject.GetComponent<PlayerHeroVarsCS>();
			if(marineData.Selected == true) {
				MarineManager.Instance.SelectMarine(child.gameObject, marineData);
				break;
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	  // no need to do input processing during alien turn	
	  if(TurnManager.Instance.IsAlienTurn())
			return;
	
	  RaycastHit hit;
	  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	   //Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
	  
//	  if(Input.GetAxis("Mouse X")!=0 || Input.GetAxis("Mouse Y") != 0) {
//	  	lastMouseMove = Input.mousePosition;
//		Debug.Log("mouse is moved:"+lastMouseMove);
//	  }
#if !UNITY_IPHONE
	  if(Input.GetMouseButtonUp(0)) {
#else
	  if(Input.touchCount == 1 && Input.GetTouch(0).tapCount > 1) {
#endif
	  	if(Physics.Raycast(ray, out hit)) {
	  	  Debug.Log("rayhit...");
	  	  if(hit.transform.gameObject==null) 
	  	  	return; //user doesn't click anything
	  	  	
	  	  // selecting a unit	
		  if(SelectMarine (hit))
			return;
				
		  if(MoveMarine(hit)) 
			return; //user tries to move a marine
				
		  if(ShootAlien(hit))
			return;
				
	  	}
	  }else if(lastGameObj != null) {	
	    // draw a mouse cursor walk target
		DrawCursorTarget(ray);
	  }    
	}
	
	private void DrawCursorTarget(Ray ray) {
		RaycastHit hit;
		// return out if the mouse hasn't moved or if the ray hits nothing.
		if((lastMousePosition == Input.mousePosition) || !(Physics.Raycast(ray, out hit)))
			return;
		
		if(hit.transform.gameObject.name.Equals("Tile")){
	    	var walkTarget = GameObject.FindGameObjectWithTag("WalkTarget");
			if(!walkTarget.renderer.enabled)
				walkTarget.renderer.enabled	= true;
			walkTarget.transform.position =  hit.transform.position;
			lastMousePosition = Input.mousePosition;
			return;
	    }
		
		if(hit.transform.gameObject.CompareTag("Alien")){
			GameObject alien = hit.transform.gameObject;
			AlienData adata = alien.GetComponent<AlienData>();
				
			if(adata.unitStatus == AlienData.UnitStatus.DEAD) {
				//lastAlienTarget.GetComponent<PlayerHeroVarsCS>().Selected=false;
				lastAlienTarget = null;
				return;
			}
		  	adata.Selected=true;
			lastAlienTarget = alien; 
		} else if (lastAlienTarget != null){
			//hit.transform.gameObject.BroadcastMessage("Selected", false);
			lastAlienTarget.GetComponent<PlayerHeroVarsCS>().Selected=false;
			lastAlienTarget = null;
		}
	}
	
	
	/**
	 * Player clicks on an alien while having a marine selected
	 **/
	private bool ShootAlien(RaycastHit hit) {
		if(MarineManager.Instance.CanHitTarget(lastGameObj, hit.transform.gameObject)) {
			lastGameObj.BroadcastMessage("OnShootAlien", hit.transform.gameObject);
			Debug.Log("Shooting an alien!!");
			return true;
		}
		
		return false;
	}
	
	
	/**
	 * user selects a marine with mouse
	 **/
	bool SelectMarine(RaycastHit hit) {
		bool activate = false;
		if(hit.transform.gameObject.CompareTag("Player")){ // i think we clicked a marine
			PlayerHeroVarsCS phvars;
			phvars = hit.transform.gameObject.GetComponent<PlayerHeroVarsCS>();
		  	if(phvars != null) { //ok we did click an marine
				MarineManager.Instance.SelectMarine(hit.transform.gameObject, phvars);
				activate = true;
		  	}
		}
		return activate;
	}

	/**
	 * Show moves that are only valid in the  
	 **/
	public void showValidMoves(GameObject mob, PlayerHeroVarsCS mobData) {
 
		GameObject tiles = GameObject.Find("Tiles");
		tiles.GetComponent<TilesManager>().showValidMoves(mob, mobData);
		
	}
	
	
	
	/** 
	 * User tries to move a marine
	 **/
	bool MoveMarine(RaycastHit hit) {
		 bool activate = false;
		
		 if(lastGameObj != null && hit.transform.gameObject.name.Equals("Tile")){	
			activate = true;
		    Debug.Log("move click "+hit.point);
			Debug.Log("total distance!!!!! "+Vector3.Distance (hit.point, lastGameObj.transform.position));
			TileObject tcomponent = hit.transform.gameObject.GetComponent<TileObject>();
			if(!tcomponent.selected)
				return activate; // not a valid spot to move to
 		    AstarAI aiMove = lastGameObj.GetComponent<AstarAI>();
		    aiMove.setTargetPosition(hit.transform.position);
			
			// hid moves while the marine is moving
			GameObject tiles = GameObject.Find("Tiles");
		    tiles.GetComponent<TilesManager>().clearSelectedTiles();
	  	 } else {
			Debug.Log("badmove lastobj="+lastGameObj+" tileClicked="+hit.transform.gameObject.name);	
		 }
		 return activate;
	}
	
	/**
	 * change which camera is active. Must be done in a way so that there is always 
     * an active camera
	 **/ 
	public void SetActiveCamera(Camera newActiveCamera) {
		Camera oldCamera = masterCamera;
		masterCamera = newActiveCamera;
		//masterCamera.enabled = true;
		masterCamera.camera.active = true;
		
		if(oldCamera != null && oldCamera != masterCamera) {
			oldCamera.camera.active = false;
			//oldCamera.enabled = false;
		}
		
		masterCamera.GetComponent<AudioListener>().enabled = true;
		
		Debug.Log("Camera updated to be "+newActiveCamera.GetInstanceID());
	}



/**
function OnGUI () {

    var evt = Event.current;

    if (evt.isMouse && Input.GetMouseButton (0))

    {

       // Send a ray to collide with the plane

       var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
       var hit: RaycastHit;
       if (collider.Raycast (ray, hit, Mathf.Infinity)) {

         // Find the u,v coordinate of the Texture

         var uv: Vector2;

         uv.x = (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;

         uv.y = (hit.point.y - hit.collider.bounds.min.y) / hit.collider.bounds.size.y;

         // Paint it red

         var tex: Texture2D;
         tex = hit.transform.gameObject.renderer.sharedMaterial.mainTexture;

         tex.SetPixel ((uv.x * tex.width), (uv.y * tex.height), Color.red);

         tex.Apply ();

       }

    }

}**/
	
}
