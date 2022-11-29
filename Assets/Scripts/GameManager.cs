using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Enums;
using static Configuration;


//TODO: [NEXT] Audio, SFX, VFX, Taptic

//TODO: [OPTIMIZATION] Change all publics into privates and get only through methods
//TODO: [FUTURE] Purchase/Restore purchase
//TODO: [FUTURE] Bonus game with keys and safes

public class GameManager : Singleton<GameManager>, ISubscribeUnsubscribe
{
    public bool TEST_MODE;

    public bool internetRequired;

    [SerializeField] LevelProgressionManager lvlPrgs_Manager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] SoundManager soundManager;
    SaveSystemExperimental sse;

    [SerializeField] CameraController cameraController;
    [SerializeField] Configuration config;

    [SerializeField] Transform finish;
    [SerializeField] Vector3 finish_values;

    public Customizable_SO UnlockedItem;
    public Configuration Config { get => config; }

    public CSVData CurrentLevelData;

    [SerializeField] UIManager ui;
    [SerializeField] GameScreen ui_GameScreen;
    [SerializeField] Character player;
    public Character Player
    {
        get => player;
        private set
        {
            player = value;
        }
    }
    [SerializeField] List<Character> aiPlayers = new List<Character>();

    public List<Character> playersOnFinish = new List<Character>();

    public bool IsPlayerWin { get; private set; }

    bool initialPlay;
    bool levelRestarted = false;

    [SerializeField] float playerSuccesRate;
    public float PlayerSuccessRate
    { 
        get => playerSuccesRate;
        set
        {
                    if(value >= .95f) playerSuccesRate = 2f;
            else    if(value >= .85f) playerSuccesRate = 1f;

            if(!IsPlayerFirstToFinish()) playerSuccesRate = 0f;

            //moneyFromTheLevel = Mathf.Ceil((Config.moneyBase * value ) * ( playerSuccesRate + 1 ));
            moneyFromTheLevel = Mathf.Ceil(Config.moneyBase * ( playerSuccesRate + 1 ));
        }
    }

    public float moneyFromTheLevel;

    [SerializeField] int currentLevelIndex = 0;
    public int CurrentLevelIndex
    {
        get { return currentLevelIndex; }
        set { currentLevelIndex = value; }
	}

    public bool disableLoop;
    public bool theGameIsFinished = false;

    public void Initialize()
    {
        if(sse == null) sse = SaveSystemExperimental.GetInstance();

        currentLevelIndex = sse.GetCurrentLevel();

        if(disableLoop && currentLevelIndex >= Config.csvData.Count) theGameIsFinished = true;

        ui.Initialize();

        ui_GameScreen = ui.GetScreen<GameScreen>();

        InitializeStartScreen();

        Subscribe();
    }

    private void OnApplicationQuit()
    {
        UnSubscribe();
    }

    public bool LevelAlreadyInitialized = false;

    public void InitializeStartScreen( bool _goToShop = false )
    {
        LevelAlreadyInitialized = false;

        if(AdMob.IsInitialized)
        {
            AdMob.Instance.RequestAll();
        }

        if(_goToShop)
            ui.ShowScreen<ShopScreen>();
        else
        {
            CurrentLevelData = GetLevelDescription(currentLevelIndex);

            lvlPrgs_Manager.LoadCurrentLevel();

            ui.PrepareTheButtons(CurrentLevelData.clothTypes);

            player = lvlPrgs_Manager.GetPlayer();
            aiPlayers = lvlPrgs_Manager.GetAIs();
            finish_values = lvlPrgs_Manager.GetFinishTransform();

            cameraController.SetTarget(Player.transform);

            player.Init(this);

            player.finish_values = finish_values;

            player.ui_GameScreen = ui_GameScreen;

            foreach(var p in aiPlayers)
                p.Init(this);

            UpdatePlayerAfterShop();

            LevelAlreadyInitialized = true;

            ui.ShowScreen<StartScreen>();
        }

        /*if(AdMob.IsInitialized)
        {
            AdMob.Instance.RequestAll();
        }

        CurrentLevelData = GetLevelDescription( currentLevelIndex );

        lvlPrgs_Manager.LoadCurrentLevel();

        ui.PrepareTheButtons(CurrentLevelData.clothTypes);

        player = lvlPrgs_Manager.GetPlayer();
        aiPlayers = lvlPrgs_Manager.GetAIs();
        finish_values = lvlPrgs_Manager.GetFinishTransform();

        cameraController.SetTarget(Player.transform);

        player.Init(this);

        player.finish_values = finish_values;

        player.ui_GameScreen = ui_GameScreen;

        foreach(var p in aiPlayers)
            p.Init(this);

        UpdatePlayerAfterShop();

        if(!_goToShop)
            ui.ShowScreen<StartScreen>();
        else
            ui.ShowScreen<ShopScreen>();*/
    }

    Coroutine delayedInitializeScreen;
    public void LoadLevel( bool _goToShop = false )
    {
        if(disableLoop && currentLevelIndex >= Config.csvData.Count)
        {
            ui.PlayerReachedTheLastLevel();
            return;
        }

        CustomGameEventList.OnChangeGameState.Invoke(GameState.LoadLevel);

        IsPlayerWin = false;

        player.move = false;

        foreach(var p in aiPlayers)
            p.move = false;

        lvlPrgs_Manager.DestroyLevel();

        if(delayedInitializeScreen != null) StopCoroutine(delayedInitializeScreen);

        delayedInitializeScreen = StartCoroutine( DelayedInitializeStartScreen(_goToShop) );
    }
    

    
    public void StartLevel()
    {
        StopCoroutine("StartLevelCoroutine");
        StartCoroutine("StartLevelCoroutine");



    }
    public void CompleteLevel()
    {
        if(IsPlayerFirstToFinish())
            IsPlayerWin = true;

        CustomGameEventList.PlayerCrossedTheFinishLine.Invoke( IsPlayerWin );



        StopCoroutine("EnableLevelCompleteFailScreen");
        StartCoroutine("EnableLevelCompleteFailScreen");
    }
    public void SkinUnlockedScreen()
    {
        if(disableLoop && currentLevelIndex >= Config.csvData.Count)
        {
            ui.PlayerReachedTheLastLevel();
            return;
        }

        ui.ShowScreen<SkinUnlockedScreen>();
    }
    void SkipCutScene()
    {
        foreach(var ai in aiPlayers)
        {
            ai.move = false ;
            ai.StoppedAtTheStopLine = true ;
		}

        player.move = false;
        player.StoppedAtTheStopLine = true;

        PlayerSuccessRate = 2f;

        //ChangeLevel(true);

        IsPlayerWin = true;

        CustomGameEventList.PlayerCrossedTheFinishLine.Invoke(IsPlayerWin);

        StopCoroutine("EnableLevelCompleteFailScreen");
        StartCoroutine("EnableLevelCompleteFailScreen");
    }
    public bool IsPlayerFirstToFinish() { return playersOnFinish.Count != 0 && player.Equals(playersOnFinish.First());  }
    public void SelectAppearance(Appearance appearance) 
    {
        player.PlayerChangedDress(appearance);
    }
    void ChangeLevel(bool _levelCompleteStatus)
    {
        var lvl = new Dictionary<string, object>();

        currentLevelIndex = _levelCompleteStatus ? currentLevelIndex + 1 : currentLevelIndex;

        if( !disableLoop )
            if(currentLevelIndex >= Config.csvData.Count) currentLevelIndex = 1;

        if( _levelCompleteStatus )
        {
            UnlockedItem = CheckForItemUnlock(currentLevelIndex - 1);

            if( UnlockedItem != null )
            {
                LockStatus _ls = LockStatus.UNLOCKED;
                SelectionState _ss = UnlockedItem.selectionState;
                HighLight _hl = UnlockedItem.highlight;
                LockType _lt = LockType.NONE;
                int price = UnlockedItem.lockInfo.priceToUnlock;

                if(UnlockedItem.lockInfo.priceToUnlock != -1 )
                {
                    _ls = LockStatus.LOCKED;
                    _lt = LockType.WATCH_AD;
                    price = 1;
                }

                shopManager.UnlockItem(UnlockedItem, _ls, _ss, _hl, _lt, price);

                sse.SyncAndSaveChanges();
            }

            levelRestarted = false;

            lvl.Add("level", currentLevelIndex - 1);
            AnalyticEvents.ReportEvent("level_complete", lvl);
        }
        else
        {
            levelRestarted = true;
            lvl.Add("level", currentLevelIndex );
            AnalyticEvents.ReportEvent("level_fail", lvl);
        }

        sse.SaveCurrentLevel(currentLevelIndex);
    }

    public Customizable_SO GetItemToFocusOn()
    {
        return UnlockedItem;
    }

    public void UpdatePlayerAfterShop()
    {
        player.UpdateAppearanceAndAccessories();
    }

    IEnumerator StartLevelCoroutine()
    {
        UnlockedItem = null;

        if(!initialPlay)
        {
            initialPlay = true;
            AnalyticEvents.ReportEvent("game_launch");
        }

        var lvl = new Dictionary<string, object>();

        if( levelRestarted )
        {
            lvl.Add("level", CurrentLevelIndex );

            AnalyticEvents.ReportEvent("lvl_restart", lvl );
        }
        else
        {
            lvl.Add("level", CurrentLevelIndex );

            AnalyticEvents.ReportEvent("level_start", lvl);
        }

        PlayerSuccessRate = 0f;

        cameraController.cameraInGameSettings.chaseSpeed = 100;

        playersOnFinish.Clear();
        ui.ShowScreen<GameScreen>();

        player.OnStart();

        foreach(var p in aiPlayers)
            p.OnStart();

        yield return new WaitForSeconds(1.0f);

        player.move = true;

        foreach(var p in aiPlayers)
            p.move = true;

        cameraController.cameraInGameSettings.chaseSpeed = 2.0f;
    }
    IEnumerator EnableLevelCompleteFailScreen()
    {
        //TODO: stop looping if the last level

        yield return new WaitForSeconds(2f);

        if(!IsPlayerWin)
                ui.ShowScreen<EndLevelScreen>();
        else
        {
            if(currentLevelIndex == 1)
                LoadLevel(false); //TODO: [FUTURE] call loading screen
            else
                ui.ShowScreen<LevelCompleteScreen>();

            initialPlay = false;
        }
    }

    //TODO: Replace with loading Screen
    IEnumerator DelayedInitializeStartScreen( bool _goToShop = false )
    {
        yield return new WaitForSeconds(0.1f);

        InitializeStartScreen(_goToShop);
    }

    #region Provide ConfigData
    public CSVData GetLevelDescription(int _levelIndex = 0) 
    {
        if(disableLoop && currentLevelIndex >= Config.csvData.Count)
        {
            ui.PlayerReachedTheLastLevel();
            currentLevelIndex = _levelIndex = Config.csvData.Count - 1;
        }

        return Config.csvData[_levelIndex];
    }
    public MovementSpeedInfo GetSpeedInfo() { return Config.characterSpeeds; }
    public bool GetRandomizeLevel() { return Config.randomizeLevels; }
    public int GetAICount() { return CurrentLevelData.AICount; }
    public int GetNumberOfLocationsToGenerate() { return CurrentLevelData.levelLength; }
    public string GetLocationsToGenerate() { return CurrentLevelData.locationToGenerate; }
    public string GetClothTypes() { return CurrentLevelData.clothTypes; }
    public string GetLevelDifficulties() { return CurrentLevelData.levelDifficulty; }
    public List<AI_behavior> GetAIBehaviors() { return Config.ai_behavior_description; }
    public float GetBasicDecisionTime() { return Config.baseDecisionMakingTime; }
    public int GetUnlockSkinRate() { return Config.unlockSkinRate; }
    public float GetBaseMoney() { return Config.moneyBase; }
    public Customizable_SO CheckForItemUnlock( int levelWon )
    {
        string presentName = string.Empty;

        presentName = config.GetGiftName(levelWon);
        //presentName = sse.GetGiftName(levelWon);

        return presentName != string.Empty? sse.GetItemFromRuntimeSOList(presentName) : null;
    }
    #endregion

    #region Tutorial Controller

    public bool GetTutorialStatus() { return CurrentLevelData.isTutorial; }

    #endregion

    public int SubscribedTimes { get; set; }
    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"GameManager: {SubscribedTimes}");

        CustomGameEventList.SkipLevel += SkipCutScene;
        CustomGameEventList.PlayerCrossedTheFinishLine += ChangeLevel;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        CustomGameEventList.SkipLevel -= SkipCutScene;
        CustomGameEventList.PlayerCrossedTheFinishLine -= ChangeLevel;
    }

    public void AcceptConsent(bool accept)
    {
        PlayerPrefs.SetInt("consent", Convert.ToInt16(accept));
        PlayerPrefs.Save();

        //IronSourceManager.Instance.SetConsent(accept);

        /*
        var parameters = new Dictionary<string, object>();
        parameters.Add("accept", accept);
        AnalyticEvents.ReportEvent("persadspopup_request", parameters);
        */
    }
}
