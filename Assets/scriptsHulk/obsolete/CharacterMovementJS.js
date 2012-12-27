#pragma strict

var speed:float = 0.0025F;
var rotationSpeed:float = 0.004F;
var curTilePos:Vector3;
var myTransform:Transform;
var anim:Animation;
var walkAnimationName = "run_forward";

private var controller: CharacterController;
var IsMoving;
    
function Start () {
    controller = this.GetComponent("CharacterController");
	myTransform = transform;
}

function Update () {
  if(IsMoving)
  	MoveTowards(curTilePos);
}

function StartMovingV(dest:Vector3) {
	IsMoving = true;
	var updatedDestination = dest;
	updatedDestination.y = 0;
	curTilePos=updatedDestination;
}
	
function MoveTowards(position:Vector3) {
        //mevement direction
        var dir:Vector3 = position - myTransform.position;
        dir.y = 0;

        // Rotate towards the target
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
            Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            
                //transform.Rotate(0, Input.GetAxis ("Horizontal") * rotateSpeed, 0);

        var forwardDir:Vector3 = myTransform.forward;
        forwardDir = forwardDir * speed;
//continued after the break
        var speedModifier:float = Vector3.Dot(dir.normalized, myTransform.forward);
        forwardDir *= speedModifier;
        if (speedModifier > 0.95f && (Mathf.Abs(dir.x) > 0.01 || Mathf.Abs(dir.y) > 0.01))
        {
            Debug.Log("moving..."+dir.x+" "+dir.z+" "+forwardDir);
            controller.SimpleMove(forwardDir);
            if (!anim[walkAnimationName].enabled)
                anim.CrossFade(walkAnimationName);
        }
        else if (!anim["idle"].enabled){
            Debug.Log("back to idle...");
            anim.CrossFade("idle");
            IsMoving = false;
        }
    }