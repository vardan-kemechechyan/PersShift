using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private float _hudRefreshRate = 0.5f;

    private float _timer;

    private void Update()
    {
        if(Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = $"{fps} FPS";
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}
