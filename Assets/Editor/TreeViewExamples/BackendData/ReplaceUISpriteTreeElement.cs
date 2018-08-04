using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TreeViewExamples;
using System;

[Serializable]
internal class ReplaceUISpriteTreeElement : TreeElement 
{
	public GameObject Go;
	public string GameObjectName;
	public string Atlas;
	public string Sprite;
	public bool Replace;
	public string Path;

	public ReplaceUISpriteTreeElement(GameObject go,string path, string name,int depth,int id):base(name,depth,id)
	{
		if(go != null)
		{
            this.Go = go;
            GameObjectName = go.name;
            UISprite sprite = go.GetComponent<UISprite>();
            if (sprite != null)
            {
                Atlas = sprite.atlas.name;
                Sprite = sprite.spriteName;
            }
		}
		Path = path;
		Replace = true;
	}
}
