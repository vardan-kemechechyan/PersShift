using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour, ISubscribeUnsubscribe
{
    public LocationManager mng_Location;
    GameManager gm;
    
    public Vector3 finish;
    public Character player;
    public List<Character> aiPlayers;
    public List<Character> activeAIs = new List<Character>();

    [SerializeField] List<Transform> StopLines;

    [SerializeField] ParticleSystem[] FinishConfettis;
    
    [Tooltip( "true - Generate random level based on locations;\nfalse - Prepare the level based on hierarchy in the prefab list" )]
    [SerializeField] bool RandomizeLevels;

	private void Start()
	{
        Subscribe();
    }

	private void OnDisable()
	{
        UnSubscribe();
    }

	private void OnDestroy()
	{
        UnSubscribe();
    }

	public void Load()
    {
        PrepareTheLevel();
    }

    void PrepareTheLevel()
    {
        gameObject.SetActive( true );

        if( gm == null ) gm = GameManager.GetInstance();

        RandomizeLevels = gm.GetRandomizeLevel();

        int ai_Count = gm.GetAICount();

		foreach(var ai in aiPlayers)
            ai.gameObject.SetActive(false);

        if(ai_Count == 1) aiPlayers[UnityEngine.Random.Range(0, aiPlayers.Count)].gameObject.SetActive(true);
        else if( ai_Count != 0 ) 
                foreach(var ai in aiPlayers)
                    if(!(gm.disableLoop && gm.CurrentLevelIndex >= gm.Config.csvData.Count))
                        ai.gameObject.SetActive(true);

        List<AIDifficulty> diffs = GetAndParseAIInformation();
        List<AI_behavior> behaviors = gm.GetAIBehaviors();
        
        ReturnActiveAIs();

        for(int i = 0; i < activeAIs.Count; i++)
        {
            if(!diffs.Contains(AIDifficulty.TUTORIAL))
                activeAIs[i].SetAI(behaviors.First((x) => x.difficulty_type == diffs[i]));
            else
                activeAIs[i].SetAI(behaviors.First((x) => x.difficulty_type == AIDifficulty.TUTORIAL));
        }

        RandomizeLevels = !diffs.Contains(AIDifficulty.TUTORIAL);

        mng_Location.PrepareTheLevel( RandomizeLevels );
    }

    public void ReturnActiveAIs()
    {
        activeAIs.Clear();

        foreach(var ai in aiPlayers)
            if(ai.gameObject.activeSelf)
                activeAIs.Add(ai);
    }

    public void SkipTheCutscene()
    {
		foreach(var ai in activeAIs)
            ai.SkipToFinishLines(StopLines[0].position.z);

        player.SkipToFinishLines(StopLines[(int)gm.PlayerSuccessRate].position.z);

        FinishConfettis[(int)gm.PlayerSuccessRate].Play();
    }

    public void SkipLevel()
    {
		/*foreach(var ai in activeAIs)
            ai.SkipToFinishLines(StopLines[0].position.z);*/

        gm.PlayerSuccessRate = 2;

        player.SkipToFinishLines(StopLines[(int)gm.PlayerSuccessRate].position.z);
    }

    public List<AIDifficulty> GetAndParseAIInformation()
    {
        string LevelDifficulties = gm.GetLevelDifficulties();

        List<string> ai_difficulties = new List<string>(LevelDifficulties.Split('-'));

        AIDifficulty[] ap = Enum.GetValues(typeof(AIDifficulty)).OfType<AIDifficulty>().ToArray();

        List<AIDifficulty> diffs = new List<AIDifficulty>();

        foreach(var _ai in ai_difficulties)
            diffs.Add(ap.First((x) => x.ToString() == _ai));

        return diffs;
    }

    public int SubscribedTimes { get; set; }

    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"LevelManager: {SubscribedTimes}");

        CustomGameEventList.CutSceneSkipped += SkipTheCutscene;
        //CustomGameEventList.SkipLevel += SkipLevel;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        CustomGameEventList.CutSceneSkipped -= SkipTheCutscene;
        //CustomGameEventList.SkipLevel -= SkipLevel;
    }
}
