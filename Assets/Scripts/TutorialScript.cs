using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
	[SerializeField] Appearance appearanceForTutorial;
	[SerializeField] Transform startPosition;
	[SerializeField] Transform endPosition;

	[SerializeField] Location attachedLocation;

	[SerializeField] bool isTutorial;

	private void Start()
	{
		attachedLocation = GetComponent<Location>();
		appearanceForTutorial = attachedLocation.GetLocationDressTypes()[0];

		for(int i = 0; i < transform.childCount; i++)
		{
			if(transform.GetChild(i).name == "Start")	
				startPosition = transform.GetChild(i);
			if(transform.GetChild(i).name == "End")		
				endPosition = transform.GetChild(i);
		}
	}

	public List<Vector3> ReturnStartEndPositions() 
	{
		return new List<Vector3>() { startPosition.position, endPosition.position };
	}
}
