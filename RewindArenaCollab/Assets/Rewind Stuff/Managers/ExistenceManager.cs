using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExistenceManager : RewindManager {

	Timeline<ExistenceData> tLine = new Timeline<ExistenceData>();
	bool assumeCreatedAtTimelineStart;
	bool assumeLastStateAtTimelineEnd;


	public void Initialize(bool createdAtStart, bool maintainAtEnd){
		assumeCreatedAtTimelineStart = createdAtStart;
		assumeLastStateAtTimelineEnd = maintainAtEnd;
	}

	public override void OnNormalTimeForward(){
		tLine.Save (new ExistenceData(obj.activeInHierarchy), currentTime);
	}

	public override void OnBeginTimeControl(){
		
	}

	public override void OnContinueTimeControl(){
		TimePeriod<ExistenceData> currentPeriod = tLine.AtTime (currentTime);
		if (currentPeriod.Position() == TimePosition.Begin){
			if (currentPeriod.LerpValue(currentTime) == 1)
				obj.SetActive(currentPeriod.second.Value().Load());
			else
				obj.SetActive(assumeCreatedAtTimelineStart);
		} else if (currentPeriod.Position() == TimePosition.End){
			if (currentPeriod.LerpValue(currentTime) == 0)
				obj.SetActive(currentPeriod.first.Value().Load());
			else
				obj.SetActive(assumeLastStateAtTimelineEnd && currentPeriod.first.Value().Load());
		} else {
			if (currentPeriod.LerpValue(currentTime) == 1)
				obj.SetActive(currentPeriod.second.Value().Load());
			else
				obj.SetActive(currentPeriod.first.Value().Load());
		}
	}

	public override void OnEndTimeControl(){
		
	}
}
