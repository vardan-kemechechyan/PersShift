using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Configuration;

public class LevelProgressionManager : MonoBehaviour, ISubscribeUnsubscribe
{
	[SerializeField] GameManager GM_Manager;
	[SerializeField] UIManager UI_Manager;

	[ SerializeField] List<LevelManager> Levels;

	LevelManager CurrentLevel;

	int currentLevelIndex = 0;

	private void Start()
	{
		Subscribe();
	}

	private void OnDestroy()
	{
		UnSubscribe();
	}

	private void OnDisable()
	{
		UnSubscribe();
	}

	public void LoadCurrentLevel()
	{
		if( CurrentLevel == null ) 
			CurrentLevel = Instantiate( Levels[ currentLevelIndex ], transform);

		CurrentLevel.Load();
	}

	public Character GetPlayer()
	{
		return CurrentLevel.player;
	}

	public List<Character> GetAIs()
	{
		return CurrentLevel.activeAIs;
	}

	public Vector3 GetFinishTransform()
	{
		return CurrentLevel.finish;
	}

	public void DestroyLevel()
	{
		Destroy(CurrentLevel.gameObject);
	}

	void ChangeLevel( bool _levelCompleteStatus )
	{
		//currentLevelIndex = _levelCompleteStatus ? currentLevelIndex + 1 : currentLevelIndex;
	}

	public int SubscribedTimes { get; set; }

	public void Subscribe()
	{
		++SubscribedTimes;

		//print($"LevelProgressionManager: {SubscribedTimes}");

		CustomGameEventList.PlayerCrossedTheFinishLine += ChangeLevel;
	}

	public void UnSubscribe()
	{
		SubscribedTimes--;

		SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

		CustomGameEventList.PlayerCrossedTheFinishLine -= ChangeLevel;
	}
}
