﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase {

	public object Sender;
	public bool IsCancable;
	bool isUnsubscribe;

	public EventBase(object sender, bool isCancable) {
		if (GameSettings.SettingLogEvents.Value)
			Debug.Log("[Event] Sender: " + sender + " Type: " + this);
		Sender = sender;
		IsCancable = isCancable;
	}

	public bool IsCancel {
		get { return IsUnsubscribe; }
		set { IsUnsubscribe = IsCancable && value; }
	}
	
	public bool IsUnsubscribe { get; set; }
}
