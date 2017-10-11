using UnityEngine;
using System.Collections;

public class RigidBodyManager : RewindManager{

	Timeline<RigidBodyData> tLine = new Timeline<RigidBodyData>();
	Rigidbody body;
	bool isKinematic;

	Vector3 currentVel;
	Vector3 currentAngVel;

	/*
	public void Register(GameObject obj, Rigidbody newBody, bool kinematic){
		body = newBody;
		isKinematic = kinematic;
		Register (obj);
	}
	*/

	public void Initialize(Rigidbody newBody, bool kinematic){
		body = newBody;
		isKinematic = kinematic;
	}

	public override void OnNormalTimeForward(){
		tLine.Save (new RigidBodyData (body), currentTime);
	}

	public override void OnBeginTimeControl(){
		currentVel = body.velocity;
		currentAngVel = body.angularVelocity;
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		body.isKinematic = true;
	}

	public override void OnContinueTimeControl(){
		TimePeriod<RigidBodyData> currentPeriod = tLine.AtTime (currentTime);
		if (currentPeriod.Position () == TimePosition.Begin) {
			currentVel = currentPeriod.second.Value ().LoadVelocity ();
			currentAngVel = currentPeriod.second.Value ().LoadAngularVelocity ();
		} else if (currentPeriod.Position () == TimePosition.End) {
			currentVel = currentPeriod.first.Value ().LoadVelocity ();
			currentAngVel = currentPeriod.first.Value ().LoadAngularVelocity ();
		} else {
			currentVel = Vector3.Lerp (currentPeriod.second.Value ().LoadVelocity (),
				currentPeriod.first.Value ().LoadVelocity (), currentPeriod.LerpValue (currentTime));
			currentAngVel = Vector3.Lerp (currentPeriod.second.Value ().LoadAngularVelocity (),
				currentPeriod.first.Value ().LoadAngularVelocity (), currentPeriod.LerpValue (currentTime));
		}
	}

	public override void OnEndTimeControl(){
		if(!isKinematic){
			body.isKinematic = false;
		}
		body.velocity = currentVel;
		body.angularVelocity = currentAngVel;
		print ("Rewinding to: " + currentTime);
		tLine.RewindTo (currentTime);
	}

}
