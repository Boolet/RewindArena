using UnityEngine;
using System.Collections;

public class TransformManager : RewindManager {

	Timeline<TransformData> tLine = new Timeline<TransformData>();

	public override void OnNormalTimeForward(){
		tLine.Save (new TransformData (obj.transform), currentTime);
	}

	public override void OnBeginTimeControl(){

	}

	public override void OnContinueTimeControl(){
		TimePeriod<TransformData> currentPeriod =  tLine.AtTime (currentTime);
		if (currentPeriod.Position () == TimePosition.Begin) {
			obj.transform.position = currentPeriod.second.Value ().LoadPosition ();
			obj.transform.rotation = currentPeriod.second.Value ().LoadRotation ();
		} else if (currentPeriod.Position () == TimePosition.End) {
			obj.transform.position = currentPeriod.first.Value ().LoadPosition ();
			obj.transform.rotation = currentPeriod.first.Value ().LoadRotation ();
		} else {
			obj.transform.position = Vector3.Lerp (currentPeriod.first.Value ().LoadPosition (),
				currentPeriod.second.Value ().LoadPosition (), currentPeriod.LerpValue (currentTime));
			obj.transform.rotation = Quaternion.Lerp (currentPeriod.first.Value ().LoadRotation (),
				currentPeriod.second.Value ().LoadRotation (), currentPeriod.LerpValue (currentTime));
			//print ("Lerp position: " + currentPeriod.LerpValue (currentTime));
		}
	}

	public override void OnEndTimeControl(){
		//print ("Rewinding to: " + currentTime);
		tLine.RewindTo (currentTime);
	}

}
