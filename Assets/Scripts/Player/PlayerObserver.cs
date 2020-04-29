using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerObserver : MonoBehaviour {

    private List<GameObject> players;
    private int currentIndex;
    
    private void Start() {
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        currentIndex = Utils.rnd.Next(players.Count);
        CameraFollower.singleton.Target = players[currentIndex].transform;
        PlayerInputHandler.singleton.screenButton.GetComponent<Image>().raycastTarget = true;
        PlayerInputHandler.singleton.screenButton.GetComponent<Button>().onClick.AddListener(OnScreenClick);
    }
    
    public void OnScreenClick() {
        players.RemoveAll(g => !g);
        currentIndex = currentIndex + 1 >= players.Count ? 0 : currentIndex + 1;
        CameraFollower.singleton.Target = players[currentIndex].transform;

        // TODO лагучая херня отсылающая кучу пакетов за клик. Закешировать команды на клиенте
        EnemyPointer[] pointers = FindObjectsOfType<EnemyPointer>();
        foreach (EnemyPointer pointer in pointers)
            Destroy(pointer.gameObject);

        foreach (GameObject player in players)
            if (player != players[currentIndex])
                new EnemyPointerColorMessage(players[currentIndex].GetComponent<NetworkIdentity>(), player.GetComponent<NetworkIdentity>()).SendToServer();
    }
}
