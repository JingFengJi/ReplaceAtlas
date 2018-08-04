using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.TreeViewExamples;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;
using UnityEditor;
using System;

internal class ReplaceUISpriteTreeView : TreeViewWithTreeModel<ReplaceUISpriteTreeElement> 
{
    enum MyColumns
    {
		GameObjectName,
		UIAtlas,
		UISprite,
        ChildPath,
		Replace,
    }

    public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
    {
        if (root == null)
            return;
        if (result == null)
            return;

        result.Clear();

        if (root.children == null)
            return;

        Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
        for (int i = root.children.Count - 1; i >= 0; i--)
            stack.Push(root.children[i]);

        while (stack.Count > 0)
        {
            TreeViewItem current = stack.Pop();
            result.Add(current);

            if (current.hasChildren && current.children[0] != null)
            {
                for (int i = current.children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.children[i]);
                }
            }
        }
    }

    public ReplaceUISpriteTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<ReplaceUISpriteTreeElement> model) : base(state, multicolumnHeader, model)
    {
		rowHeight = 20;
		columnIndexForTreeFoldouts = 2;
		showAlternatingRowBackgrounds = true;
		showBorder = true;
		customFoldoutYOffset = (rowHeight - EditorGUIUtility.singleLineHeight) * 0.5f;
		extraSpaceBeforeIconAndLabel = 18f;
		Reload();
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        var rows = base.BuildRows(root);
        SortIfNeeded(root, rows);
        return rows;
    }

    void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
    {
        if (rows.Count <= 1)
            return;

        if (multiColumnHeader.sortedColumnIndex == -1)
        {
            return; // No column to sort for (just use the order the data are in)
        }

        // Sort the roots of the existing tree items
        //SortByMultipleColumns();
        TreeToList(root, rows);
        Repaint();
    }

    void SortByMultipleColumns()
    {
        
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (TreeViewItem<ReplaceUISpriteTreeElement>)args.item;

        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
        }
    }

    void CellGUI(Rect cellRect, TreeViewItem<ReplaceUISpriteTreeElement> item, MyColumns column, ref RowGUIArgs args)
    {
        // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
        CenterRectUsingSingleLineHeight(ref cellRect);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        switch (column)
        {
            case MyColumns.GameObjectName:
				EditorGUI.LabelField(cellRect,item.data.GameObjectName,style);
                break;
            case MyColumns.UIAtlas:
				EditorGUI.LabelField(cellRect,item.data.Atlas,style);
                break;
            case MyColumns.UISprite:
				EditorGUI.LabelField(cellRect,item.data.Sprite,style);
                break;
            case MyColumns.ChildPath:
                EditorGUI.LabelField(cellRect,item.data.Path,style);
                break;
            case MyColumns.Replace:
                Rect toggleRect = new Rect(cellRect.x,cellRect.y,18,cellRect.height);
                Rect labelRect = new Rect(toggleRect.xMax,cellRect.y,cellRect.width / 2,cellRect.height);
                
                item.data.Replace = EditorGUI.Toggle(toggleRect,item.data.Replace);
                
                style.alignment = TextAnchor.MiddleLeft;
                if(item.data.Replace)
                {
                    EditorGUI.LabelField(labelRect,"替换",style);
                }
                else
                {
                    EditorGUI.LabelField(labelRect,"不替换",style);
                }
                break;
        }
    }

    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
    {
        int headNum = 5;
        float _width = treeViewWidth / headNum;
        var columns = new[]
        {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("GameObject Name","This is GameObject Name"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = _width,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Atlas",""),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = _width,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("UISprite",""),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = _width,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Child Path",""),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = _width,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("是否替换",""),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = _width,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                }
				// ,
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "Lorem ipsum dolor sit amet, consectetur adipiscing elit. "),
                //     contextMenuText = "GameObject Name",                      //���������Ĳ˵����л��ɼ��ԣ����û�����ã�ʹ��headerContent
				// 	headerTextAlignment = TextAlignment.Center,     //ͷ�����ֶ���
				// 	sortedAscending = true,                         //��ֵ���Ƹ����Ƿ�������߽�������
				// 	sortingArrowAlignment = TextAlignment.Right,    //�����ͷ����
				// 	width = 30,                                     //���
				// 	minWidth = 30,                                  //��С���
				// 	maxWidth = 60,                                  //�����
				// 	autoResize = false,
                //     allowToggleVisibility = true                    //ѡ���Ƿ�����Ӳ˵���������
				// },
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Sed hendrerit mi enim, eu iaculis leo tincidunt at."),
                //     contextMenuText = "Type",
                //     headerTextAlignment = TextAlignment.Center,
                //     sortedAscending = true,
                //     sortingArrowAlignment = TextAlignment.Right,
                //     width = 30,
                //     minWidth = 30,
                //     maxWidth = 60,
                //     autoResize = false,
                //     allowToggleVisibility = false
                // },
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent("Name","This is Name Tips"),
                //     headerTextAlignment = TextAlignment.Center,
                //     sortedAscending = true,
                //     sortingArrowAlignment = TextAlignment.Center,
                //     width = 150,
                //     minWidth = 60,
                //     autoResize = false,
                //     allowToggleVisibility = false
                // },
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent("Multiplier", "In sed porta ante. Nunc et nulla mi."),
                //     headerTextAlignment = TextAlignment.Right,
                //     sortedAscending = true,
                //     sortingArrowAlignment = TextAlignment.Left,
                //     width = 110,
                //     minWidth = 60,
                //     autoResize = true
                // },
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent("Material", "Maecenas congue non tortor eget vulputate."),
                //     headerTextAlignment = TextAlignment.Right,
                //     sortedAscending = true,
                //     sortingArrowAlignment = TextAlignment.Left,
                //     width = 95,
                //     minWidth = 60,
                //     autoResize = true,
                //     allowToggleVisibility = true
                // },
                // new MultiColumnHeaderState.Column
                // {
                //     headerContent = new GUIContent("Note", "Nam at tellus ultricies ligula vehicula ornare sit amet quis metus."),
                //     headerTextAlignment = TextAlignment.Right,
                //     sortedAscending = true,
                //     sortingArrowAlignment = TextAlignment.Left,
                //     width = 70,
                //     minWidth = 60,
                //     autoResize = true
                // }
            };

        //���ԱȽ������Ƿ���ȣ������ӡError Log��Number of columns should match number of enum values: You probably forgot to update one of them.
        Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

        var state = new MultiColumnHeaderState(columns);
        return state;
    }
}