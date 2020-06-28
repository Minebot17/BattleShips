using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CreateMapClientMessage : GameMessage {

    public CreateMapClientMessage() { }

    public CreateMapClientMessage(string name, Vector2 size, List<(string, Vector3, Vector3)> objectsWithoutNI) {
        Writer.Write(new StringListMessage(objectsWithoutNI.Select(objs => objs.Item1).ToList()));
        Writer.Write(new Vector3ListMessage(objectsWithoutNI.Select(objs => objs.Item2).ToList()));
        Writer.Write(new Vector3ListMessage(objectsWithoutNI.Select(objs => objs.Item3).ToList()));
        Writer.Write(name);
        Writer.Write(size);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        List<string> elementNames = reader.ReadMessage<StringListMessage>().Value;
        List<Vector3> elementPositions = reader.ReadMessage<Vector3ListMessage>().Value;
        List<Vector3> elementsEulerAngles = reader.ReadMessage<Vector3ListMessage>().Value;
        for (int i = 0; i < elementNames.Count; i++) {
            GameObject element = MonoBehaviour.Instantiate(Map.mapElements[elementNames[i]]);
            element.transform.position = elementPositions[i];
            element.transform.localEulerAngles = elementsEulerAngles[i];
        }
        
        Map map = new GameObject(reader.ReadString()).AddComponent<Map>();
        map.size = reader.ReadVector2();
    }
}