using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Textures : MonoBehaviour {
    public Texture2D[] textures;
    public int textureSize;
    public List<SpriteStruct> sprites = new List<SpriteStruct>();

    //текстуры маск нижнего слоя
    [Space(10)]
    [Header("Текстуры маск нижнего слоя")]
    public Sprite[] maskSend;
    public Sprite[] maskDirt;
    [Space(10)]
    [Header("Текстуры пород среднего слоя")]
    //текстуры пород среднего слоя
    public Sprite[] textureRock;

    void Start() {
        for (int i = 0; i < textures.Length; i++) {
            SpriteStruct spriteStruct = new SpriteStruct();
            spriteStruct.sprite = Resources.LoadAll<Sprite>(textures[i].name);
            spriteStruct.countWidthSprite = Mathf.FloorToInt(textures[i].width / textureSize);
            spriteStruct.countHeightSprite = Mathf.FloorToInt(textures[i].height / textureSize);
            sprites.Add(spriteStruct);
        }
    }

    public Sprite getSprite(int x, int y, int id) {
        x -= Mathf.FloorToInt(x / sprites[id].countWidthSprite) * sprites[id].countWidthSprite;
        y -= Mathf.FloorToInt(y / sprites[id].countHeightSprite) * sprites[id].countHeightSprite;
        int textureId = x + y * sprites[id].countWidthSprite;
        return sprites[id].sprite[textureId];
    }

    public void setTile(int x, int y, int lowerId, int averageId) {
        //lower
        int xT = (int)Node.sMapClass.calibrationX(x);
        xT -= Mathf.FloorToInt(xT / sprites[lowerId].countWidthSprite) * sprites[lowerId].countWidthSprite;
        int yT = (int)Node.sMapClass.calibrationX(y);
        yT -= Mathf.FloorToInt(yT / sprites[lowerId].countHeightSprite) * sprites[lowerId].countHeightSprite;
        yT = sprites[lowerId].countHeightSprite - 1 - yT;
        int textureId = xT + yT * sprites[lowerId].countWidthSprite;

        Tile texture = ScriptableObject.CreateInstance<Tile>();
        texture.sprite = sprites[lowerId].sprite[textureId];

        Node.sMapClass.getLowerTilemap(lowerId).SetTile(new Vector3Int(x, y, 0), texture);
        Node.sMapClass.getLowerTilemap(lowerId).SetTile(new Vector3Int(x, y, 1), null);
        if (lowerId != 0) setMask(x, y, lowerId);

        //average
        if (averageId == 33) {
            bool LD = false, L = false, LU = false, U = false, RU = false, R = false, RD = false, D = false;
            for (int x2 = x - 1; x2 <= x + 1; x2++) {
                for (int y2 = y - 1; y2 <= y + 1; y2++) {
                    if (Node.sMapClass.averageMapArr[(int)Node.sMapClass.calibrationX(x2), (int)Node.sMapClass.calibrationY(y2)] == 33) {
                        if (x2 == x - 1 && y2 == y - 1) LD = true;
                        else if (x2 == x - 1 && y2 == y) L = true;
                        else if (x2 == x - 1 && y2 == y + 1) LU = true;
                        else if (x2 == x && y2 == y + 1) U = true;
                        else if (x2 == x + 1 && y2 == y + 1) RU = true;
                        else if (x2 == x + 1 && y2 == y) R = true;
                        else if (x2 == x + 1 && y2 == y - 1) RD = true;
                        else if (x2 == x && y2 == y - 1) D = true;
                    }
                }
            }
            int idTeksture = getIdTeksture(LD, L, LU, U, RU, R, RD, D);
            if (idTeksture != -1) {
                Tile b = ScriptableObject.CreateInstance<Tile>();
                b.sprite = textureRock[idTeksture];
                Node.sMapClass.averageRock.SetTile(new Vector3Int(x, y, 0), b);
            }
        }
    }

    public void setMask(int x, int y, int lowerId) {
        for (int i = x - 1; i <= x + 1; i++) {
            for (int j = y - 1; j <= y + 1; j++) {
                if ((i == x && j == y) || (Node.sMapClass.lowerMapArr[(int)Node.sMapClass.calibrationX(i), (int)Node.sMapClass.calibrationY(j)] == lowerId)) continue;
                bool LD = false, L = false, LU = false, U = false, RU = false, R = false, RD = false, D = false;
                for (int i2 = i - 1; i2 <= i + 1; i2++) {
                    for (int j2 = j - 1; j2 <= j + 1; j2++) {
                        if (Node.sMapClass.getLowerTilemap(lowerId).GetTile(new Vector3Int(i2, j2, 0)) != null) {
                            if (i2 == i - 1 && j2 == j - 1) LD = true;
                            else if (i2 == i - 1 && j2 == j) L = true;
                            else if (i2 == i - 1 && j2 == j + 1) LU = true;
                            else if (i2 == i && j2 == j + 1) U = true;
                            else if (i2 == i + 1 && j2 == j + 1) RU = true;
                            else if (i2 == i + 1 && j2 == j) R = true;
                            else if (i2 == i + 1 && j2 == j - 1) RD = true;
                            else if (i2 == i && j2 == j - 1) D = true;
                        }
                    }
                }

                int idMask = getIdMask(LD, L, LU, U, RU, R, RD, D);
                if (idMask != -1) {
                    Tile b = ScriptableObject.CreateInstance<Tile>();
                    b.sprite = getMask(lowerId, idMask);
                    Node.sMapClass.getLowerTilemap(lowerId).SetTile(new Vector3Int(i, j, 1), b);
                    Node.sMapClass.lowerMapMaskArr[(int)Node.sMapClass.calibrationX(i), (int)Node.sMapClass.calibrationY(j)] = true;
                }
                else Node.sMapClass.getLowerTilemap(lowerId).SetTile(new Vector3Int(i, j, 1), null);
            }
        }
    }

    public Sprite getMask(int lowerId, int idMask) {
        if (lowerId == 2) return maskSend[idMask];
        else if(lowerId == 3) return maskDirt[idMask];
        else return maskSend[idMask];
    }

    private int getIdMask(bool LD, bool L, bool LU, bool U, bool RU, bool R, bool RD, bool D) {
        if (L && U && R && D) return 45;
        else if (D && L && U) return 44;
        else if (R && D && L) return 43;
        else if (U && R && D) return 42;
        else if (L && U && R) return 41;
        else if (U && D) return 40;
        else if (L && R) return 39;
        else if (L && D && RU) return 38;
        else if (R && D && LU) return 37;
        else if (U && R && LD) return 36;
        else if (L && U && RD) return 35;
        else if (L && D) return 34;
        else if (R && D) return 33;
        else if (U && R) return 32;
        else if (L && U) return 31;
        else if (D && LU && RU) return 30;
        else if (D && RU) return 29;
        else if (D && LU) return 28;
        else if (D) return 27;
        else if (R && LD && LU) return 26;
        else if (R && LU) return 25;
        else if (R && LD) return 24;
        else if (R) return 23;
        else if (U && RD && LD) return 22;
        else if (U && RD) return 21;
        else if (U && LD) return 20;
        else if (U) return 19;
        else if (L && RU && RD) return 18;
        else if (L && RD) return 17;
        else if (L && RU) return 16;
        else if (L) return 15;
        else if (LD && LU && RU && RD) return 14;
        else if (LD && LU && RD) return 13;
        else if (LD && RU && RD) return 12;
        else if (LU && RU && RD) return 11;
        else if (LD && LU && RU) return 10;
        else if (LU && RD) return 9;
        else if (LD && RU) return 8;
        else if (LD && RD) return 7;
        else if (RU && RD) return 6;
        else if (LU && RU) return 5;
        else if (LD && LU) return 4;
        else if (RD) return 3;
        else if (RU) return 2;
        else if (LU) return 1;
        else if (LD) return 0;
        else return -1;
    }

    private int getIdTeksture(bool LD, bool L, bool LU, bool U, bool RU, bool R, bool RD, bool D) {
        if (LD && L && LU && U && RU && R && RD && D) return 0;
        else if (!LD && L && LU && U && RU && R && RD && D) return 1;
        else if (LD && L && !LU && U && RU && R && RD && D) return 2;
        else if (LD && L && LU && U && !RU && R && RD && D) return 3;
        else if (LD && L && LU && U && RU && R && !RD && D) return 4;
        else if (!LD && L && !LU && U && RU && R && RD && D) return 5;
        else if (LD && L && !LU && U && !RU && R && RD && D) return 6;
        else if (LD && L && LU && U && !RU && R && !RD && D) return 7;
        else if (!LD && L && LU && U && RU && R && !RD && D) return 8;
        else if (!LD && L && LU && U && !RU && R && RD && D) return 9;
        else if (LD && L && !LU && U && RU && R && !RD && D) return 10;
        else if (!LD && L && !LU && U && !RU && R && RD && D) return 11;
        else if (LD && L && !LU && U && !RU && R && !RD && D) return 12;
        else if (!LD && L && LU && U && !RU && R && !RD && D) return 13;
        else if (!LD && L && !LU && U && RU && R && !RD && D) return 14;
        else if (!LD && L && !LU && U && !RU && R && !RD && D) return 15;

        else if (!L && U && RU && R && RD && D) return 16;
        else if (!L && U && !RU && R && RD && D) return 17;
        else if (!L && U && RU && R && !RD && D) return 18;
        else if (!L && U && !RU && R && !RD && D) return 19;

        else if (LD && L && !U && R && RD && D) return 20;
        else if (!LD && L && !U && R && RD && D) return 21;
        else if (LD && L && !U && R && !RD && D) return 22;
        else if (!LD && L && !U && R && !RD && D) return 23;

        else if (LD && L && LU && U && !R && D) return 24;
        else if (!LD && L && LU && U && !R && D) return 25;
        else if (LD && L && !LU && U && !R && D) return 26;
        else if (!LD && L && !LU && U && !R && D) return 27;

        else if (L && LU && U && RU && R && !D) return 28;
        else if (L && !LU && U && RU && R && !D) return 29;
        else if (L && LU && U && !RU && R && !D) return 30;
        else if (L && !LU && U && !RU && R && !D) return 31;

        else if (!L && !U && R && RD && D) return 32;
        else if (LD && L && !U && !R && D) return 33;
        else if (L && LU && U && !R && !D) return 34;
        else if (!L && U && RU && R && !D) return 35;

        else if (!L && !U && R && !RD && D) return 36;
        else if (!LD && L && !U && !R && D) return 37;
        else if (L && !LU && U && !R && !D) return 38;
        else if (!L && U && !RU && R && !D) return 39;

        else if (!L && U && !R && D) return 40;
        else if (L && !U && R && !D) return 41;

        else if (!L && !U && !R && D) return 42;
        else if (L && !U && !R && !D) return 43;
        else if (!L && U && !R && !D) return 44;
        else if (!L && !U && R && !D) return 45;

        else if (!L && !U && !R && !D) return 46;
        else return -1;
    }

    public void removeTile(Tilemap tileMap, int x, int y) {
        tileMap.SetTile(new Vector3Int(x, y, 0), null);
    }

    public struct SpriteStruct {
        public Sprite[] sprite;
        public int countWidthSprite;
        public int countHeightSprite;
    }

    public struct cost {
        public Sprite sprite;
    }
}
