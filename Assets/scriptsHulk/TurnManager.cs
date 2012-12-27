using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {
	private static TurnManager _instance = new TurnManager();
    //public static TurnManager Instance { get { return _instance; } }
	
	public static TurnManager Instance {
		get {
		    if (_instance == null) {
		        _instance = (TurnManager)FindObjectOfType(typeof(TurnManager));
		        if (_instance == null)
		            _instance = (new GameObject("TurnManager")).AddComponent<TurnManager>();
		    }
		    return _instance;
		}
	}
	
	public enum TurnTypes
    {
	   MARINE,
	   ALIEN
    }
	
	public TurnTypes currentTurn = TurnTypes.MARINE;

	// Use this for initialization
	void Start () {
		MarineManager.Instance.StartMarineTurn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/**
	 * Next turn button has been triggered
	 **/
	public void NextTurn() {
		GameObject tiles = GameObject.Find("Tiles");
		TilesManager tileMgr = tiles.GetComponent<TilesManager>();
			
		if(currentTurn == TurnTypes.MARINE){
			// hid moves while the marine is moving
			tileMgr.clearSelectedTiles();
			
			// start alien turn
			currentTurn = TurnTypes.ALIEN;
			Debug.Log ("Turn now = "+currentTurn);
			AlienManager.Instance.StartAlienTurn();
		} else { // begin marine turn
			currentTurn = TurnTypes.MARINE;
			Debug.Log ("Turn now = "+currentTurn);
			tileMgr.ClearFlameFromTiles();
			MarineManager.Instance.StartMarineTurn();
		}
		
	}
	
	public bool IsAlienTurn() {
		return currentTurn == TurnManager.TurnTypes.ALIEN; 	
	}
	
	public bool IsMarineTurn() {
		return currentTurn == TurnManager.TurnTypes.MARINE; 
	}
}
