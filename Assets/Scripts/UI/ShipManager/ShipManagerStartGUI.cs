using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipManagerStartGUI : MonoBehaviour {
    private bool showShips = false;
    private LastButton lastButton = LastButton.None;
    private List<string> shipNames = new List<string>();
    private string shipName;
    public static string openedShipName;
    
    public void Start() {
        /*Ship ship = new Ship();
        ShipCell main = new ShipCell(new Vector2Int(0, 0));
        main.module = new ShipModule(new Vector2Int(0, 0), "AICore");
        ship.shipCells.Add(main);
        byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(ship));
        new FileStream(Application.streamingAssetsPath + "/empty.ship", FileMode.Create)
            .Write(bytes, 0, bytes.Length);*/

        shipNames = Directory.GetFiles(Application.streamingAssetsPath + "/ships")
                          .Where(s => !s.Contains(".meta"))
                          .Select(Path.GetFileName).ToList();
        shipNames.Remove("empty.ship");
    }

    public void OnGUI() {
        shipName = GUILayout.TextField(shipName);
        if (shipNames.Contains(shipName))
            GUILayout.Label("This ship already exists");
        else if (GUILayout.Button("Create ship")) {
            File.Copy(Application.streamingAssetsPath + "/ships/empty.ship",
            Application.streamingAssetsPath + "/ships/" + shipName, false);
            openedShipName = shipName;
            SceneManager.LoadScene("Scenes/ShipEditor");
        }
        else if (GUILayout.Button("Edit ship")) {
            lastButton = LastButton.Edit;
        }
        else if (GUILayout.Button("Remove ship")) {
            lastButton = LastButton.Remove;
        }

        if (lastButton != LastButton.None) {
            for (int i = 0; i < shipNames.Count; i++)
                if (GUILayout.Button(shipNames[i])) {
                    switch (lastButton) {
                        case LastButton.Edit:
                            openedShipName = shipNames[i];
                            SceneManager.LoadScene("Scenes/ShipEditor");
                            break;

                        case LastButton.Remove:
                            shipNames.RemoveAt(i);
                            i--;
                            break;
                    }
                }
        }
    }
    
    private enum LastButton {
        None, Edit, Remove
    }
}
