using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SaveSystemManager : Singleton<SaveSystemManager>
{
	public SaveSystemExperimental SSP;

	[SerializeField] string SAVE_LEVEL_PROGRESS;
	[SerializeField] int defaultLevel;

	[Space(10)]

	[SerializeField] string SAVE_MONEY;
	[SerializeField] int defaultMoney;

	[Space(10)]
	[SerializeField] string watchedAdPrefix;

	#region Accessory management
	[Space(10)]
	[Tooltip("Default Unlocked Accessories")] [SerializeField] List<AccessoriesToSave> defaul_unlocked_accessories;
	[Tooltip("Change the purchase status of the given accessory")] [SerializeField] AccessoriesToSave Accessory_to_save_or_select;

	Dictionary<CustomizableItems, List<AccessoriesToSave>> AccessoriesPurchasedHistory;
	#endregion

	#region Costume management
	[Space(10)]
	[Tooltip("Default Unlocked Costumes")] [SerializeField] List<CostumeToSave> defaul_unlocked_costumes;
	[Tooltip("Change the purchase status of the given costume")] [SerializeField] CostumeToSave Costume_to_save_or_unlock;

	Dictionary<Appearance, List<CostumeToSave>> CostumePurchasedHistory;
	#endregion

	[Space(10)]
	[SerializeField] List<AccessoriesTypeGameObject> AllCustomizableAccessories;
	[SerializeField] List<GameObject> AllCostumes;

	string _firstLaunch = "FIRST LAUNCH";

	override protected void Awake()
	{
		base.Awake();

		//InitialAccessoryLoading();
	}

	public void SaveCurrentLevel(int _levelIndex)
	{
		PlayerPrefs.SetInt(SAVE_LEVEL_PROGRESS, _levelIndex);
		PlayerPrefs.Save();
	}
	public int GetCurrentLevel()
	{
		return PlayerPrefs.HasKey(SAVE_LEVEL_PROGRESS) ? PlayerPrefs.GetInt(SAVE_LEVEL_PROGRESS) : defaultLevel;
	}

	public void SaveMoney(int _moneyAmount)
	{
		PlayerPrefs.SetInt(SAVE_MONEY, _moneyAmount);
		PlayerPrefs.Save();
	}
	public int GetMoney()
	{
		return PlayerPrefs.HasKey(SAVE_MONEY) ? PlayerPrefs.GetInt(SAVE_MONEY) : defaultMoney;
	}

	public void UnlockSelect_and_SaveAccessory(AccessoriesToSave _accessory )
	{
		string accessoryName;

		/*if( !AccessoriesPurchasedHistory.Keys.Contains( _accessory.itemType ))
			AccessoriesPurchasedHistory.Add(_accessory.itemType, new List<AccessoriesToSave>());

		if(!AccessoriesPurchasedHistory[_accessory.itemType].Exists((acc) => acc.skinType == _accessory.skinType))
		{
			AccessoriesPurchasedHistory[_accessory.itemType].Add(_accessory);
			
			accessoryName = _accessory.itemType.ToString() + "_" + _accessory.skinType;

			PlayerPrefs.SetInt(accessoryName, (int)_accessory.purchaseStatus);
		}
		else
			for(int i = 0; i < AccessoriesPurchasedHistory[_accessory.itemType].Count; i++)
			{
				if(AccessoriesPurchasedHistory[_accessory.itemType][i].skinType == _accessory.skinType)
				{
					AccessoriesPurchasedHistory[_accessory.itemType][i] = _accessory;

					accessoryName = _accessory.itemType.ToString() + "_" + _accessory.skinType;

					PlayerPrefs.SetInt(accessoryName, (int)_accessory.purchaseStatus);
				}
				else
				{
					AccessoriesPurchasedHistory[_accessory.itemType][i] = new AccessoriesToSave() {
						itemType = AccessoriesPurchasedHistory[_accessory.itemType][i].itemType,
						skinType = AccessoriesPurchasedHistory[_accessory.itemType][i].skinType,
						purchaseStatus = (_accessory.purchaseStatus == LockStatus.SELECTED ? LockStatus.UNLOCKED : _accessory.purchaseStatus)
					};

					accessoryName = AccessoriesPurchasedHistory[_accessory.itemType][i].itemType.ToString() + "_" + AccessoriesPurchasedHistory[_accessory.itemType][i].skinType;

					PlayerPrefs.SetInt(accessoryName, (int)AccessoriesPurchasedHistory[_accessory.itemType][i].purchaseStatus);
				}
			}*/
	}
	
	
	public void UnlockSelect_and_SaveCostume( CostumeToSave _costume )
	{
		string costumeName;

		//if(CostumePurchasedHistory[_costume.costumeType] == null || CostumePurchasedHistory[_costume.costumeType].Count == 0)
		if(!CostumePurchasedHistory.Keys.Contains(_costume.costumeType))
			CostumePurchasedHistory.Add(_costume.costumeType, new List<CostumeToSave>());

		if(!CostumePurchasedHistory[_costume.costumeType].Exists((acc) => acc.skinType == _costume.skinType))
		{
			CostumePurchasedHistory[_costume.costumeType].Add(_costume);

			costumeName = _costume.costumeType.ToString() + "_" + _costume.skinType;

			PlayerPrefs.SetInt(costumeName, (int)_costume.purchaseStatus);
		}
		else
			for(int i = 0; i < CostumePurchasedHistory[_costume.costumeType].Count; i++)
			{
				if(CostumePurchasedHistory[_costume.costumeType][i].skinType == _costume.skinType)
				{
					CostumePurchasedHistory[_costume.costumeType][i] = _costume;

					costumeName = _costume.costumeType.ToString() + "_" + _costume.skinType;

					PlayerPrefs.SetInt(costumeName, (int)_costume.purchaseStatus);
				}
				/*else
				{
					CostumePurchasedHistory[_costume.costumeType][i] = new CostumeToSave()
					{
						costumeType = CostumePurchasedHistory[_costume.costumeType][i].costumeType,
						skinType = CostumePurchasedHistory[_costume.costumeType][i].skinType,
						purchaseStatus = (_costume.purchaseStatus == LockStatus.SELECTED ? LockStatus.UNLOCKED : _costume.purchaseStatus)
					};

					costumeName = CostumePurchasedHistory[_costume.costumeType][i].costumeType.ToString() + "_" + CostumePurchasedHistory[_costume.costumeType][i].skinType;

					PlayerPrefs.SetInt(costumeName, (int)CostumePurchasedHistory[_costume.costumeType][i].purchaseStatus);
				}*/
			}
	}

	public void WatchedAccessoryAd( AccessoriesToSave _acc )
	{
		
	}
	
	public int GetWatchedAccessoryAdCount()
	{
		return 0;
	}

	public AccessoriesToSave PopulateAccessoriesDictionary(string _accessoryFullName)
	{
		CustomizableItems[] accArray = Enum.GetValues(typeof(CustomizableItems)).OfType<CustomizableItems>().ToArray();
		
		CustomizableItems itemType = accArray.First((x) => x.ToString() == _accessoryFullName.Split('_')[0]);
		string skinType = _accessoryFullName.Split('_')[1];

		AccessoriesToSave accessoryToCreate = new AccessoriesToSave()
		{
			itemType = itemType,
			skinType = skinType,
			purchaseStatus = (LockStatus) PlayerPrefs.GetInt(_accessoryFullName)
		};

		return accessoryToCreate;
	}
	public CostumeToSave PopulateCostumeDictionary(string _costumeFullName, bool _getFromSaves = true)
	{
		Appearance[] accArray = Enum.GetValues(typeof(Appearance)).OfType<Appearance>().ToArray();

		Appearance itemType = accArray.First((x) => x.ToString() == _costumeFullName.Split('_')[0]);
		string skinType = _costumeFullName.Split('_')[1];

		CostumeToSave costumeToCreate = new CostumeToSave()
		{
			costumeType = itemType,
			skinType = skinType,
			purchaseStatus = _getFromSaves ? (LockStatus)PlayerPrefs.GetInt(_costumeFullName) : LockStatus.LOCKED
		};

		return costumeToCreate;
	}
/*
	public Dictionary<Appearance, List<string>> ReturnNamesOfSelectedCostumes( List<Appearance> _locationNamesForTheLevel, Control _playerType )
	{
		Dictionary<Appearance, List<string>> CostumeNames = new Dictionary < Appearance, List<string>>();

		foreach(var location in _locationNamesForTheLevel)
		{
			if(_playerType == Control.Player)
			{
				CostumeToSave foundCostume = CostumePurchasedHistory[location].First((cost) => cost.costumeType == location && cost.purchaseStatus == LockStatus.SELECTED);

				CostumeNames.Add(location, new List<string>() { foundCostume.costumeType + "_" + foundCostume.skinType });
			}
			else
			{
				CostumeNames.Add(location, new List<string>());

				foreach(var costume in AllCostumes)
					if(location.ToString() == costume.name.Split('_')[0])
						CostumeNames[location].Add(costume.name);
			}
		}

		return CostumeNames;
	}
	*/
	public GameObject ReturnCostumePrefab( string _costumeName)
	{
		return AllCostumes.First((go) => go.name == _costumeName);
	}
/*
	public List<(CustomizableItems _itemType, string skinType)> ReturnListOfAccessories( Control _playerType )
	{
		List<(CustomizableItems _itemType, string skinType)> accessoriesToWear = new List<(CustomizableItems _itemType, string skinType)>();

		if(_playerType == Control.Player)
		{
			foreach(var accType in AccessoriesPurchasedHistory)
				foreach(var acc in accType.Value)
					if(acc.purchaseStatus == LockStatus.SELECTED)
					{
						accessoriesToWear.Add((acc.itemType, acc.skinType.ToString()));
						break;
					}
		}
		else
		{
			foreach(var accType in AllCustomizableAccessories)
			{
				if(_playerType == Control.AI)
					if(accType.itemType == CustomizableItems.Hairstyle)
					{
						if(UnityEngine.Random.value <= .04) continue;
					}
					else
					{
						if(UnityEngine.Random.value <= .5) continue;
					}

				string n = accType.item[UnityEngine.Random.Range(0, accType.item.Count)].name.Split('_')[1];

				accessoriesToWear.Add((accType.itemType, n));
			}
		}

		return accessoriesToWear;
	}
	*/
	public void InitialAccessoryLoading()
	{
		AccessoriesPurchasedHistory = new Dictionary<CustomizableItems, List<AccessoriesToSave>>();
		CostumePurchasedHistory		= new Dictionary<Appearance, List<CostumeToSave>>();

		if(!PlayerPrefs.HasKey(_firstLaunch))
		{
			PlayerPrefs.SetInt(_firstLaunch, 1);

			foreach(var accFromDefault in defaul_unlocked_accessories)
				UnlockSelect_and_SaveAccessory(accFromDefault);

			foreach(var costFromDefault in defaul_unlocked_costumes)
				UnlockSelect_and_SaveCostume(costFromDefault);
		}
		else
		{
			AccessoriesToSave _newAcc;

			foreach(var accFromDic in AllCustomizableAccessories)
			{
				foreach( var go in accFromDic.item )
				{
					string accessoryName = go.name;

					if(PlayerPrefs.HasKey(accessoryName))
					{
						_newAcc = PopulateAccessoriesDictionary(accessoryName);

						//if(AccessoriesPurchasedHistory[_newAcc.itemType] == null || AccessoriesPurchasedHistory[_newAcc.itemType].Count == 0 )
						if(!AccessoriesPurchasedHistory.Keys.Contains(_newAcc.itemType))
							AccessoriesPurchasedHistory.Add(_newAcc.itemType, new List<AccessoriesToSave>());

						//if(!AccessoriesPurchasedHistory[_newAcc.itemType].Exists((acc) => acc.skinType == _newAcc.skinType))
							AccessoriesPurchasedHistory[_newAcc.itemType].Add(_newAcc);
						/*else
							for(int i = 0; i < AccessoriesPurchasedHistory[_newAcc.itemType].Count; i++)
							{
								if(AccessoriesPurchasedHistory[_newAcc.itemType][i].skinType == _newAcc.skinType)
								{
									AccessoriesPurchasedHistory[_newAcc.itemType][i] = _newAcc;
								}
							}*/
					}
				}
			}
			
			CostumeToSave _newCostume;

			foreach(var cost in AllCostumes)
			{
				string costumeName = cost.name;

				if(PlayerPrefs.HasKey(costumeName))
				{
					_newCostume = PopulateCostumeDictionary(costumeName);

					//if(CostumePurchasedHistory[_newCostume.costumeType] == null || CostumePurchasedHistory[_newCostume.costumeType].Count == 0 )
					if(!CostumePurchasedHistory.Keys.Contains(_newCostume.costumeType))
						CostumePurchasedHistory.Add(_newCostume.costumeType, new List<CostumeToSave>());

					//if(!CostumePurchasedHistory[_newCostume.costumeType].Exists((cos) => cos.skinType == _newCostume.skinType))
						CostumePurchasedHistory[_newCostume.costumeType].Add(_newCostume);
					/*else
						for(int i = 0; i < CostumePurchasedHistory[_newCostume.costumeType].Count; i++)
						{
							if(CostumePurchasedHistory[_newCostume.costumeType][i].skinType == _newCostume.skinType)
							{
								CostumePurchasedHistory[_newCostume.costumeType][i] = _newCostume;
							}
						}*/
				}
			}
		}

		PlayerPrefs.Save();
	}

	#region Delete Region
	public void DeleteAllSaves() { PlayerPrefs.DeleteAll(); /*SSP.DeleteSave();*/ }
	public void DeleteSave( string _keyValue ) { PlayerPrefs.DeleteKey(_keyValue); }
	#endregion

	#region Save From Editor Buttons

	public void SaveCurrentLevel() { SaveCurrentLevel(defaultLevel); }
	public void SaveMoney() { SaveMoney(defaultMoney); }

	/*public void UnlockSelect_and_SaveAccessory() 
	{ 
		if(PlayerPrefs.HasKey(_firstLaunch) && Accessory_to_save_or_select.itemType != CustomizableItems.NONE)
			UnlockSelect_and_SaveAccessory(Accessory_to_save_or_select);
	}*/
	public void UnlockSelect_and_SaveCostume() 
	{ 
		if(PlayerPrefs.HasKey(_firstLaunch) && Costume_to_save_or_unlock.costumeType != Appearance.None )
			UnlockSelect_and_SaveCostume(Costume_to_save_or_unlock);
	}

	#endregion
}

[System.Serializable]
public struct AccessoriesToSave
{
	public CustomizableItems itemType;
	public LockStatus purchaseStatus;
	public string skinType;
	public int unlockedAfterAmountOfAdsWatched;
}

/*[System.Serializable] 
public struct CostumeToSave
{
	public Appearance costumeType;
	public string skinType;
	public CusomizableItemPurchaseStatus purchaseStatus;
}*/

[Serializable]
public struct AccessoriesTypeGameObject
{
	public CustomizableItems itemType;
	public List<GameObject> item;
}

[Serializable]
public struct CostumeToSave
{
	public Appearance costumeType;
	public string skinType;
	public LockStatus purchaseStatus;
}