using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

//This component deletes itself after resolving, leaving a manager pointer on the object in its place.
//this should be put on objects that can be rewound that are already spawned at start.
public class RewindableObject : MonoBehaviour {

	[SerializeField] float maxTime = 5f;
	[SerializeField] bool saveTransform = true;
	[SerializeField] bool saveRigidBody = true;
	bool saveExistence = false;	//not finished
	[SerializeField] bool enableDisplay = false;
	[SerializeField] Vector3 displayOffset = Vector3.up;

	[System.Serializable] public class RewindSetupEvent : UnityEvent <ManagerPointer> {}
	[SerializeField] RewindSetupEvent onSetupComplete;

	List<RewindManager> managerList = new List<RewindManager> ();
	GameObject rewinderHolder;
	RewindOverseer overseer;
	ManagerPointer pointer;

	void Start(){
		rewinderHolder = new GameObject ();
		CreateRewindManagers ();
		CreateMasterRewinder ();
		CreateOverseerPointer ();
		CreateRewindDisplay ();
		SetupComplete ();
		Destroy (this);
	}

	//makes all of the rewind managers for the object
	void CreateRewindManagers(){
		if(saveTransform){
			TransformManager t = rewinderHolder.AddComponent<TransformManager> ();
			managerList.Add (t);
		}
		if (saveRigidBody) {
			RigidBodyManager r = rewinderHolder.AddComponent<RigidBodyManager> ();
			Rigidbody b = GetComponent<Rigidbody> ();
			r.Initialize (b, b.isKinematic);
			managerList.Add (r);
		}
		if (saveExistence) {
			ExistenceManager e = rewinderHolder.AddComponent<ExistenceManager> ();
			managerList.Add (e);
		}
		/*	//this is for all of the rewindable scripts that may be attached to the object
		foreach(RewindManager scr in GetComponents<RewindManager>()){
			managerList.Add (scr);
		}
		*/
	}

	//creates the rewind overseer that controls all of the managers
	void CreateMasterRewinder(){
		overseer = rewinderHolder.AddComponent<RewindOverseer> ();
		overseer.Initialize (maxTime, gameObject, managerList.ToArray());

		foreach(RewindManager m in managerList){
			m.SetOverseer (overseer);
		}
	}

	//make a manager pointer so that this object knows what's managing it
	void CreateOverseerPointer (){
		pointer = gameObject.AddComponent<ManagerPointer>();
		pointer.SetManager (overseer);
	}

	//create some text to float near the object and display the time on its clock
	void CreateRewindDisplay(){
		if (enableDisplay) {
			GameObject g = new GameObject ();
			TextMesh m = g.AddComponent<TextMesh> ();
			RewindDisplay d = g.AddComponent<RewindDisplay> ();
			d.Initialize (m, gameObject, overseer, displayOffset);
		}
	}

	//informs any delegates that this script is finished and will be destroyed.
	void SetupComplete(){
		if(onSetupComplete != null){
			onSetupComplete.Invoke(pointer);
		}
	}
}

//this class exists to maintain a reference from a rewindable game object to its rewind managers
public class ManagerPointer : MonoBehaviour {
	RewindOverseer overseer = null;

	void Start(){
		print ("Pointer created!");
	}

	public void SetManager(RewindOverseer o){
		if(overseer != null){
			Debug.LogError ("OVERRIDING OVERSEER POINTER IS ILLEGAL");
			return;
		}
		overseer = o;
	}

	public RewindOverseer GetOverseer(){
		return overseer;
	}

	/*
	public RewindManager[] GetManagers(){
		return overseer.GetComponents<RewindManager> ();
	}
	*/

	void OnDestroy(){
		//this case should only occur when the game or scene terminates.
		if (overseer == null)
			return;
		
		overseer.ObjectDestroyed ();
	}
}

public class RewindDisplay : MonoBehaviour{
	TextMesh displayMesh;
	GameObject rewindObject;
	RewindOverseer overseer;
	Vector3 offset;

	public void Initialize(TextMesh t, GameObject g, RewindOverseer m, Vector3 o){
		rewindObject = g;
		overseer = m;
		offset = o;
		displayMesh = t;
		displayMesh.anchor = TextAnchor.MiddleCenter;
		displayMesh.alignment = TextAlignment.Center;
		displayMesh.fontSize = 40;
		displayMesh.characterSize = 0.1f;
	}

	void Update(){
		FollowObject ();
		UpdateText ();
		FacePlayer ();
	}

	void FollowObject(){
		transform.position = rewindObject.transform.position + offset;
	}

	void UpdateText(){
		displayMesh.text = overseer.GetCurrentTime ().ToString();
		if(displayMesh.text.Length > 3){
			displayMesh.text = displayMesh.text.Substring (0, 3);
		}
	}

	void FacePlayer(){
		Vector3 targetVector = Camera.main.transform.position - transform.position;
		transform.forward = -targetVector.normalized;
	}
}
