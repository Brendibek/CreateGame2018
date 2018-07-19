using UnityEngine;

public class OtherPlayer : MonoBehaviour {
    public int playerId;
    public string playerName;
    public float x, y;
    public string position;

    void FixedUpdate() {
        this.gameObject.transform.position = new Vector2(
        x - Player.x > 32 ? x - Map.mapWidth : (x - Player.x < -32 ? x + Map.mapWidth : x),
        y - Player.y > 32 ? y - Map.mapHeight : (y - Player.y < -32 ? y + Map.mapHeight : y));
    }
}
