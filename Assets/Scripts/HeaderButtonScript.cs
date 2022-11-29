using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderButtonScript : MonoBehaviour
{
    [SerializeField] float alphaValueEnabled;
    [SerializeField] float alphaValueDisabled;

    [SerializeField] Image icon;

    public void SelectedScreen( bool _enabledStatus )
    {
        Color tempColor = icon.color;
        tempColor.a = _enabledStatus ? alphaValueEnabled : alphaValueDisabled;

        icon.color = tempColor;
    }
}
