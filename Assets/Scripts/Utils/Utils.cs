using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class Utils {
    public static readonly bool debug = false;
    public static readonly float sizeOfOne = 0.64f;
    public static readonly System.Random rnd = new System.Random();
    public static readonly LayerMask shipCellsMask = 1 << 8;
    public static readonly LayerMask defaultMask = LayerMask.GetMask("Default");
    public static readonly LayerMask mapMask = LayerMask.GetMask("Map");
    public static readonly LayerMask shieldMask = LayerMask.GetMask("Shield");
    public static readonly Vector2Int[] neighborBypassOrder = { 
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };
    
    private static Vector2Int[][] externalPointsFromSide = new Vector2Int[neighborBypassOrder.Length][];

    static Utils() {
        for (int i = 0; i < neighborBypassOrder.Length; i++) {
            Vector2[] p = {
                new Vector2(1, 1), 
                new Vector2(1, -1)
            };
            p[0] = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(neighborBypassOrder[i].y, neighborBypassOrder[i].x)) * p[0];
            p[1] = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(neighborBypassOrder[i].y, neighborBypassOrder[i].x)) * p[1];
            externalPointsFromSide[i] = new []{ new Vector2Int((int)Mathf.Round(p[0].x), (int)Mathf.Round(p[0].y)), new Vector2Int((int)Mathf.Round(p[1].x), (int)Mathf.Round(p[1].y)) };
        }
    }

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
    
    public static Vector2 ToVector2(this Vector2Int vec) {
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

    public static Ship DeserializeShipPartsFromJson(GameObject shipObject, string json) {
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

        return ship;
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
        enemyPointer.GetComponent<EnemyPointer>().Target = to.GetState<CommonState>().ShipIdentity.Value.gameObject;
        enemyPointer.GetComponentInChildren<Image>().color = 
            NetworkManagerCustom.singleton.gameMode.GetEnemyPointerColor(from, to).ToColor();
    }

    public static void MarkServerChange(this Rigidbody2D rigidbody) {
        NetworkSyncVelocity syncVelocity = rigidbody.gameObject.GetComponent<NetworkSyncVelocity>();
        if (!syncVelocity) 
            return;
        
        syncVelocity.LastVelocity = rigidbody.velocity;
        syncVelocity.TargetMarkChangeVelocity(rigidbody.GetComponent<NetworkIdentity>().clientAuthorityOwner, rigidbody.velocity);
    }
    
    /// <summary>
    /// Вычисляет коэффициенты общего уравнения эллипса в зависимости от строения корабля для энерго-щита
    /// </summary>
    public static double[] CalculateShieldEllipseVariables(List<Vector2Int> modulesPosition) {
        // Выделение внешних угловых точек корабля
        Vector2 centerPoint = new Vector2();
        HashSet<Vector2Int> strokePoints = new HashSet<Vector2Int>();
        foreach (Vector2Int modulePosition in modulesPosition) {
            centerPoint += modulePosition.ToVector2() * sizeOfOne;
            List<int> freeSides = new List<int>();

            for (int i = 0; i < neighborBypassOrder.Length; i++) {
                if (!modulesPosition.Contains(modulePosition + neighborBypassOrder[i]))
                    freeSides.Add(i);
            }

            foreach (int freeSide in freeSides) {
                strokePoints.Add(externalPointsFromSide[freeSide][0] + modulePosition * 2);
                strokePoints.Add(externalPointsFromSide[freeSide][1] + modulePosition * 2);
            }
        }
        centerPoint /= modulesPosition.Count;

        // Создание выпуклой фигуры из угловых точек
        List<Vector2Int> convexPoints = ConvexHull.GetConvexHull(strokePoints.ToList());
        List<Vector2> ellipsePoints = new List<Vector2>();
        for (int i = 0; i < convexPoints.Count; i++)
            ellipsePoints.Add(convexPoints[i].ToVector2() * (sizeOfOne/2));

        if (ellipsePoints.Count == 4)
            ellipsePoints.Add(new Vector2(0, ellipsePoints.Max(p => p.y) / (Mathf.Sqrt(2f)/2f)));

        if (debug) {
            GameObject shipObj = GameObject.Find("Ship(Clone)");
            for (int i = 0; i < ellipsePoints.Count; i++) {
                GameObject go =
                    MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Point"), shipObj.transform);
                go.transform.localPosition = ellipsePoints[i].ToVector3(-0.3f);
            }
            GameObject go0 =
                MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Point"), shipObj.transform);
            go0.GetComponent<SpriteRenderer>().color = Color.green;
            go0.transform.localPosition = centerPoint.ToVector3(-0.3f);
        }

        // Создание локальных вариантов уравнения эллипса: Ax^2 + Bxy + Cy^2 + Dx + Ey = 1
        double[,] storage = new double[ellipsePoints.Count,5];
        for (int i = 0; i < ellipsePoints.Count; i++) {
            storage[i, 0] = ellipsePoints[i].x * ellipsePoints[i].x;
            storage[i, 1] = ellipsePoints[i].x * ellipsePoints[i].y;
            storage[i, 2] = ellipsePoints[i].y * ellipsePoints[i].y;
            storage[i, 3] = ellipsePoints[i].x;
            storage[i, 4] = ellipsePoints[i].y;
        }

        double[] vec = new double[storage.GetLength(0)];
        for (int i = 0; i < storage.GetLength(0); i++)
            vec[i] = 1d;
        
        // Вычисляем коэффициенты A,B,C,D,E и возвращаем их
        Matrix<double> matrix = DenseMatrix.OfArray(storage);
        Vector<double> result = matrix.Solve(Vector.Build.DenseOfArray(vec));
        return result.Storage.AsArray();
    }

}