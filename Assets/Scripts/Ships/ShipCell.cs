using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipCell {
    public Vector2Int positionOnShip;
    public ShipModule module;

    public ShipCell() {
    }

    public ShipCell(Vector2Int positionOnShip) {
        this.positionOnShip = positionOnShip;
    }
}
