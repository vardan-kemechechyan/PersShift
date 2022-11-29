using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaginationManager : MonoBehaviour
{
	[SerializeField] List<PageScript> pageButtons;
	[SerializeField] List<GameObject> tempPageButtons;
	[SerializeField] Transform tempPageButtonHolder;
	[SerializeField] SectionManager sectionManager;
	[SerializeField] ButtonColorManagement bc_Manager;

	int currentSubPage = 0;
	int numberOfPages = 0;

	public void SetupPageButtons( int itemCount , int sectionIndex)
	{
		if(sectionIndex == 2)	tempPageButtonHolder.localPosition = new Vector3(0f, 550f, 0f);
		else					tempPageButtonHolder.localPosition = new Vector3(0f, 682f, 0f);

		numberOfPages = Mathf.CeilToInt(itemCount / 6f);

		for(int i = 0; i < pageButtons.Count; i++)
		{
			pageButtons[i].EnableAndSet( i < numberOfPages);
		}

		currentSubPage = sectionManager.GetActivePage(); 

		ManageTempPageImages();

		//bc_Manager.ButtonClicked(sectionManager.GetActivePage());
		bc_Manager.NonCellButtonClicked(sectionManager.GetActivePage());
	}

	public void SelectPage( int _pageIndex )
	{
		currentSubPage = _pageIndex;
		sectionManager.LoadSubPage(currentSubPage);

		ManageTempPageImages();
	}

	public void SelectPageTemp( int _pageIndex ) //-1 left, 1 right
	{
		currentSubPage += _pageIndex;
		bc_Manager.NonCellButtonClicked(currentSubPage);
		sectionManager.LoadSubPage(currentSubPage);

		ManageTempPageImages();
	}

	void ManageTempPageImages()
	{
		if(numberOfPages == 1) tempPageButtonHolder.gameObject.SetActive(false);
		else
		{
			tempPageButtonHolder.gameObject.SetActive(true);

			if((currentSubPage + 1) < numberOfPages)	tempPageButtons[1].SetActive(true);
			else										tempPageButtons[1].SetActive(false);

			if(currentSubPage > 0)						tempPageButtons[0].SetActive(true);
			else										tempPageButtons[0].SetActive(false);
		}
	}
}
