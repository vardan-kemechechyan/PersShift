using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCircle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelInfo;
    [SerializeField] Image checkMark;

    [SerializeField] Image FillImage;
    [SerializeField] Image Outline;

    public void SetLevel( int _levelNumberToPrint, bool _completed ) 
    {
        //_levelNumberToPrint = Mathf.Clamp(_levelNumberToPrint, 1, 100000);

        //_levelNumberToPrint++;

        levelInfo.text = _levelNumberToPrint.ToString();

        checkMark.gameObject.SetActive( _completed );
        levelInfo.gameObject.SetActive( !_completed );
    }

    public void SetColor(ColorSwap _colorToSwitch)
    {
        FillImage.color = _colorToSwitch.FillImage;
        Outline.color = _colorToSwitch.BottomImage;
    }
}
