using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {

    public static Scoreboard singleton;
    public static float visibleSeconds = 5f;
    [SerializeField] Sprite startSprite;
    [SerializeField] Sprite centerSprite;
    [SerializeField] Sprite endSprite;
    [SerializeField] Vector2Int partSize = new Vector2Int(4, 16);
    [SerializeField] float deltaScoreSpeed = 1f;

    Dictionary<GameObject, int> deltaScore = new Dictionary<GameObject, int>();
    Dictionary<GameObject, float> initialPosition = new Dictionary<GameObject, float>();

    void Start() {
        singleton = this;
        IEnumerable<GameState> gStates = Players.GetStates<GameState>();
        if (NetworkManagerCustom.singleton.IsServer) {
            Initialize(
            gStates.Select(d => d.ShipJson.Value).ToList(),
            gStates.Select(d => d.Score.Value).ToList(),
            NetworkManagerCustom.singleton.gameMode.GetScoreDelta(gStates.ToDictionary(d => d.GetParent().Conn, d => d.Kills.Value)).Values.ToList(),
                NetworkManagerCustom.singleton.scoreForWin
            );
        }
        else
            new ScoreboardInfoMessage().SendToServer();
    }

    public void Initialize(List<string> ships, List<int> score, List<int> delta, int scoreForWin) {
        SpawnBoardPart(startSprite, -partSize.x/2 * scoreForWin);
        SpawnBoardPart(endSprite, partSize.x/2 * scoreForWin);
        for (int i = 0; i < scoreForWin - 1; i++)
            SpawnBoardPart(centerSprite, partSize.x/2 * (i*2 - scoreForWin + 2));
        
        int yPerPlayer = partSize.y / (ships.Count + 1);
        for (int i = 0; i < ships.Count; i++) {
            GameObject shipObject = new GameObject("Ship");
            Utils.DeserializeShipPartsFromJson(shipObject, ships[i]);
            shipObject.transform.parent = transform;
            shipObject.transform.localPosition = new Vector3(
                -partSize.x/2 * scoreForWin + partSize.x * score[i], 
                partSize.y/2 - yPerPlayer * (i + 1), -0.1f
            );
            
            shipObject.transform.localEulerAngles = new Vector3(0, 0, -90);
            deltaScore[shipObject] = delta[i];
            if (score[i] + delta[i] > scoreForWin)
                deltaScore[shipObject] = scoreForWin - score[i];
            
            initialPosition[shipObject] = shipObject.transform.localPosition.x;
        }
    }

    void LateUpdate() {
        foreach (GameObject ship in deltaScore.Keys) {
            float deltaPosition = initialPosition[ship] + partSize.x * deltaScore[ship];
            if (ship.transform.localPosition.x + deltaScoreSpeed * Time.deltaTime < deltaPosition)
                ship.transform.localPosition += new Vector3(deltaScoreSpeed * Time.deltaTime, 0, 0);
        }
    }

    void SpawnBoardPart(Sprite sprite, int positionX) {
        GameObject boardPart = new GameObject { name = sprite.name };
        boardPart.transform.parent = transform;
        boardPart.transform.localPosition = new Vector3(positionX, 0, 0);
        
        SpriteRenderer spriteRenderer = boardPart.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
