﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Move : MonoBehaviour {
    public float speed = 0.05f;
    private float dspeed;
    private bool left = false, right = false, up = false, down = false;
    public GameObject mapGO;
    Map map;
    bool first = true;

    void Awake() {
        //добавление игровых объектов на сцену start
        mapGO = Instantiate(mapGO);

        dspeed = Mathf.Sqrt((speed * speed) / 2);
    }

    void Start() {
        map = mapGO.GetComponent<Map>();
    }

	void FixedUpdate () {
        if (Player.connect) {
            if (first) {
                Player.blockId = Map.getBlockId(Player.x, Player.y);
                map.updateChunks(Player.x, Player.y, "all");
                this.gameObject.transform.position = new Vector2(Player.x, Player.y);
                first = false;
            } else {
                Player.x = this.gameObject.transform.position.x;
                Player.y = this.gameObject.transform.position.y;

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) left = true;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) right = true;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) up = true;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) down = true;

                if(left && up) {
                    if (collision("L", dspeed, "mapBlock")) Player.x -= dspeed;
                    else Player.x -= (Player.x - 0.75f) - Mathf.FloorToInt(Player.x);
                    if (collision("U", dspeed, "mapBlock")) Player.y += dspeed;
                    else Player.y += Mathf.CeilToInt(Player.y) - (Player.y + 0.75f);
                    Player.position = "LU";
                } else if (left && down) {
                    if (collision("L", dspeed, "mapBlock")) Player.x -= dspeed;
                    else Player.x -= (Player.x - 0.75f) - Mathf.FloorToInt(Player.x);
                    if (collision("D", dspeed, "mapBlock")) Player.y -= dspeed;
                    else Player.y -= (Player.y - 0.75f) - Mathf.FloorToInt(Player.y);
                    Player.position = "LD";
                } else if (right && up) {
                    if (collision("R", dspeed, "mapBlock")) Player.x += dspeed;
                    else Player.x += Mathf.CeilToInt(Player.x) - (Player.x + 0.75f);
                    if (collision("U", dspeed, "mapBlock")) Player.y += dspeed;
                    else Player.y += Mathf.CeilToInt(Player.y) - (Player.y + 0.75f);
                    Player.position = "RU";
                } else if (right && down) {
                    if (collision("R", dspeed, "mapBlock")) Player.x += dspeed;
                    else Player.x += Mathf.CeilToInt(Player.x) - (Player.x + 0.75f);
                    if (collision("D", dspeed, "mapBlock")) Player.y -= dspeed;
                    else Player.y -= (Player.y - 0.75f) - Mathf.FloorToInt(Player.y);
                    Player.position = "RD";
                } else if (left && !right) {
                    if (collision("L", speed, "mapBlock")) Player.x -= speed;
                    else Player.x -= (Player.x - 0.75f) - Mathf.FloorToInt(Player.x);
                    Player.position = "L";
                } else if (right && !left) {
                    if (collision("R", speed, "mapBlock")) Player.x += speed;
                    else Player.x += Mathf.CeilToInt(Player.x) - (Player.x + 0.75f);
                    Player.position = "R";
                } else if (up && !down) {
                    if (collision("U", speed, "mapBlock")) Player.y += speed;
                    else Player.y += Mathf.CeilToInt(Player.y) - (Player.y + 0.75f);
                    Player.position = "U";
                } else if (down && !up) {
                    if (collision("D", speed, "mapBlock")) Player.y -= speed;
                    else Player.y -= (Player.y - 0.75f) - Mathf.FloorToInt(Player.y);
                    Player.position = "D";
                }

                float loc1 = Mathf.Floor(Player.blockId % Map.nWidth) * Map.blockSize;
                float loc2 = Mathf.Floor(Player.blockId / Map.nWidth) * Map.blockSize;

                float rightBorder = loc1 + Map.blockSize;
                float leftBorder = loc1;
                float upBorder = loc2 + Map.blockSize;
                float downBorder = loc2;

                if (((Player.x <= leftBorder) && (Mathf.Abs(leftBorder - Player.x) < 1) && left) ||
                    ((Player.x >= rightBorder) && (Mathf.Abs(rightBorder - Player.x) < 1) && right) ||
                    ((Player.y <= downBorder) && (Mathf.Abs(downBorder - Player.y) < 1) && down) ||
                    ((Player.y >= upBorder) && (Mathf.Abs(upBorder - Player.y) < 1) && up)) {
                    map.updateChunks(Player.x, Player.y, "calc");
                }
                Player.x = Map.calibrationX(Player.x);
                Player.y = Map.calibrationY(Player.y);

                if ((this.gameObject.transform.position.x != Player.x) || (this.gameObject.transform.position.y != Player.y)) {
                    this.gameObject.transform.position = new Vector2(Player.x, Player.y);

                    JObject obj = new JObject();
                    obj.Add(new JProperty("id", ServerController.Operation.sendMyXY));
                    obj.Add(new JProperty("x", Player.x));
                    obj.Add(new JProperty("y", Player.y));
                    obj.Add(new JProperty("position", Player.position));
                    ServerController.send(obj);
                }

                if ((left == true) && (Input.GetKey(KeyCode.A) == false) && (Input.GetKey(KeyCode.LeftArrow) == false)) left = false;
                if ((right == true) && (Input.GetKey(KeyCode.D) == false) && (Input.GetKey(KeyCode.RightArrow) == false)) right = false;
                if ((up == true) && (Input.GetKey(KeyCode.W) == false) && (Input.GetKey(KeyCode.UpArrow) == false)) up = false;
                if ((down == true) && (Input.GetKey(KeyCode.S) == false) && (Input.GetKey(KeyCode.DownArrow) == false)) down = false;
            }
        }
    }

    private bool collision(string sideMove, float speed, string type) {
        float shiftX = 0, shiftY = 0;
        switch (sideMove) {
            case "L": {
                    shiftX = -speed;
                    break;
                }
            case "R": {
                    shiftX = +speed;
                    break;
                }
            case "U": {
                    shiftY = +speed;
                    break;
                }
            case "D": {
                    shiftY = -speed;
                    break;
                }
            
        }

        float LX = Map.calibrationX(Player.x - 0.25f + shiftX);
        float RX = Map.calibrationX(Player.x + 0.25f + shiftX);
        float UY = Map.calibrationY(Player.y + 0.25f + shiftY);
        float DY = Map.calibrationY(Player.y - 0.25f + shiftY);
        
        switch (type) {
            case "mapBlock": {
                    //0.5 из-за того что даются координаты центра объекта
                    int blockL_X = Mathf.FloorToInt(Map.calibrationX(Player.x - 1 + 0.5f));
                    int blockL_Y = Mathf.FloorToInt(Map.calibrationY(Player.y + 0.5f));
                    float blockL_RX = blockL_X + 0.5f;
                    float blockL_UY = blockL_Y + 0.5f;
                    float blockL_DY = blockL_Y - 0.5f;
                    
                    int blockR_X = Mathf.FloorToInt(Map.calibrationX(Player.x + 1 + 0.5f));
                    int blockR_Y = Mathf.FloorToInt(Map.calibrationY(Player.y + 0.5f));
                    float blockR_LX = blockR_X - 0.5f;
                    float blockR_UY = blockR_Y + 0.5f;
                    float blockR_DY = blockR_Y - 0.5f;

                    int blockU_X = Mathf.FloorToInt(Map.calibrationX(Player.x + 0.5f));
                    int blockU_Y = Mathf.FloorToInt(Map.calibrationY(Player.y + 1 + 0.5f));
                    float blockU_LX = blockU_X - 0.5f;
                    float blockU_RX = blockU_X + 0.5f;
                    float blockU_DY = blockU_Y - 0.5f;

                    int blockD_X = Mathf.FloorToInt(Map.calibrationX(Player.x + 0.5f));
                    int blockD_Y = Mathf.FloorToInt(Map.calibrationY(Player.y - 1 + 0.5f));
                    float blockD_LX = blockD_X - 0.5f;
                    float blockD_RX = blockD_X + 0.5f;
                    float blockD_UY = blockD_Y + 0.5f;

                    int blockLU_X = Mathf.FloorToInt(Map.calibrationX(Player.x - 1 + 0.5f));
                    int blockLU_Y = Mathf.FloorToInt(Map.calibrationY(Player.y + 1 + 0.5f));
                    float blockLU_LX = blockLU_X - 0.5f;
                    float blockLU_RX = blockLU_X + 0.5f;
                    float blockLU_UY = blockLU_Y + 0.5f;
                    float blockLU_DY = blockLU_Y - 0.5f;

                    int blockLD_X = Mathf.FloorToInt(Map.calibrationX(Player.x - 1 + 0.5f));
                    int blockLD_Y = Mathf.FloorToInt(Map.calibrationY(Player.y - 1 + 0.5f));
                    float blockLD_LX = blockLD_X - 0.5f;
                    float blockLD_RX = blockLD_X + 0.5f;
                    float blockLD_UY = blockLD_Y + 0.5f;
                    float blockLD_DY = blockLD_Y - 0.5f;

                    int blockRU_X = Mathf.FloorToInt(Map.calibrationX(Player.x + 1 + 0.5f));
                    int blockRU_Y = Mathf.FloorToInt(Map.calibrationY(Player.y + 1 + 0.5f));
                    float blockRU_LX = blockRU_X - 0.5f;
                    float blockRU_RX = blockRU_X + 0.5f;
                    float blockRU_UY = blockRU_Y + 0.5f;
                    float blockRU_DY = blockRU_Y - 0.5f;

                    int blockRD_X = Mathf.FloorToInt(Map.calibrationX(Player.x + 1 + 0.5f));
                    int blockRD_Y = Mathf.FloorToInt(Map.calibrationY(Player.y - 1 + 0.5f));
                    float blockRD_LX = blockRD_X - 0.5f;
                    float blockRD_RX = blockRD_X + 0.5f;
                    float blockRD_UY = blockRD_Y + 0.5f;
                    float blockRD_DY = blockRD_Y - 0.5f;
                    

                    switch (sideMove) {
                        case "L": {
                                if ((((UY > blockL_DY && UY < blockL_UY) || (DY > blockL_DY && DY < blockL_UY)) && (LX < blockL_RX) && (Mathf.Abs(LX - Map.calibrationX(blockL_RX)) < 1) && Map.MapArr[blockL_X, blockL_Y] != '!') ||
                                ((UY > blockLU_DY && UY < blockLU_UY) && (LX < blockLU_RX) && (Mathf.Abs(LX - Map.calibrationX(blockLU_RX)) < 1) && Map.MapArr[blockLU_X, blockLU_Y] != '!') ||
                                ((DY > blockLD_DY && DY < blockLD_UY) && (LX < blockLD_RX) && (Mathf.Abs(LX - Map.calibrationX(blockLD_RX)) < 1) && Map.MapArr[blockLD_X, blockLD_Y] != '!')) return false;
                                else return true;
                            }
                        case "R": {
                                if ((((UY > blockR_DY && UY < blockR_UY) || (DY > blockR_DY && DY < blockR_UY)) && (RX > Map.calibrationX(blockR_LX)) && (Mathf.Abs(RX - Map.calibrationX(blockR_LX)) < 1) && Map.MapArr[blockR_X, blockR_Y] != '!') ||
                                ((UY > blockRU_DY && UY < blockRU_UY) && (RX > Map.calibrationX(blockRU_LX)) && (Mathf.Abs(RX - Map.calibrationX(blockRU_LX)) < 1) && Map.MapArr[blockRU_X, blockRU_Y] != '!') ||
                                ((DY > blockRD_DY && DY < blockRD_UY) && (RX > Map.calibrationX(blockRD_LX)) && (Mathf.Abs(RX - Map.calibrationX(blockRD_LX)) < 1) && Map.MapArr[blockRD_X, blockRD_Y] != '!')) return false;
                                else return true;
                            }
                        case "U": {
                                if ((((LX > blockU_LX && LX < blockU_RX) || (RX > blockU_LX && RX < blockU_RX)) && (UY > Map.calibrationY(blockU_DY)) && (Mathf.Abs(UY - Map.calibrationX(blockU_DY)) < 1) && Map.MapArr[blockU_X, blockU_Y] != '!') ||
                                ((LX > blockLU_LX && LX < blockLU_RX) && (UY > Map.calibrationY(blockLU_DY)) && (Mathf.Abs(UY - Map.calibrationX(blockLU_DY)) < 1) && Map.MapArr[blockLU_X, blockLU_Y] != '!') ||
                                ((RX > blockRU_LX && RX < blockRU_RX) && (UY > Map.calibrationY(blockRU_DY)) && (Mathf.Abs(UY - Map.calibrationX(blockRU_DY)) < 1) && Map.MapArr[blockRU_X, blockRU_Y] != '!')) return false;
                                else return true;
                            }
                        case "D": {
                                if ((((LX > blockD_LX && LX < blockD_RX) || (RX > blockD_LX && RX < blockD_RX)) && (DY < blockD_UY) && (Mathf.Abs(DY - Map.calibrationX(blockD_UY)) < 1) && Map.MapArr[blockD_X, blockD_Y] != '!') ||
                                ((LX > blockLD_LX && LX < blockLD_RX) && (DY < blockLD_UY) && (Mathf.Abs(DY - Map.calibrationX(blockLD_UY)) < 1) && Map.MapArr[blockLD_X, blockLD_Y] != '!') ||
                                ((RX > blockRD_LX && RX < blockRD_RX) && (DY < blockRD_UY) && (Mathf.Abs(DY - Map.calibrationX(blockRD_UY)) < 1) && Map.MapArr[blockRD_X, blockRD_Y] != '!')) return false;
                                else return true;
                            }
                    }
                    break;
                }
        }
        return false;
    }
}