using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour {
    public Texture2D[] textures;
    public int textureSize;
    public static List<SpriteStruct> sprites = new List<SpriteStruct>();

    void Start() {
        for (int i = 0; i < textures.Length; i++) {
            SpriteStruct spriteStruct = new SpriteStruct();
            spriteStruct.sprite = Resources.LoadAll<Sprite>(textures[i].name);
            spriteStruct.countWidthSprite = Mathf.FloorToInt(textures[i].width / textureSize);
            spriteStruct.countHeightSprite = Mathf.FloorToInt(textures[i].height / textureSize);
            sprites.Add(spriteStruct);
        }
    }

    public static Sprite getSprite(int x, int y, int id) {
        x -= Mathf.FloorToInt(x / sprites[id].countWidthSprite) * sprites[id].countWidthSprite;
        y -= Mathf.FloorToInt(y / sprites[id].countHeightSprite) * sprites[id].countHeightSprite;
        int textureId = x + y * sprites[id].countWidthSprite;
        return sprites[id].sprite[textureId];
    }

    public struct SpriteStruct {
        public Sprite[] sprite;
        public int countWidthSprite;
        public int countHeightSprite;
    }
}
