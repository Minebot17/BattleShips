using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour {

    private Text text;
    
    private void Start() {
        text = GetComponent<Text>();
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer() {
        int timer = Players.GetGlobal().RoundTime.Value;
        
        while (timer > 0) {
            timer--;
            text.text = timer + "";
            if (timer < 20)
                text.color = Color.red;;
            
            yield return new WaitForSeconds(1f);
        }

        if (NetworkManagerCustom.singleton.IsServer)
            NetworkManagerCustom.singleton.RoundOver();
    }
}
