using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystemExperimental : Singleton<SaveSystemExperimental>, ISubscribeUnsubscribe
{
    public static Action ApplyChangesFromShop;
    public static Action<string> CustomizableUnlocked;

    [SerializeField] int defaultLevel;
    [SerializeField] int defaultMoney;

    [SerializeField] Configuration Config;
    [SerializeField] Customizable_SO ItemInfoTemplate;

    public List<Customizable_SO> ItemDescriptions = new List<Customizable_SO>();
    
    Dictionary<ShopSection, Dictionary<string, List<Customizable_SO>>> OriginalCustomizables = new Dictionary<ShopSection, Dictionary<string, List<Customizable_SO>>>();
    public Dictionary<ShopSection, Dictionary<string, List<Customizable_SO>>> RuntimeList = new Dictionary<ShopSection, Dictionary<string, List<Customizable_SO>>>();
    
    Dictionary<CustomizableItems, List<AccessoryTypeName>> AllAccessories = new Dictionary<CustomizableItems, List<AccessoryTypeName>>();

    PlayerData data = new PlayerData();
    public CharacterBuild build = new CharacterBuild();

    string savePath = "/playerJSONdata.weave";

    [SerializeField] string json_save;

    bool firstLaunch = true;

    protected override void Awake()
    {
        base.Awake();

        DataCheck();

        PopulateItemInfo();

        Subscribe();
    }

	private void OnDestroy()
	{
        UnSubscribe();
    }

	void DataCheck()
    {
        if(File.Exists(Application.persistentDataPath + savePath))
        {
            firstLaunch = false;

            Load();
        }
        else Save();
    }

    void PopulateItemInfo()
    {
        ShopSection[] ss = Enum.GetValues(typeof(ShopSection)).OfType<ShopSection>().ToArray();
        LockStatus[] ips = Enum.GetValues(typeof(LockStatus)).OfType<LockStatus>().ToArray();
        Appearance[] ap = Enum.GetValues(typeof(Appearance)).OfType<Appearance>().ToArray();
        CustomizableItems[] ci = Enum.GetValues(typeof(CustomizableItems)).OfType<CustomizableItems>().ToArray();
        LockType[] lt = Enum.GetValues(typeof(LockType)).OfType<LockType>().ToArray();
        HighLight[] hl = Enum.GetValues(typeof(HighLight)).OfType<HighLight>().ToArray();
        SelectionState[] sst = Enum.GetValues(typeof(SelectionState)).OfType<SelectionState>().ToArray();

        foreach(var item in data.ShopItemInfos)
        {
            ItemDescriptions.Add(new Customizable_SO
            {
                itemName = item.itemName,
                shopIcon = Resources.Load<Sprite>("Icons/" + item.shopSection + "/" + item.itemName),
                orderInShop = item.orderInShop,
                myAttachedButtonIndex = item.orderInShop % 6,
                shopSection = ss.First((x) => x.ToString() == item.shopSection),
                itemTypeToString = item.itemName.Split('_')[0],
                skinTypeToString = item.itemName.Split('_')[1],
                selectionState = sst.First((x) => x.ToString() == item.selectionState),
                highlight = hl.First((x) => x.ToString() == item.highlight),
                initialHighlight = hl.First((x) => x.ToString() == item.highlight),
                lockInfo = new LockInfo
                {
                    lockStatus = ips.First((x) => x.ToString() == item.lockStatus),
                    lockType = lt.First((x) => x.ToString() == item.lockType),
                    priceToUnlock = item.priceToUnlock,
                    levelToUnlock = item.levelToUnlock
                },
                statusHasChanged = false,
            });

            if(item.shopSection != "Costume")
            {
                CustomizableItems c = ci.First((x) => x.ToString() == item.itemName.Split('_')[0]);

                if(!AllAccessories.ContainsKey(c)) AllAccessories.Add(c, new List<AccessoryTypeName>());

                AllAccessories[c].Add(new AccessoryTypeName
                {
                    itemType = c,
                    name = item.itemName
                });
			}

            Customizable_SO lastElement = ItemDescriptions[ItemDescriptions.Count - 1];

            Dictionary<string, List<Customizable_SO>> temp1 = new Dictionary<string, List<Customizable_SO>>();
            List<Customizable_SO> temp2 = new List<Customizable_SO>();

            if(!OriginalCustomizables.ContainsKey(lastElement.shopSection))
            {
                OriginalCustomizables.Add(lastElement.shopSection, new Dictionary<string, List<Customizable_SO>>());
                RuntimeList.Add(lastElement.shopSection, new Dictionary<string, List<Customizable_SO>>());
            }

            temp1 = OriginalCustomizables[lastElement.shopSection];

            if(!temp1.ContainsKey(lastElement.itemTypeToString))
            {
                temp1.Add(lastElement.itemTypeToString, new List<Customizable_SO>());
                RuntimeList[lastElement.shopSection].Add(lastElement.itemTypeToString, new List<Customizable_SO>());
            }

            temp2 = temp1[lastElement.itemTypeToString];

            temp2.Add(lastElement);

            build.HairColor = data.HairColor;

            RuntimeList[lastElement.shopSection][lastElement.itemTypeToString].Add(new Customizable_SO 
            {
                itemName = lastElement.itemName,
                shopIcon = Resources.Load<Sprite>("Icons/" + lastElement.shopSection + "/" + lastElement.itemName),
                orderInShop = lastElement.orderInShop,
                myAttachedButtonIndex = lastElement.myAttachedButtonIndex,
                itemTypeToString = lastElement.itemTypeToString,
                skinTypeToString = lastElement.skinTypeToString,
                shopSection = lastElement.shopSection,
                selectionState = lastElement.selectionState,
                highlight = lastElement.highlight,
                initialHighlight = lastElement.highlight,
                lockInfo = new LockInfo
                {
                    lockStatus = lastElement.lockInfo.lockStatus,
                    lockType = lastElement.lockInfo.lockType,
                    priceToUnlock = lastElement.lockInfo.priceToUnlock,
                    levelToUnlock = lastElement.lockInfo.levelToUnlock
                },
                statusHasChanged = lastElement.statusHasChanged,
            });

            if(build.CustomComponents == null || build.CustomComponents.Count == 0) build.CustomComponents = new List<Customizable_SO>();

            if(lastElement.lockInfo.lockStatus == LockStatus.UNLOCKED && lastElement.selectionState == SelectionState.SELECTED )
                build.CustomComponents.Add(lastElement);
        }
    }

    public void RefreshTheItemList()
    {
        foreach(var section in RuntimeList)
        {
            foreach(var itemType in section.Value)
            {
                foreach(var item in itemType.Value)
                {
                    if(item.lockInfo.lockStatus == LockStatus.LOCKED && item.lockInfo.lockType == LockType.BUY_FOR_MONEY )
                    {
                        item.selectionState = SelectionState.NOT_SELECTED;
                        item.highlight = HighLight.NONE;
                    }

                    if(item.selectionState == SelectionState.SELECTED)
                    {
                        item.highlight = HighLight.DEFAULT;
                        item.initialHighlight = HighLight.DEFAULT;
                    }
                    else
                    {
                        item.highlight = HighLight.NONE;
                        item.initialHighlight = HighLight.NONE;
                    }
                }
            }
        }
    }

    public void SyncAndSaveChanges()
    {
        RefreshTheItemList();

        UpdateItemDescriptions_SO();

        UpdateSaveFile();

        Save();
    }

    #region Save Zone
    public void UpdateItemDescriptions_SO()
    {
        build = new CharacterBuild();
        build.HairColor = data.HairColor;

		foreach(var shopSection in RuntimeList)
		{
			foreach(var itemType in shopSection.Value)
			{
				for(int i = 0; i < itemType.Value.Count; ++i)
				{
                    if
                    (
                        itemType.Value[i].lockInfo.lockStatus == LockStatus.LOCKED || 
                        itemType.Value[i].lockInfo.lockType != LockType.NONE || 
                        itemType.Value[i].lockInfo.priceToUnlock > 0                   
                    )
                    {

                        itemType.Value[i].lockInfo.lockStatus = LockStatus.LOCKED;
                        continue;
					}

                    OriginalCustomizables[shopSection.Key][itemType.Key][i].statusHasChanged = true;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].lockInfo.lockStatus = itemType.Value[i].lockInfo.lockStatus;

                    OriginalCustomizables[shopSection.Key][itemType.Key][i].selectionState = itemType.Value[i].selectionState;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].orderInShop = itemType.Value[i].orderInShop;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].highlight = itemType.Value[i].highlight;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].initialHighlight = itemType.Value[i].initialHighlight;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].lockInfo.lockType = itemType.Value[i].lockInfo.lockType;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].lockInfo.priceToUnlock = itemType.Value[i].lockInfo.priceToUnlock;
                    OriginalCustomizables[shopSection.Key][itemType.Key][i].lockInfo.levelToUnlock = itemType.Value[i].lockInfo.levelToUnlock;

                    if( OriginalCustomizables[shopSection.Key][itemType.Key][i].selectionState == SelectionState.SELECTED )
                        build.CustomComponents.Add(OriginalCustomizables[shopSection.Key][itemType.Key][i]);
                }
			}
		}
	}
    public void UpdateSaveFile()
    {
        foreach( var so in ItemDescriptions )
		{
            RuntimeList[so.shopSection][so.itemTypeToString].First(x => x.itemName == so.itemName).lockInfo.lockStatus = so.lockInfo.lockStatus;

            if(so.statusHasChanged)
            {
                Configuration.ShopItemInfo GeneratedSaveFile = new Configuration.ShopItemInfo();
                
                GeneratedSaveFile = data.ShopItemInfos.First(x => x.itemName == so.itemName);

                GeneratedSaveFile.itemName = so.itemName;
                GeneratedSaveFile.orderInShop = so.orderInShop;
                GeneratedSaveFile.shopSection = so.shopSection.ToString();
                GeneratedSaveFile.lockStatus = so.lockInfo.lockStatus.ToString();
                GeneratedSaveFile.lockType = so.lockInfo.lockType.ToString();
                GeneratedSaveFile.selectionState = so.selectionState.ToString();
                GeneratedSaveFile.highlight = so.highlight.ToString();
                GeneratedSaveFile.levelToUnlock = so.lockInfo.levelToUnlock;
                GeneratedSaveFile.priceToUnlock = so.lockInfo.priceToUnlock;
            }
        }
    }
    public void SaveMoney( int money ) { data.Money = money; }
    public void SaveHairColor( string hex_value) { build.HairColor = data.HairColor = hex_value; }
    public void SaveCurrentLevel(int currentLevelIndex)
    {
        data.CurrentLevel = currentLevelIndex;
    }

    public void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt("currentLevel", defaultLevel);
        PlayerPrefs.Save();
    }
    public void Save()
    {
        if(firstLaunch) FirstSave();

        PlayerPrefs.SetInt("currentLevel", data.CurrentLevel);

        json_save = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + savePath, json_save);

        foreach(var so in ItemDescriptions)
            if(so.statusHasChanged == true) so.statusHasChanged = false;

        PlayerPrefs.Save();
    }
    void FirstSave()
    {
        firstLaunch = false;

        PlayerPrefs.SetInt("currentLevel", defaultLevel);
        PlayerPrefs.Save();

        data = new PlayerData {

            Money = defaultMoney,
            CurrentLevel = PlayerPrefs.GetInt("currentLevel"),
            Ads_Removed = false,

            ShopItemInfos = Config.itemInfo
        };
    }
    #endregion

    #region Load Zone
    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + savePath))
        {
            json_save = File.ReadAllText(Application.persistentDataPath + savePath);
            data = JsonUtility.FromJson<PlayerData>(json_save);

            data.CurrentLevel = PlayerPrefs.GetInt("currentLevel");
        }
    }
    #endregion

    #region Get Zone

    /*public string GetGiftName(int _levelIndex)
    {
        string giftName = string.Empty;

        Debug.Log($"LevelIndex: {_levelIndex} and itemInfo length: {itemInfo.Count}");

        if(itemInfo.Any(x => x.levelToUnlock == _levelIndex))
            giftName = itemInfo.First(x => x.levelToUnlock == _levelIndex).itemName;

        return giftName;
    }*/

    public Customizable_SO GetItemFromByName( string _itemName )
    {
        return ItemDescriptions.First(x => x.itemName == _itemName);
	}
    public Customizable_SO GetItemFromRuntimeSOList(string _itemName)
    {
		foreach(var sections in RuntimeList)
		{
			foreach(var itemLists in sections.Value)
			{
				foreach(var item in itemLists.Value)
				{
                    if(item.itemName == _itemName)
                        return item;
                }
            }
        }

        return null;
    }
    public Customizable_SO GetItemFromRuntimeSOList( ShopSection _ss, string _itemtype, string itemName)
    {
        return RuntimeList[_ss][_itemtype].First(x => x.itemName == itemName);
	}
    public List<(string _itemType, string skinType)> ReturnListOfAccessories(Control _playerType)
    {
        List<(string _itemType, string skinType)> accessoriesToWear = new List<(string _itemType, string skinType)>();
        CustomizableItems[] ci = Enum.GetValues(typeof(CustomizableItems)).OfType<CustomizableItems>().ToArray();

        if(_playerType == Control.Player)
        {
            foreach(var accType in build.CustomComponents)
                if(accType.shopSection != ShopSection.Costume)
                    accessoriesToWear.Add((ci.First(x=> x.ToString() == accType.itemTypeToString).ToString(), accType.skinTypeToString));
        }
        else
        {
            foreach(var accType in AllAccessories)
            {
                if(accType.Key == CustomizableItems.Hairstyle) { if(UnityEngine.Random.value <= .04) continue; }
                else
                    if(UnityEngine.Random.value <= .5) continue;

                accessoriesToWear.Add((accType.Key.ToString(), accType.Value[UnityEngine.Random.Range(0, accType.Value.Count)].name.Split('_')[1]));
            }
        }

        return accessoriesToWear;
    }
    public Dictionary<Appearance, List<string>> ReturnNamesOfSelectedCostumes(List<Appearance> _locationNamesForTheLevel, Control _playerType)
    {
        Dictionary<Appearance, List<string>> Costumes = new Dictionary<Appearance, List<string>>();

        foreach(var location in _locationNamesForTheLevel)
        {
            if(_playerType == Control.Player)
            {
                //print(location.ToString() + " - ");
                //print(" -- " + build.CustomComponents.First(x => x.itemTypeToString == location.ToString()).itemName);
                Costumes.Add(location, new List<string> { build.CustomComponents.First(x => x.itemTypeToString == location.ToString()).itemName });
			}
            else
            {
                List<Customizable_SO> allCostumesOfType = ItemDescriptions.FindAll(x => x.itemTypeToString == location.ToString());

                Costumes.Add(location, new List<string>());

                foreach(var so in allCostumesOfType)
				{
                    if(location.ToString() == so.itemTypeToString)
                        Costumes[location].Add(so.itemName);
                }
            }
        }

        return Costumes;
    }
    public int GetMoney() { return data.Money; }
    public string GetHairColor( bool player = true) { return player ? data.HairColor : Config.hairStyleColors[UnityEngine.Random.Range(0, Config.hairStyleColors.Length)]; }
    public int GetCurrentLevel() { return data.CurrentLevel; }
    public List<Customizable_SO> ReturnItemsInSection(ShopSection _shopSection )
    {
        List<Customizable_SO> itemsToShip = new List<Customizable_SO>();

		foreach(var item in RuntimeList[_shopSection])
		{
			foreach(var so in item.Value)
			{
                itemsToShip.Add(so);
			}
        }

        return itemsToShip;
    }
    public CharacterBuild ReturnCopyOfBuild( Customizable_SO _focus) //Costume focus
    {
        var t = new CharacterBuild();

        foreach(var item in build.CustomComponents)
        {
            t.CustomComponents.Add(RuntimeList[item.shopSection][item.itemTypeToString].First(x => x.itemName == item.itemName));

            t.CustomComponents[t.CustomComponents.Count - 1].selectionState = SelectionState.SELECTED;
            
            if(t.CustomComponents[t.CustomComponents.Count - 1].shopSection != ShopSection.Costume)
            {
                t.CustomComponents[t.CustomComponents.Count - 1].highlight = HighLight.HIGHLIGHTED;
                t.CustomComponents[t.CustomComponents.Count - 1].initialHighlight = HighLight.DEFAULT;
			}
        }

        if(_focus != null && build.CustomComponents.Any(x => x.itemName == _focus.itemName && _focus.shopSection == ShopSection.Costume))
        {
            Customizable_SO cost = t.CustomComponents.First((x) => x.itemName == _focus.itemName);

            cost.highlight = HighLight.HIGHLIGHTED;
            cost.initialHighlight = HighLight.DEFAULT;
        }
        else 
        {
            Customizable_SO cost = t.CustomComponents.First((x) => x.selectionState == SelectionState.SELECTED && 
                                                                    x.shopSection == ShopSection.Costume);
            cost.highlight = HighLight.HIGHLIGHTED;
            cost.initialHighlight = HighLight.DEFAULT;
		}


        t.HairColor = build.HairColor;

        return t;
    }
	#endregion

	#region Delete saves zone
    public void DeleteSave( int deleteType ) // 0 - all, 1 - Level, 2 - money
    {
        if(File.Exists(Application.persistentDataPath + savePath))
        {
            if(deleteType == 0)
            {
                File.Delete(Application.persistentDataPath + savePath);
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("currentLevel", defaultLevel);
                PlayerPrefs.Save();
                return;
            }
            else if(deleteType == 1)
            {
                PlayerPrefs.SetInt("currentLevel", defaultLevel);
                PlayerPrefs.Save();
                //data.CurrentLevel = defaultLevel;
            }
            else if(deleteType == 2) data.Money = defaultMoney;

            Save();
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region Modifying the player build

    #endregion

    public int SubscribedTimes { get; set; }

    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"SSE: {SubscribedTimes}");

        ApplyChangesFromShop += UpdateItemDescriptions_SO;
    }
	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        ApplyChangesFromShop -= UpdateItemDescriptions_SO;
    }
}

[System.Serializable]
public class PlayerData
{
    public int Money;
    public int CurrentLevel;
    public bool Ads_Removed;
    public string HairColor;

    public List<Configuration.ShopItemInfo> ShopItemInfos = new List<Configuration.ShopItemInfo>();
}

[System.Serializable]
public class CharacterBuild
{
    public string HairColor;
    public List<Customizable_SO> CustomComponents = new List<Customizable_SO>();

    public static CharacterBuild CopyFrom(CharacterBuild itemToCopyFrom )
    {
        CharacterBuild clone = new CharacterBuild();

        clone.HairColor = itemToCopyFrom.HairColor;

        foreach(var item in itemToCopyFrom.CustomComponents)
        {
            clone.CustomComponents.Add(new Customizable_SO
            {
                itemName = item.itemName,
                itemTypeToString = item.itemTypeToString,
                skinTypeToString = item.skinTypeToString,
                shopIcon = Resources.Load<Sprite>("Icons/" + item.shopSection + "/" + item.name),
                shopSection = item.shopSection,
                orderInShop = item.orderInShop,
                statusHasChanged = item.statusHasChanged,
                lockInfo = new LockInfo
                {
                    lockStatus = item.lockInfo.lockStatus,
                    lockType = item.lockInfo.lockType,
                    priceToUnlock = item.lockInfo.priceToUnlock,
                    levelToUnlock = item.lockInfo.levelToUnlock
                }
            });
        }

        return clone;
    }
}

[System.Serializable]
public class AccessoryTypeName
{
    public CustomizableItems itemType;
    public string name;

    public static AccessoryTypeName CopyFrom(AccessoryTypeName itemToCopyFrom)
    {
        return new AccessoryTypeName
        {
            itemType = itemToCopyFrom.itemType,
            name = itemToCopyFrom.name,
        };
    }
}
