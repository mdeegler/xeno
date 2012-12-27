#pragma strict

var selected;
var selectedMarker:GameObject;

function Start () {
 selected=false;

 //selectedMarker = GameObject.FindGameObjectWithTag("SelectionTag");
 // selectedMarker.renderer.enabled = selected;
 var controller : CharacterController = GetComponent(CharacterController);
 //controller.camera = GetComponent(Camera);
  Debug.Log("gotController...");
 
}

function Update () {

//renderer.enabled= false;
    if(selectedMarker != null)
    	selectedMarker.renderer.enabled = selected;

}