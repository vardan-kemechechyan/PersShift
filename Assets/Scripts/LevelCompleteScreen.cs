using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteScreen : UIScreen
{
    public static Action DiamondFinishedAnimation;

    GameManager gameManager;
    MoneyManagementSystem moneySystem;
    SaveSystemExperimental sse;

    [SerializeField] GiftAnimationController giftController;

    [SerializeField] TextMeshProUGUI claimButtonMultiplierText;
    [SerializeField] TextMeshProUGUI claimButtonRewardText;
    [SerializeField] Slider bonusMultiplier;
    [SerializeField] float bonusBarSpeed = 1.0f;

    [SerializeField] GameObject GetBonusButton;
    [SerializeField] GameObject GetExtraRewardButton;
    [SerializeField] GameObject RunningSlider;
    [SerializeField] TextMeshProUGUI GetBonusButtonMoney;

    [SerializeField] Image diamond;
    [SerializeField] List<Image> Diamonds = new List<Image>();
    [SerializeField] Transform DiamondHolder;
    [SerializeField] Transform JourneyStart;
    [SerializeField] Transform JourneyEnd;

    [SerializeField] FakeInactivity fakeNoRewardeButton;
    [SerializeField] FakeInactivity fakeWithAdButton;

    [SerializeField] float animationTimeInSeconds; //default: 1f
    [SerializeField] int crystalAmount; //default 20

    int numberOfDiamondRecieved = 0;

    bool skipInterstitial;

    float[] bonusPointerPositions = new float[] { 0.1685f, 0.3875f, 0.6075f, 0.8275f, 1f};
    int[] bonusPointerMultipliers = new int[] { 2, 3, 4, 3, 2 };

    bool invertBonusBar;

    int reward;
    int rewardValue;
    private int Reward
    {
        get => reward;
        set
        {
            //moneyText.text = value.ToString();
            reward = value;
        }
    }

    private void Start()
	{
        DiamondFinishedAnimation += delegate () 
                                    { 
                                        numberOfDiamondRecieved++;

                                        /*if(numberOfDiamondRecieved >= Diamonds.Count)
                                            Invoke("NextLevel", 0.25f);*/
                                    };
    }

	public override void Open()
    {
        base.Open();

        skipInterstitial = false;

        //IronSourceManager.Instance.skipInterstitial = false;
        AdMob.Instance.skipInterstitial = false;

        if(gameManager == null)
        {
            gameManager = GameManager.GetInstance();
            moneySystem = gameManager.GetComponent<MoneyManagementSystem>();
            sse = SaveSystemExperimental.GetInstance();
        }

        if(Diamonds.Count == 0)
        {
			for(int i = 0; i < crystalAmount; i++)
                Diamonds.Add(Instantiate(diamond, DiamondHolder));
        }

        numberOfDiamondRecieved = 0;

        GetBonusButtonMoney.text = gameManager.moneyFromTheLevel.ToString();

        bool showTheMultiplier = gameManager.CurrentLevelIndex % gameManager.Config.moneyMultiplierInterval == 0;
        
        CancelInvoke("UpdateRewardButton");
        
        fakeWithAdButton.FadeIn(true);
        fakeNoRewardeButton.FadeIn(true);

        StopCoroutine(FloatingBonusBar());
        
        StopCoroutine(EnableGetAndContinueButton());
       
        if(showTheMultiplier)
        {
            GetExtraRewardButton.SetActive(true);

            RunningSlider.SetActive(true);

            InvokeRepeating("UpdateRewardButton", 0, 1);

            StartCoroutine(FloatingBonusBar());
            
            StartCoroutine(EnableGetAndContinueButton());
		}
        else
        {
            RunningSlider.SetActive(false);

            GetExtraRewardButton.SetActive(false);

            GetBonusButton.SetActive(true);

            fakeNoRewardeButton.FadeIn(false);
        }

        foreach(var diam in Diamonds)
            diam.gameObject.SetActive(false);

        if(gameManager.CurrentLevelIndex > 1) 
            giftController.UpdateGiftProgress();
    }

    private void OnDisable()
    {
        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/

        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;
    }

    public override void Close()
    {
        base.Close();

        CancelInvoke("UpdateRewardButton");
        StopCoroutine(FloatingBonusBar());
    }

    public void NextLevel()
    {
        fakeWithAdButton.FadeIn(true);

        if(skipInterstitial)
        {
            NextScreen();
        }
        else
        {
            AdMob.Instance.Show("Interstitial_lvl", success =>
            {
                NextScreen();
            });

            /*IronSourceManager.Instance.ShowInterstitial("Interstitial_lvl", success =>
            {
                AnalyticEvents.ReportEvent("victory_interstitial");

                NextScreen();
            });*/
        }
    }

    void NextScreen()
    {
        if(giftController.GetSkinProgressValue() == 1f)
        {
            gameManager.SkinUnlockedScreen();
        }
        else
        {
            gameManager.LoadLevel(false);
        }
    }

    private void UpdateRewardButton()
    {
        bool isAdReady = false;

        isAdReady = AdMob.Instance.IsReady("Reward_ClaimX");
        //isAdReady = IronSourceManager.Instance.IsRewardedReady("Reward_ClaimX");

        fakeWithAdButton.FadeIn(!isAdReady);

        //claimRewardButton.interactable = isAdReady;
    }

    public void GetReward()
    {
        AnalyticEvents.ReportEvent("victory_claim");

        CancelInvoke("UpdateRewardButton");
        StopCoroutine(FloatingBonusBar());

        Continue(true);
    }

    public void Continue(bool bonus)
    {
        CancelInvoke("UpdateRewardButton");
        StopCoroutine(FloatingBonusBar());

        if(bonus)
        {
            AdMob.OnRewarded -= OnRewarded;
            AdMob.OnRewardedFailed -= OnRewardedFailed;
            AdMob.OnRewarded += OnRewarded;
            AdMob.OnRewardedFailed += OnRewardedFailed;

            AdMob.Instance.Show("Reward_ClaimX");

            /*IronSourceManager.OnRewarded -= OnRewarded;
            IronSourceManager.OnRewardedFailed -= OnRewardedFailed;
            IronSourceManager.OnRewarded += OnRewarded;
            IronSourceManager.OnRewardedFailed += OnRewardedFailed;

            IronSourceManager.Instance.ShowRewardedVideo("Reward_ClaimX");*/
        }
        else
        {
            GetBonusWithoutAd();
        }
    }

    public void GetBonusWithoutAd()
    {
        CancelInvoke("UpdateRewardButton");
        StopCoroutine(FloatingBonusBar());

        rewardValue = (int)gameManager.moneyFromTheLevel;
        Reward = rewardValue;

        StopCoroutine(AnimateMoneyNumbers());
        StartCoroutine(AnimateMoneyNumbers());
    }

    public void OnRewarded()
    {
        skipInterstitial = true;

        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        AdMob.Instance.skipInterstitial = true;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;

        IronSourceManager.Instance.skipInterstitial = true;*/

        StopCoroutine(AnimateMoneyNumbers());
        StartCoroutine(AnimateMoneyNumbers());
    }

    private void OnRewardedFailed()
    {
        skipInterstitial = true;

        AdMob.OnRewarded -= OnRewarded;
        AdMob.OnRewardedFailed -= OnRewardedFailed;

        AdMob.Instance.skipInterstitial = true;

        /*IronSourceManager.OnRewarded -= OnRewarded;
        IronSourceManager.OnRewardedFailed -= OnRewardedFailed;*/

        GetBonusWithoutAd();
    }
    private int GetBonusValue()
    {
        for(int i = 0; i < bonusPointerPositions.Length; i++)
            if(bonusMultiplier.value <= bonusPointerPositions[i])
                return bonusPointerMultipliers[i];

        return 1;
    }

    IEnumerator EnableGetAndContinueButton()
    {
        GetBonusButton.SetActive(false);

        yield return new WaitForSeconds(2f);

        GetBonusButton.SetActive(true);

        fakeNoRewardeButton.FadeIn(false);
    }

    IEnumerator AnimateMoneyNumbers()
    {
        fakeNoRewardeButton.FadeIn(true);
        fakeWithAdButton.FadeIn(true);

        float currentMoney = moneySystem.Money;
  
        float finalSum = moneySystem.Money + Reward;

        int numberOfDiamondCreated = 0;

        float timeStep = 0.025f;
        int stepCount = Mathf.CeilToInt( animationTimeInSeconds / timeStep );
        float deltaMoney = Reward / stepCount;
        int numberOfDiamondsToAnimate = stepCount;

        if( numberOfDiamondsToAnimate > Diamonds.Count )
        {
            int additionalDiamonds = numberOfDiamondsToAnimate - Diamonds.Count;

            for(int i = 0; i < additionalDiamonds; i++)
                Diamonds.Add(Instantiate(diamond, DiamondHolder));
		}

        while( currentMoney < finalSum )
        {
            currentMoney += deltaMoney;
            moneySystem.Money += Mathf.Ceil(deltaMoney);

            if(numberOfDiamondCreated < numberOfDiamondsToAnimate )
                Diamonds[numberOfDiamondCreated].GetComponent<DiamondAnimationManager>().StartAnimation(animationTimeInSeconds + 0.25f, JourneyStart.position, JourneyEnd.position);

            numberOfDiamondCreated++;

            yield return timeStep;
        }

        moneySystem.Money = finalSum;

        sse.SaveMoney((int)moneySystem.Money);

        sse.Save();

        while(numberOfDiamondRecieved < numberOfDiamondsToAnimate ) yield return null;

        Invoke("NextLevel", 0.5f); //TODO: [FUTURE] call loading screen
    }

    WaitForEndOfFrame bonusBarDelay = new WaitForEndOfFrame();
    IEnumerator FloatingBonusBar()
    {
        rewardValue = (int)gameManager.moneyFromTheLevel;
        Reward = rewardValue;

        invertBonusBar = false;
        bonusMultiplier.value = 0;

        while(true)
        {
            if(invertBonusBar)
                bonusMultiplier.value -= bonusBarSpeed;
            else
                bonusMultiplier.value += bonusBarSpeed;
            yield return bonusBarDelay;

            Reward = rewardValue * GetBonusValue();

            claimButtonMultiplierText.text = $"GET X{GetBonusValue()}";
            claimButtonRewardText.text = Reward.ToString();

            if(bonusMultiplier.value == 1 && !invertBonusBar) invertBonusBar = true;
            else if(bonusMultiplier.value == 0 && invertBonusBar) invertBonusBar = false;
        }
    }
}
