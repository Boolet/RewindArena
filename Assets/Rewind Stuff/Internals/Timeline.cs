using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineObject<T>{
	T value;
	float timestamp;

	public TimelineObject(T storedObject, float time){
		value = storedObject;
		timestamp = time;
	}

	public T Value(){
		return value;
	}

	public float Timestamp(){
		return timestamp;
	}
}

public enum TimePosition{
	Begin, Normal, End
};

public class TimePeriod<T>{
	public readonly TimelineObject<T> first;
	public readonly TimelineObject<T> second;

	public TimePeriod(TimelineObject<T> f, TimelineObject<T> s){
		first = f;
		second = s;
	}

	public TimePosition Position(){
		if (first == null)
			return TimePosition.Begin;
		if (second == null)
			return TimePosition.End;
		return TimePosition.Normal;
	}

	public float LerpValue(float time){
		if (Position() == TimePosition.Begin){
			return time == second.Timestamp() ? 1 : 0;
		}
		if (Position() == TimePosition.End){
			return time == first.Timestamp() ? 0 : 1;
		}
		return (time - first.Timestamp ()) / (second.Timestamp () - first.Timestamp());
	}
}

public class TimelineException : System.Exception{
	public TimelineException () : base (){ }
	public TimelineException (string message) : base(message){ }
	public TimelineException (string message, System.Exception inner) : base(message, inner) { }

	protected TimelineException(System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) { }
}

public class Timeline<T>{

	List<TimelineObject<T>> saves = new List<TimelineObject<T>>();

	public void Save(T obj, float timeStamp){
		Save (new TimelineObject<T> (obj, timeStamp));
	}

	public void Save(TimelineObject<T> obj){
		if (saves.Count > 0 && obj.Timestamp () <= saves[saves.Count - 1].Timestamp ())
			throw new TimelineException ("Saving non-sequential timestamp:" +
				" Previous - " + saves[saves.Count - 1].Timestamp () + " / Tried - " + obj.Timestamp () + ".");
		saves.Add (obj);
	}

	public TimelineObject<T> LoadMostRecent(){
		if (saves.Count == 0)
			throw new TimelineException ("Loading from empty timeline.");
		return saves[saves.Count - 1];
	}

	public TimePeriod<T> AtTime(float time){
		if(saves.Count == 0)
			throw new TimelineException ("Rewinding through empty timeline.");
		if (time >= saves[saves.Count - 1].Timestamp ()) {
			return new TimePeriod<T> (saves[saves.Count - 1], null);	//after last save
		}

		int current = saves.Count - 1;
		while(time < saves[current].Timestamp() && current > 0){
			--current;
		}
		if (current == 0) {
			return new TimePeriod<T> (null, saves[0]);	//time is before first save
		}
		return new TimePeriod<T> (saves[current], saves[current + 1]);	//between saves
	}

	public void RewindTo(float time){
		int current = saves.Count - 1;
		while(time < saves[current].Timestamp() && current > 0){
			--current;
		}
		saves.RemoveRange (current, saves.Count - current);
	}
}
