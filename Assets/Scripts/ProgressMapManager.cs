using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMapManager : MonoBehaviour
{
    List<ProgressCircle> ProgressCircles = new List<ProgressCircle>();

    [SerializeField] ColorSwap levelCompleteColor;
    [SerializeField] ColorSwap levelNotCompleteLevel;
    [SerializeField] ColorSwap currentLevelColor;

    public void SetLevelNumberInCircles( int _startingLevelNumber, int _currentLevelNumber, bool _lastLevelOWon = false )
    {
        //transform.parent.gameObject.SetActive(_startingLevelNumber != 0);

        if( ProgressCircles == null || ProgressCircles.Count == 0)
            for(int i = 0; i < transform.childCount; i++)
                ProgressCircles.Add(transform.GetChild(i).GetComponent<ProgressCircle>());

        for(int i = 0; i < ProgressCircles.Count; i++)
		{
            ProgressCircles[i].SetLevel( _startingLevelNumber, _startingLevelNumber <_currentLevelNumber);

            if (_startingLevelNumber == _currentLevelNumber )
                ProgressCircles[i].SetColor(currentLevelColor);
            else
                ProgressCircles[i].SetColor(_startingLevelNumber <= _currentLevelNumber ? levelCompleteColor : levelNotCompleteLevel );
            
                _startingLevelNumber++;

            if(_lastLevelOWon )
            {
                ProgressCircles[i].SetColor(levelCompleteColor);
                ProgressCircles[i].SetLevel(_startingLevelNumber, true);
            }    
        }

        /*foreach(var circle in ProgressCircles)
            circle.SetLevel( _startingLevelNumber++ );*/
	}
}

[System.Serializable]
public struct ColorSwap
{
    public Color FillImage;
    public Color BottomImage;
}