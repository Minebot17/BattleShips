using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class Utils {
    public static float sizeOfOne = 0.64f;
    public static System.Random rnd = new System.Random();
    
    public static Vector2 ToVector2(this Vector3 vec) {
        return new Vector2(vec.x, vec.y);
    }
    
    public static Vector3 ToVector3(this Vector2 vec) {
        return new Vector3(vec.x, vec.y, 0);
    }

    public static int RoundSinged(float number) {
        return (int)(number > 0 ? Mathf.Floor(number) : Mathf.Ceil(number));
    }

    public static string CreateEmptyShip() {
        Ship ship = new Ship();
        ship.shipModules.Add(new ShipModule(Vector2Int.zero, "AICoreModule"));
        return JsonUtility.ToJson(ship);
    }

    public static GameObject DeserializeShipFromJson(string json, bool withController = false) {
        GameObject shipObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Ship"));
        shipObject.GetComponent<ShipController>().enabled = withController;
        shipObject.name = "Ship";
        shipObject.transform.localPosition = Vector3.zero;

        DeserializeShipPartsFromJson(shipObject, json);

        return shipObject;
    }

    public static void DeserializeShipPartsFromJson(GameObject shipObject, string json) {
        Ship ship = JsonUtility.FromJson<Ship>(json);
        for (int i = 0; i < ship.shipModules.Count; i++) {
            Vector2Int position = ship.shipModules[i].position;
            GameObject shipCell = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), shipObject.transform);
            GameObject module = MonoBehaviour.Instantiate(
            Resources.Load<GameObject>("Prefabs/Modules/" + ship.shipModules[i].prefabName), shipCell.transform);
            module.name = ship.shipModules[i].prefabName;
            module.transform.localPosition = new Vector3(0, 0, -0.1f);
            shipCell.name = "ShipCell " + position.x + " " + position.y;
            shipCell.transform.localPosition = new Vector3(position.x * sizeOfOne, position.y * sizeOfOne, 0);
        }
    }

    public static string SerializeShip(GameObject shipObject) {
        Ship ship = new Ship();
        for (int i = 0; i < shipObject.transform.childCount; i++) {
            if (!shipObject.transform.GetChild(i).name.StartsWith("ShipCell"))
                continue;

            GameObject shipCell = shipObject.transform.GetChild(i).gameObject;
            string[] splittedName = shipCell.name.Split(' ');
            Vector2Int position = new Vector2Int(int.Parse(splittedName[1]), int.Parse(splittedName[2]));
            ship.shipModules.Add(new ShipModule(position, shipCell.transform.GetChild(0).name));
        }

        return JsonUtility.ToJson(ship);
    }
}