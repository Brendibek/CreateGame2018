using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Tilemaps;

public class Move : MonoBehaviour {
    public float speed = 0.05f;
    private float dspeed;
    public bool left = false, right = false, up = false, down = false, joystick = false;
    bool first = true;

    void Awake () {
        dspeed = Mathf.Sqrt((speed * speed) / 2);
    }

	void FixedUpdate () {
        if (Node.sPlayerGO_sPlayerClass.connect) {
            if (first) {
                Node.sPlayerGO_sPlayerClass.blockId = Node.sMapGO_sMapClass.getBlockId(Node.sPlayerGO_sPlayerClass.x, Node.sPlayerGO_sPlayerClass.y);
                Node.sMapGO_sMapClass.updateChunks(Node.sPlayerGO_sPlayerClass.x, Node.sPlayerGO_sPlayerClass.y, "all");
                this.gameObject.transform.position = new Vector2(Node.sPlayerGO_sPlayerClass.x, Node.sPlayerGO_sPlayerClass.y);
                first = false;
            } else {
                Node.sPlayerGO_sPlayerClass.x = this.gameObject.transform.position.x;
                Node.sPlayerGO_sPlayerClass.y = this.gameObject.transform.position.y;

                if (left && up) {
                    if (collision("L", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x -= dspeed;
                    else Node.sPlayerGO_sPlayerClass.x -= (Node.sPlayerGO_sPlayerClass.x - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x);
                    if (collision("U", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y += dspeed;
                    else Node.sPlayerGO_sPlayerClass.y += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.y) - (Node.sPlayerGO_sPlayerClass.y + 0.51f);
                    Node.sPlayerGO_sPlayerClass.position = "LU";
                } else if (left && down) {
                    if (collision("L", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x -= dspeed;
                    else Node.sPlayerGO_sPlayerClass.x -= (Node.sPlayerGO_sPlayerClass.x - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x);
                    if (collision("D", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y -= dspeed;
                    else Node.sPlayerGO_sPlayerClass.y -= (Node.sPlayerGO_sPlayerClass.y - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y);
                    Node.sPlayerGO_sPlayerClass.position = "LD";
                } else if (right && up) {
                    if (collision("R", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x += dspeed;
                    else Node.sPlayerGO_sPlayerClass.x += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.x) - (Node.sPlayerGO_sPlayerClass.x + 0.75f);
                    if (collision("U", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y += dspeed;
                    else Node.sPlayerGO_sPlayerClass.y += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.y) - (Node.sPlayerGO_sPlayerClass.y + 0.51f);
                    Node.sPlayerGO_sPlayerClass.position = "RU";
                } else if (right && down) {
                    if (collision("R", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x += dspeed;
                    else Node.sPlayerGO_sPlayerClass.x += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.x) - (Node.sPlayerGO_sPlayerClass.x + 0.75f);
                    if (collision("D", dspeed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y -= dspeed;
                    else Node.sPlayerGO_sPlayerClass.y -= (Node.sPlayerGO_sPlayerClass.y - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y);
                    Node.sPlayerGO_sPlayerClass.position = "RD";
                } else if (left && !right) {
                    if (collision("L", speed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x -= speed;
                    else Node.sPlayerGO_sPlayerClass.x -= (Node.sPlayerGO_sPlayerClass.x - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.x);
                    Node.sPlayerGO_sPlayerClass.position = "L";
                } else if (right && !left) {
                    if (collision("R", speed, "mapBlock")) Node.sPlayerGO_sPlayerClass.x += speed;
                    else Node.sPlayerGO_sPlayerClass.x += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.x) - (Node.sPlayerGO_sPlayerClass.x + 0.75f);
                    Node.sPlayerGO_sPlayerClass.position = "R";
                } else if (up && !down) {
                    if (collision("U", speed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y += speed;
                    else Node.sPlayerGO_sPlayerClass.y += Mathf.CeilToInt(Node.sPlayerGO_sPlayerClass.y) - (Node.sPlayerGO_sPlayerClass.y + 0.51f);
                    Node.sPlayerGO_sPlayerClass.position = "U";
                } else if (down && !up) {
                    if (collision("D", speed, "mapBlock")) Node.sPlayerGO_sPlayerClass.y -= speed;
                    else Node.sPlayerGO_sPlayerClass.y -= (Node.sPlayerGO_sPlayerClass.y - 0.75f) - Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y);
                    Node.sPlayerGO_sPlayerClass.position = "D";
                }

                float loc1 = Mathf.Floor(Node.sPlayerGO_sPlayerClass.blockId % Node.sMapGO_sMapClass.nWidth) * Node.sMapGO_sMapClass.blockSize;
                float loc2 = Mathf.Floor(Node.sPlayerGO_sPlayerClass.blockId / Node.sMapGO_sMapClass.nWidth) * Node.sMapGO_sMapClass.blockSize;

                float rightBorder = loc1 + Node.sMapGO_sMapClass.blockSize;
                float leftBorder = loc1;
                float upBorder = loc2 + Node.sMapGO_sMapClass.blockSize;
                float downBorder = loc2;

                if (((Node.sPlayerGO_sPlayerClass.x <= leftBorder) && (Mathf.Abs(leftBorder - Node.sPlayerGO_sPlayerClass.x) < 1) && left) ||
                    ((Node.sPlayerGO_sPlayerClass.x >= rightBorder) && (Mathf.Abs(rightBorder - Node.sPlayerGO_sPlayerClass.x) < 1) && right) ||
                    ((Node.sPlayerGO_sPlayerClass.y <= downBorder) && (Mathf.Abs(downBorder - Node.sPlayerGO_sPlayerClass.y) < 1) && down) ||
                    ((Node.sPlayerGO_sPlayerClass.y >= upBorder) && (Mathf.Abs(upBorder - Node.sPlayerGO_sPlayerClass.y) < 1) && up)) {
                    Node.sMapGO_sMapClass.updateChunks(Node.sPlayerGO_sPlayerClass.x, Node.sPlayerGO_sPlayerClass.y, "calc");
                }
                Node.sPlayerGO_sPlayerClass.x = Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x);
                Node.sPlayerGO_sPlayerClass.y = Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y);

                if ((this.gameObject.transform.position.x != Node.sPlayerGO_sPlayerClass.x) || (this.gameObject.transform.position.y != Node.sPlayerGO_sPlayerClass.y)) {
                    if ((Mathf.Floor(this.gameObject.transform.position.x) != Mathf.Floor(Node.sPlayerGO_sPlayerClass.x)) || (Mathf.Floor(this.gameObject.transform.position.y) != Mathf.Floor(Node.sPlayerGO_sPlayerClass.y))) {
                        Node.sMapGO_sMapClass.updateVisibleBlocks();
                    }
                    this.gameObject.transform.position = new Vector2(Node.sPlayerGO_sPlayerClass.x, Node.sPlayerGO_sPlayerClass.y);
                    JObject obj = new JObject();
                    obj.Add(new JProperty("id", ServerController.Operation.sendMyXY));
                    obj.Add(new JProperty("x", Node.sPlayerGO_sPlayerClass.x));
                    obj.Add(new JProperty("y", Node.sPlayerGO_sPlayerClass.y));
                    obj.Add(new JProperty("position", Node.sPlayerGO_sPlayerClass.position));
                    ServerController.sendMessage(obj);
                }

                if (!joystick) {
                    if ((left == true) && (Input.GetKey(KeyCode.A) == false) && (Input.GetKey(KeyCode.LeftArrow) == false)) left = false;
                    if ((right == true) && (Input.GetKey(KeyCode.D) == false) && (Input.GetKey(KeyCode.RightArrow) == false)) right = false;
                    if ((up == true) && (Input.GetKey(KeyCode.W) == false) && (Input.GetKey(KeyCode.UpArrow) == false)) up = false;
                    if ((down == true) && (Input.GetKey(KeyCode.S) == false) && (Input.GetKey(KeyCode.DownArrow) == false)) down = false;
                }
            }
        }
    }

    private bool collision(string sideMove, float speed, string type) {
        switch (type) {
            case "mapBlock": {
                    //return true;
                    switch (sideMove) {
                        case "L": {
                                //обработка координат игрока
                                float LX = Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x - 0.25f - speed);
                                float UY = Node.sPlayerGO_sPlayerClass.y/* + 0.25f*/;
                                float DY = Node.sPlayerGO_sPlayerClass.y - 0.25f;

                                //обработка координат блока
                                int blockL_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x - 1 + 0.5f));
                                int blockL_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 0.5f));

                                float blockL_LX, blockL_RX, blockL_UY, blockL_DY, blockLU_UY, blockLU_DY, blockLD_UY, blockLD_DY;
                                blockL_RX = Node.sMapGO_sMapClass.calibrationX(blockL_X + 0.5f);
                                blockL_LX = blockL_RX - 1;
                                blockL_UY = blockL_Y + 0.5f;
                                blockL_DY = blockL_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockL_Y) >= 2) {
                                    if (blockL_UY >= 128) { blockL_UY = Node.sMapGO_sMapClass.calibrationY(blockL_UY); blockL_DY = blockL_UY - 1; }
                                    else if (blockL_DY < 0) { blockL_DY = Node.sMapGO_sMapClass.calibrationY(blockL_DY); blockL_UY = blockL_DY + 1; }
                                }

                                int blockLU_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 1 + 0.5f));
                                blockLU_UY = blockLU_Y + 0.5f;
                                blockLU_DY = blockLU_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockLU_Y) >= 2) {
                                    if (blockLU_UY >= 128) { blockLU_UY = Node.sMapGO_sMapClass.calibrationY(blockLU_UY); blockLU_DY = blockLU_UY - 1; }
                                    else if (blockLU_DY < 0) { blockLU_DY = Node.sMapGO_sMapClass.calibrationY(blockLU_DY); blockLU_UY = blockLU_DY + 1; }
                                }

                                int blockLD_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y - 1 + 0.5f));
                                blockLD_UY = blockLD_Y + 0.5f;
                                blockLD_DY = blockLD_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockLD_Y) >= 2) {
                                    if (blockLD_UY >= 128) { blockLD_UY = Node.sMapGO_sMapClass.calibrationY(blockLD_UY); blockLD_DY = blockLD_UY - 1; }
                                    else if (blockLD_DY < 0) { blockLD_DY = Node.sMapGO_sMapClass.calibrationY(blockLD_DY); blockLD_UY = blockLD_DY + 1; }
                                }

                                if ((((UY > blockL_DY && UY < blockL_UY) || (DY > blockL_DY && DY < blockL_UY)) && (LX > blockL_LX && LX < blockL_RX) && Node.sMapGO_sMapClass.averageMapArr[blockL_X, blockL_Y] != 32) ||
                                ((UY > blockLU_DY && UY < blockLU_UY) && (LX > blockL_LX && LX < blockL_RX) && Node.sMapGO_sMapClass.averageMapArr[blockL_X, blockLU_Y] != 32) ||
                                ((DY > blockLD_DY && DY < blockLD_UY) && (LX > blockL_LX && LX < blockL_RX) && Node.sMapGO_sMapClass.averageMapArr[blockL_X, blockLD_Y] != 32)) return false;
                                else return true;
                            }
                        case "R": {
                                //обработка координат игрока
                                float RX = Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 0.25f + speed);
                                float UY = Node.sPlayerGO_sPlayerClass.y/* + 0.25f*/;
                                float DY = Node.sPlayerGO_sPlayerClass.y - 0.25f;

                                //обработка координат блока
                                int blockR_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 1 + 0.5f));
                                int blockR_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 0.5f));

                                float blockR_LX, blockR_RX, blockR_UY, blockR_DY, blockRU_UY, blockRU_DY, blockRD_UY, blockRD_DY;
                                blockR_LX = Node.sMapGO_sMapClass.calibrationX(blockR_X - 0.5f);
                                blockR_RX = blockR_LX + 1;
                                blockR_UY = blockR_Y + 0.5f;
                                blockR_DY = blockR_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockR_Y) >= 2) {
                                    if (blockR_UY >= 128) { blockR_UY = Node.sMapGO_sMapClass.calibrationY(blockR_UY); blockR_DY = blockR_UY - 1; }
                                    else if (blockR_DY < 0) { blockR_DY = Node.sMapGO_sMapClass.calibrationY(blockR_DY); blockR_UY = blockR_DY + 1; }
                                }

                                int blockRU_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 1 + 0.5f));
                                blockRU_UY = blockRU_Y + 0.5f;
                                blockRU_DY = blockRU_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockRU_Y) >= 2) {
                                    if (blockRU_UY >= 128) { blockRU_UY = Node.sMapGO_sMapClass.calibrationY(blockRU_UY); blockRU_DY = blockRU_UY - 1; }
                                    else if (blockRU_DY < 0) { blockRU_DY = Node.sMapGO_sMapClass.calibrationY(blockRU_DY); blockRU_UY = blockRU_DY + 1; }
                                }

                                int blockRD_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y - 1 + 0.5f));
                                blockRD_UY = blockRD_Y + 0.5f;
                                blockRD_DY = blockRD_Y - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.y - blockRD_Y) >= 2) {
                                    if (blockRD_UY >= 128) { blockRD_UY = Node.sMapGO_sMapClass.calibrationY(blockRD_UY); blockRD_DY = blockRD_UY - 1; }
                                    else if (blockRD_DY < 0) { blockRD_DY = Node.sMapGO_sMapClass.calibrationY(blockRD_DY); blockRD_UY = blockRD_DY + 1; }
                                }

                                if ((((UY > blockR_DY && UY < blockR_UY) || (DY > blockR_DY && DY < blockR_UY)) && (RX > blockR_LX && RX < blockR_RX) && Node.sMapGO_sMapClass.averageMapArr[blockR_X, blockR_Y] != 32) ||
                                ((UY > blockRU_DY && UY < blockRU_UY) && (RX > blockR_LX && RX < blockR_RX) && Node.sMapGO_sMapClass.averageMapArr[blockR_X, blockRU_Y] != 32) ||
                                ((DY > blockRD_DY && DY < blockRD_UY) && (RX > blockR_LX && RX < blockR_RX) && Node.sMapGO_sMapClass.averageMapArr[blockR_X, blockRD_Y] != 32)) return false;
                                else return true;
                            }
                        case "U": {
                                //обработка координат игрока
                                float UY = Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 0.01f + speed);
                                float LX = Node.sPlayerGO_sPlayerClass.x - 0.25f;
                                float RX = Node.sPlayerGO_sPlayerClass.x + 0.25f;

                                //обработка координат блока
                                int blockU_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 0.5f));
                                int blockU_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y + 1 + 0.5f));

                                float blockU_LX, blockU_RX, blockU_UY, blockU_DY, blockLU_LX, blockLU_RX, blockRU_LX, blockRU_RX;
                                blockU_DY = Node.sMapGO_sMapClass.calibrationY(blockU_Y - 0.5f);
                                blockU_UY = blockU_DY + 1;
                                blockU_RX = blockU_X + 0.5f;
                                blockU_LX = blockU_X - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockU_X) >= 2) {
                                    if (blockU_RX >= 128) { blockU_RX = Node.sMapGO_sMapClass.calibrationY(blockU_RX); blockU_LX = blockU_RX - 1; }
                                    else if (blockU_LX < 0) { blockU_LX = Node.sMapGO_sMapClass.calibrationY(blockU_LX); blockU_RX = blockU_LX + 1; }
                                }

                                int blockLU_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x - 1 + 0.5f));
                                blockLU_LX = blockLU_X - 0.5f;
                                blockLU_RX = blockLU_X + 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockLU_X) >= 2) {
                                    if (blockLU_RX >= 128) { blockLU_RX = Node.sMapGO_sMapClass.calibrationY(blockLU_RX); blockLU_LX = blockLU_RX - 1; }
                                    else if (blockLU_LX < 0) { blockLU_LX = Node.sMapGO_sMapClass.calibrationY(blockLU_LX); blockLU_RX = blockLU_LX + 1; }
                                }

                                int blockRU_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 1 + 0.5f));
                                blockRU_LX = blockRU_X - 0.5f;
                                blockRU_RX = blockRU_X + 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockRU_X) >= 2) {
                                    if (blockRU_RX >= 128) { blockRU_RX = Node.sMapGO_sMapClass.calibrationY(blockRU_RX); blockRU_LX = blockRU_RX - 1; }
                                    else if (blockRU_LX < 0) { blockRU_LX = Node.sMapGO_sMapClass.calibrationY(blockRU_LX); blockRU_RX = blockRU_LX + 1; }
                                }

                                if ((((LX > blockU_LX && LX < blockU_RX) || (RX > blockU_LX && RX < blockU_RX)) && (UY > blockU_DY && UY < blockU_UY) && Node.sMapGO_sMapClass.averageMapArr[blockU_X, blockU_Y] != 32) ||
                                ((LX > blockLU_LX && LX < blockLU_RX) && (UY > blockU_DY && UY < blockU_UY) && Node.sMapGO_sMapClass.averageMapArr[blockLU_X, blockU_Y] != 32) ||
                                ((RX > blockRU_LX && RX < blockRU_RX) && (UY > blockU_DY && UY < blockU_UY) && Node.sMapGO_sMapClass.averageMapArr[blockRU_X, blockU_Y] != 32)) return false;
                                else return true;
                            }
                        case "D": {
                                //обработка координат игрока
                                float DY = Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y - 0.25f - speed);
                                float LX = Node.sPlayerGO_sPlayerClass.x - 0.25f;
                                float RX = Node.sPlayerGO_sPlayerClass.x + 0.25f;

                                //обработка координат блока
                                int blockD_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 0.5f));
                                int blockD_Y = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationY(Node.sPlayerGO_sPlayerClass.y - 1 + 0.5f));

                                float blockD_LX, blockD_RX, blockD_UY, blockD_DY, blockLD_LX, blockLD_RX, blockRD_LX, blockRD_RX;
                                blockD_UY = Node.sMapGO_sMapClass.calibrationY(blockD_Y + 0.5f);
                                blockD_DY = blockD_UY - 1;
                                blockD_RX = blockD_X + 0.5f;
                                blockD_LX = blockD_X - 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockD_X) >= 2) {
                                    if (blockD_RX >= 128) { blockD_RX = Node.sMapGO_sMapClass.calibrationY(blockD_RX); blockD_LX = blockD_RX - 1; }
                                    else if (blockD_LX < 0) { blockD_LX = Node.sMapGO_sMapClass.calibrationY(blockD_LX); blockD_RX = blockD_LX + 1; }
                                }

                                int blockLD_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x - 1 + 0.5f));
                                blockLD_LX = blockLD_X - 0.5f;
                                blockLD_RX = blockLD_X + 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockLD_X) >= 2) {
                                    if (blockLD_RX >= 128) { blockLD_RX = Node.sMapGO_sMapClass.calibrationY(blockLD_RX); blockLD_LX = blockLD_RX - 1; }
                                    else if (blockLD_LX < 0) { blockLD_LX = Node.sMapGO_sMapClass.calibrationY(blockLD_LX); blockLD_RX = blockLD_LX + 1; }
                                }

                                int blockRD_X = Mathf.FloorToInt(Node.sMapGO_sMapClass.calibrationX(Node.sPlayerGO_sPlayerClass.x + 1 + 0.5f));
                                blockRD_LX = blockRD_X - 0.5f;
                                blockRD_RX = blockRD_X + 0.5f;
                                if (Mathf.Abs(Node.sPlayerGO_sPlayerClass.x - blockRD_X) >= 2) {
                                    if (blockRD_RX >= 128) { blockRD_RX = Node.sMapGO_sMapClass.calibrationY(blockRD_RX); blockRD_LX = blockRD_RX - 1; }
                                    else if (blockRD_LX < 0) { blockRD_LX = Node.sMapGO_sMapClass.calibrationY(blockRD_LX); blockRD_RX = blockRD_LX + 1; }
                                }

                                if ((((LX > blockD_LX && LX < blockD_RX) || (RX > blockD_LX && RX < blockD_RX)) && (DY > blockD_DY && DY < blockD_UY) && Node.sMapGO_sMapClass.averageMapArr[blockD_X, blockD_Y] != 32) ||
                                ((LX > blockLD_LX && LX < blockLD_RX) && (DY > blockD_DY && DY < blockD_UY) && Node.sMapGO_sMapClass.averageMapArr[blockLD_X, blockD_Y] != 32) ||
                                ((RX > blockRD_LX && RX < blockRD_RX) && (DY > blockD_DY && DY < blockD_UY) && Node.sMapGO_sMapClass.averageMapArr[blockRD_X, blockD_Y] != 32)) return false;
                                else return true;
                            }
                    }
                    return true;
                }
            case "otherPlayer": {
                    for(int i = -1; i <= 1; i++) {
                        for(int j = -1; j <= 1; j++) {
                            Debug.Log(Node.sServerControllerGO_sServerControllerClass.playersInCells[(int)Node.sMapGO_sMapClass.calibrationX(Mathf.Floor(Node.sPlayerGO_sPlayerClass.x + 0.5f) + i), (int)Node.sMapGO_sMapClass.calibrationY(Mathf.FloorToInt(Node.sPlayerGO_sPlayerClass.y + 0.5f) - j)].Count);
                        }
                    }
                    break;
                }
        }
        return true;
    }
}