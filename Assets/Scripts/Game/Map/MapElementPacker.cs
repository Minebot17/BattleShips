using System;
using System.Collections;
using UnityEngine;

public class MapElementPacker : MonoBehaviour {
    private void Start() {
        StartCoroutine(WaitMapSpawn());
    }

    private IEnumerator WaitMapSpawn() {
        if (!Map.singleton || Map.singleton == null)
            yield return new WaitForSeconds(0.1f);

        transform.parent = Map.singleton.gameObject.transform;
        Destroy(this);
    }
}
