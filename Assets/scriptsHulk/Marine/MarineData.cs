using UnityEngine;
using System.Collections;

public class MarineData : PlayerHeroVarsCS {
	
	public GameObject damagePrefab;
	public Transform damageEffectTransform;
	public string unitName;
	public XenoWeapon.DamageTypes xenoWeaponType = XenoWeapon.DamageTypes.GUN;
	
	[HideInInspector]
	public XenoWeapon weapon;
	
	private ParticleEmitter damageEffect;
	
	public void Awake () {
		if (damagePrefab) {
			if (damageEffectTransform == null)
				damageEffectTransform = transform;			
		}
		 
	}

	// Use this for initialization
	void Start () {
		weapon = XenoWeapon.CreateWeapon(xenoWeaponType);
		
		/**
		 * if(xenoWeaponType == XenoWeapon.DamageTypes.FLAME) {
			//disable regular gun effects
			
			GameObject obj = gameObject.transform.FindChild("WeaponSlot");
			obj.GetComponent<Auto
			AutoFire af = gameObject.GetComponentInChildren<AutoFire>();
			af.enabled = false;
		}
		**/
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void ResetMarineData() {
		// delete this unit if its dead
		if(unitStatus == UnitStatus.DEAD) {
			Destroy(gameObject);
			return;
		}
		ResetData();
	}
	
	void OnShootAlien(GameObject target) {
		if(currentMoves < 1)
			return;
		if(weapon.FireWeapon()) {
			StartCoroutine(ShootAlien(target));
			currentMoves--;
			GameObject tiles = GameObject.Find("Tiles");
			tiles.GetComponent<TilesManager>().clearSelectedTiles();
		    tiles.GetComponent<TilesManager>().showValidMoves(gameObject, this);
		}
	}
	
	IEnumerator ShootAlien(GameObject target) {
		BroadcastMessage("OnStartFire");
		target.BroadcastMessage("OnImHit");
		if(xenoWeaponType == XenoWeapon.DamageTypes.FLAME)
			target.BroadcastMessage("OnImOnFire");
		yield return new WaitForSeconds(0.5f);
		target.BroadcastMessage("OnImHit");
		yield return new WaitForSeconds(0.5f);
		target.BroadcastMessage("OnImHit");
		yield return new WaitForSeconds(0.5f);
		target.BroadcastMessage("OnImHit");
		yield return new WaitForSeconds(0.5f);
		target.BroadcastMessage("OnImHit");
		BroadcastMessage("OnStopFire");
		target.BroadcastMessage("OnDeath");
		
	}
	
	
	public void OnBeingAttacked() {
		currentHealth = 0;
		Debug.Log("i'm dying!!!");
		StartCoroutine(GloriousDeath());
		
	}
	
	public IEnumerator GloriousDeath() {
		unitStatus = UnitStatus.DEAD;
		yield return new WaitForSeconds(1.0f);
		ResetMarineData();
		
	}
	
	public void ReloadWeapon() {
		if(currentMoves < 1)
			return;
		
		if(weapon.ReloadWeapon())
			currentMoves--;
	}
	
}
