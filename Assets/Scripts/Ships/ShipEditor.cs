using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipEditor : MonoBehaviour {
    public ShipEditor singleton;
    public string selectedModule = null;
    private GameObject currentShip;

    public void Start() {
        singleton = this;
        
        OpenForEdit(ShipManagerStartGUI.openedShipName);
    }

    public void OpenForEdit(string shipName) {
        if (currentShip)
            Destroy(currentShip);

        currentShip = Utils.DeserializeShip(shipName);
    }

    public void OnConstructorClick() {
        Vector3 p = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        p = new Vector3(p.x/Utils.sizeOfOne - 0.5f*(p.x > 0 ? -1 : 1), p.y/Utils.sizeOfOne - 0.5f*(p.y > 0 ? -1 : 1), 0f);
        Vector2Int position = new Vector2Int(Utils.RoundSinged(p.x), Utils.RoundSinged(p.y));
        GameObject shipCell = currentShip.transform.Find("ShipCell " + position.x + " " + position.y)?.gameObject;

        if (position.x == 0 && position.y == 0)
            return;
        
        if (selectedModule == null) {
            if (shipCell == null) {
                GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), currentShip.transform);
                cell.name = "ShipCell " + position.x + " " + position.y;
                cell.transform.localPosition = new Vector3(position.x * Utils.sizeOfOne, position.y * Utils.sizeOfOne, 0);
            }
            else
                Destroy(shipCell);
        }
        else if (shipCell != null) { // TODO module size
            GameObject module = Instantiate(
            Resources.Load<GameObject>("Prefabs/" + selectedModule + "Module"), shipCell.transform);
            module.name = selectedModule;
            module.transform.localPosition = new Vector3(0, 0, -0.1f);
        }
    }

    public void OnSelectModuleClick(GameObject buttonObject) {
        selectedModule = buttonObject.name.Equals("ShipCell") ? null : buttonObject.name;
    }

    public void OnSaveAndBackClick() {
        if (currentShip != null)
            Utils.SerializeShip(currentShip);

        SceneManager.LoadScene("Scenes/Menu");
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustomGUI>().enabled = true;
    }
}
