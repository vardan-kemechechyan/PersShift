using UnityEngine;
using UnityEngine.UI;
using UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class EndLevelScreen : UIScreen
{
    GameManager gameManager;


    public override void Open()
    {
        base.Open();

        if(gameManager == null)
        {
            gameManager = GameManager.GetInstance();
        }
    }

    public void Restart()
    {
        AdMob.Instance.Show("Interstitial_restart", success =>
        {
            Invoke("SkipLevel", 0.25f);
        });

        /*IronSourceManager.Instance.ShowInterstitial("Interstitial_restart", success =>
        {
            AnalyticEvents.ReportEvent("restart_interstitial");
            Invoke("SkipLevel", 0.25f);
        });*/
    }

    void SkipLevel()
    {
        gameManager.LoadLevel();
    }
}