using UnityEngine;
using UnityEngine.UI;
using UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Enums;

public class GameScreen : UIScreen, ISubscribeUnsubscribe
{
    [SerializeField] CardSelector[] appearanceButtons;
    [SerializeField] AnimatedInTheFinishlin moneyAnimateEffect;
    [SerializeField] Slider progressBar;
    [SerializeField] GameManager gm;
    [SerializeField] List<Appearance> currentLevelAppearances = new List<Appearance>();

    [SerializeField] Button skipLevelButton;

    [SerializeField] GameObject settingsHolder;
    [SerializeField] GameObject moneyHolder;
    [SerializeField] GameObject successBar;
    [SerializeField] GameObject keysContainer;
    [SerializeField] GameObject skipButton;
    [SerializeField] Image HintBlockScreen;

    [SerializeField] Transform successHorizontalLine;
    [SerializeField] Image successGauge;

    [SerializeField] Image PraiseText;

    [SerializeField] float y_TOP;
    public override void Open()
    {
        base.Open();

        skipButton.SetActive(false);

        settingsHolder.SetActive(true);
        moneyHolder.SetActive(true);

        skipLevelButton.gameObject.SetActive(GameManager.GetInstance().TEST_MODE);

        //IronSourceManager.Instance.DestroyAd();
        AdMob.Instance.Hide();

        moneyAnimateEffect.gameObject.SetActive(false);

        Subscribe();
    }

	public override void Close()
	{
		base.Close();

        skipButton.SetActive(false);

        UnSubscribe();
    }

	public void UpdateLevelProgress(float progress) 
    {
        progressBar.value = progress;

        successHorizontalLine.localPosition = new Vector3(successHorizontalLine.localPosition .x, -y_TOP * ( 1f - progress), successHorizontalLine.localPosition.z);

        successGauge.fillAmount = progress;
    }

    public void LoadAppearanceButtons( string _clothesForTheLevel )
    {
        successBar.SetActive(true);

        successHorizontalLine.localPosition = new Vector3(successHorizontalLine.localPosition.x, -y_TOP, successHorizontalLine.localPosition.z);

        successGauge.fillAmount = 0f;

        PraiseText.enabled = false;

        currentLevelAppearances.Clear();

        foreach(var btn in appearanceButtons) btn.gameObject.SetActive(false);

        List<string> appearances_string = new List<string>(_clothesForTheLevel.Split('-'));

        Appearance[] ap = Enum.GetValues(typeof(Appearance)).OfType<Appearance>().ToArray();

        foreach(var _appearance in appearances_string)
            currentLevelAppearances.Add(ap.First((x) => x.ToString() == _appearance));

        int appearancesLength = appearances_string.Count;

        for(int i = 0; i < appearancesLength; ++i)
        {
            appearanceButtons[i].gameObject.SetActive(true);
            appearanceButtons[i].DefaultState();
            appearanceButtons[i].SetTheButton(currentLevelAppearances[i]);
		}
    }

    public void ShowPraiseText( float _reationTime)
    {
        if(_reationTime <= .75f)
            PraiseText.GetComponent<Animator>().Play("Show Praise");
    }

    void EnableHInt( Appearance _dressToCheck )
    {
        if( gm.CurrentLevelIndex  == 0 )
        {
            Color t = HintBlockScreen.color;

            t.a = 1f;

            HintBlockScreen.color = t;
		}

		foreach(var card in appearanceButtons)
		{
            if(card.gameObject.activeSelf)
            {
                card.EnableHint(card.GetAppearance() == _dressToCheck);
			}
        }
    }

    void DisableHint( bool _status )
    {
        if(_status) return;

        if(gm.CurrentLevelIndex == 0)
        {
            Color t = HintBlockScreen.color;

            t.a = 0f;

            HintBlockScreen.color = t;
		}

        foreach(var card in appearanceButtons)
        {
            if(card.gameObject.activeSelf)
                card.DefaultState();
        }
    }

    public void SkipLevel()
    {
        CustomGameEventList.SkipLevel.Invoke();
    }

    public int SubscribedTimes { get; set; }

    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"GameScreen: {SubscribedTimes}");

        CustomGameEventList.OnChangeGameState += OnChangeGameState;
        CustomGameEventList.PlayerChangedDress += ShowPraiseText;
        CustomGameEventList.TutorialCheckpoinReached += EnableHInt;
        CustomGameEventList.TurnOnHint += DisableHint;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        CustomGameEventList.OnChangeGameState -= OnChangeGameState;
        CustomGameEventList.PlayerChangedDress -= ShowPraiseText;
        CustomGameEventList.TutorialCheckpoinReached -= EnableHInt;
        CustomGameEventList.TurnOnHint -= DisableHint;
    }

    void OnChangeGameState(GameState _newState)
    {
        if(_newState == GameState.FinishLine)
        {
            foreach(var btn in appearanceButtons) btn.gameObject.SetActive(false);

            settingsHolder.SetActive(false);

            moneyHolder.SetActive(false);

            successBar.SetActive(false);

            keysContainer.SetActive(false);

            skipButton.SetActive(true);

            if( gm.IsPlayerFirstToFinish() && gm.CurrentLevelIndex > 0 )
                moneyAnimateEffect.StartTheAnimation((int)gm.PlayerSuccessRate, gm.Config.moneyBase, gm.moneyFromTheLevel);
        }
    }
}
