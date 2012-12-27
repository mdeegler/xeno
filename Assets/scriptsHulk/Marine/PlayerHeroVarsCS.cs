using UnityEngine;
using System.Collections;

public class PlayerHeroVarsCS : MonoBehaviour {
	
	public GameObject selectedMarker;
	public Camera unitCamera;
	public int maxMoves = 4;
	public int currentMoves =4;
	public int maxHealth = 3;
	public int currentHealth = 3;
	public GameObject currentTile;
	public UnitStatus unitStatus = UnitStatus.NOT_MOVED;
	
	public enum UnitStatus
    {
	   NOT_MOVED, // no path has been computed yet
	   READY_TO_MOVE, //a path has been computed for this unit
	   MOVING,
	   MOVED,
	   DEAD,
	   ATTACKING
	}
	
	private bool _selected=false;
	public bool Selected {
		get {
			return _selected;
		}
		set {
			_selected = value;
			if(selectedMarker != null)
				selectedMarker.renderer.enabled = _selected;
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		 if(selectedMarker != null)
    		selectedMarker.renderer.enabled = Selected;
	}
	
	public void ResetData() {
		Selected = false;
		currentMoves = maxMoves;
	}
	
	
	
	// Use this to update which tile i'm touching
	void OnControllerColliderHit(ControllerColliderHit hit) {
		//Debug.Log("I've controlcollided with something!!!!!!");
	     
		if(hit.transform.gameObject.name.Equals("Tile")){	
			currentTile = hit.transform.gameObject;
		}
	}
	
	public bool IsMyPosition(Vector3 aTilePosition) {
		bool match = false;
		if(currentTile == null)
			return match;
		//return (currentTile.transform.position.Equals(aTilePosition));
		if((Mathf.Round(currentTile.transform.position.x*10.0f) == Mathf.Round(aTilePosition.x*10.0f)) 
			&& (Mathf.Round(currentTile.transform.position.z*10.0f) == Mathf.Round(aTilePosition.z*10.0f)))
			match = true;

		return match;
	}
}
