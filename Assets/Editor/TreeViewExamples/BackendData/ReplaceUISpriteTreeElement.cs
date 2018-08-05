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
	public string AtlasStr;
	public UIAtlas Atlas;
	public string SpriteName;
	public Texture2D SpriteTexture;
	public UISpriteData SpriteData;
	public bool Replace;
	public UISpriteData TargetSpriteData;
	public Texture2D TargetSpriteTexture;
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
                AtlasStr = sprite.atlas.name;
				Atlas = sprite.atlas;
                SpriteName = sprite.spriteName;
                SpriteData = (Atlas != null) ? Atlas.GetSprite(NGUISettings.selectedSprite) : null;
                if (SpriteData != null)
                    SpriteTexture = Atlas.texture as Texture2D;
            }
		}
        
		Path = path;
		Replace = true;
	}

	public void SetReplaceTargetAtlas(UIAtlas targetAtlas)
	{
		if(targetAtlas == null)
		{
			TargetSpriteData = null;
			TargetSpriteTexture = null;
		}
		if(string.IsNullOrEmpty(SpriteName)) return;
		TargetSpriteData = (targetAtlas != null) ? targetAtlas.GetSprite(SpriteName) : null;
		if(TargetSpriteData != null)
			TargetSpriteTexture = targetAtlas.texture as Texture2D;
	}
}
