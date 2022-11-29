using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class LocationManager : MonoBehaviour
{
    public LevelManager mng_Level;
    GameManager gm;

    [Tooltip( "Vertical placement Correction" )]
    [SerializeField] float Y_position;

    [Tooltip( "This number decides the amount of locations that the level will be contructed of." )]
    [SerializeField] int NumberOfLocationsToGenerate;

    [SerializeField] string levelString;
    [SerializeField] string appearanceString;

    [SerializeField] List<string> locations_string = new List<string>();
    [SerializeField] List<Locations> locations = new List<Locations>();

    [SerializeField] List<string> appearances_string = new List<string>();
    [SerializeField] List<Appearance> appearances = new List<Appearance>();

    [SerializeField] Location StartLocation;
    [SerializeField] Location FinishLocation;

    [SerializeField] List<Location> LocationsForThisLevel = new List<Location>();

    List<Location> FinalGeneratedLevels = new List<Location>();

    public void PrepareTheLevel( bool _randomizeLevels )
    {
        if(gm == null) gm = GameManager.GetInstance();

        NumberOfLocationsToGenerate = gm.GetNumberOfLocationsToGenerate();

        levelString = gm.GetLocationsToGenerate();

        appearanceString = gm.GetClothTypes();

        ReadLevelDataFromString( levelString, appearanceString);

        RandomizeTheLevelsInTheList( _randomizeLevels );

        ConstructLevelBasedOnLocations();
    }

    void ConstructLevelBasedOnLocations()
    {
		for ( int i = 0; i < FinalGeneratedLevels.Count; i++ )
		{
            if(i == 0)                                                                      FinalGeneratedLevels[i].EnableLocationVariant(0);
            else if(FinalGeneratedLevels[i].GetLocationType() != Enums.Locations.FINISH)    FinalGeneratedLevels[i].EnableLocationVariant();

            FinalGeneratedLevels[i].LocationIndexAfterStart = ( i == 0 || FinalGeneratedLevels[i].GetLocationType() == Enums.Locations.FINISH) ? -1 : i;

            FinalGeneratedLevels[ i ].transform.position = i == 0 ? new Vector3(0f, Y_position, 0f ) : NewPosition( FinalGeneratedLevels[ i - 1] );

            if ( FinalGeneratedLevels[ i ].GetLocationType() == Enums.Locations.FINISH )
                mng_Level.finish = FinalGeneratedLevels[ i ].transform.position;

            FinalGeneratedLevels[ i ].gameObject.SetActive( true );
        }
	}

    List<(Appearance, string)> appearaneLocationNamePair = new List<(Appearance, string)>();

    void ReadLevelDataFromString( string _levelDescription, string _appearancesDescription )
    {
        locations.Clear();
        appearances.Clear();

        _levelDescription = _levelDescription.Replace("\\", "-");

        locations_string = new List<string>(_levelDescription.Split('-'));

        Locations[] l = Enum.GetValues(typeof(Locations)).OfType<Locations>().ToArray();

        foreach(var _location in locations_string)
            locations.Add(l.First((x) => x.ToString() == _location.Substring(0, _location.Length-3)));
        /////////////////////////////
        _appearancesDescription = _appearancesDescription.Replace("\\", "-");

        appearances_string = new List<string>(_appearancesDescription.Split('-'));

        Appearance[] ap = Enum.GetValues(typeof(Appearance)).OfType<Appearance>().ToArray();

        foreach(var _appearance in appearances_string)
            appearances.Add(ap.First((x) => x.ToString() == _appearance));
    }

    public Appearance FirstAppearingLocation;

    void RandomizeTheLevelsInTheList( bool _randomizeLevels ) 
    {
        //Create list of paired values (dress type, Location Name)

        if( _randomizeLevels )
        {
            int _randomIndex= -1;

            string lastLocationLoaded = "";

            Extensions.Shuffle(locations_string);
            List<string> newLocationString = new List<string>(locations_string);

            Dictionary<Appearance, List<Location>> LocationsBasedOnType = new Dictionary<Appearance, List<Location>>();

            for(int i = 0; i < NumberOfLocationsToGenerate; i++)
            {
                if(newLocationString.Count == 0)
                {
                    int minimumLength = 999;
                    KeyValuePair<Appearance, List<Location>> locType = new KeyValuePair<Appearance, List<Location>>();

					foreach(var keyValuePair in LocationsBasedOnType)
					{
                        if( minimumLength >= keyValuePair.Value.Count )
                        {
                            minimumLength = keyValuePair.Value.Count;
                            locType = keyValuePair;
                        }
					}

                    string notRepeatedLocation = locations_string.First((x) => locType.Value.Any( loc => loc.gameObject.name == x));

                    LocationsForThisLevel.Add(PoolingScript.GetInstance().Pull(notRepeatedLocation).GetComponent<Location>());

                    LocationsBasedOnType[LocationsForThisLevel.Last().GetLocationDressTypes()[0]].Add(LocationsForThisLevel.Last());

                    lastLocationLoaded = notRepeatedLocation;

                    Extensions.Shuffle(locations_string);
                }
                else
                {
                    //if(newLocationString.Count == locations_string.Count)
                    //{
                        _randomIndex = UnityEngine.Random.Range(0, newLocationString.Count);
                        lastLocationLoaded = newLocationString[_randomIndex];
					//}
                    //else
                    //{
                        //lastLocationLoaded = newLocationString.First(x => LocationsForThisLevel.Last().GetLocationDressTypes()[0] != LocationsBasedOnType.FirstOrDefault(locArr => locArr.Value.FirstOrDefault(loc => loc.gameObject.name == x)).Key);
                        //_randomIndex = newLocationString.IndexOf(lastLocationLoaded);
                    //}

                    LocationsForThisLevel.Add(PoolingScript.GetInstance().Pull(lastLocationLoaded).GetComponent<Location>());

                    newLocationString.RemoveAt(_randomIndex);

                    if(LocationsBasedOnType.ContainsKey(LocationsForThisLevel.Last().GetLocationDressTypes()[0]))
                        LocationsBasedOnType[LocationsForThisLevel.Last().GetLocationDressTypes()[0]].Add(LocationsForThisLevel.Last());
                    else
                        LocationsBasedOnType.Add(LocationsForThisLevel.Last().GetLocationDressTypes()[0], new List<Location>() { LocationsForThisLevel.Last() });
                }
            }

            int numberOfOccurences = -1;

            string newlocationstr = "";

            while(numberOfOccurences != 0)
            {
                numberOfOccurences = 0;

                for(int i = 0; i < LocationsForThisLevel.Count; i++)
			    {
                    if(i != 0 && i != LocationsForThisLevel.Count - 1 )
                    {
                        if( LocationsForThisLevel[i].GetLocationDressTypes()[0] == LocationsForThisLevel[i - 1].GetLocationDressTypes()[0] )
                        {
                            numberOfOccurences++;
                            Extensions.Move<Location>(LocationsForThisLevel, i, i + 1);
						}
                        else if(LocationsForThisLevel[i].GetLocationDressTypes()[0] == LocationsForThisLevel[i + 1].GetLocationDressTypes()[0])
                        {
                            numberOfOccurences++;
                            Extensions.Move<Location>(LocationsForThisLevel, i, i - 1);
						}
                    }
                    
                    newlocationstr += LocationsForThisLevel[i].gameObject.name + "/";
			    }

                print(numberOfOccurences + " : " + newlocationstr);
                newlocationstr = "";
            }
		}
        else
        {
            foreach(var _location in locations_string)
                LocationsForThisLevel.Add(PoolingScript.GetInstance().Pull(_location).GetComponent<Location>());
        }

        FirstAppearingLocation = LocationsForThisLevel[0].GetLocationDressTypes()[0];

        FinalGeneratedLevels.Add( StartLocation );
        FinalGeneratedLevels.AddRange( LocationsForThisLevel );
        FinalGeneratedLevels.Add( FinishLocation );
    }



    public List<Appearance> ReturnLevelAppearanceList() { return appearances; }

    Vector3 NewPosition( Location PreviouseBlock, Location CurrentBlock )
    {
        Vector3 PosA = new Vector3( PreviouseBlock.transform.position.x, Y_position, PreviouseBlock.transform.position.z );
        Vector3 PosB = new Vector3( PosA.x, Y_position, PosA.z + ( PreviouseBlock.ReturnPlaneLocalScale().z + CurrentBlock.ReturnPlaneLocalScale().z ) / 2f );

        return PosB;
    }

    Vector3 NewPosition( Location PreviouseBlock )
    {
        return PreviouseBlock.EndPositionCoordinate();
    }
}
