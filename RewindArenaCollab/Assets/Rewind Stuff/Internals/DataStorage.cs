using UnityEngine;
using System.Collections;

//Maybe make a superclass that can just read from game objects in general?

public interface GameObjectData{
	void Load (GameObject g);
}

public class TransformData {	//I may need to implement a version for local transforms.
	Vector3 position;
	Quaternion rotation;

	public TransformData(Transform t){
		position = t.position;
		rotation = t.rotation;
	}

	public void Load(Transform t){
		t.position = position;
		t.rotation = rotation;
	}

	public Vector3 LoadPosition(){
		return position;
	}

	public Quaternion LoadRotation(){
		return rotation;
	}
}

public class RigidBodyData {
	Vector3 linearVelocity;
	Vector3 rotationalVelocity;

	public RigidBodyData(Rigidbody r){
		linearVelocity = r.velocity;
		rotationalVelocity = r.angularVelocity;
	}

	public void Load(Rigidbody r){
		r.velocity = linearVelocity;
		r.angularVelocity = rotationalVelocity;
	}

	public Vector3 LoadVelocity(){
		return linearVelocity;
	}

	public Vector3 LoadAngularVelocity(){
		return rotationalVelocity;
	}
}

public class ExistenceData{
	bool existsThisFrame;

	public ExistenceData(bool exists){
		existsThisFrame = exists;
	}

	public bool Load(){
		return existsThisFrame;
	}
}
