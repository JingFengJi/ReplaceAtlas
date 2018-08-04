using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;

class ReplaceAtalas : EditorWindow 
{

	private static ReplaceAtalas window = null;
	private static List<string> prefabPathList = new List<string> ();
	private static string assetPath;

    TreeViewState m_TreeViewState;
    MultiColumnHeaderState m_MultiColumnHeaderState;
    ReplaceUISpriteTreeView m_TreeView;
    public ReplaceUISpriteTreeView treeView
    {
        get { return m_TreeView; }
    }
    
	Rect SearchFieldRect
	{
		get
		{
			return new Rect(interval,interval,position.width * 0.3f,20f);
		}
	}

	Rect prefabListRect 
	{
        get { return new Rect (interval, interval + SearchFieldRect.yMax, SearchFieldRect.width, window.position.height - SearchFieldRect.yMax - 2 * interval); }
    }

	Rect replaceAtalsRect
	{
		get{return new Rect(prefabListRect.xMax + interval,interval,window.position.width - SearchFieldRect.width - 3 * interval,120);}
	}
    Rect replaceUISpriteTreeViewRect
    {
        get { return new Rect(replaceAtalsRect.x, replaceAtalsRect.yMax + interval, replaceAtalsRect.width, window.position.height - replaceAtalsRect.height - 3 * interval); }
    }

	private List<ReplaceUISpriteTreeElement> replaceSpriteTreeElementList = new List<ReplaceUISpriteTreeElement>();

	private Vector2 scrollWidgetPos;
	private float interval = 20f;
	private string searchStr = "";
	private SearchField searchField;
	private bool initialized = false;

	[MenuItem("Assets/替换图集", false, 2001)]
	public static void OpenWindow()
	{
		string selectedAssetPath = AssetDatabase.GetAssetPath (Selection.activeObject);
		if(!string.IsNullOrEmpty(selectedAssetPath) && selectedAssetPath.EndsWith(".prefab"))
		{
			ReplaceAtalas window = ShowWindow();
			if(window != null)
			{
				window.AutoSelctPrefab(selectedAssetPath);
			}
		}
	}

	[MenuItem("Tools/ReplaceAtalas")]
	public static ReplaceAtalas ShowWindow() 
	{
		prefabPathList.Clear ();
        assetPath = Application.dataPath;
        GetFiles (new DirectoryInfo (assetPath), "*.prefab", ref prefabPathList);
		if (window == null)
			window = EditorWindow.GetWindow(typeof(ReplaceAtalas)) as ReplaceAtalas;
		window.titleContent = new GUIContent("ReplaceAtalas");
		window.Show();
		return window;
	}

	public static void GetFiles (DirectoryInfo directory, string pattern, ref List<string> fileList) 
	{
        if (directory != null && directory.Exists && !string.IsNullOrEmpty (pattern)) {
            try {
                foreach (FileInfo info in directory.GetFiles (pattern)) {
                    string path = info.FullName.ToString ();
                    fileList.Add (path.Substring (path.IndexOf ("Assets")));
                }
            } catch (System.Exception) 
			{
                throw;
            }
            foreach (DirectoryInfo info in directory.GetDirectories ()) 
			{
                GetFiles (info, pattern, ref fileList);
            }
        }
    }

	private void OnGUI() 
	{
		InitIfNeeded();
		DrawWindow();
	}

	private void DrawWindow()
	{
		DrawSearchField();
		DrawPrefabList();
		DrawReplaceAtalasTool();
        DoTreeView(replaceUISpriteTreeViewRect);
	}

    void DoTreeView(Rect rect)
    {
        m_TreeView.OnGUI(rect);
    }

	private void InitIfNeeded()
	{
		if(!initialized)
		{
			if (null == searchField)
            	searchField = new SearchField ();
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            bool firstInit = m_MultiColumnHeaderState == null;

            var headerState = ReplaceUISpriteTreeView.CreateDefaultMultiColumnHeaderState(replaceUISpriteTreeViewRect.width);

            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
            m_MultiColumnHeaderState = headerState;

            var multiColumnHeader = new MultiColumnHeader(headerState);
            if (firstInit)
                multiColumnHeader.ResizeToFit();

            TreeModel<ReplaceUISpriteTreeElement> treeModel = new TreeModel<ReplaceUISpriteTreeElement>(GetData());

            m_TreeView = new ReplaceUISpriteTreeView(m_TreeViewState, multiColumnHeader, treeModel);

        }
        initialized = true;
	}

	private void DrawSearchField()
	{
		GUI.backgroundColor = Color.white;
		searchStr = searchField.OnGUI (SearchFieldRect, searchStr);
		searchStr = searchStr.ToLower();
	}

	private void DrawPrefabList()
	{
		GUI.backgroundColor = Color.white;
			
		GUI.Box(prefabListRect,"");
		GUILayout.BeginArea(prefabListRect);
		scrollWidgetPos = EditorGUILayout.BeginScrollView(scrollWidgetPos);
		for (int i = 0; i < prefabPathList.Count; i++)
		{
			if(CheckShowPrefab(prefabPathList[i],searchStr))
			{
				if(GUILayout.Button(prefabPathList[i]))
				{
					curReplacePrefabPath = prefabPathList[i];
					curPrefabAtlas = GetPrefabAllAtlas(curReplacePrefabPath);
                    m_TreeView.treeModel.SetData(GetData());
                    m_TreeView.Reload();
				}
			}
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private string curReplacePrefabPath = "";

	private bool CheckShowPrefab(string path,string searchstr)
	{
		if(string.IsNullOrEmpty(searchStr)) return true;
		if(string.IsNullOrEmpty(path)) return false;
		return GetFileNameWithSuffix(path.ToLower()).Contains(searchStr);
	}

	//包括后缀名
	private string GetFileNameWithSuffix(string path)
	{
		if(string.IsNullOrEmpty(path)) return string.Empty;
		return path.Substring(path.LastIndexOf("/")+1);
	}

	private UIAtlas curAtlas;
	private UIAtlas targetAtlas;

	void OnSelectAtlas (Object obj)
	{
		UIAtlas atlas = obj as UIAtlas;
		if(isSelectCurAtlas)
		{
			curAtlas = obj as UIAtlas;
		}
		else if(isSelectTargetAtlas)
		{
			targetAtlas = obj as UIAtlas;
		}
		isSelectCurAtlas = false;
		isSelectTargetAtlas = false;
	}

	private bool isSelectCurAtlas = false;
	private bool isSelectTargetAtlas = false;
	private List<UIAtlas> curPrefabAtlas = new List<UIAtlas>();
	private void DrawReplaceAtalasTool()
	{
		GUI.backgroundColor = Color.white;
        GUI.Box(replaceAtalsRect, "");
		GUILayout.BeginArea(replaceAtalsRect);
		EditorGUILayout.LabelField(curReplacePrefabPath);

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("该预制体含有的所有图集:",GUILayout.Width(150));
		if(curPrefabAtlas.Count > 0)
		{
			for (int i = 0; i < curPrefabAtlas.Count; i++)
			{
				if(GUILayout.Button(curPrefabAtlas[i].name))
				{
					curAtlas = curPrefabAtlas[i];
				}
				if(GUILayout.Button("Edit"))
				{
					//NGUISettings.atlas = mFont.atlas;
					//NGUISettings.selectedSprite = sym.spriteName;
					NGUIEditorTools.Select(curPrefabAtlas[i].gameObject);
				}
			}
		}
		GUILayout.EndHorizontal();

		//原图集
		GUILayout.BeginHorizontal();
		curAtlas = (UIAtlas)EditorGUILayout.ObjectField("被替换图集", curAtlas, typeof(UIAtlas), true);
		if (NGUIEditorTools.DrawPrefixButton("选择Atlas",GUILayout.Width(200)))
		{
			isSelectCurAtlas = true;
			ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
		}
		GUILayout.EndHorizontal();

		//目标图集
		GUILayout.BeginHorizontal();
        targetAtlas = (UIAtlas)EditorGUILayout.ObjectField("目标图集", targetAtlas, typeof(UIAtlas), true);		
		
		if (NGUIEditorTools.DrawPrefixButton("选择Atlas",GUILayout.Width(200)))
		{
			isSelectTargetAtlas = true;
			ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
		}
		GUILayout.EndHorizontal();

		if(GUILayout.Button("互换"))
		{
			UIAtlas tmpCurAtlas = curAtlas;
			curAtlas = targetAtlas;
			targetAtlas = tmpCurAtlas;
		}

		//替换按钮
		if(GUILayout.Button("替换图集"))
		{
			if(string.IsNullOrEmpty(curReplacePrefabPath))
			{
				EditorUtility.DisplayDialog("提示", "请先选择一个预制体!", "确定");
			}
			else if (curAtlas == null)
			{
				EditorUtility.DisplayDialog("提示", "请先指定被替换图集!", "确定");
			}
			else
			{
				bool ReplaceSucc = false;
				if(targetAtlas == null)
				{
					if(EditorUtility.DisplayDialog("提示", "原图集将被清空，确定替换吗？", "确定","取消"))
					{
						ReplacePrefabAtalas(curReplacePrefabPath);
						ReplaceSucc = true;
					}
				}
				else
				{
					ReplacePrefabAtalas(curReplacePrefabPath);
					ReplaceSucc = true;
				}
				if(ReplaceSucc)
				{
                    m_TreeView.treeModel.SetData(GetData());
                    m_TreeView.Reload();
				}
			}
		}
		//保存按钮
		
		//撤销按钮

		GUILayout.EndArea();
	}

    IList<ReplaceUISpriteTreeElement> GetData()
    {
        //List<ReplaceUISpriteTreeElement> data = new List<ReplaceUISpriteTreeElement>();
        replaceSpriteTreeElementList.Clear();
        ReplaceUISpriteTreeElement root = new ReplaceUISpriteTreeElement(null,"","Root", -1, 0);
        replaceSpriteTreeElementList.Add(root);
		if(!string.IsNullOrEmpty(curReplacePrefabPath))
		{
            GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(curReplacePrefabPath);
			if(gameObj != null)
			{
				UISprite[] sprites = gameObj.GetComponentsInChildren<UISprite>(true);
				if(sprites != null && sprites.Length > 0)
				{
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        ReplaceUISpriteTreeElement element1 = new ReplaceUISpriteTreeElement(sprites[i].gameObject,GetPath(gameObj,sprites[i].gameObject),sprites[i].gameObject.name, 0, i + 1);
                        replaceSpriteTreeElementList.Add(element1);
                    }
				}
			}
		}
        return replaceSpriteTreeElementList;
    }

	private string GetPath(GameObject root,GameObject child)
	{
		if(root == null || child == null) return "";
		List<string> parentList = new List<string>();
		parentList.Add(child.name);
		while(child.transform.parent != null && child.transform.parent != root)
		{
			child = child.transform.parent.gameObject;
			parentList.Add(child.name);
		}
		string path = "";
		for (int i = parentList.Count - 1; i >= 0; i--)
		{
			path += parentList[i];
			if(i != 0)
			{
				path += "/";
			}
		}
		return path;
	}

	private void ReplacePrefabAtalas(string path)
	{
		if(string.IsNullOrEmpty(path)) return;
		GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        ReplacePrefabAtalas(gameObj,curAtlas,targetAtlas);
	}	

	private List<UIAtlas> GetPrefabAllAtlas(string path)
	{
		if(string.IsNullOrEmpty(path)) return null;
		GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
		return GetPrefabAllAtlas(gameObj);
	}

	private List<UIAtlas> GetPrefabAllAtlas(GameObject prefab)
	{
		if(null == prefab) return null;
		List<UIAtlas> atlass = new List<UIAtlas>();
		UISprite[] sprites = prefab.GetComponentsInChildren<UISprite>(true);
		if(sprites != null && sprites.Length > 0)
		{
			int num = sprites.Length;
			for (int i = 0; i < num; i++)
			{
				UISprite s = sprites[i];
				if(s != null && !atlass.Contains(s.atlas))
				{
					atlass.Add(s.atlas);
				}
			}
		}
		return atlass;
	}

	private void ReplacePrefabAtalas(GameObject prefab , UIAtlas atlas,UIAtlas targetAtlas)
	{
		if(prefab == null || atlas == null) return;
		UISprite[] sprites = prefab.GetComponentsInChildren<UISprite>(true);
		if(sprites != null && sprites.Length > 0)
		{
			int num = sprites.Length;
			for (int i = 0; i < num; i++)
			{
				UISprite s = sprites[i];
				//因为replaceSpriteTreeElementList的第一个是root
				if(s != null && s.atlas == atlas && i < replaceSpriteTreeElementList.Count - 1 
					&& replaceSpriteTreeElementList[i+1] != null && replaceSpriteTreeElementList[i+1].Replace)
				{
					s.atlas = targetAtlas;
					EditorUtility.SetDirty(s);
				}
			}
		}
		AssetDatabase.SaveAssets();
	}

	public void AutoSelctPrefab(string prefabPath)
	{
		if(string.IsNullOrEmpty(prefabPath)) return;
		curReplacePrefabPath = prefabPath;
		curPrefabAtlas = GetPrefabAllAtlas(curReplacePrefabPath);
		curAtlas = null;
		targetAtlas = null;
	}
}