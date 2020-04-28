using UnityEngine.Networking;

public class TimerInEditorMessage : GameMessage {

    public TimerInEditorMessage() { }

    public TimerInEditorMessage(int timeBeforeClosing) {
        Writer.Write(timeBeforeClosing);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new TimerInEditorMessage(ShipEditor.singleton.timeBeforeClosing).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        ShipEditor.singleton.SetTimer(reader.ReadInt32() - 1); // -1 нужно чтобы у клиентов таймер чуть был меньше из-за задержки пакетов
    }
}