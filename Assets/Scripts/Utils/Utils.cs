using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class Utils {
    public static float sizeOfOne = 0.64f;
    public static System.Random rnd = new System.Random();
    public static LayerMask shipCellsMask = 1 << 8;

    [Serializable]
    public class MessagesList : List<MessageBase> { }

    [Serializable]
    public class StringList : List<string> { }

    [Serializable]
    public class MultyStringList : List<List<string>> { }
	
    [Serializable]
    public class Vector3List : List<Vector3> { }
	
    [Serializable]
    public class IntegerList : List<int> {  }
    
    public static Vector2 ToVector2(this Vector3 vec) {
        return new Vector2(vec.x, vec.y);
    }
    
    public static Vector3 ToVector3(this Vector2 vec) {
        return new Vector3(vec.x, vec.y, 0);
    }
    
    public static Vector3 ToVector3(this Vector2 vec, float z) {
        return new Vector3(vec.x, vec.y, z);
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
    
    public static Color ToColor(this int HexVal) {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color(R, G, B, 255);
    }
    
    public static int ToHex(this Color color) {
        int hex = 0;
        hex += (int)(color.b * 255);
        hex += (int)(color.g * 255) << 8;
        hex += (int)(color.r * 255) << 16;
        return hex;
    }

    public static IEnumerable<Type> FindChildesOfType(Type parent) {
        return typeof(Utils).Assembly.GetTypes().Where(t => t.IsSubclassOf(parent));
    }
    
    public static U Get<T, U>(this Dictionary<T, U> dict, T key) where U : class {
        U val;
        dict.TryGetValue(key, out val);
        return val;
    }

    public static void SpawnPointer(Player from, Player to) {
        GameObject enemyPointer = MonoBehaviour.Instantiate(
        NetworkManagerCustom.singleton.enemyPointerPrefab, GameObject.Find("Canvas").transform);
        enemyPointer.GetComponent<EnemyPointer>().Target = to.GetState<GameState>().ShipIdentity.Value.gameObject;
        enemyPointer.GetComponentInChildren<Image>().color = 
            NetworkManagerCustom.singleton.gameMode.GetEnemyPointerColor(from, to).ToColor();
    }

    public static Player GetPlayerFromIdentity(NetworkIdentity identity) {
        return Players.GetPlayer(identity.clientAuthorityOwner);
    }

    public static void MarkServerChange(this Rigidbody2D rigidbody) { // TODO не работает со взрывами
        NetworkSyncVelocity syncVelocity = rigidbody.gameObject.GetComponent<NetworkSyncVelocity>();
        if (syncVelocity) {
            syncVelocity.LastVelocity = rigidbody.velocity;
            syncVelocity.TargetMarkChangeVelocity(rigidbody.GetComponent<NetworkIdentity>().clientAuthorityOwner, rigidbody.velocity);
        }
    }
}