using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestSystem : MonoBehaviour {

	[SerializeField] float maxBuffer = 1.5f;
	[SerializeField] float maxScrollPerUpdate = 0.025f;
	[SerializeField] float scrollFactor = 0.1f;

	//smoothing features - optional
	float scrollBuffer = 0f;
	//^^^

	RewindOverseer overseer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		SelectTarget ();
		SmoothRewind();
	}

	void SelectTarget(){
		if(Input.GetMouseButtonDown(1)){	//on right click...
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100)){	//do a raycast...
				ManagerPointer p = hit.collider.gameObject.GetComponent<ManagerPointer> ();
				if (p != null) {	//if the object can be rewound...
					if (overseer == p.GetOverseer ()) {	//if the object is already the selected object...
						//unpause and release
						Clear ();
					} else if (overseer == null) {	//if no object is selected...
						//add and pause
						Select (p);
					} else {	//if some other object is selected...
						Clear ();	//unpause and deselect the old object
						Select (p);	//and add and pause the new object
					}
				}
			}
		}
	}

	void Clear(){
		UnpauseObject(overseer);
		overseer = null;
		scrollBuffer = 0f;
	}

	void Select(ManagerPointer p){
		scrollBuffer = 0f;
		overseer = p.GetOverseer ();
		PauseObject (overseer);
	}

	void PauseObject(RewindOverseer o){
		o.SetTimeControl (true);
	}

	void UnpauseObject(RewindOverseer o){
		o.SetTimeControl (false);
	}

	void Rewind(){
		Vector2 scrollAmount = Input.mouseScrollDelta;
		float rewindAmount = scrollAmount.y * scrollFactor;
		//print (rewindAmount);
		overseer.AdjustTime (rewindAmount);

	}

	void SmoothRewind(){
		if (overseer == null)
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

		overseer.AdjustTime (rewindAmount);
	}
}
