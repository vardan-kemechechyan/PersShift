using UnityEngine;
using UnityEngine.UI;
using UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class StartScreen : UIScreen
{
    GameManager gm;
    SaveSystemExperimental sse;

    [SerializeField] UIManager ui;
    [SerializeField] ProgressMapManager progressBarManager;
    [SerializeField] GameObject TapToStart;

    int unlockSkinRate = 0;

    public override void Open()
    {
        base.Open();

        

        if(gm == null) gm = GameManager.GetInstance();
        if(sse == null) sse = SaveSystemExperimental.GetInstance();

        int currentLevelIndex = sse.GetCurrentLevel();

        if(unlockSkinRate == 0)
            unlockSkinRate = gm.GetUnlockSkinRate();

        progressBarManager.transform.parent.gameObject.SetActive( currentLevelIndex != 0 );
        
        if( currentLevelIndex != 0 )
        {
            int beginningNumber = ( currentLevelIndex  - currentLevelIndex % unlockSkinRate ) + 1;

            if( currentLevelIndex % unlockSkinRate == 0 ) beginningNumber = currentLevelIndex - ( unlockSkinRate - 1 );

            if(gm.disableLoop && currentLevelIndex >= gm.Config.csvData.Count)
            {
                TapToStart.SetActive(false);

                //currentLevelIndex = gm.Config.csvData.Count - 1;
                //beginningNumber = currentLevelIndex;
            }

            progressBarManager.SetLevelNumberInCircles( beginningNumber, currentLevelIndex, ( gm.disableLoop && currentLevelIndex >= gm.Config.csvData.Count ));
		}
    }

    public void OpenShop()
    {
        AnalyticEvents.ReportEvent("shop_open");

        ui.ShowScreen<ShopScreen>();
    }
}
