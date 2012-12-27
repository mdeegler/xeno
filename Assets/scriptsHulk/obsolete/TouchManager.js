#pragma strict

var lastGameObj: GameObject;
//var orthographicSizeMin:int = 1;
//var orthographicSizeMax:int = 6;
    
function Start () {
  InitCamera();
}

function InitCamera() {

  //Camera.main.transform.rotation.x = 57f;
  //Camera.main.transform.rotation.y = 85f;
  //Camera.main.transform.rotation.z = 358f;
  
  //camera.transform.position.x = 93f;
  //camera.transform.position.y = 9.6f;
  //camera.transform.position.z = 142f;
  
}

function Update () {
  var hit:RaycastHit;
  var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  
  MouseZoomCheck();
  
  if(Input.GetMouseButtonUp(0)) {
  	
  	if(Physics.Raycast(ray,hit)) {
  	  Debug.Log("rayhit...");
  	  //if(hit.transform.tag=="PlayerHero") {
  	  //	var ph1 : GameObject[];
  	  //	ph1 = GameObject.FindGameObjectsWithTag("PlayerHero");
  	  var phvars:PlayerHeroVars;
  	  //var cmove:CharacterMovement;
  	  
  	  // figure out where we need to walk to
     if(lastGameObj != null && hit.transform.gameObject.name.Equals("GameBoard")){
       var cmove:CharacterMovementJS = lastGameObj.GetComponent(typeof(CharacterMovementJS));
       cmove.StartMovingV(hit.point);
       return;
  	 }
 
  	  if(hit.transform.gameObject==null) 
  	  	return;
  	  	
  	  // selecting a unit	
  	  phvars = hit.transform.gameObject.GetComponent("PlayerHeroVars");	
  	  if(phvars != null) {
  	  	phvars.selected=true;
  	  	lastGameObj = hit.transform.gameObject;
  	  	Debug.Log("unit click");
  	  }
  	}
  }else if(lastGameObj != null) {
    // draw a walk target
    if(Physics.Raycast(ray,hit) && hit.transform.gameObject.name.Equals("GameBoard")){
    	var walkTarget = GameObject.FindGameObjectWithTag("WalkTarget");
    	walkTarget.transform.position=hit.point;
    }
  }    
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

function MouseZoomCheck() {

    if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
    {
        Debug.Log("zoom in...");
        //Camera.main.orthographicSize++;
        Camera.main.fieldOfView -= 3f;
        
    }
    if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
    {
        Debug.Log("zoom out...");
        Camera.main.fieldOfView += 3f;
        //Camera.main.orthographicSize--;
    }

    //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax );
    
    }