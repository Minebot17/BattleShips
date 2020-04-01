using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public void Start() {
        OpenForEdit(ShipManagerStartGUI.openedShipName);
    }

    public void OpenForEdit(string shipName) {
        Ship ship = JsonUtility.FromJson<Ship>(File.ReadAllText(Application.streamingAssetsPath + "/ships/" + shipName));
        GameObject shipObject = new GameObject("Ship");
        for (int i = 0; i < ship.shipCells.Count; i++) {
            GameObject shipCell = Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), shipObject.transform);
            shipCell.name = "ShipCell " + ship.shipCells[i].positionOnShip.x + " " + ship.shipCells[i].positionOnShip.y;
            shipCell.transform.localPosition = new Vector3(ship.shipCells[i].positionOnShip.x, ship.shipCells[i].positionOnShip.y, 0);

            if (ship.shipCells[i].module != null) {
                GameObject cellModule = Instantiate(Resources.Load<GameObject>(
                    "Prefabs/" + ship.shipCells[i].module.type + "Module"), shipCell.transform);
                cellModule.transform.localPosition = new Vector3(0, 0, -0.1f);
            }
        }
    }
}
