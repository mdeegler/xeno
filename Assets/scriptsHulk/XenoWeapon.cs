using UnityEngine;
using System.Collections;

public class XenoWeapon : ScriptableObject { 
	public string weaponName;
	public int range;
	public int maxAmmo;
	public int currentAmmo;
	public bool reloadable = true;
	public bool jammed = false;
	public bool hasOverwatch = true;
	public DamageTypes damageType; 
		
	public enum DamageTypes {GUN, FLAME}

	public static XenoWeapon CreateGun() {
		XenoWeapon weapon = new XenoWeapon();
		weapon.weaponName = "PULSE RIFLE";
		weapon.range = 100;
		weapon.maxAmmo = 3;
		weapon.currentAmmo = weapon.maxAmmo;
		weapon.reloadable = true;
		weapon.damageType = DamageTypes.GUN;
		
		return weapon;
	}
	
	public static XenoWeapon CreateFlamer() {
		XenoWeapon weapon = new XenoWeapon();
		weapon.weaponName = "FLAME THROWER";
		weapon.range = 12;
		weapon.maxAmmo = 6;
		weapon.currentAmmo = weapon.maxAmmo;
		weapon.reloadable = false;
		weapon.hasOverwatch = false;
		weapon.damageType = DamageTypes.FLAME;
		
		return weapon;
	}
	
	public static XenoWeapon CreateWeapon(DamageTypes type) {
		if(type==DamageTypes.FLAME)
			return CreateFlamer();
		return CreateGun();
	}
	
	public bool FireWeapon() {
		if(currentAmmo <= 0)
			return false;
		currentAmmo--;
		return true;
	}
	
	public bool ReloadWeapon() {
		if(reloadable && currentAmmo < maxAmmo) {
			currentAmmo = maxAmmo;	
			return true;
		}
		return false;
	}
}
