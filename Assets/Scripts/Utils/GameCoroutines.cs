using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameCoroutines : MonoBehaviour {

    public static GameCoroutines Singleton;
    private const int suicideSeconds = 5;

    private void Awake() {
        Singleton = this;
    }

    public static IEnumerator SuicideCoroutine(NetworkIdentity ship) {
        int timer = suicideSeconds;
        SpriteRenderer coreSprite = ship.GetComponent<ShipController>().GetAiCoreModule().transform.parent.GetComponent<SpriteRenderer>();
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new [] { new GradientColorKey(Color.white, 1f), new GradientColorKey(Color.red, 0f) }, 
            new [] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
        );

        while (timer > 0) {
            timer--;
            coreSprite.color = gradient.Evaluate(timer / (float) suicideSeconds);
            yield return new WaitForSeconds(1f);
        }

        if (NetworkManagerCustom.singleton.IsServer)
            ExplosionManager.suicideExplosion.Explode(ship.gameObject.transform.position, ship);
    }
}