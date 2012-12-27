using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public Texture[] buttonIcons;
	
	private const int BULLET = 0;
	private const int ENDTURN = 1;
	private const int ENERGY = 2;
	private const int HEART = 3;
	private const int OVERWATCH = 4;
	private const int MARINE = 5;
	private const int RELOAD = 6;
	private const int TICK = 7;
	private const int TICK_OFF = 8;
	
	private const int BUTTON_WIDTH = 76;
	private const int BUTTON_HEIGHT = 50;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		
		string turnButtonText = "Aliens Moving...";
		if(TurnManager.Instance.currentTurn == TurnManager.TurnTypes.MARINE){
			turnButtonText = "End Turn";	
		} 
		
		// end turn button
		GUIContent content = new GUIContent(turnButtonText, buttonIcons[ENDTURN]);
		GUIStyle style = GUI.skin.button;
		style.imagePosition = ImagePosition.ImageAbove;
			
		if (GUI.Button (new Rect (Screen.width/2 + BUTTON_WIDTH*2+100,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
			print ("You clicked the end turn button!");
			TurnManager.Instance.NextTurn();
		}
		
		// reload button
		content.text = "Reload";
		content.image = buttonIcons[RELOAD];
		if (GUI.Button (new Rect (Screen.width/2 + BUTTON_WIDTH+100,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
			MarineManager.Instance.CurrentMarineReloadsWeapon();	
		}
		
		// target area button
		
		
		// overwatch button
		content.text = "Overwatch";
		content.image = buttonIcons[OVERWATCH];
		if (GUI.Button (new Rect (Screen.width/2 +100 ,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
			
		}
		
		// marines
		MarineData data;
		float locX = Screen.width/2 - 500;
		float locY = Screen.height - BUTTON_HEIGHT-20-2;
		int index = 0;
		foreach(Transform marineTrans in MarineManager.Instance.Marines.transform) {
			data = marineTrans.GetComponent<MarineData>();
			MarineGUI(locX+(112*index), locY, data);
			index++;
		}
		
	
	}
	
	private void MarineGUI(float locX, float locY, MarineData data) {
		GUIStyle style = GUI.skin.box;
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = 9;
		//GUI.contentColor = Color.white;
		if(data.Selected)
			GUI.contentColor = Color.yellow;
		string displayName = data.unitName+" - "+data.weapon.weaponName;
		if (GUI.Button (new Rect (locX, locY, 110,BUTTON_HEIGHT+20), displayName, style)) {
			print ("You clicked the marine button!");
			MarineManager.Instance.SelectMarine(data.gameObject, data);
		}
		
		
		GUI.Label(new Rect (locX+5 , locY, 64, 64), buttonIcons[MARINE]);
		
		UnitStatusBar(locX+53, locY+52, buttonIcons[HEART], data.maxHealth, data.currentHealth);	
		UnitStatusBar(locX+67, locY+52, buttonIcons[ENERGY], data.maxMoves, data.currentMoves);
		UnitStatusBar(locX+81, locY+52, buttonIcons[BULLET], data.weapon.maxAmmo, data.weapon.currentAmmo);
		GUI.contentColor = Color.white;
	}
	
	/** 
	 * A stat bar for a marine
	 **/
	private void UnitStatusBar(float locX, float locY, Texture icon, int max, int current) {
		// draw icon
		GUI.Label(new Rect(locX, locY, 18, 18), icon);
		
		// draw ticks
		for(int index=0; index < max; index++) {
			Texture tex = buttonIcons[TICK];
			if(index>=current)
				tex = buttonIcons[TICK_OFF];
			
			GUI.DrawTexture(new Rect(locX, locY-3-(5*index), 10, 4), tex, ScaleMode.StretchToFill, true);	
		}
	}
}
