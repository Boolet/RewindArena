using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This behavior should be attached to the player game object.
 * 
 * It may need to be on a child of the player game object that has its own
 * collider in order to detect guns around itself to interact with.
 * 
 * 
 */

[RequireComponent(typeof(Collider))]	//this is the detection collider, defining the player's reach when picking up guns.
public class GunHolder : MonoBehaviour {

	List<Gun> nearbyGuns = new List<Gun>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*-----------------------------------------------------------------------------------------------
	 * Called when any other collider enters the trigger area on this game object
	 * 
	 * Stores a reference to the Gun script on the entering object if it exists
	 ----------------------------------------------------------------------------------------------*/
	void OnTriggerEnter(Collider other){
		Gun collidedGun = null;
		if ((collidedGun = other.transform.GetComponent<Gun>()) != null)
			nearbyGuns.Add(collidedGun);
	}

	/*-----------------------------------------------------------------------------------------------
	 * Called when any other collider leaves the trigger area on this game object
	 ----------------------------------------------------------------------------------------------*/
	void OnTriggerExit(Collider other){
		Gun collidedGun = null;
		if ((collidedGun = other.transform.GetComponent<Gun>()) != null)
			if (nearbyGuns.Contains(collidedGun))
				nearbyGuns.Remove(collidedGun);
	}
}
