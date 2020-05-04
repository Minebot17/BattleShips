using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnemyPointerColorMessage : GameMessage {

    public EnemyPointerColorMessage() { }

    public EnemyPointerColorMessage(NetworkIdentity from, NetworkIdentity target) {
        Writer.Write(from);
        Writer.Write(target);
    }

    public EnemyPointerColorMessage(NetworkIdentity target, int color) {
        Writer.Write(target);
        Writer.Write(color);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity from = reader.ReadNetworkIdentity();
        NetworkIdentity target = reader.ReadNetworkIdentity();
        new EnemyPointerColorMessage(target, NetworkManagerCustom.singleton.gameMode.GetEnemyPointerColor(from.clientAuthorityOwner, target))
            .SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity target = reader.ReadNetworkIdentity();
        int color = reader.ReadInt32();
		
        GameObject enemyPointer = Object.Instantiate(
        NetworkManagerCustom.singleton.enemyPointerPrefab, GameObject.Find("Canvas").transform);
        enemyPointer.GetComponent<EnemyPointer>().Target = target.gameObject;
        enemyPointer.GetComponentInChildren<Image>().color = color.ToColor();
    }
}