using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipModule {

    public Vector2Int position;
    public string prefabName;

    public ShipModule() {
    }

    public ShipModule(Vector2Int position, string prefabName) {
        this.position = position;
        this.prefabName = prefabName;
    }
}
