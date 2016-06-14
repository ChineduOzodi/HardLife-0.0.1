using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteManager{

    string spritesPath = "Sprites";
    private Dictionary<string,Sprite> sprites;
    public SpriteManager()
    {
        Sprite[] allsprites = Resources.LoadAll<Sprite>(spritesPath);
        foreach (Sprite sprite in allsprites)
        {
            sprites.Add(sprite.name, sprite);
        }
    }

	public Sprite[] GetSprites(string name, int spriteNum){
		Sprite[] pickedSprites = new Sprite[spriteNum];
		for (int i = 0; i < spriteNum; i++) {
			pickedSprites[i] = sprites[name+"_"+i.ToString()];
		}

		return pickedSprites;
	}

}
