using System;
using System.Collections;
using UnityEngine;

public class MapElementPacker : MonoBehaviour {
    private void Start() {
        if (!GetComponentInParent<Map>())
            StartCoroutine(WaitMapSpawn());
        else
            Destroy(this);
    }

    private IEnumerator WaitMapSpawn() {
        if (!Map.singleton || Map.singleton == null)
            yield return new WaitForSeconds(0.1f);

        transform.parent = Map.singleton.gameObject.transform;
        Destroy(this);
    }
}
