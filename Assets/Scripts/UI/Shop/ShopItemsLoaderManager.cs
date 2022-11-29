using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemsLoaderManager : MonoBehaviour
{
	SaveSystemExperimental SSP;

	[SerializeField] ShopSection shopSection; 

	public ButtonColorManagement bc_Manager;
	[SerializeField] ButtonColorManagement hairColor_Manager;

	//[SerializeField] List<ItemInShop> ItemDescriptions;

	List<Customizable_SO> itemsArrivedFromSaveFile = new List<Customizable_SO>();

	public void LoadIcons( int _subPageIndex)
	{
		if(itemsArrivedFromSaveFile == null || itemsArrivedFromSaveFile.Count == 0) PopulateItems();

		bc_Manager.LoadItems(itemsArrivedFromSaveFile, _subPageIndex);
	}

	public void UpdateSectionWithTheSubsectionPage( int _subPageIndex)
	{
		if(itemsArrivedFromSaveFile == null || itemsArrivedFromSaveFile.Count == 0) PopulateItems();

		bc_Manager.UpdateCurrentPage(itemsArrivedFromSaveFile.Count, _subPageIndex);
	}

	public void AutoClickMyButton( int buttonIndex, bool accessoryDontUpdat = false)
	{
		bc_Manager.ButtonClickedAccessoryOnly(buttonIndex, accessoryDontUpdat);
	}

	void PopulateItems()
	{
		//ItemDescriptions = new List<ItemInShop>();

		if(SSP == null) SSP = SaveSystemExperimental.GetInstance();

		itemsArrivedFromSaveFile = SSP.ReturnItemsInSection(shopSection);
		/*
		foreach(var item in itemsArrivedFromSaveFile)
		{
			
			ItemDescriptions.Add(new ItemInShop
			{
				blockedStatus = item.lockInfo.lockStatus == LockStatus.LOCKED,
				unlockForAds = item.lockInfo.lockType == LockType.WATCH_AD,
				unlockForLevel = item.lockInfo.lockType == LockType.LEVEL_UNLOCK,
				unlockForMoney = item.lockInfo.lockType == LockType.BUY_FOR_MONEY || (item.lockInfo.lockType == LockType.LEVEL_UNLOCK && item.lockInfo.priceToUnlock > 0),
				highlighed = item.lockInfo.lockStatus == LockStatus.SELECTED_AS_WELL,
				currentlySelected = item.lockInfo.lockStatus == LockStatus.SELECTED,
				itemName = item.itemName,
				shopSection = item.shopSection,
				priceToUnlock = item.lockInfo.priceToUnlock,
				levelToUnlock = item.lockInfo.levelToUnlock,
				IconToLoad = item.shopIcon,
				orderInShop = item.orderInShop
			});
			
		}*/
	}

	public void PrepareHair()
	{
		string hairColor = SSP.GetHairColor( true );

		hairColor_Manager.HairButtonAutoClick(hairColor);
	}

	//public void DropItemInShop() { ItemDescriptions.Clear(); }

	public int ReturnItemCount() { return itemsArrivedFromSaveFile.Count; }

	[System.Serializable]
	public class ItemInShop
	{
		public string itemName;
		public ShopSection shopSection;
		public Sprite IconToLoad;
		public bool blockedStatus;
		public bool unlockForAds;
		public bool unlockForLevel;
		public bool unlockForMoney;
		public bool highlighed;
		public bool currentlySelected;
		public int priceToUnlock;
		public int levelToUnlock;
		public int orderInShop;
	}
}
