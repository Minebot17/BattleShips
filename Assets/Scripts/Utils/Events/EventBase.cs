﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase {

	public object Sender;
	public bool IsCancable;
	private bool isCancel;

	public EventBase(object sender, bool isCancable) {
		if (GameSettings.SettingLogEvents.Value)
			Debug.Log("[Event] Sender: " + sender + " Type: " + this);
		Sender = sender;
		IsCancable = isCancable;
	}

	public bool IsCancel {
		get { return isCancel; }
		set { isCancel = IsCancable && value; }
	}
}
