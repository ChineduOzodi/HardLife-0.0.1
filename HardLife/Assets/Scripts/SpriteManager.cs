using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteManager{

	public Sprite[] GetSprites(string name, int spriteNum){
		Sprite[] sprites = new Sprite[spriteNum];
		for (int i; i < spriteNum; i++) {
			sprites[i] = Resources.Load (name+"_"+i.ToString()) as Sprite;
		}

		return sprites;
	}

}
