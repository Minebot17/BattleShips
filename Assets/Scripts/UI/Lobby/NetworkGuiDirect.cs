﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManagerCustom))]
public class NetworkGuiDirect : MonoBehaviour {
	public string IpAddress;
	public string Port;
	private bool _started;

	private void Start() {
		_started = false;
		IpAddress = "localhost";
		Port = "7777";
	}

	private void OnGUI() {
		if (!_started) {
			GUILayout.Label("Ip:");
			IpAddress = GUILayout.TextField(IpAddress, GUILayout.Width(100));
			GUILayout.Label("Port:");
			Port = GUILayout.TextField(Port, 5);

			if (GUILayout.Button("Connect")) {
				_started = true;
				NetworkManagerCustom.singleton.IsServer = false;
				IpAddress = IpAddress.Trim();
				Port = Port.Trim();
				NetworkManager.singleton.networkAddress = IpAddress.Equals("localhost") ? "127.0.0.1" : IpAddress;
				NetworkManager.singleton.networkPort = int.Parse(Port);
				NetworkManager.singleton.StartClient();
			}

			if (GUILayout.Button("Host FFA")) {
				IpAddress = IpAddress.Trim();
				Port = Port.Trim();
				NetworkManagerCustom.singleton.StartArguments.Add("gamemode:" + "ffa");
				_started = true;
				NetworkManager.singleton.networkPort = int.Parse(Port);
				NetworkManager.singleton.StartHost();
			}
			
			if (GUILayout.Button("Host Commands")) {
				IpAddress = IpAddress.Trim();
				Port = Port.Trim();
				NetworkManagerCustom.singleton.StartArguments.Add("gamemode:" + "commands");
				_started = true;
				NetworkManager.singleton.networkPort = int.Parse(Port);
				NetworkManager.singleton.StartHost();
			}
		}
		else {
			if (GUILayout.Button("Disconnect")) {
				string path = Application.dataPath + "/../" + "BattleShips.exe";
				System.Diagnostics.Process.Start(path);
				_started = false;
				NetworkManager.singleton.StopHost();
				Application.Quit();
			}
		}
	}
}
