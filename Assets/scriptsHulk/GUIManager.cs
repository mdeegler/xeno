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
	
	private string statusMessageText = "Airlock Breached. Xenos Detected.";
	private Rect messageTextPos;
	private GUIStyle messageTextStyle;
	private Color messageTextColor1;
	private Color messageTextColor2;
	private float fadeoutRate=0.1f;
	private int actionButtonOffset=400;
	
	public string StatusMessageText { 
		set {
			statusMessageText =  value;
			messageTextColor1 = Color.white;
			messageTextColor2 = Color.gray;
		}
		
	}
			

	// Use this for initialization
	void Start () {
		messageTextPos =  new Rect (Screen.width/2 -100 , 20 , 200 ,30);
		messageTextStyle = new GUIStyle();
		messageTextStyle.alignment = TextAnchor.MiddleCenter;
		messageTextStyle.fontSize = 20;
		
		
		//StatusMessageText = "Airlock Breached. Xenos Detected.";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		
		string turnButtonText = "Aliens Moving...";
		
		// display action buttons only during marines turn
		if(TurnManager.Instance.currentTurn == TurnManager.TurnTypes.MARINE){
			turnButtonText = "End Turn";	

			// end turn button
			GUIContent content = new GUIContent(turnButtonText, buttonIcons[ENDTURN]);
			GUIStyle style = GUI.skin.button;
			style.imagePosition = ImagePosition.ImageAbove;
				
			if (GUI.Button (new Rect (Screen.width-actionButtonOffset + BUTTON_WIDTH*2+100,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
				print ("You clicked the end turn button!");
				StatusMessageText = "Xenos moving...";
				TurnManager.Instance.NextTurn();
			}
			
			// reload button
			content.text = "Reload";
			content.image = buttonIcons[RELOAD];
			if (GUI.Button (new Rect (Screen.width-actionButtonOffset + BUTTON_WIDTH+100,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
				MarineManager.Instance.CurrentMarineReloadsWeapon();	
			}
			
			// target area button
			
			
			// overwatch button
			content.text = "Overwatch";
			content.image = buttonIcons[OVERWATCH];
			if (GUI.Button (new Rect (Screen.width-actionButtonOffset +100 ,Screen.height - 52,BUTTON_WIDTH,BUTTON_HEIGHT), content, style)) {
				StatusMessageText = "Feature coming soon! Hang in there marine!";	
			}
		}
		
		// marines
		MarineData data;
		float locX = 30;
		float locY = Screen.height - BUTTON_HEIGHT-20-2;
		int index = 0;
		foreach(Transform marineTrans in MarineManager.Instance.Marines.transform) {
			data = marineTrans.GetComponent<MarineData>();
			MarineGUI(locX+(112*index), locY, data);
			index++;
		}
		
		DisplayStatusMessage();
		
	
	}
	
	/**
	 * This text will slowly fade out. Disable once it is invisible
	 **/
	private void DisplayStatusMessage() {
		if(statusMessageText.Equals(""))
			return;
		
		messageTextColor1.a = messageTextColor1.a - fadeoutRate * Time.deltaTime;
		messageTextColor2.a = messageTextColor2.a - fadeoutRate * Time.deltaTime;
		if(messageTextColor1.a <= 0) {
			statusMessageText = "";
			return;
		}
		DrawOutlinedText(messageTextPos, statusMessageText, messageTextStyle, messageTextColor2, messageTextColor1);	
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
	
	public static void DrawOutlinedText(Rect position, string text, GUIStyle style, Color outColor, Color inColor) {
	    GUIStyle backupStyle = style;
	    style.normal.textColor = outColor;
	    position.x--;
	    GUI.Label(position, text, style);
	    position.x +=2;
	    GUI.Label(position, text, style);
	    position.x--;
	    position.y--;
	    GUI.Label(position, text, style);
	    position.y +=2;
	    GUI.Label(position, text, style);
	    position.y--;
	    style.normal.textColor = inColor;
	    GUI.Label(position, text, style);
	    style = backupStyle;
	}
}
