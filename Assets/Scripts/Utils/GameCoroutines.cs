using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameCoroutines : MonoBehaviour {

    public static GameCoroutines Singleton;
    private const int suicideSeconds = 5;

    private void Awake() {
        Singleton = this;
    }

    public static IEnumerator SuicideCoroutine(NetworkIdentity ship) {
        if (!ship)
            yield break;
        
        int timer = suicideSeconds;
        SpriteRenderer coreSprite = ship.GetComponent<ShipController>().GetAiCoreModule().transform.parent.GetComponent<SpriteRenderer>();
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new [] { new GradientColorKey(Color.white, 1f), new GradientColorKey(Color.red, 0f) }, 
            new [] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
        );

        if (NetworkManagerCustom.singleton.IsServer) {
            Transform shieldRenderer = ship.transform.Find("ShieldRenderer");
            if (shieldRenderer)
                shieldRenderer.GetChild(0).GetComponent<IDeath>().OnDead(null);
        }

        while (timer > 0) {
            if (!ship)
                yield break;
        
            timer--;
            coreSprite.color = gradient.Evaluate(timer / (float) suicideSeconds);
            yield return new WaitForSeconds(1f);
        }

        if (NetworkManagerCustom.singleton.IsServer)
            ExplosionManager.suicideExplosion.Explode(ship.gameObject.transform.position, ship);
    }

    public static IEnumerator DestroyAmmo(GameObject toDestroy, float afterTime) {
        yield return new WaitForSeconds(afterTime);
        
        if (!toDestroy)
            yield break;

        if (NetworkManagerCustom.singleton.IsServer)
            NetworkServer.Destroy(toDestroy);
        else
            Destroy(toDestroy);
    }
}