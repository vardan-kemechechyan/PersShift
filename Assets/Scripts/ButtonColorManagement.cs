using Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorManagement : MonoBehaviour
{
	public List<ButtonVisual> button;
	List<ButtonVisual> AdsContainingCell;

	[SerializeField] bool scriptDisabledState = false;
	[SerializeField] ShopFooterManager sh_Manager;
	[SerializeField] ShopItemsLoaderManager si_Manager;

	[SerializeField] ShopSection shopSection;

	[SerializeField] List<Image> hairColors;
	List<Customizable_SO> ItemsInThisSection;

	public Customizable_SO ActiveClickedCell;

	bool _openForMoney = false;
	bool _adsAvailable = false;

	int CurrentStartingIndex = 0;

	void Start()
	{
		if(si_Manager == null) si_Manager = transform.parent.GetComponent<ShopItemsLoaderManager>();
	}

	private void UpdateRewardedAdsCells()
	{
		if(sh_Manager != null )
		{
			foreach(var btn in button)
			{
				if(btn.gameObject.activeSelf)
					_adsAvailable = btn.GetUnlockForAdsState();

				if(_adsAvailable) break;
			}

			sh_Manager.ManagerFooterBar(_openForMoney, _adsAvailable);

			AdsContainingCell = new List<ButtonVisual>();

			foreach(var cell in button)
				if(cell.gameObject.activeSelf)
					if(cell.GetUnlockForAdsState()) 
						AdsContainingCell.Add(cell);
		}
	}

	public void LoadItems(List<Customizable_SO> _itemsToLoad, int startIngIndex = 0)
	{
		ItemsInThisSection = _itemsToLoad;

		CurrentStartingIndex = startIngIndex;

		/*for(int i = 0; i < _itemsToLoad.Count; i++)
		{
			_itemsToLoad[i].myAttachedButtonIndex = i % 6;
		}*/

		for(int i = 0; i < button.Count; i++)
		{
			int spriteIndex = CurrentStartingIndex * 6 + i;

			button[i].gameObject.SetActive(spriteIndex < _itemsToLoad.Count);

			if( spriteIndex < _itemsToLoad.Count )
			{
				button[i].LoadIcon(i, _itemsToLoad[spriteIndex]);
				button[i].UpdateCell();
				
				if(_itemsToLoad[spriteIndex].highlight == HighLight.HIGHLIGHTED && _itemsToLoad[spriteIndex].lockInfo.priceToUnlock <= 0)
				{
					UpdateActiveButton(_itemsToLoad[spriteIndex]);
				}
			}
		}

		UpdateRewardedAdsCells();
	}

	public void UpdateCurrentPage( int itemCount, int startIngIndex = 0)
	{
		CurrentStartingIndex = startIngIndex;

		for(int i = 0; i < button.Count; i++)
		{
			int spriteIndex = CurrentStartingIndex * 6 + i;

			button[i].gameObject.SetActive(spriteIndex < itemCount );

			if(spriteIndex < itemCount)
			{
				button[i].UpdateCell();
			}
		}

		UpdateRewardedAdsCells();
	}

	public void ButtonClicked( int _indexInArray )
	{
		Customizable_SO item = button[_indexInArray].GetItemAttahcedToButton();

		if(!scriptDisabledState)
		{
			if(button[_indexInArray].GetBlockStatus()) return;

			foreach(var so in ItemsInThisSection)
			{
				RefreshItemSelectionState(so, item);
			}

			for(int i = 0; i < button.Count; i++)
			{
				if(!button[i].gameObject.activeSelf) continue;

				button[i].ChangeColorStyle(_indexInArray, button[_indexInArray].GetItemAttahcedToButton());

				//item.myAttachedButtonIndex = _indexInArray;

				if(_indexInArray == i)
					_openForMoney = button[_indexInArray].GetUnlockForMoneyStatus();
			}

			if( item.highlight == HighLight.HIGHLIGHTED )
			{
				UpdateActiveButton(item);
			}

			ShopPreviewManager.ChangeTheModel();

			foreach(var btn in button)
			{
				if(!btn.gameObject.activeSelf) continue;

				_adsAvailable = btn.GetUnlockForAdsState();

				if(_adsAvailable) break;
			}

			if(sh_Manager != null)
				sh_Manager.ManagerFooterBar(_openForMoney, _adsAvailable);
		}
	}

	public void ButtonClickedAccessoryOnly(int _indexInArray, bool accessoryDontUpdat = false)
	{
		Customizable_SO item = button[_indexInArray].GetItemAttahcedToButton();

		if(!scriptDisabledState)
		{
			if(button[_indexInArray].GetBlockStatus()) return;

			foreach(var so in ItemsInThisSection)
			{
				RefreshItemSelectionState(so, item, accessoryDontUpdat);
			}

			for(int i = 0; i < button.Count; i++)
			{
				if(!button[i].gameObject.activeSelf) continue;

				button[i].ChangeColorStyle(_indexInArray, button[_indexInArray].GetItemAttahcedToButton(), accessoryDontUpdat);

				if(_indexInArray == i)
					_openForMoney = button[_indexInArray].GetUnlockForMoneyStatus();
			}

			if(item.highlight == HighLight.HIGHLIGHTED)
			{
				UpdateActiveButton(item);
			}

			ShopPreviewManager.ChangeTheModel();

			foreach(var btn in button)
			{
				if(!btn.gameObject.activeSelf) continue;

				_adsAvailable = btn.GetUnlockForAdsState();

				if(_adsAvailable) break;
			}

			if(sh_Manager != null)
				sh_Manager.ManagerFooterBar(_openForMoney, _adsAvailable);
		}
	}
	public void NonCellButtonClicked(int _indexInArray)
	{
		for(int i = 0; i < button.Count; i++)
		{
			button[i].NonCellButtonClicked(_indexInArray == i );
		}
	}

	public void HairButtonAutoClick( string color )
	{
		for(int i = 0; i < hairColors.Count; i++)
		{
			if(ColorUtility.ToHtmlStringRGB(hairColors[i].color) == color)
				NonCellButtonClicked(i);
		}
	}

	public void UpdateActiveButton(Customizable_SO _activeCell, bool _deselectedInShop = false)
	{
		ActiveClickedCell = _activeCell;
	}

	public void NullifyActiveCellsAndLegitCellsList() 
	{
		ActiveClickedCell = null;
	}

	public void ClickRandomAdsCell()
	{
		AdsContainingCell = new List<ButtonVisual>();

		foreach(var cell in button)
			if(cell.gameObject.activeSelf)
				if(cell.GetUnlockForAdsState()) 
					AdsContainingCell.Add(cell);

		sh_Manager.HideFooter();

		var lvl = new Dictionary<string, object>();
		lvl.Add("tab_name", shopSection.ToString());

		AnalyticEvents.ReportEvent("shop_skin_reward", lvl);

		StopCoroutine(Roulette());
		StartCoroutine(Roulette());
	}

	public void RefreshItemSelectionState(Customizable_SO cd, Customizable_SO _clickedCell, bool accessoryDontUpdat = false)
	{
		if(cd.shopSection == ShopSection.Accessory && accessoryDontUpdat)
		{
			if(_clickedCell.orderInShop == cd.orderInShop)
			{
				cd.selectionState = SelectionState.SELECTED;
				cd.highlight = HighLight.HIGHLIGHTED;
			}

			return;
		}

		if(cd.orderInShop == _clickedCell.orderInShop)
		{
			if(cd.lockInfo.lockStatus == LockStatus.LOCKED)
			{
				if(cd.shopSection == ShopSection.Accessory)
				{
					cd.highlight = cd.highlight == HighLight.HIGHLIGHTED ? HighLight.NONE : HighLight.HIGHLIGHTED;
				}
				else
				{
					cd.highlight = HighLight.HIGHLIGHTED;
				}
			}
			else
			{
				if(cd.shopSection == ShopSection.Accessory)
				{
					cd.selectionState = cd.selectionState == SelectionState.NOT_SELECTED ? SelectionState.SELECTED : SelectionState.NOT_SELECTED;
					cd.highlight = cd.highlight == HighLight.HIGHLIGHTED ? cd.initialHighlight : HighLight.HIGHLIGHTED;
				}
				else
				{
					cd.selectionState = SelectionState.SELECTED;
					cd.highlight = HighLight.HIGHLIGHTED;

					//if(cd.shopSection != ShopSection.Costume)
					//{
						cd.initialHighlight = HighLight.DEFAULT;
					//}
				}
			}
		}
		else
		{
			if(cd.itemTypeToString == _clickedCell.itemTypeToString)
			{
				cd.highlight = cd.initialHighlight;

				if(_clickedCell.lockInfo.lockStatus == LockStatus.UNLOCKED)
				{
					cd.selectionState = SelectionState.NOT_SELECTED;
					
					//Added Later
					cd.highlight = HighLight.NONE;
					cd.initialHighlight = HighLight.NONE;
				}
			}
			else if(cd.lockInfo.lockStatus == LockStatus.LOCKED && cd.shopSection != ShopSection.Accessory)
			{
				cd.highlight = HighLight.NONE;
			}
			else
			{
				if(_clickedCell.lockInfo.lockStatus == LockStatus.LOCKED)
				{
					if(cd.shopSection != ShopSection.Accessory)
					{
						cd.highlight = cd.initialHighlight;
					}
					else
					{
						if(cd.highlight == HighLight.HIGHLIGHTED) 
						{
							cd.highlight = HighLight.HIGHLIGHTED;
						}
						else
						{
							cd.highlight = cd.initialHighlight;
						}
					}
				}
				else
				{
					if(cd.shopSection == ShopSection.Costume)
					{
						cd.highlight = cd.initialHighlight;
					}
					else if(cd.shopSection == ShopSection.Accessory)
					{

					}
					else
					{
						cd.highlight = cd.initialHighlight;
						cd.selectionState = SelectionState.NOT_SELECTED;
					}
				}
			}
		}
	}

	[SerializeField] float rouletteTime = 1.5f;
	[SerializeField] float jumpInterval = 0.1f;

	IEnumerator Roulette()
	{
		rouletteTime = 2f;

		float timePassed = 0f;
		jumpInterval = 0.1f;

		bool skipRoullete = false;

		ButtonVisual currentPickedCell = AdsContainingCell[Random.Range(0, AdsContainingCell.Count)];
		Customizable_SO randomItem = currentPickedCell.GetItemAttahcedToButton();

		if(AdsContainingCell.Count <= 1) skipRoullete = true;

		while( timePassed < rouletteTime && !skipRoullete )
		{
			List<ButtonVisual> RandomCellToPickFrom = new List<ButtonVisual>();

			foreach(var cell in AdsContainingCell)
				if(cell.GetInstanceID() != currentPickedCell.GetInstanceID() ) RandomCellToPickFrom.Add(cell);

			currentPickedCell = RandomCellToPickFrom[Random.Range(0, RandomCellToPickFrom.Count)];

			currentPickedCell.AnimateAdsCell( jumpInterval );

			randomItem = currentPickedCell.GetItemAttahcedToButton();

			yield return new WaitForSeconds(jumpInterval);

			timePassed += jumpInterval;

			jumpInterval += 0.1f;
		}

		currentPickedCell.AnimateAdsCell(jumpInterval);

		yield return new WaitForSeconds(jumpInterval);

		ShopManager.RandomRouletteItem(randomItem);

		foreach(var btn in button)
		{
			if(btn.gameObject.activeSelf)
				_adsAvailable = btn.GetUnlockForAdsState();

			if(_adsAvailable) break;
		}

		sh_Manager.ManagerFooterBar(_openForMoney, _adsAvailable);

		foreach(var cell in AdsContainingCell)
		{
			cell.transform.localScale = Vector3.one;
		}
	}
}
