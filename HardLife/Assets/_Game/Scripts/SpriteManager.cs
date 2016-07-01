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

    public Sprite GetSprite(string name)
    {
        List<Sprite> pickedSprites = new List<Sprite>();
        int spriteNum = 5;
        name = TextureNames(name);
        name = name.ToLower();

        for (int i = 0; i < spriteNum+1; i++)
        {
            try
            {
                pickedSprites.Add(sprites[name + "_" + i.ToString()]);
            }
            catch ( KeyNotFoundException)
            {
                if (i == 0)
                {
                    pickedSprites.Add(sprites["error"]);
                    Debug.Log("Cannot find sprite: " + name + "_0");
                }
                break;
            }
            
        }

        Sprite pickedSprite = pickedSprites[UnityEngine.Random.Range(0, pickedSprites.Count)];

        return pickedSprite;
    }

    public Sprite GetSprite(BaseObjectModel item)
    {
        string name = item.type;

        if (item.classType == "Tree")
        {
            Tree tree = (Tree)item;
           
            if (tree.ageText != null)
                name += "_" + tree.ageText;

            
            if (tree.fruit > 0)
                name += "_fruit";
            else if (tree.state != "Normal")
                name += "_" + tree.state;
        } else if (item.classType == "Bush")
        {
            Bush bush = (Bush)item;


            if (bush.fruit > 0)
                name += "_fruit";
            else if (bush.state != "Normal")
                name += "_dying";
        }

        return GetSprite(name.ToLower());
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
