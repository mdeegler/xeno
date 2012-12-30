using UnityEngine;
using System.Collections;
using Pathfinding;

public class TilesManager : MonoBehaviour {
	
	/**
	private static TilesManager _instance = new TilesManager();

	public static TilesManager Instance {
		get {
		    if (_instance == null) {
		        _instance = (TilesManager)FindObjectOfType(typeof(TilesManager));
		        if (_instance == null)
		            _instance = (new GameObject("TilesManager")).AddComponent<TilesManager>();
		    }
		    return _instance;
		}
	}
	**/
	
	public float distanceCheckMultiplier = 10f;
	private GameObject tiles;
	private int tilesCount;
	private int currentTile = 0;
	private GameObject selectedUnit;
	private PlayerHeroVarsCS selectedUnitData;
	
	// Use this for initialization
	void Start () {
		tiles = GameObject.Find("Tiles");
		tilesCount = tiles.transform.GetChildCount();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/**
	 * show on the gameboard all valid moves for the selected unit
	 **/
	public void showValidMoves(GameObject mob, PlayerHeroVarsCS mobData) {
		currentTile = 0;
		if(mobData.currentMoves < 1)
			return;
		selectedUnit = mob;
		selectedUnitData = mobData;
		CheckTile(currentTile);
	}
	
	/**
	 * performs a A* path check to this target tile
	 **/
	private void CheckTile(int tileIndex) {
		Transform tileTransform = tiles.transform.GetChild(tileIndex);
		// do a raw check to see if its anywhere in the ballpark
		float dist = Vector3.Distance (tileTransform.position, selectedUnit.transform.position);
		//Debug.Log ("Distance = "+dist);
		if(dist > distanceCheckMultiplier){
			currentTile ++;
			if(currentTile < tilesCount) 
				CheckTile(currentTile);
			return;
		}
		
		// ok its close, compute path
		//Debug.Log ("Distance IN THRESHHOLD = "+dist);
		Seeker seeker = selectedUnit.GetComponent<Seeker>();
		seeker.StartPath (selectedUnit.transform.position, tileTransform.position, OnPathChecked);	
	}
	
	public void OnPathChecked (Path p) {
		//Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			if(p != null && p.vectorPath.Length <= selectedUnitData.currentMoves+1) { 
				Transform tileTransform = tiles.transform.GetChild(currentTile);
				TileObject tileObj = tileTransform.gameObject.GetComponent<TileObject>();
				//float dist = Vector3.Distance (tileTransform.position, selectedUnit.transform.position);
		       // Debug.Log ("Distance = "+dist);
				if(tileObj.GetUnit() == null) // only highlight this tile if there is no one standing here
					tileObj.selected=true;
			}
		}
		currentTile ++;
		if(currentTile < tilesCount) 
			CheckTile(currentTile);
	}
	
	public void clearSelectedTiles() {
		TileObject tcomp;
		foreach (Transform child in tiles.transform) {
			tcomp = child.gameObject.GetComponent<TileObject>();
            tcomp.selected = false;
        }
	}
	
	public void ClearFlameFromTiles() {
		TileObject tcomp;
		foreach (Transform child in tiles.transform) {
			tcomp = child.gameObject.GetComponent<TileObject>();
            tcomp.FireDiesOut();
        }

	}
	
	public ArrayList FindSpawnPoints() {
		ArrayList spawnPoints = new ArrayList();
		TileObject tcomp;
		foreach(Transform child in tiles.transform){
			tcomp = child.gameObject.GetComponent<TileObject>();
			if(tcomp.spawner)
				spawnPoints.Add(child.gameObject);
		}
		
		return spawnPoints;
	}
	
	
	/**
	 * return true if the tile is on fire or if there is a marine or alien
	 * currently occupying this spot
	 **/
	public bool IsUnitAt(Vector3 aTilePosition, GameObject ignoreThisUnit) {
		// check to see if tile is on fire
		TileObject tile = GetTileAt(aTilePosition);
		if(tile != null && tile.OnFire)
			return true;
		
		// check to see if there is a unit blocking
		GameObject unit = GetUnitAt(aTilePosition);
		if(unit == null || ((ignoreThisUnit != null) && unit.Equals(ignoreThisUnit)))
			return false;
		
		return true;
	}
	
	/**
	 * search through the list of marines and returns a marine at the set vector that 
	 * matches the tile they are on. If none can be found at that location then return null
	 **/
	public GameObject GetUnitAt(Vector3 aTilePosition) {
		PlayerHeroVarsCS pcomp;
		foreach (Transform child in MarineManager.Instance.Marines.transform) {
			pcomp = child.gameObject.GetComponentInChildren<PlayerHeroVarsCS>();	
			if(pcomp.IsMyPosition(aTilePosition))
				return child.gameObject;
		}
		
		foreach (Transform achild in AlienManager.Instance.Aliens.transform) {
			pcomp = achild.gameObject.GetComponentInChildren<PlayerHeroVarsCS>();	
			if(pcomp.IsMyPosition(aTilePosition))
				return achild.gameObject;
		}
		
		return null;
	}
	
	/**
	 * return a list of tiles that touch this tile
	 **/
	public ArrayList FindNearbyTiles(TileObject tile) {
		ArrayList alist = new ArrayList();
		foreach(Transform child in tiles.transform){
			if(tile.IsInsideFlameRadius(child.position))
				alist.Add(child.gameObject);
		}
		
		return alist;
	}
	
	/**
	 * search through all the tiles and return the TileObject of the 
	 * tile that matches the position passed in
	 **/
	private TileObject GetTileAt(Vector3 aPosition) {
		TileObject tileObj;
		foreach(Transform child in tiles.transform){
			tileObj = child.gameObject.GetComponent<TileObject>();
			if(tileObj.IsMyPosition(aPosition))
				return tileObj;
		}	
		return null;
	}
	
}
