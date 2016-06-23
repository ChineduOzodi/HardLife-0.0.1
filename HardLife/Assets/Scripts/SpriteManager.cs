using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpriteManager{

    string spritesPath = "Sprites";
    private Dictionary<string,Sprite> sprites;
    public SpriteManager()
    {
        Sprite[] allsprites = Resources.LoadAll<Sprite>(spritesPath);
        sprites = new Dictionary<string, Sprite>();

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

    public Sprite GetSprite(string name, int spriteNum = 0)
    {
        Sprite[] pickedSprites = new Sprite[spriteNum+1];
        name = TextureNames(name);
        name = name.ToLower();

        for (int i = 0; i < spriteNum+1; i++)
        {
            try
            {
                pickedSprites[i] = sprites[name + "_" + i.ToString()];
            }
            catch ( KeyNotFoundException)
            {
                pickedSprites[i] = sprites["error"];
                Debug.Log("Cannot find sprite: " + name + "_" + i.ToString());
            }
            
        }

        Sprite pickedSprite = pickedSprites[UnityEngine.Random.Range(0, spriteNum)];

        return pickedSprite;
    }

    private string TextureNames(string name)
    {
        name = name.ToLower();
        if (name == "desert")
        {
            name = "sand";
        }
        return name;
    }
}
