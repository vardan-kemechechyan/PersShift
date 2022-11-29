using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Customizable_SO : ScriptableObject
{
	public string itemName;
	public Sprite shopIcon;
	public int orderInShop;
	public string itemTypeToString;
	public string skinTypeToString;
	public bool statusHasChanged;// This is done, to quickly find changed items, to make the save quicker
	public ShopSection shopSection;
	public SelectionState selectionState;
	public HighLight highlight;
	public HighLight initialHighlight;
	public LockInfo lockInfo;
	public int myAttachedButtonIndex;
}

[System.Serializable]
public struct LockInfo
{
	public LockStatus lockStatus; //Change to unlocked when acquired
	public LockType lockType; // Change tp none when acquired
	public int priceToUnlock; // if -1 get for free and default
	public int levelToUnlock; // if -1 get for money without level attachement
}