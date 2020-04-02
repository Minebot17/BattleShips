using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Utils {
    public static float sizeOfOne = 0.64f;
    
    public static int RoundSinged(float number) {
        return (int)(number > 0 ? Mathf.Floor(number) : Mathf.Ceil(number));
    }

    public static void CreateEmptyShip() {
        Ship ship = new Ship();
        ShipCell main = new ShipCell(new Vector2Int(0, 0));
        main.module = new ShipModule(new Vector2Int(0, 0), "AICore");
        ship.shipCells.Add(main);
        byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(ship));
        new FileStream(Application.streamingAssetsPath + "ships/empty.ship", FileMode.Create)
            .Write(bytes, 0, bytes.Length);
    }

    public static GameObject DeserializeShip(string shipName) {
        Ship ship = JsonUtility.FromJson<Ship>(File.ReadAllText(Application.streamingAssetsPath + "/ships/" + shipName + ".ship"));
        GameObject shipObject = new GameObject(shipName + "Ship");
        for (int i = 0; i < ship.shipCells.Count; i++) {
            GameObject shipCell = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), shipObject.transform);
            shipCell.name = "ShipCell " + ship.shipCells[i].positionOnShip.x + " " + ship.shipCells[i].positionOnShip.y;
            shipCell.transform.localPosition = new Vector3(
            ship.shipCells[i].positionOnShip.x * sizeOfOne,
            ship.shipCells[i].positionOnShip.y * sizeOfOne, 0);

            ShipModule module = ship.shipCells[i].module;
            if (module != null) {
                if (module.mainPosition != ship.shipCells[i].positionOnShip) {
                    GameObject partOfModule = new GameObject(
                        "PartOf" + module.type + " " + module.mainPosition.x + " " + module.mainPosition.y);
                    partOfModule.transform.parent = shipCell.transform;
                    partOfModule.transform.localPosition = Vector3.zero;
                }
                else {
                    GameObject cellModule = MonoBehaviour.Instantiate(Resources.Load<GameObject>(
                    "Prefabs/" + module.type + "Module"
                    ), shipCell.transform);
                    cellModule.transform.localPosition = new Vector3(0, 0, -0.1f);
                    cellModule.name = module.type;
                }
            }
        }

        return shipObject;
    }
    
    public static void SerializeShip(GameObject shipObject) {
        Ship ship = new Ship();
        for (int i = 0; i < shipObject.transform.childCount; i++) {
            GameObject shipCell = shipObject.transform.GetChild(i).gameObject;
            ShipCell cell = new ShipCell();
            string[] splittedName = shipCell.name.Split(' ');
            cell.positionOnShip = new Vector2Int(int.Parse(splittedName[1]), int.Parse(splittedName[2]));

            if (shipCell.transform.childCount != 0) {
                GameObject moduleObject = shipCell.transform.GetChild(0).gameObject;
                if (moduleObject.name.StartsWith("PartOf")) {
                    string[] splittedModuleName = moduleObject.name.Split(' ');
                    cell.module = new ShipModule(
                        new Vector2Int(int.Parse(splittedModuleName[1]), int.Parse(splittedModuleName[2])), 
                        splittedModuleName[0].Substring(6));
                }
                else
                    cell.module = new ShipModule(cell.positionOnShip, moduleObject.name);
            }
            ship.shipCells.Add(cell);
        }
        File.WriteAllText(
            Application.streamingAssetsPath + "/ships/" + shipObject.name.Substring(0, shipObject.name.Length - 4) + ".ship",
            JsonUtility.ToJson(ship));
    }
}