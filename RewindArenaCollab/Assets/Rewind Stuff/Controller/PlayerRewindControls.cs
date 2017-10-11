using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class PlayerRewindControls : MonoBehaviour {

	/*
	 * This class currently does no checking to see if the game is overriding the rewind system on the object and
	 * trying to keep it on or off for any reason. If that mechanic is going to be added, this will need to check
	 * for such cases.
	 */

	[SerializeField] bool multiTarget;	//whether the player can rewind multiple objects at once
	[SerializeField] float maxBuffer = 1.5f;	//the maximum time displacement the player's scrubber can be from current object time
	[SerializeField] float maxScrollPerUpdate = 0.025f;	//maximum time change per frame; needs to be adjusted for frame rate
	[SerializeField] float scrollFactor = 0.1f;	//multiplier for the values returned by the scroll wheel (player-adjustable)

	float scrollBuffer = 0f;
	RewindOverseer currentTarget;

	void Update(){
		Selector ();
		Rewinder ();
	}

	//--------------------------------------
	// The functionality for player-input selection of what to rewind
	//--------------------------------------

	//manages the slection of rewindable objects
	void Selector(){
		RewindOverseer o = null;
		if (Input.GetMouseButtonDown (1)) {
			o = RaycastForRewinder ();
		}
		if(o != null){
			SelectTarget (o);
		}
	}

	//performs the selection of a new target, or the reselection of an old target
	void SelectTarget(RewindOverseer o){
		if (multiTarget) {
			MultiSelect (o);
		} else {
			SingleSelect (o);
		}
	}

	//executes the behavior for single-target rewinding. If the player pauses a new thing, the old thing will unpause.
	void SingleSelect(RewindOverseer o){
		if(currentTarget == o){										//if the object is already the selected object...
			currentTarget.SetTimeControl(!currentTarget.IsRewinding());	//toggle whether the object is rewinding.
		} else {													//otherwise...
			if(currentTarget != null)
				currentTarget.SetTimeControl (false);					//stop the old target rewinding,
			currentTarget = o;											//set a new target,
			currentTarget.SetTimeControl (true);						//begin rewinding the new target.
		}
	}

	/*
	 * executes the behavior for multi-target rewinding. If the player pauses a new thing, the old thing will
	 * remain paused until the player selects it again and then right-clicks it once more.
	 */
	void MultiSelect(RewindOverseer o){
		if (currentTarget == o) {
			currentTarget.SetTimeControl (!currentTarget.IsRewinding ());
		} else {
			currentTarget = o;
			currentTarget.SetTimeControl (true);
		}
	}

	//does a raycast along transform.forward and tries to find a ManagerPointer on the target. Returns the associated overseer.
	RewindOverseer RaycastForRewinder(){
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		ManagerPointer p = null;
		if (Physics.Raycast (ray, out hit, 100)) {	//do a raycast...
			p = hit.collider.gameObject.GetComponent<ManagerPointer> ();
		}
		return p == null ? null : p.GetOverseer();
	}

	//--------------------------------------
	// The functionality for player-input rewinding
	//--------------------------------------

	//OLD VERSION. Gotta clean it up.
	void Rewinder(){
		if (currentTarget == null || !currentTarget.IsRewinding())
			return;

		Vector2 scrollAmount = Input.mouseScrollDelta;
		float rewindAmount = scrollAmount.y * scrollFactor;
		scrollBuffer += rewindAmount;

		if(scrollBuffer == 0){
			return;
		} else if(scrollBuffer < 0){
			rewindAmount = Mathf.Max (scrollBuffer, -maxScrollPerUpdate);
		} else if(scrollBuffer > 0){
			rewindAmount = Mathf.Min (scrollBuffer, maxScrollPerUpdate);
		}
		scrollBuffer -= rewindAmount;
		scrollBuffer = Mathf.Clamp (scrollBuffer, -maxBuffer, maxBuffer);

		currentTarget.AdjustTime (rewindAmount);
	}
}
