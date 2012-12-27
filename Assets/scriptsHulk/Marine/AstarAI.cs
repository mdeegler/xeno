using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AstarAI : MonoBehaviour {
	//The point to move to
	public float distanceTheshold = 1.1f;
	public Vector3 targetPosition;
	public float rotationSpeed = 100;
	private Seeker seeker;
	private CharacterController controller;
	//The calculated path
	public Path path;
	//The AI's speed per second
	public float speed = 100;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	private Vector3 lastCursorScreenPosition;
	private Vector3 lastCursorWorldPosition;
	
	private MoveModeType moveMode = MoveModeType.STOP;
	
	public enum MoveModeType
    {
	   STOP,
	   FORWARD
    }
	
	public void Start () {
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();

	}
	
	public void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	
	/**
	 * Set the move target and pick off an a* calculation for the move
	 **/
	public void setTargetPosition(Vector3 pos) {
		targetPosition = pos;
		seeker.StartPath (transform.position,targetPosition, OnPathComplete);
	}
	
	private bool IsDoneMoving(bool isBlocked) {
		
		PlayerHeroVarsCS unitVars = GetComponent<PlayerHeroVarsCS>();
		int currentMoves = unitVars.currentMoves;
		
		if (isBlocked || currentWaypoint >= path.vectorPath.Length || currentWaypoint > currentMoves+1) {
			//Debug.Log ("FinalWaypoint="+currentWaypoint);
			Debug.Log ("End Of Path Reached");
			Debug.Log ("Final waypoint: "+path.vectorPath[currentWaypoint-1]);
			Debug.Log ("My final position: "+transform.position);
			path = null;
			unitVars.currentMoves -= (currentWaypoint - 1); // subtract out the number of tiles this unit has moved

			if(TurnManager.Instance.currentTurn == TurnManager.TurnTypes.MARINE){
				// display updated move markers
				GameObject tiles = GameObject.Find("Tiles");
		    	tiles.GetComponent<TilesManager>().showValidMoves(gameObject, gameObject.GetComponent<PlayerHeroVarsCS>());
			} else {
				if(gameObject.GetComponent<AlienData>().unitStatus != AlienData.UnitStatus.ATTACKING)
					gameObject.GetComponent<AlienData>().unitStatus = AlienData.UnitStatus.MOVED;
				AlienManager.Instance.CheckForTurnOver();
			}
			
			return true;
		}
		
		return false;
	}
	
	public void FixedUpdate () {
		bool isAlienTurn = TurnManager.Instance.currentTurn == TurnManager.TurnTypes.ALIEN? true : false;
		
		if (path == null) {
			//We have no path to move after yet
			return;
		}
		
		// don't move aliens if its not their turn to move
		if(isAlienTurn && (gameObject.GetComponent<AlienData>().unitStatus != AlienData.UnitStatus.MOVING))
			return;
			
		if(IsDoneMoving(false))
			return;
		
		// should we jump to the next waypoint?
		Vector3 currentWPVector = path.vectorPath[currentWaypoint];
		

		//Direction to the next waypoint
		
		//Debug.Log ("Current Waypoint: "+currentWPVector);
		//	Debug.Log ("Current Position: "+transform.position);
		Vector3 dir = (currentWPVector-transform.position).normalized;

		//Debug.Log ("moving: "+dir);
		
		// rotate //
        dir.y = 0;
		Quaternion rotation = Quaternion.LookRotation(dir);
		float str = Mathf.Min (rotationSpeed * Time.deltaTime, 1); 
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, str);
		
		// Check direction angle. If greater than 60° then first turn without moving, otherwise full throttle ahead.
		var angle = Vector3.Angle(dir, transform.forward);
		if (angle > 60.0 && !(ReadyForNextWaypoint(currentWPVector))) {
			moveMode = MoveModeType.STOP;
			//Debug.Log("stopangle="+angle);
		} else {
			moveMode = MoveModeType.FORWARD;
		}

		// move //
		dir *= speed * Time.fixedDeltaTime;
		int moveforward=1;
		if(moveMode == MoveModeType.STOP)
			moveforward = 0;	
		controller.SimpleMove (dir * moveforward);
		//transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		
		if(ReadyForNextWaypoint(currentWPVector)) {
			currentWaypoint++;
			if(currentWaypoint >= path.vectorPath.Length)
				return;
				
			currentWPVector = path.vectorPath[currentWaypoint];
			if(IsPathBlocked(currentWPVector)) {
				Debug.Log("Marine Blocking Me! Kill It!");
				IsDoneMoving(true);
			}
		}	
	}
	
	private bool ReadyForNextWaypoint(Vector3 currentWPVector) {
		
		Vector3 start = transform.position;
		Vector3 end = currentWPVector;
		
		start.y = 0;
		end.y = 0;
		float dist = Vector3.Distance (start,end); 
		
		if (dist >= nextWaypointDistance)
			return false;
		
		return ((currentWaypoint < path.vectorPath.Length-1) 
			|| (currentWPVector == transform.position) 
			|| dist < distanceTheshold 
			|| (currentWPVector.x < distanceTheshold && currentWPVector.z < distanceTheshold)); 
	}
	
	private bool IsPathBlocked(Vector3 currentWPVector) {
		GameObject tiles = GameObject.Find("Tiles");
		TilesManager tileMgr = tiles.GetComponent<TilesManager>();
		
		// this is the next place i'm walking to
		if(tileMgr.IsUnitAt(currentWPVector, gameObject)) {
			if (TurnManager.Instance.currentTurn == TurnManager.TurnTypes.ALIEN )
				gameObject.GetComponent<AlienData>().QueueAttackAt(currentWPVector);	
			return true;
		}

		return false;
	}	
	
	void OnShootAlien(GameObject target) {
		float tempRotationSpeed = rotationSpeed;
		rotationSpeed = 10000;
		//RotateToFaceCursor();
		Vector3 dir = (target.transform.position-transform.position).normalized;
        dir.y = 0;
		Quaternion rotation = Quaternion.LookRotation(dir);
		float str = Mathf.Min (rotationSpeed * Time.deltaTime, 1); 
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, str);
		
		
		rotationSpeed = tempRotationSpeed;
	}
	
	private void RotateToFaceCursor() {
		GameObject mgr = GameObject.Find("Manager Scripts");
		TouchManagerCS touch = mgr.GetComponent<TouchManagerCS>();
		Vector3 cursorScreenPosition = touch.lastMouseMove;
		//Vector3 cursorScreenPosition = Input.mousePosition;
						
		// Find out where the mouse ray intersects with the movement plane of the player
		//GameObject gboard = GameObject.Find("GameBoard");
		float cursorPlaneHeight = 0.0f;
		Plane playerMovementPlane = new Plane(gameObject.transform.up, gameObject.transform.position + gameObject.transform.up * cursorPlaneHeight);
		Camera unitCamera = gameObject.GetComponent<PlayerHeroVarsCS>().unitCamera;
		Vector3 cursorWorldPosition;
		if(lastCursorScreenPosition.Equals(cursorScreenPosition))
			cursorWorldPosition = lastCursorWorldPosition;
		else {
			cursorWorldPosition = ScreenPointToWorldPointOnPlane (cursorScreenPosition, playerMovementPlane, unitCamera);
		} 
		
		// trying to rotate
		Vector3 dir = (cursorWorldPosition-transform.position).normalized;
		dir.y = 0;
		Quaternion rotation = Quaternion.LookRotation(dir);
		float str = Mathf.Min (rotationSpeed * Time.deltaTime, 1); 
		//Transform cameraPosition = unitCamera.transform;
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, str);
		//unitCamera.transform.position = cameraPosition.position;
		
		lastCursorScreenPosition = cursorScreenPosition;
		lastCursorWorldPosition	= cursorWorldPosition;
		
	}
	
	public static Vector3 ScreenPointToWorldPointOnPlane (Vector3 screenPoint, Plane plane, Camera camera) {
		// Set up a ray corresponding to the screen position
		Ray ray = camera.ScreenPointToRay (screenPoint);
		
		// Find out where the ray intersects with the plane
		return PlaneRayIntersection (plane, ray);
	}
	
	public static Vector3 PlaneRayIntersection (Plane plane, Ray ray) {
		float dist;
		plane.Raycast(ray, out dist);
		return ray.GetPoint(dist);
	}
	
}
