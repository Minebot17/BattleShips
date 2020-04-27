﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {

	public static readonly List<SettingValue> values = new List<SettingValue>();
	
	public static readonly BoolSettingValue SettingLogEvents = new BoolSettingValue("SettingLogEvents", false);
	public static readonly StringSettingValue SettingLanguageCode = new StringSettingValue("LanguageCode", "en");
	public static readonly StringSettingValue SettingNick = new StringSettingValue("Nick", "ip");

	public static void Save() {
		values.ForEach(val => val.Save());
	}

	public static void Load() {
		values.ForEach(val => val.Load());
	}
}
