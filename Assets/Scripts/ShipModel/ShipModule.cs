using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipModule {
    
    public Vector2Int mainPosition;
    public string type;

    public ShipModule() {
    }

    public ShipModule(Vector2Int mainPosition, string type) {
        this.mainPosition = mainPosition;
        this.type = type;
    }

    public Vector2Int GetSize() {
        return new Vector2Int(1, 1);
    }
}
