using UnityEngine;
using System.Collections;

/*
 * The rewind manager is an object that persists through the destruction of the object it manages,
 * and also can exist before the object it manages even spawns.
 * 
 * For convenience, there should be a way for managers to be created for every rewindable object in existence
 * when the scene starts, and to make a new manager for objects that are spawned mid-scene.
 * 
 * Objects that have custom scripts need to have a reference to this object?
 */

public class RewindManager : MonoBehaviour{

	//------
	private RewindOverseer overseer = null;

	public void SetOverseer(RewindOverseer o){
		if(overseer != null){
			Debug.LogError ("OVERRIDING OVERSEER IS ILLEGAL");
			return;
		}
		overseer = o;
	}

	protected float maxTime 		{get {return overseer.GetMaxTime ();}}
	protected float endOfTimeline 	{get {return overseer.GetEndOfTimeline ();}}
	protected float currentTime		{get {return overseer.GetCurrentTime ();}}
	protected GameObject obj		{get {return overseer.GetControlledObj ();}}
	protected bool objectExists		{get {return overseer.GetObjectExists ();}}

	bool timeControl;
	//-------

	public virtual void OnNormalTimeForward (){}
	public virtual void OnBeginTimeControl (){}
	public virtual void OnContinueTimeControl (){}
	public virtual void OnEndTimeControl (){}
	public virtual void OnObjectDestroyed(){}
	public virtual void OnObjectSpawned(){}
}
