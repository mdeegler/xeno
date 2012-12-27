using UnityEngine;
using System.Collections;

public class FlamerGun : MonoBehaviour {
	private GameObject fireball;
	private GameObject smoke;

	// Use this for initialization
	void Start () {
		fireball = transform.FindChild("Fireball Particle System").gameObject;
		smoke = transform.FindChild("Smoke Particle System").gameObject;
		OnStopFire();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnStartFire () {
		fireball.SetActive(true);
		smoke.SetActive(true);
	}

	public void OnStopFire () {
		fireball.SetActive(false);
		smoke.SetActive(false);
	}
}
