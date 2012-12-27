using UnityEngine;
using System.Collections;

public class AlienManager : MonoBehaviour {
	
	private static AlienManager _instance;
 
	public static AlienManager Instance {
		get {
		    if (_instance == null) {
		        _instance = (AlienManager)FindObjectOfType(typeof(AlienManager));
		        if (_instance == null)
		            _instance = (new GameObject("AlienManager")).AddComponent<AlienManager>();
		    }
		    return _instance;
		}
	}

	public GameObject Aliens {get {
			if(_aliens == null)
				_aliens = GameObject.Find("Aliens");
			return _aliens;
		}
	}
	
	public int spawnsPerTurn=2;
	public GameObject alienPrefab;
	private GameObject _aliens;
	private ArrayList _spawnPoints;

	// Use this for initialization
	void Start () {
		GameObject mgr = GameObject.Find("Tiles");
		TilesManager tilesMgr = mgr.GetComponent<TilesManager>();
		_spawnPoints = tilesMgr.FindSpawnPoints();
		ResetAlienData();
	}
	
	// Update is called once per frame
	void Update () {
		if(TurnManager.Instance.currentTurn == TurnManager.TurnTypes.MARINE)
			return;
		
		if(IsAnyoneMoving())
			return;
		
		AlienData alien = GetNextAlienReadyToMove();
		if(alien != null)
			alien.BeginMove();	
		else
			CheckForTurnOver();
		
	}
	
	public void StartAlienTurn () {
		ResetAlienData();
		SpawnAliens();
		MoveAliens();
		
		// alien turn over
		//Debug.Log("Alien Turn Over!");
		
	}
	
	void SpawnAliens() {
		// find random spawn point
		int spawnIndex = Random.Range(0,_spawnPoints.Count-1);
		object[] temp = _spawnPoints.ToArray();
		GameObject selectSpawn = (GameObject)temp[spawnIndex];
		
		// spawn alien and initialize
		GameObject newAlien = (GameObject) Instantiate(alienPrefab, selectSpawn.transform.position, selectSpawn.transform.rotation);
		newAlien.transform.parent = _aliens.transform;
		AlienData alienData = newAlien.transform.GetComponent<AlienData>();
		alienData.unitCamera.active = false;
		alienData.currentTile = selectSpawn;
		alienData.ResetAlienData();
		
	}
	
	// updates aliens data so that none have moved.
	void ResetAlienData() {
		foreach (Transform child in Aliens.transform) {	
			AlienData aData = child.GetComponent<AlienData>();
			aData.ResetAlienData();		
		}
	}
	
	void MoveAliens() {
	  	// move all the aliens, starting with the closest one.	
		
		GameObject marines = GameObject.Find("Marines");
		//int marineCount = marines.transform.GetChildCount();
		
	    foreach (Transform child in Aliens.transform) {	
			AlienData aData = child.GetComponent<AlienData>();
			aData.ComputePath(child, marines);			
		}
	}
	
	/**
	 * let's see if any of the aliens are currently moving. Used to help make sure
	 * we only have 1 moving at a time.
	 **/
	public bool IsAnyoneMoving() {
		foreach (Transform child in Aliens.transform) {		
			AlienData aData = child.GetComponent<AlienData>();
			if(aData.unitStatus == AlienData.UnitStatus.MOVING || aData.unitStatus == AlienData.UnitStatus.ATTACKING) {
				//Debug.Log("someone still moving or attacking: "+aData.unitStatus);
				return true;
			}
		}	
		return false;
	}
	
	/**
	 * Check to see if each of the aliens have moved.
	 **/
	public void CheckForTurnOver() {
		
		foreach (Transform child in Aliens.transform) {		
			AlienData aData = child.GetComponent<AlienData>();
			if(aData.unitStatus != AlienData.UnitStatus.MOVED)
				return;
		}
		
		Debug.Log("Alien Turn Over!");
		TurnManager.Instance.NextTurn();
	}
	
	/**
	 * iterate through all of the aliens and find one to move that has the shortest path to go.
	 **/
	private AlienData GetNextAlienReadyToMove() {
		AlienData bestAlien = null;
		foreach (Transform child in Aliens.transform) {		
			AlienData aData = child.GetComponent<AlienData>();
			if(aData.unitStatus != AlienData.UnitStatus.READY_TO_MOVE)
				continue;
			if((bestAlien == null) || (aData.PathLength() < bestAlien.PathLength()))
				bestAlien = aData;
		}	
		
		return bestAlien;
	}
	
}
