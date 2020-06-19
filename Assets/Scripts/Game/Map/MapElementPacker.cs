using System;
using System.Collections;
using UnityEngine;

public class MapElementPacker : MonoBehaviour {
    void Start() {
        StartCoroutine(WaitMapSpawn());
    }

    IEnumerator WaitMapSpawn() {
        if (!Map.singleton || Map.singleton == null)
            yield return new WaitForSeconds(0.1f);

        transform.parent = Map.singleton.gameObject.transform;
        Destroy(this);
    }
}
