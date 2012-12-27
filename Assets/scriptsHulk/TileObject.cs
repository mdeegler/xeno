using UnityEngine;
using System.Collections;

public class TileObject : MonoBehaviour {
	public GameObject selectedMarker;
	public GameObject fire;
	public GameObject activeFire;
	public bool selected;
	public bool spawner = false;
	private bool _onFire = false;
	public bool OnFire { 
		get {
			return _onFire;
		}
		set {
			if(!_onFire && value)
				RenderTileOnFire();
			_onFire = value;
		}
	}

	// Use this for initialization
	void Start () {
		selected=false;

	}
	
	// Update is called once per frame
	void Update () {
		if(selectedMarker != null)
    		selectedMarker.renderer.enabled = selected;
	
	}
	
	private void RenderTileOnFire() {
		GameObject newFire = (GameObject)Instantiate(fire, transform.position, Quaternion.LookRotation(Vector3.up));	
		newFire.transform.parent = transform;
		activeFire =  newFire;
	}
	
	public void FireDiesOut() {
		OnFire =false;
		if(activeFire == null)
			return;
		GameObject fireball = activeFire.transform.FindChild("Fireball Particle System").gameObject;
		if(fireball.activeSelf)
			fireball.SetActive(false);
		else {
			Destroy(activeFire);
			activeFire = null;
		}
	}
	
	/** 
	 * fire spreads to this location. Mark it on fire and 
	 * kill any aliens located here.
	 **/
	public void SpreadFire() {
		OnFire = true;	
		GameObject aUnit = GetUnit();
		if(aUnit == null)
			return;
		
		//AlienData adata = aUnit.GetComponent<AlienData>();
		//if(adata)
		//	adata.OnDeath();
		aUnit.BroadcastMessage("OnDeath");
	}
	
	/**
	 * check to see if this point/object is insdie the flame radius for this tile
	 **/
	public bool IsInsideFlameRadius(Vector3 point){
	   Vector3 center;
	   Vector3 direction;
	   Ray ray;
	   RaycastHit hitInfo;
	   bool hit;
	
	   // Use collider bounds to get the center of the collider. May be inaccurate
	   // for some colliders (i.e. MeshCollider with a 'plane' mesh)
	   Transform flamer = transform.FindChild("FlamerTarget");
	   center = flamer.collider.bounds.center;
	
	   // Cast a ray from point to center
		
	   if(center.Equals(point))
			return false; // don't treat the current spot as if it is in the zone	
	   direction = center - point;

	   ray = new Ray(point, direction);
	   hit = flamer.collider.Raycast(ray, out hitInfo, direction.magnitude);
	
	   // If we hit the collider, point is outside. So we return !hit
	   return !hit;
	}
	
	public bool IsMyPosition(Vector3 aTilePosition) {
		bool match = false;

		if((Mathf.Round(transform.position.x*10.0f) == Mathf.Round(aTilePosition.x*10.0f)) 
			&& (Mathf.Round(transform.position.z*10.0f) == Mathf.Round(aTilePosition.z*10.0f)))
			match = true;

		return match;
	}
	
	/**
	 * returns the unit at this location. If there is no 
	 * unit here, then return null
	 **/
	public GameObject GetUnit() {
		GameObject tiles = GameObject.Find("Tiles");
		TilesManager tileMgr = tiles.GetComponent<TilesManager>();
		GameObject aUnit = tileMgr.GetUnitAt(transform.position);
		
		return aUnit;
	}
	

}
