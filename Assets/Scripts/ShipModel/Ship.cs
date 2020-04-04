using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ship {
    public List<ShipCell> shipCells = new List<ShipCell>();

    public Ship() {
    }
}
