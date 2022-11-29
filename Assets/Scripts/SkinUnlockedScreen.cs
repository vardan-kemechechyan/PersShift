using Enums;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class SkinUnlockedScreen : UIScreen
{
    GameManager gameManager;

    [SerializeField] UIManager ui;

    [SerializeField] ShopManager shopManager;

    [SerializeField] GameObject loseIt;
    [SerializeField] GameObject getDefaultSkin;
    [SerializeField] GameObject getRewardedButton;

    [SerializeField] GiftPositionCorrector giftPositioner;

    [SerializeField] Animator scaleAnimator;
    [SerializeField] Animator rotationAnimator;

    public override void Open()
    {
        base.Open();

        //IronSourceManager.Instance.skipInterstitial = false;
        AdMob.Instance.skipInterstitial = false;

        if(gameManager == null)
        {
            gameManager = GameManager.GetInstance();
        }

        ui.EnableCamera(2);

        loseIt.SetActive(false);
        getDefaultSkin.SetActive(false);

        giftPositioner.PositionTheItem(gameManager.UnlockedItem);

        CancelInvoke("UpdateRewardButton");

        StopCoroutine(ShowDelayedLoseButton());

        if(gameManager.UnlockedItem.lockInfo.lockStatus == LockStatus.UNLOCKED)
        {
            getRewardedButton.SetActive(false);
            getDefaultSkin.SetActive(true);
        }
        else
        {
            InvokeRepeating("UpdateRewardButton", 0, 1);

            StartCoroutine(ShowDelayedLoseButton());
		}

        scaleAnimator.Play("ScaleAnimation", 0, 0);
        rotationAnimator.Play("InfiniteAnimation", 0, 0);
    }

    private void OnDisable()
    {
        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/
    }

    public override void Close()
    {
        base.Close();

        ui.EnableCamera(0);
    }

    private void UpdateRewardButton()
    {
        bool isAdReady = false;

        //isAdReady = IronSourceManager.Instance.IsRewardedReady("Reward_Skin");
        isAdReady = AdMob.Instance.IsReady("Reward_Skin");

        getRewardedButton.SetActive(isAdReady);
    }

    private void OnRewarded()
    {
        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        AdMob.Instance.skipInterstitial = true;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;

        IronSourceManager.Instance.skipInterstitial = true;*/

        LockStatus _ls = LockStatus.UNLOCKED;
        SelectionState _ss = gameManager.UnlockedItem.selectionState;
        HighLight _hl = gameManager.UnlockedItem.highlight;
        LockType _lt = LockType.NONE;
        int price = 0;

        AnalyticEvents.ReportEvent("victory_claim");

        shopManager.UnlockItem(gameManager.UnlockedItem, _ls, _ss, _hl, _lt, price);

        NextScreen();
    }

    private void OnRewardedFailed()
    {
        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/

        LoseGift();
    }

    public void WatchAndGetGift()
    {
        getRewardedButton.SetActive(false);

        CancelInvoke("UpdateRewardButton");

        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;
        AdMob.OnRewarded += OnRewarded;
        AdMob.OnRewardedFailed += OnRewardedFailed;

        AdMob.Instance.Show("Reward_Skin");

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;
        IronSourceManager.OnRewarded += OnRewarded;
        IronSourceManager.OnRewardedFailed += OnRewardedFailed;

        IronSourceManager.Instance.ShowRewardedVideo("Reward_Skin");*/
    }

    public void GetDefaultGift()
    {
        CancelInvoke("UpdateRewardButton");

        LockStatus _ls = LockStatus.UNLOCKED;
        SelectionState _ss = gameManager.UnlockedItem.selectionState;
        HighLight _hl = gameManager.UnlockedItem.highlight;
        LockType _lt = LockType.NONE;
        int price = 0;

        shopManager.UnlockItem(gameManager.UnlockedItem, _ls, _ss, _hl, _lt, price);

        gameManager.LoadLevel(true);
        
        //shopManager.BuyAccessory(gameManager.UnlockedItem);
    }

    public void LoseGift()
    {
        CancelInvoke("UpdateRewardButton");

        LockStatus _ls = gameManager.UnlockedItem.lockInfo.lockStatus;
        SelectionState _ss = gameManager.UnlockedItem.selectionState;
        HighLight _hl = gameManager.UnlockedItem.highlight;
        LockType _lt = gameManager.UnlockedItem.lockInfo.lockType == LockType.NONE ? LockType.NONE : LockType.BUY_FOR_MONEY;
        int price = gameManager.UnlockedItem.lockInfo.priceToUnlock == -1 ? 0 : 2000;

        shopManager.UnlockItem(gameManager.UnlockedItem, _ls, _ss, _hl, _lt, price);

        NextScreen();
    }

    void NextScreen()
    {
        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/

        gameManager.LoadLevel(true);
    }

    IEnumerator ShowDelayedLoseButton()
    {
        yield return new WaitForSeconds(2f);

        loseIt.SetActive(true);
    }
}
