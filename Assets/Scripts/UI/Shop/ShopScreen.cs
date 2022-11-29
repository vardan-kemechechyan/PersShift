using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class ShopScreen : UIScreen
{
    [SerializeField] UIManager ui;
    [SerializeField] SectionManager sm;
    [SerializeField] ShopPreviewManager spm;
    [SerializeField] SaveSystemExperimental SSE;
    GameManager gm;

    [Space(10)]

    [SerializeField] GameObject MainDirectionalLight;
    [SerializeField] GameObject PreviewDirectionalLight;

    bool initialized = false;

	public override void Open()
    {
        base.Open();

        if(SSE == null)
            SSE = SaveSystemExperimental.GetInstance();

        if(gm == null)
            gm = GameManager.GetInstance();

        spm.gameObject.SetActive(true);

        ui.EnableCamera(1);

        spm.InitialStart();

        sm.InitialStart();

        //spm.Transition(0);

        MainDirectionalLight.SetActive(false);
        PreviewDirectionalLight.SetActive(true);

        //IronSourceManager.Instance.DestroyAd();
        AdMob.Instance.Hide();
    }

    public override void Close()
    {
        base.Close();

        ui.EnableCamera(0);

        MainDirectionalLight.SetActive(true);
        PreviewDirectionalLight.SetActive(false);
    }

    public void GoToStartScreen()
    {
        if( gm.LevelAlreadyInitialized )
            ui.ShowScreen<StartScreen>();
        else
            gm.InitializeStartScreen();
    }
}
