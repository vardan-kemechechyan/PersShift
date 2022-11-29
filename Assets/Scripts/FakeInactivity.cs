using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FakeInactivity : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> allTextToFade;
    [SerializeField] List<Image> allImagesToFade;
    [SerializeField] List<Button> allButtonToFade;

    [SerializeField] float fadeInValue;
    [SerializeField] float fadeOutValue;

    public void FadeIn( bool _fadeIn )
    {
        float v = _fadeIn ? fadeInValue : fadeOutValue;

        foreach(var img in allImagesToFade)
		{
            Color t = img.color;

            t.a = v;

            img.color = t;
        }

        foreach(var btn in allButtonToFade)
        {
            btn.interactable = !_fadeIn;
		}

        foreach(var txt in allTextToFade)
        {
            Color t = txt.color;

            t.a = v;

            txt.color = t;
        }
    }
}
