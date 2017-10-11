using UnityEngine;
using System.Collections;

public class RewindOverseer : MonoBehaviour {

	RewindManager[] managers;	//all of the object's rewind managers

	protected float maxTime = 5f;
	protected float endOfTimeline = 0f;
	protected float currentTime = 0f;
	protected GameObject controlledObj;
	protected bool objectExists = true;

	bool timeControl;	//this determines if the whole rewinding behavior is enabled!

	/*
	public void Register(GameObject child){
		controlledObj = child;
		//GlobalRewindManager.RegisterRewinder (this);
	}
	*/

	public void Initialize(float newMaxTime, GameObject child, RewindManager[] overseenManagers){
		maxTime = newMaxTime;
		controlledObj = child;
		managers = overseenManagers;
	}

	public bool IsRewinding(){
		return timeControl;
	}

	public float GetCurrentTime(){
		return currentTime;
	}

	public float GetMaxTime(){
		return maxTime;
	}

	public float GetEndOfTimeline(){
		return endOfTimeline;
	}

	public GameObject GetControlledObj(){
		return controlledObj;
	}

	public bool GetObjectExists(){
		return objectExists;
	}

	//updates the object's timeline with a normal change in time; this assumes the object is not rewinding
	void NormalTimeForward(float deltaTime){
		if (currentTime >= maxTime) {
			SetTimeControl (true);	//CAREFUL!
			return;
		}
		currentTime += deltaTime;
		endOfTimeline = currentTime;	//theck this too
		OnNormalTimeForward ();
	}

	//enables or disables the rewinding functionality of an object
	public void SetTimeControl(bool isEnabled){
		if (timeControl && !isEnabled) {
			endOfTimeline = currentTime;	//check this
			OnEndTimeControl ();
		} else if (!timeControl && isEnabled) {
			OnBeginTimeControl ();
		}
		timeControl = isEnabled;
	}

	//this is what is used for rewinding the object after time control has been enabled
	public void AdjustTime(float deltaTime){	//note that it takes the CHANGE IN TIME instead of the current time
		if(timeControl){
			currentTime += deltaTime;
			currentTime = Mathf.Clamp (currentTime, 0, endOfTimeline);	//check this one also!
			currentTime = Mathf.Clamp (currentTime, 0, maxTime);
		}
	}

	void Update(){		//built with Update in mind; testing fixedUpdate
		if (!timeControl) {
			NormalTimeForward (Time.deltaTime);
		} else {
			OnContinueTimeControl ();
		}
		//print (currentTime);
	}

	public void ObjectDestroyed(){
		OnObjectDestroyed ();
		controlledObj = null;
	}

	public void ObjectSpawned(GameObject newObj){
		controlledObj = newObj;
		OnObjectSpawned ();
	}

	void OnDestroy(){
		//GlobalRewindManager.ResignRewinder (this);
	}

	protected void OnNormalTimeForward 		(){foreach (RewindManager m in managers) {m.OnNormalTimeForward ();}}
	protected void OnBeginTimeControl 		(){foreach (RewindManager m in managers) {m.OnBeginTimeControl ();}}
	protected void OnContinueTimeControl 	(){foreach (RewindManager m in managers) {m.OnContinueTimeControl ();}}
	protected void OnEndTimeControl 		(){foreach (RewindManager m in managers) {m.OnEndTimeControl ();}}
	protected void OnObjectDestroyed		(){foreach (RewindManager m in managers) {m.OnObjectDestroyed ();}}
	protected void OnObjectSpawned			(){foreach (RewindManager m in managers) {m.OnObjectSpawned ();}}
}
