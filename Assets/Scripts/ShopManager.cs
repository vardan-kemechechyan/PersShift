using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour, ISubscribeUnsubscribe
{
	public static Action<Customizable_SO, bool> PlayerBoughtItem;
	public static Action<Customizable_SO> RandomRouletteItem;

	[SerializeField] SectionManager sm;
	[SerializeField] SaveSystemExperimental sse;
	[SerializeField] SoundManager soundManager;
	
	MoneyManagementSystem moneySystem;
	GameManager gameManager;

	[SerializeField] ShopPreviewManager spm;

	List<string> shopItemsToApply;
	List<string> shopCostumesToApply;

	private void Start()
	{
		if(gameManager == null)
		{
			gameManager = GameManager.GetInstance();
			moneySystem = gameManager.GetComponent<MoneyManagementSystem>();
			sse = SaveSystemExperimental.GetInstance();
		}

		Subscribe();
	}

	private void OnDestroy()
	{
		UnSubscribe();
	}

	public void BuyAccessory(Customizable_SO cellDescription, bool boughtByPressingBuyButton = false)
	{
		UpdateReferences();

		if(moneySystem.Money >= cellDescription.lockInfo.priceToUnlock)
		{
			if(boughtByPressingBuyButton)
			{
				var lvl = new Dictionary<string, object>();
				lvl.Add("prefabname", cellDescription.itemName);

				AnalyticEvents.ReportEvent("shop_skin_purchased", lvl);

				soundManager.ItemAquiredInShop();
			}

			moneySystem.Money -= cellDescription.lockInfo.priceToUnlock;

			UnlockItem(cellDescription, LockStatus.UNLOCKED, SelectionState.NOT_SELECTED, HighLight.NONE, LockType.NONE, 0);

			sse.SaveMoney((int)moneySystem.Money);

			sm.AutoClick(cellDescription.myAttachedButtonIndex, true);

			SaveAndApplyChanges();

			spm.previewBuild = sse.ReturnCopyOfBuild(cellDescription);

			sm.UpdateActivePage();
		}
	}

	public void UnlockItem( Customizable_SO cellDescription, LockStatus _lockStatus, SelectionState _selectState, HighLight _highlight, LockType lockType, int _itemPrice )
	{
		cellDescription.highlight = _highlight;
		cellDescription.initialHighlight = _highlight;
		cellDescription.selectionState = _selectState;
		cellDescription.lockInfo.lockStatus = _lockStatus;
		cellDescription.lockInfo.lockType = lockType;
		cellDescription.lockInfo.priceToUnlock = _itemPrice;
	}

	public void RandomItemRoullete( Customizable_SO cellDescription )
	{
		cellDescription.lockInfo.priceToUnlock--;

		if( cellDescription.lockInfo.priceToUnlock <= 0 )
		{
			var lvl = new Dictionary<string, object>();
			lvl.Add("prefabname", cellDescription.itemName);

			AnalyticEvents.ReportEvent("shop_skin_unlocked", lvl);

			soundManager.ItemAquiredInShop();

			UnlockItem(cellDescription, LockStatus.UNLOCKED, SelectionState.NOT_SELECTED, HighLight.NONE, LockType.NONE, 0);

			sm.AutoClick(cellDescription.myAttachedButtonIndex, true);

			SaveAndApplyChanges();

			spm.previewBuild = sse.ReturnCopyOfBuild(cellDescription);

			sm.UpdateActivePage();
		}
		else 
		{
			soundManager.PressUIButton();
			sse.UpdateItemDescriptions_SO();
			sse.UpdateSaveFile();
			sse.Save();
			sm.UpdateActivePage();
		}
	}

	public void SaveChanges()
	{
		sse.SyncAndSaveChanges();
	}

	public void SaveAndApplyChanges()
	{
		SaveChanges();

		if(gameManager.Player != null )
			gameManager.UpdatePlayerAfterShop();
	}

	void UpdateReferences()
	{
		if(gameManager == null)
		{
			gameManager = GameManager.GetInstance();
			moneySystem = gameManager.GetComponent<MoneyManagementSystem>();
			sse = SaveSystemExperimental.GetInstance();
		}
	}

	public int SubscribedTimes { get; set; }

	public void Subscribe()
	{
		++SubscribedTimes;

		//print($"ShopManager: {SubscribedTimes}");

		PlayerBoughtItem += BuyAccessory;
		RandomRouletteItem += RandomItemRoullete;
	}

	public void UnSubscribe()
	{
		SubscribedTimes--;

		SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

		PlayerBoughtItem -= BuyAccessory;
		RandomRouletteItem -= RandomItemRoullete;
	}
}