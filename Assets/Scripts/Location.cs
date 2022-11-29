using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Location : MonoBehaviour
{
    Transform planeTransorm;

    [SerializeField] Locations location;
    [SerializeField] List<Appearance> appearancesForLocation;

    [SerializeField] List<GameObject> locationVariants;

    [SerializeField] Transform startPosition;

    [SerializeField] Transform endPosition;

    int locationIndexAfterStart = 0;

    public int LocationIndexAfterStart { get => locationIndexAfterStart; set { locationIndexAfterStart = value; } }


    private void OnEnable()
	{
        CustomGameEventList.OnChangeGameState += OnChangeGameState;
    }

	private void OnDestroy()
	{
        CustomGameEventList.OnChangeGameState -= OnChangeGameState;
    }

	private void OnDisable()
	{
        CustomGameEventList.OnChangeGameState -= OnChangeGameState;
    }

	public void EnableLocationVariant(int index = -1) //-1 - Random, 0 - small, 1 - medium, 2 - large
    {
        if(index == -1)
            index = Random.Range(0, locationVariants.Count);

        foreach(var loc in locationVariants)
            loc?.SetActive(false);
        
        if( locationVariants.Count != 0 )
        {
            locationVariants[index].SetActive(true);
            startPosition = locationVariants[index].transform.Find("Start");
            endPosition = locationVariants[index].transform.Find("End");
		}
        else
        {
            startPosition = transform.Find("Start");
            endPosition = transform.Find("End");
        }
    }

    public Vector3 StartPositionCoordinate() { return startPosition.position; }
    public Vector3 EndPositionCoordinate() { return endPosition.position; }

    public Locations GetLocationType() { return location; }
    public List<Appearance> GetLocationDressTypes() { return appearancesForLocation; }

    public Vector3 ReturnPlaneLocalScale() { return planeTransorm.localScale; }

    void OnChangeGameState( GameState _newState )
    {
       if(_newState == GameState.LoadLevel && ( gameObject.name != "Start" && gameObject.name != "Finish"))
       {
            //print("Trying to return to pool the object " + gameObject.name);
            PoolingScript.GetInstance().ReturnToPool( gameObject );
	   }
    }
}
