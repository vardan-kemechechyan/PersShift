using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System;
using System.Linq;
using DG.Tweening;

public class Character : MonoBehaviour, ISubscribeUnsubscribe
{
    GameManager gameManager;
    SaveSystemExperimental sse;
    SoundManager sm;

    [HideInInspector] public GameScreen ui_GameScreen;

    public Control control;
    public AIDifficulty ai_difficulty;
    public AI_behavior ai;

    public float BasicDecisionTime;

    [SerializeField] Animator animator;

    [SerializeField] LocationManager loc_Manager;

    Dictionary<Appearance, GameObject> PrefabsForTheLevel = new Dictionary<Appearance, GameObject>();
    GameObject CurrentActivePrefab;
    [SerializeField] GameObject modelHolder;

    [SerializeField] List<Appearance> areaAppearance;

    Appearance tempAppearance;
    public Appearance appearance;

    [SerializeField] List<Appearance> LevelAppearance;

    MovementSpeedInfo speedInfo;

    public Vector3 finish_values;

    [SerializeField] float moveSpeed;

    public bool move;
    bool tempMove;
    bool crossedTheFinishLine = false;
    public bool CrossedTheFinishLine
    {
        get { return crossedTheFinishLine; }
        set { crossedTheFinishLine = value; }
    }
    bool stoppedAtTheStopLine = false;
    public bool StoppedAtTheStopLine
    {
        get { return stoppedAtTheStopLine; }
        set { stoppedAtTheStopLine = value; }
    }

    public Action<List<Appearance>, List<Vector3>, int, Locations> OnEnterArea;
    public Action<int> OnEnterStopLine;

    float aiMoveDelay;

    bool correctDressEnabled = true;
    float successRateValue = 0f;

    [SerializeField] float timeAfterCrossingLocation = 0f;

    List<float> TutorialEnablePathRatio = new List<float>();
    int tutorialToEnable = -1;
    bool isTutorial = false;
    List<Vector3> locationStartEndPositions = new List<Vector3>();

    bool levelSkipped;

    [SerializeField] ParticleSystem goodParticle;
    [SerializeField] ParticleSystem badParticle;

    private void OnDisable() { UnSubscribe(); }

	private void OnDestroy() { UnSubscribe(); }

    float timeInLevel = 0f;

    int levelIndex = 0;
    int locationIndex = 0;
    string locationName = "";

    EmojiController myEmojies;
    WindController myWind;

    DressCrossState dressChangeState;

    public void OnStart()
    {
        //appearance = Appearance.CASUAL;

        UpdateAppearance();

        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);

        timeInLevel = 0f;

        move = true;

        levelSkipped = false;
    }

    private void UpdateAppearance() 
    {
        UpdateReferences(appearance );

        UpdateAnimation();

        if (areaAppearance.Contains(Appearance.None) || areaAppearance.Count == 0 ) 
        {
            moveSpeed = speedInfo.default_speed;
            correctDressEnabled = true;
        }
        else if (areaAppearance.Contains(appearance))
        {
            moveSpeed = speedInfo.max_speed;
            correctDressEnabled = true;
        }
        else 
        {
            moveSpeed = speedInfo.min_speed;
            correctDressEnabled = false;
        }
    }

    private void UpdateAnimation() 
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (move)
        {
            int _animationIndex = 0;

            if( areaAppearance == null || areaAppearance.Count == 0 /*|| areaAppearance[0] == Appearance.None*/) _animationIndex = 0;
            else if(areaAppearance[0] == Appearance.SWIMMING)   _animationIndex = 2;
            else /*if(areaAppearance[0] != Appearance.None ) */ _animationIndex = 1;

            animator.Play( areaAppearance.Contains(appearance) || areaAppearance[0] == Appearance.None ?
            gameManager.Config.appearanceAnimations[_animationIndex].correct[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[_animationIndex].correct.Length)].name :
            gameManager.Config.appearanceAnimations[_animationIndex].wrong[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[_animationIndex].wrong.Length)].name, 0, 0);
        }
        else 
        {
            if( !StoppedAtTheStopLine )
            {
                //animator.Play("Idle", 0, -1);
                if(areaAppearance != null && areaAppearance.Count != 0 && areaAppearance[0] == Appearance.SWIMMING)
                    animator.Play(gameManager.Config.appearanceAnimations[2].wrong[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[2].wrong.Length)].name, 0, 0);
                else
                    animator.Play(gameManager.Config.appearanceAnimations[0].correct[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[0].correct.Length)].name, 0, 0);
    		}
        }
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        if(control == Control.Player)
        {
            if(myEmojies == null )
                myEmojies = GetComponent<EmojiController>();

            if(myWind == null )
                myWind = GetComponent<WindController>();

            myEmojies.HideEmojis();
            myWind.DisableAllWinds();
        }

        dressChangeState = DressCrossState.NOCHANGE;

        levelIndex = gameManager.CurrentLevelIndex;

        if(sm == null) sm = SoundManager.GetInstance();

        TutorialEnablePathRatio = gameManager.Config.TutorialEnablePathRatio;

        Subscribe();

        correctDressEnabled = true;

        successRateValue = 0f;

        speedInfo = gameManager.GetSpeedInfo();
        
        moveSpeed = speedInfo.default_speed;

        BasicDecisionTime = gameManager.GetBasicDecisionTime();

        isTutorial = GameManager.GetInstance().GetTutorialStatus();

        tutorialToEnable = isTutorial && levelIndex == 0 ? - 1 : 6;

        if(LevelAppearance == null || LevelAppearance.Count == 0)
            LevelAppearance = new List<Appearance>(loc_Manager.ReturnLevelAppearanceList());

        LoadApearancesForTheLevel();

        UpdateAppearance();

        if(gameManager.theGameIsFinished)
            gameObject.SetActive(false);
    }

    public void UpdateAppearanceAndAccessories()
    {
		foreach(var pref in PrefabsForTheLevel)
            Destroy(pref.Value);

        PrefabsForTheLevel.Clear();

        LoadApearancesForTheLevel();

        UpdateAppearance();
    }

    void LoadApearancesForTheLevel()
    {
        if(sse == null) sse = SaveSystemExperimental.GetInstance();

        Dictionary<Appearance, List<string>> CostumeNamesForTheLevel = sse.ReturnNamesOfSelectedCostumes(LevelAppearance, control);

		foreach(var costName in CostumeNamesForTheLevel)
		{
            GameObject GO;

            if(control == Control.AI)
                GO = Instantiate(Resources.Load<GameObject>("Costume/" + costName.Value[UnityEngine.Random.Range(0, costName.Value.Count)]), modelHolder.transform);
            else
                GO = Instantiate(Resources.Load<GameObject>("Costume/" + costName.Value[0]), modelHolder.transform);

            PrefabsForTheLevel.Add(costName.Key, GO);
        }

        foreach(var item in PrefabsForTheLevel)
        {
            if(item.Key != loc_Manager.FirstAppearingLocation)
            {
                item.Value.SetActive(true);
                appearance = item.Key;
                CurrentActivePrefab = item.Value;
                animator = CurrentActivePrefab.GetComponent<Animator>();
                break;
            }
        }

        DressUp();
    }

    void UpdateReferences( Appearance _toactivate)
    {
		foreach(var item in PrefabsForTheLevel)
		{
            if(item.Key == _toactivate)
            {
                CurrentActivePrefab = item.Value;
                item.Value.SetActive(true);
            }
            else
                item.Value.SetActive(false);
		}
        
        animator = CurrentActivePrefab.GetComponent<Animator>();
    }

    void DressUp()
    {
        if(sse == null) sse = SaveSystemExperimental.GetInstance();

        List<(string, string)> AccessoriesToWear = sse.ReturnListOfAccessories(control);

        string hairColor = sse.GetHairColor(control == Control.Player);

        foreach(var item in PrefabsForTheLevel)
        {
            item.Value.GetComponent<AccessoruManagement>().LoadAccessory(AccessoriesToWear);
            item.Value.GetComponent<AccessoruManagement>().ApplyDye("#"+hairColor);
		}
    }
    private void OnAreaEnter(List<Appearance> appearance, List<Vector3> StartEndPositions, int _locationIndex, Locations _locName )
    {
        if(control == Control.Player)
            if(!(areaAppearance.Contains(Appearance.None) || areaAppearance.Count == 0))
            {
                if(dressChangeState == DressCrossState.NOCHANGE)
                {
                    /*var lvl = new Dictionary<string, object>();
                    var locIndex = new Dictionary<string, object>();
                    var locName = new Dictionary<string, object>();
                    var dress = new Dictionary<string, object>();

                    dress.Add(locationIndex.ToString(), dressChangeState.ToString());

                    locIndex.Add(levelIndex.ToString(), dress);
                    locName.Add(locationName.ToString(), dressChangeState.ToString());

                    lvl.Add("Level", locIndex);
                    lvl.Add("Location", locName);

                    AnalyticEvents.ReportEvent("Change_clothes", lvl);*/

                    var lvl = new Dictionary<string, object>();
                    var loc = new Dictionary<string, object>();

                    lvl.Add("Level", levelIndex);
                    lvl.Add("Location", locationIndex + "_" + locationName);
                    lvl.Add("DressStatus", dressChangeState.ToString());

                    AnalyticEvents.ReportEvent("ClothesStatus_Level", lvl, false);

                    var lvl_json = new Dictionary<string, object>();
                    var locIndex_json = new Dictionary<string, object>();
                    var locName_json = new Dictionary<string, object>();
                    var dress_json = new Dictionary<string, object>();

                    dress_json.Add(locationIndex + "_" + locationName, dressChangeState.ToString());

                    locIndex_json.Add(levelIndex.ToString(), dress_json);
                    locName_json.Add(locationName.ToString(), dressChangeState.ToString());

                    lvl_json.Add("Level", locIndex_json);

                    var str_json = YMMJSONUtils.JSONEncoder.Encode(lvl_json);
                    AnalyticEvents.ReportEvent("ClothesStatus_Level", str_json);



                    loc.Add("LocationName", locationName);
                    loc.Add("DressStatus", dressChangeState.ToString());
                    AnalyticEvents.ReportEvent("Change_clothes_location_based", loc, false);




                    var loc_json = new Dictionary<string, object>();
                    loc_json.Add(dressChangeState.ToString(), locationName);

                    var str_loc_json = YMMJSONUtils.JSONEncoder.Encode(loc_json);
                    AnalyticEvents.ReportEvent("ClothesStatus_Dress_Location", str_loc_json);



                    var loc_dress_json = new Dictionary<string, object>();
                    loc_dress_json.Add(locationName, dressChangeState.ToString());

                    var str_loc_dress_json = YMMJSONUtils.JSONEncoder.Encode(loc_dress_json);
                    AnalyticEvents.ReportEvent("ClothesStatus_Location_Dress", str_loc_dress_json);

                    /*Firebase.Analytics.Parameter[] DressChangingParameters = { 
                    
                        new Firebase.Analytics.Parameter(),
                        new Firebase.Analytics.Parameter(),
                        new Firebase.Analytics.Parameter()
                    };*/
                }
            }

        dressChangeState = DressCrossState.NOCHANGE;

        areaAppearance = new List<Appearance>( appearance );
        locationIndex = _locationIndex;
        locationName = _locName.ToString();

        if(myEmojies != null )
            myEmojies.RecalculatePosition(areaAppearance.Contains(Appearance.SWIMMING) ? 1f : areaAppearance.Contains(Appearance.None) ? 1.8f : 2f);

        if(control == Control.Player)
        {
            if( move )
            {
                tutorialToEnable++;
                locationStartEndPositions = StartEndPositions;

                if( isTutorial && levelIndex == 0)      tutorialToEnable = Mathf.Clamp(tutorialToEnable, 0, 5);
                else                                    tutorialToEnable = 6;
            }

            if( !isTutorial )
                CustomGameEventList.TurnOnHint.Invoke(false);
        }

        UpdateAppearance();

        if( control == Control.Player )
        {
            StopCoroutine("IncreaseTimeAfterCrossingTheLocation");
            StartCoroutine("IncreaseTimeAfterCrossingTheLocation");
        }

        if(control == Control.AI && !areaAppearance.Contains(Appearance.None) ) 
        {
            StopCoroutine("AISelectApearance");
            StartCoroutine("AISelectApearance");
        }
    }

    private void OnStopLine( int finishLineIndex )
    {
        //UpdateAnimation();

        if(control == Control.Player)
        {
            if( ( finishLineIndex == 0 && gameManager.playersOnFinish.Count > 2) || (finishLineIndex == gameManager.PlayerSuccessRate))
            {
                StoppedAtTheStopLine = true;

                move = false;

                animator.transform.Rotate(Vector3.up, 180);

                bool PlayerIsTheWinner = gameManager.IsPlayerFirstToFinish();

                animator.Play( PlayerIsTheWinner ?
                    gameManager.Config.appearanceAnimations[3].correct[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[3].correct.Length)].name :       // PLAY HAPPY ANIMATION
                    gameManager.Config.appearanceAnimations[3].wrong[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[3].wrong.Length)].name, 0, 0);  // PLAY SAD ANIMATION
                    
                gameManager.CompleteLevel(); //FAIL

                if(myEmojies != null && !PlayerIsTheWinner)
                    myEmojies.EnableEmojiEffect(EffectStyle.LOST, 1.8f);
            }
        }
        else
        {
            StoppedAtTheStopLine = true;
            move = false;

            if( finishLineIndex == 0 )
            {
                animator.transform.Rotate(Vector3.up, 180);

                animator.Play(!gameManager.IsPlayerFirstToFinish() ?
                    gameManager.Config.appearanceAnimations[3].correct[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[3].correct.Length)].name :       // PLAY SAD ANIMATION
                    gameManager.Config.appearanceAnimations[3].wrong[UnityEngine.Random.Range(0, gameManager.Config.appearanceAnimations[3].wrong.Length)].name, 0, 0);  // PLAY HAPPY ANIMATION
            }
        }
    }

    void PlayerStopped( bool _playerWinStatus )
    {
        if(gameObject.activeSelf && control == Control.AI)
        {
            animator.transform.LookAt(gameManager.Player.transform);
            animator.Play(gameManager.Config.appearanceAnimations[4].wrong[0].name, 0, 0);
        }
    }

    public void SkipToFinishLines( float _zPositionStopLine )
    {
        if( StoppedAtTheStopLine ) return;

        if( control == Control.AI )
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _zPositionStopLine);
            gameManager.playersOnFinish.Add(this);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _zPositionStopLine);
        }
	}

    void Update()
    {
        if(levelSkipped) return;

        if(tempAppearance != appearance) 
        {
            tempAppearance = appearance;
            UpdateAppearance();
        }

        if (tempMove != move) 
        {
            tempMove = move;
            UpdateAnimation();
        }

        if (move)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            if(control == Control.Player )
            {
                if( !CrossedTheFinishLine )
                    timeInLevel += Time.deltaTime;

                if( correctDressEnabled )
                {
                    successRateValue += 1f / (finish_values.z / ( moveSpeed * Time.deltaTime) );
                
                    ui_GameScreen.UpdateLevelProgress(successRateValue);
                }

                //if( isTutorial )
                //{
                    if(tutorialToEnable > -1)
                    {
                        if(locationStartEndPositions != null && locationStartEndPositions.Count != 0 )
                            if( ( ( transform.position.z - locationStartEndPositions[0].z ))/ ( locationStartEndPositions[1].z - locationStartEndPositions[0].z ) >= TutorialEnablePathRatio[tutorialToEnable])
                            {
                                if(areaAppearance[0] != appearance )
                                {
                                    CustomGameEventList.TutorialCheckpoinReached.Invoke(areaAppearance[0]);

                                    if( levelIndex == 0)
                                        CustomGameEventList.TurnOnHint.Invoke( true );

                                    locationStartEndPositions.Clear();
                                }
                            }
					}
                //}
            }
        }
    }

    void Rotat180()
    {
        modelHolder.transform.DOLocalRotate( Vector3.up * 359, 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(delegate () {
            modelHolder.transform.localEulerAngles = Vector3.zero;

            /*if(myEmojies != null)
                myEmojies.HideEmojis();*/
        });
	}

    public void PlayerChangedDress(Appearance _appearance)
    {
        appearance = _appearance;

        UpdateAppearance();

        if( areaAppearance.Contains(appearance) )
        {
            CustomGameEventList.PlayerChangedDress.Invoke(timeAfterCrossingLocation);
            CustomGameEventList.TurnOnHint.Invoke(false);

            goodParticle.Play();

            sm.ChangeDressSound(true);

            dressChangeState = DressCrossState.CORRECT;

            /*var lvl = new Dictionary<string, object>();
            lvl.Add(gameManager.CurrentLevelIndex.ToString(), "correct");

            AnalyticEvents.ReportEvent("clothes_change", lvl);*/


            if(myEmojies != null)
                if(areaAppearance.Contains(Appearance.SWIMMING))
                    myEmojies.EnableEmojiEffect(EffectStyle.POSITIVE, 1f);
                else
                    myEmojies.EnableEmojiEffect(EffectStyle.POSITIVE);

            if(myWind != null)
                myWind.PlayRandomWind();
        }
        else
        {
            if(!areaAppearance.Contains(Appearance.None))
            {
                /*var lvl = new Dictionary<string, object>();
                lvl.Add(gameManager.CurrentLevelIndex.ToString(), "incorrect");

                AnalyticEvents.ReportEvent("clothes_change", lvl);*/

                dressChangeState = DressCrossState.INCORRECT;

                sm.ChangeDressSound(false);

                if(myEmojies != null)
                    if(areaAppearance.Contains(Appearance.SWIMMING))
                        myEmojies.EnableEmojiEffect(EffectStyle.NEGATIVE, 1f);
                    else
                        myEmojies.EnableEmojiEffect(EffectStyle.NEGATIVE );

                //badParticle.Play();
            }
        }

        if(control == Control.Player)
            if(!areaAppearance.Contains(Appearance.None))
            {
               /* var lvl = new Dictionary<string, object>();
                var locIndex = new Dictionary<string, object>();
                var locName = new Dictionary<string, object>();
                var dress = new Dictionary<string, object>();

                dress.Add(locationIndex.ToString(), dressChangeState.ToString());

                locIndex.Add(levelIndex.ToString(), dress);
                locName.Add(locationName.ToString(), dressChangeState.ToString());

                lvl.Add("Level", locIndex);
                lvl.Add("Location", locName);

                AnalyticEvents.ReportEvent("Change_clothes", lvl);*/

                var lvl = new Dictionary<string, object>();
                var loc = new Dictionary<string, object>();

                lvl.Add("Level", levelIndex);
                lvl.Add("Location", locationIndex + "_" + locationName);
                lvl.Add("DressStatus", dressChangeState.ToString());

                AnalyticEvents.ReportEvent("ClothesStatus_Level", lvl, false);

                var lvl_json = new Dictionary<string, object>();
                var locIndex_json = new Dictionary<string, object>();
                var locName_json = new Dictionary<string, object>();
                var dress_json = new Dictionary<string, object>();

                dress_json.Add(locationIndex + "_" + locationName, dressChangeState.ToString());

                locIndex_json.Add(levelIndex.ToString(), dress_json);
                locName_json.Add(locationName.ToString(), dressChangeState.ToString());

                lvl_json.Add("Level", locIndex_json);

                var str_json = YMMJSONUtils.JSONEncoder.Encode(lvl_json);
                AnalyticEvents.ReportEvent("ClothesStatus_Level", str_json);

                loc.Add("LocationName", locationName);
                loc.Add("DressStatus", dressChangeState.ToString());
                AnalyticEvents.ReportEvent("Change_clothes_location_based", loc, false);




                var loc_json = new Dictionary<string, object>();
                loc_json.Add(dressChangeState.ToString(), locationName);

                var str_loc_json = YMMJSONUtils.JSONEncoder.Encode(loc_json);
                AnalyticEvents.ReportEvent("ClothesStatus_Dress_Location", str_loc_json);




                var loc_dress_json = new Dictionary<string, object>();
                loc_dress_json.Add(locationName, dressChangeState.ToString());

                var str_loc_dress_json = YMMJSONUtils.JSONEncoder.Encode(loc_dress_json);
                AnalyticEvents.ReportEvent("ClothesStatus_Location_Dress", str_loc_dress_json);
            }

        Rotat180();

        /*if( isTutorial && !move )
        {
            CustomGameEventList.TurnOnHint.Invoke(false);
        }   */
    }

    IEnumerator AISelectApearance() 
    {
        aiMoveDelay = BasicDecisionTime * ai.card_pick_delay;

        yield return new WaitForSeconds(aiMoveDelay);

        bool isCorrect = UnityEngine.Random.value > ai.wrong_card_pick_percent;

        if(isCorrect)
        {
            foreach(var app in areaAppearance)
                if(LevelAppearance.Contains(app))
                {
                    appearance = app;
                    break;
                }
        }
        else
        {
            appearance = Enum.GetValues(typeof(Appearance)).Cast<Appearance>().
                FirstOrDefault(x => !areaAppearance.Contains(x) && !x.Equals(appearance) && LevelAppearance.Contains(x) && x != Appearance.None);

            yield return new WaitForSeconds(BasicDecisionTime);

            foreach(var app in areaAppearance)
                if(LevelAppearance.Contains(app))
                {
                    appearance = app;
                    break;
                }
        }
    }

    IEnumerator IncreaseTimeAfterCrossingTheLocation()
    {
        timeAfterCrossingLocation = 0f;

        while ( move == true )
        {
            yield return 0;

            if(!isTutorial || ( isTutorial && move ) )
                timeAfterCrossingLocation += Time.deltaTime;
        }
    }

    public void SetAI(AI_behavior _ai_from_config)
    {
        ai = _ai_from_config;
        ai_difficulty = ai.difficulty_type;
    }

    public void CharacterCrossedTheFinishLine()
    {
        CrossedTheFinishLine = true;

        gameManager.playersOnFinish.Add(this);

        if( control == Control.Player)
        {
            if(gameManager.IsPlayerFirstToFinish())
            {
                var lvl = new Dictionary<string, object>();
                lvl.Add(gameManager.CurrentLevelIndex.ToString(), timeInLevel);

                AnalyticEvents.ReportEvent("lvl_X_time", lvl);

                sm.PlayerCrossedTheFinishLine(true);
            }
            else
            {
                sm.PlayerCrossedTheFinishLine(false);
            }

            gameManager.PlayerSuccessRate = successRateValue;
		}
    }

    void EnableHInt( bool _hintStatus)
    {
        if( isTutorial /*&& !move*/)
        {
            move = !_hintStatus;

            if(control == Control.AI )
                if(_hintStatus)
                    StopCoroutine("AISelectApearance");
                else
                    StartCoroutine("AISelectApearance");
		}
    }

    public void SkipLevel() { levelSkipped = true; }

    public int SubscribedTimes { get; set; }

    enum DressCrossState
    {
        CORRECT,
        INCORRECT,
        NOCHANGE
	}
    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"Character: {SubscribedTimes}");

        OnEnterArea += OnAreaEnter;
        OnEnterStopLine += OnStopLine;
        CustomGameEventList.TurnOnHint += EnableHInt;
        CustomGameEventList.PlayerCrossedTheFinishLine += PlayerStopped;
        CustomGameEventList.SkipLevel += SkipLevel;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        OnEnterArea -= OnAreaEnter;
        OnEnterStopLine -= OnStopLine;
        CustomGameEventList.TurnOnHint -= EnableHInt;
        CustomGameEventList.PlayerCrossedTheFinishLine -= PlayerStopped;
        CustomGameEventList.SkipLevel -= SkipLevel;
    }
}