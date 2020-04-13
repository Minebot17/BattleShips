using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerObserver : MonoBehaviour {

    private GameObject[] players;
    private int currentIndex;
    
    private void Start() {
        players = GameObject.FindGameObjectsWithTag("Player");
        currentIndex = Utils.rnd.Next(players.Length);
        CameraFollower.singleton.Target = players[currentIndex].transform;
        PlayerInputHandler.singleton.screenButton.GetComponent<Image>().raycastTarget = true;
        PlayerInputHandler.singleton.screenButton.GetComponent<Button>().onClick.AddListener(OnScreenClick);
    }
    
    public void OnScreenClick() {
        currentIndex = currentIndex + 1 == players.Length ? 0 : currentIndex + 1;
        CameraFollower.singleton.Target = players[currentIndex].transform;
    }
}
