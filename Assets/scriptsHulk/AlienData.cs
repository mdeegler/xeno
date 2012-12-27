using UnityEngine;
using System.Collections;
using Pathfinding;

public class AlienData : PlayerHeroVarsCS {
	//public Vector3 bestPossibleTarget;
	private int marineCheckCount = 0; //counter to deterine which marine is currently being evaluated for targeting
	private GameObject finalMarineTarget; 
	private GameObject currentMarineTarget;
	private Path aiPath;
	
    
	public float PathLength() { 
			if(aiPath == null)
				return 999.0f;
			return aiPath.GetTotalLength();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ResetAlienData() {		
		// delete this unit if its dead
		if(unitStatus == UnitStatus.DEAD) {
			Destroy(gameObject);
			return;
		}
		
		unitStatus = UnitStatus.NOT_MOVED;
		//bestPossibleTarget = null;
		marineCheckCount = 0;
		aiPath = null;
		base.ResetData();
	}
	
	/**
	 * Find best marine target and path to him
	 **/
	public void ComputePath(Transform alien, GameObject marines) {
		if(unitStatus == UnitStatus.DEAD) // dead units don't path
			return;
		
		Transform marineTransform = marines.transform.GetChild(marineCheckCount);
		currentMarineTarget = marineTransform.gameObject;
		//Debug.Log("checking path to marine= "+marineTransform.gameObject.name);
		
		Seeker seeker = alien.gameObject.GetComponent<Seeker>();	
		
		PlayerHeroVarsCS marineData = marineTransform.GetComponent<PlayerHeroVarsCS>();
		seeker.StartPath (alien.position, marineData.currentTile.transform.position, OnPathChecked);	
	}
	
	/**
	 * Callback function that is triggered once the path has been computed for this alien
	 **/
	public void OnPathChecked (Path p) {
		
		//Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			if(p != null) { 
				//AstarAI aiMove = gameObject.GetComponent<AstarAI>();
				if(aiPath == null || p.GetTotalLength() < aiPath.GetTotalLength())
					aiPath = p;
					finalMarineTarget = currentMarineTarget;
				//unitStatus = UnitStatus.READY_TO_MOVE;
			}
		} else {
			// there was an error
			Debug.Log ("Yey, we got an alien path back but there was an error "+p.error);
		}
		
		// check the next marine to see if it has a shorter path
		GameObject marines = GameObject.Find("Marines");
		marineCheckCount ++;
		if(marineCheckCount < marines.transform.childCount) {
			ComputePath(gameObject.transform, marines);
		} else if (aiPath != null) {
			// no more marines, to check. lets go with the best one.
			unitStatus = UnitStatus.READY_TO_MOVE;
			Debug.Log("moving to marine= "+finalMarineTarget.name);
		} else {
			unitStatus = UnitStatus.MOVED;	
		}	
	}
	
	/**
	 * Called when all moves have been computed and this unit is ready to move
	 **/
	public void BeginMove() {
		unitStatus = UnitStatus.MOVING;
		Debug.Log("this bug is moving: "+gameObject.name);
		GameObject mgr = GameObject.Find("Manager Scripts");
		TouchManagerCS touch = mgr.GetComponent<TouchManagerCS>();
		touch.SetActiveCamera(unitCamera);
		
		gameObject.GetComponent<ThirdPersonMMOCamera>().Reset();
		
		MarineManager.Instance.unselectAllMarines(null);
		AstarAI aiMove = gameObject.GetComponent<AstarAI>();
		aiMove.OnPathComplete(aiPath); //tells the alien to actually move
	}
	
	public void OnDeath() {
		unitStatus = UnitStatus.DEAD;
	
		// try to make it so other units can walk through this object.
		Collider col = gameObject.GetComponent<Rigidbody>().collider;
		col.isTrigger = true;
		
	}
	
	public void QueueAttackAt(Vector3 aTilePosition) {
		GameObject tiles = GameObject.Find("Tiles");
		TilesManager tileMgr = tiles.GetComponent<TilesManager>();
		finalMarineTarget = tileMgr.GetUnitAt(aTilePosition);
		if(finalMarineTarget != null && finalMarineTarget.CompareTag("Player")) {
			Debug.Log("alien attacking a marine");
			unitStatus = UnitStatus.ATTACKING;
			StartCoroutine(AttackTarget(finalMarineTarget));
		}

	}
	
	public IEnumerator AttackTarget(GameObject finalMarineTarget) {
		gameObject.BroadcastMessage("OnStartFire");
		Debug.Log("attack animation: "+Time.time);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(BloodSplatter(finalMarineTarget));
		finalMarineTarget.BroadcastMessage("OnBeingAttacked");
		yield return new WaitForSeconds(3.0f);
		unitStatus = UnitStatus.MOVED;
		Debug.Log("done attacking: "+Time.time);
	}
	
	private IEnumerator BloodSplatter(GameObject target) {
		MarineData mdata = target.GetComponent<MarineData>();
		GameObject newBloodObject = (GameObject)Instantiate(mdata.damagePrefab, mdata.currentTile.transform.position, new Quaternion(-90, -180, -180, 0));
		newBloodObject.transform.parent = mdata.currentTile.transform;
		///newBloodObject.particleEmitter.Emit();
		yield return new WaitForSeconds(2.0f);
		newBloodObject.particleEmitter.emit = false;
		Destroy(newBloodObject);
	}
	
	/**
	 * When a unit is hit, this tells the current location and all 
	 * surrounding locations that they are on fire
	 **/
	private void OnImOnFire() {
		TileObject tile = currentTile.GetComponent<TileObject>();
		tile.OnFire = true;	
		
		//find other fire targets - tiles
		GameObject tiles = GameObject.Find("Tiles");
		ArrayList vaildTiles = tiles.GetComponent<TilesManager>().FindNearbyTiles(tile);
		object[] temp = vaildTiles.ToArray();
		GameObject childTile;
		foreach(object child in temp) {
			childTile = (GameObject)child;
			//childTile.GetComponent<TileObject>().OnFire = true;	
			childTile.GetComponent<TileObject>().SpreadFire();
		}
		
	}
}
