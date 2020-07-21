using UnityEngine;
using UnityEngine.Networking;

// TODO вообще лучше пусть клиент отслеживает состояние своего корабля и стоит свои кнопки на этом основании
public class EditUseButtonClientMessage : GameMessage {

    public EditUseButtonClientMessage() { }

    public EditUseButtonClientMessage(string moduleName, bool remove) {
        Writer.Write(moduleName);
        Writer.Write(remove);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        PlayerInputHandler.singleton.EditUseButton(reader.ReadString(), reader.ReadBoolean());
    }
}