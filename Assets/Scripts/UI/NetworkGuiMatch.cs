using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

[RequireComponent(typeof(NetworkManagerCustom))]
public class NetworkGuiMatch : MonoBehaviour {
    public string MatchName;
    private bool _started;
    private List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
    private NetworkMatch networkMatch;

    private void Start() {
        _started = false;
        networkMatch = gameObject.AddComponent<NetworkMatch>();
        StartCoroutine(UpdateMatches());
    }

    private IEnumerator UpdateMatches() {
        while (!_started) {
            yield return new WaitForSeconds(1f);
            networkMatch.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
        }
    }

    private void OnGUI() {
        if (!_started) {
            GUILayout.Label("Match list:");
            foreach (MatchInfoSnapshot matchSnapshot in matchList) {
                if (GUILayout.Button(matchSnapshot.name)) {
                    networkMatch.JoinMatch(matchSnapshot.networkId, "", "", "", 0, 0, OnMatchJoined);
                }
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Match name:");
            MatchName = GUILayout.TextField(MatchName, GUILayout.Width(100));

            if (GUILayout.Button("Host FFA")) {
                NetworkManagerCustom.singleton.StartArguments.Add("gamemode:" + "ffa");
                _started = true;
                networkMatch.CreateMatch(MatchName, 4, true, "", "", "", 0, 0, OnMatchCreate);
            }
			
            if (GUILayout.Button("Host Commands")) {
                NetworkManagerCustom.singleton.StartArguments.Add("gamemode:" + "commands");
                _started = true;
                networkMatch.CreateMatch(MatchName, 4, true, "", "", "", 0, 0, OnMatchCreate);
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

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
        if (success && matches != null && matches.Count > 0) {
            matchList = matches;
        }
        else if (!success) {
            Debug.LogError("List match failed: " + extendedInfo);
        }
    }

    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo) {
        if (success) {
            _started = true;
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            NetworkManagerCustom.singleton.IsServer = false;
            NetworkManager.singleton.StartClient(matchInfo);
        }
        else {
            Debug.LogError("Join match failed " + extendedInfo);
        }
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        if (success) {
            Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
            NetworkManager.singleton.StartHost(matchInfo);
        }
        else {
            Debug.LogError("Create match failed: " + extendedInfo);
        }
    }
}
