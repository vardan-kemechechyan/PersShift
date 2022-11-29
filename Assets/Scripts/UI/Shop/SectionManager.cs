using Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
	[SerializeField] PaginationManager paginationManager;
	[SerializeField] ShopPreviewManager spm;
	[SerializeField] SaveSystemExperimental SSE;
	[SerializeField] ShopManager shopManager;
	GameManager gameManager;
	public List<SectionReferences> Sections;
	int CurrentOpenSectionIndex = 0;
	int CurrentlyOpenSubPage = 0;

	public void InitialStart()
	{
		if(gameManager == null) gameManager = GameManager.GetInstance();

		Customizable_SO itemToFocus = gameManager.GetItemToFocusOn();

		if(itemToFocus == null)
		{
			EnableShopSection(-1);
			spm.Transition(0);
		}
		else
		{
			ShopSection ss = itemToFocus.shopSection;
			int orderinShop = itemToFocus.orderInShop;
			int buttonIndex = itemToFocus.myAttachedButtonIndex;

			if(SSE == null)
				SSE = SaveSystemExperimental.GetInstance();

			CurrentlyOpenSubPage = Mathf.FloorToInt(orderinShop / 6f);

			spm.Transition(itemToFocus.shopSection == ShopSection.Costume ? 1 : 0);

			for(int i = 0; i < Sections.Count; i++)
			{
				if(Sections[i].shopSection == ss)
				{
					CurrentOpenSectionIndex = i;
					Sections[i].ChangeOpenSubPage(CurrentlyOpenSubPage);
					break;
				}
			}

			EnableShopSection(CurrentOpenSectionIndex);

			if(itemToFocus.lockInfo.lockStatus == LockStatus.UNLOCKED)
				shopManager.BuyAccessory(itemToFocus);
			else
			{
				AutoClick(itemToFocus.myAttachedButtonIndex, true);

				SSE.SyncAndSaveChanges();
				
				spm.previewBuild = SSE.ReturnCopyOfBuild(itemToFocus);

				//if(itemToFocus.shopSection == ShopSection.Costume)
				//{
					List<Customizable_SO> allNonFocused = spm.previewBuild.CustomComponents.FindAll(x => x.shopSection == itemToFocus.shopSection && x.itemName != itemToFocus.itemName);

					foreach(var cost in allNonFocused)
					{
						cost.selectionState = SelectionState.SELECTED;
						cost.highlight = HighLight.DEFAULT;
					}

					itemToFocus.highlight = HighLight.HIGHLIGHTED;
					itemToFocus.selectionState = SelectionState.SELECTED;
				//}

				UpdateActivePage();
			}
		}
	}

	public void EnableShopSection( int _indexInArray )
	{
		bool freshStart = _indexInArray == -1;

		if(_indexInArray == -1) _indexInArray = 0;

		if(SSE == null)
			SSE = SaveSystemExperimental.GetInstance();

		CurrentOpenSectionIndex = _indexInArray;

		CurrentlyOpenSubPage = Sections[CurrentOpenSectionIndex].CurrentlyOpenSubPage;

		for(int i = 0; i < Sections.Count; i++)
		{
			Sections[i].headerButton.SelectedScreen(CurrentOpenSectionIndex == i );
			Sections[i].sectionContainer.gameObject.SetActive(CurrentOpenSectionIndex == i );
			
			//if(freshStart) Sections[i].sectionContainer.DropItemInShop();
		}

		Sections[CurrentOpenSectionIndex].sectionContainer.LoadIcons(CurrentlyOpenSubPage);

		if(CurrentOpenSectionIndex == 2)
			Sections[2].sectionContainer.PrepareHair();

		paginationManager.SetupPageButtons(Sections[CurrentOpenSectionIndex].sectionContainer.ReturnItemCount(), CurrentOpenSectionIndex);
	}

	public void LoadSubPage( int _subPageIndex = 0 )
	{
		Sections[CurrentOpenSectionIndex].ChangeOpenSubPage(_subPageIndex);

		EnableShopSection(CurrentOpenSectionIndex);
	}

	public int GetActivePage()
	{
		return Sections[CurrentOpenSectionIndex].CurrentlyOpenSubPage;
	}

	public void AutoClick(int buttonIndex, bool accessoryDontUpdate)
	{
		Sections[CurrentOpenSectionIndex].sectionContainer.AutoClickMyButton(buttonIndex, accessoryDontUpdate);
	}

	public void UpdateActivePage()
	{
		Sections[CurrentOpenSectionIndex].sectionContainer.UpdateSectionWithTheSubsectionPage(CurrentlyOpenSubPage);
	}

	[System.Serializable]
	public class SectionReferences
	{
		public HeaderButtonScript headerButton;
		public ShopItemsLoaderManager sectionContainer;
		public ShopSection shopSection;
		public int CurrentlyOpenSubPage;

		public void ChangeOpenSubPage(int _value) { CurrentlyOpenSubPage = _value; }
	}
}