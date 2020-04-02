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
        shipNames = Directory.GetFiles(Application.streamingAssetsPath + "/ships")
                          .Where(s => !s.Contains(".meta"))
                          .Select(s => Path.GetFileName(s).Split('.')[0])
                          .ToList();
        shipNames.Remove("empty");
    }

    public void OnGUI() {
        shipName = GUILayout.TextField(shipName);
        if (shipNames.Contains(shipName))
            GUILayout.Label("This ship already exists");
        else if (GUILayout.Button("Create ship")) {
            File.Copy(Application.streamingAssetsPath + "/ships/empty.ship",
            Application.streamingAssetsPath + "/ships/" + shipName + ".ship", false
            );
            openedShipName = shipName;
            SceneManager.LoadScene("Scenes/ShipEditor");
        }
        else if (GUILayout.Button("Edit ship"))
            lastButton = LastButton.Edit;
        else if (GUILayout.Button("Remove ship"))
            lastButton = LastButton.Remove;
        else if (GUILayout.Button("Back")) {
            SceneManager.LoadScene("Scenes/Menu");
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustomGUI>().enabled = true;
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
