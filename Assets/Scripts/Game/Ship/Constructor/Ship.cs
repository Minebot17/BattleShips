using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ship {
    public List<ShipModule> shipModules = new List<ShipModule>();

    public Ship() {
    }
}
