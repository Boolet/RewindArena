using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalRewindManager{

	static List<RewindManager> managers = new List<RewindManager> ();

	/*
	public static void RegisterRewinder(RewindManager m){
		if(!managers.Contains(m)){
			managers.Add (m);
		}
	}

	public static void ResignRewinder(RewindManager m){
		if(managers.Contains(m)){
			managers.Remove (m);
		}
	}

	public static void RewindAll(float rewindAmount){
		foreach(RewindManager m in managers){
			m.AdjustTime (rewindAmount);
		}
	}
	*/

}
