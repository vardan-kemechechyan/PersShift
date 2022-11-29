using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopFooterManager : MonoBehaviour
{
	[SerializeField] ButtonColorManagement CellManager;

	[SerializeField] GameObject PayBtnObject;
	[SerializeField] GameObject AdsBtnObject;

	public void ManagerFooterBar( bool _openForMoney, bool _adsAvailable )
	{
		AdsBtnObject.SetActive(false);

		CancelInvoke("UpdateRewardAvailability");

		PayBtnObject.SetActive( _openForMoney );

		if(_adsAvailable)
		{
			InvokeRepeating("UpdateRewardAvailability", 0, 1);
		}
	}

	private void OnDisable()
	{
        //IronSourceManager.OnRewardedFailed -= OnRewardedFailed;
		AdMob.OnRewardedFailed -= OnRewardedFailed;
	}

	private void UpdateRewardAvailability()
	{
		bool isAdReady = false;

        isAdReady = AdMob.Instance.IsReady("Reward_Store");
        //isAdReady = IronSourceManager.Instance.IsRewardedReady("Reward_Store");

		AdsBtnObject.SetActive(isAdReady);
	}

	public void WatchRandomAds()
	{
		AdMob.OnRewarded -= OnRewardedRandomSkin;
		AdMob.OnRewardedFailed -= OnRewardedFailed;
		AdMob.OnRewarded += OnRewardedRandomSkin;
		AdMob.OnRewardedFailed += OnRewardedFailed;

		AdMob.Instance.Show("Reward_Store");

		/*IronSourceManager.OnRewarded -= OnRewardedRandomSkin;
		IronSourceManager.OnRewardedFailed -= OnRewardedFailed;
		IronSourceManager.OnRewarded += OnRewardedRandomSkin;
		IronSourceManager.OnRewardedFailed += OnRewardedFailed;

		IronSourceManager.Instance.ShowRewardedVideo("Reward_Store");*/
	}

	private void OnRewardedRandomSkin()
	{
		AdMob.OnRewarded -= OnRewardedRandomSkin;
		AdMob.OnRewardedFailed -= OnRewardedFailed;

		/*IronSourceManager.OnRewarded -= OnRewardedRandomSkin;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/

		CancelInvoke("UpdateRewardAvailability");

		CellManager.ClickRandomAdsCell();
	}

	private void OnRewardedFailed()
	{
		AdMob.OnRewarded -= OnRewardedRandomSkin;
		AdMob.OnRewardedFailed -= OnRewardedFailed;

		/*IronSourceManager.OnRewarded -= OnRewardedRandomSkin;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/
	}

	public void Buy()
	{/*
		var lvl = new Dictionary<string, object>();
		lvl.Add("prefabname", CellManager.ActiveClickedCell.itemName);

		AnalyticEvents.ReportEvent("shop_skin_purchased", lvl);*/

		ShopManager.PlayerBoughtItem.Invoke(CellManager.ActiveClickedCell, true);
	}

	public void HideFooter()
	{
		PayBtnObject.SetActive(false);
		AdsBtnObject.SetActive(false);
	}
}
