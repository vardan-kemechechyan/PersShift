using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftAnimationController : MonoBehaviour
{
    GameManager gm;

	[SerializeField] Image loadingIMage;
	[SerializeField] TextMeshProUGUI progressText;

    float skinProgressValue;

    Coroutine specialEffectCoroutine;

    public void UpdateGiftProgress()
	{
        if(gm == null) gm = GameManager.GetInstance();

        /*if( gm.CurrentLevelIndex <= 1)
        {
            bonusMultiplier.transform.parent.gameObject.SetActive(false);
            claimRewardButton.gameObject.SetActive(false);

            continueTutorialButton.gameObject.SetActive(true);

            tutorialCompleteText.gameObject.SetActive(true);
            money.SetActive(false);
        }
        else
        {*/
            //bonusMultiplier.transform.parent.gameObject.SetActive(true);
            //claimRewardButton.gameObject.SetActive(true);

            //silhouetteOriginalScale = skinSilhouette.transform.localScale;

            float rate = gm.Config.unlockSkinRate;

            int beginningRate = gm.CurrentLevelIndex - 1;

            skinProgressValue = ( beginningRate % rate) / rate;

            if( skinProgressValue != 0f)
                loadingIMage.fillAmount = (beginningRate % rate) / rate - 1 / rate;
                //loadingIMage.fillAmount = (gm.CurrentLevelIndex % rate) / rate - 1/rate;

            UpdateSkinSilhouetteProgress(skinProgressValue);

            //Invoke("ShowNoThanks", 2.0f);

            //continueTutorialButton.gameObject.SetActive(false);

           // tutorialCompleteText.gameObject.SetActive(false);
        //}
    }

    void UpdateSkinSilhouetteProgress(float percentage)
    {
        //var availableSkins = GameManager.Instance.GetAvailableSkins().Length > 0;

        //if(!availableSkins)
            //percentage = 0;

        //foreach(var t in noMoreSkins)
            //t.enabled = !availableSkins;

        //if(availableSkins)
        //{
            //bool showSilhouette = !GameManager.Instance.IsGiftLevel;

            /*if(GameManager.Instance.IsGiftLevel)
            {
                skinSilhouette.gameObject.SetActive(showSilhouette);
                skinProgress.gameObject.SetActive(showSilhouette);
                clueOutline.SetActive(showSilhouette);
            }
            else*/
            //{
                //skinSilhouette.transform.localScale = silhouetteOriginalScale;

                //skinSilhouette.gameObject.SetActive(showSilhouette);
                //skinProgress.gameObject.SetActive(showSilhouette);
                //clueOutline.SetActive(showSilhouette);

                if(loadingIMage.fillAmount == 1) loadingIMage.fillAmount = 0f;

                if(percentage == 0) { percentage = 1; /*clueOutline.SetActive(true);*/ skinProgressValue = 1f; }
                //else clueOutline.SetActive(false);

                progressText.text = $"{percentage * 100}%";
                //SkinFillImage.fillAmount = percentage;

                if(specialEffectCoroutine != null) StopCoroutine(specialEffectCoroutine);

                specialEffectCoroutine = StartCoroutine(SilhouetteSpecialEffect(percentage));
        //}
        /*}
        else
        {
            skinSilhouette.gameObject.SetActive(true);
            skinProgress.text = "";
        }*/
    }

    IEnumerator SilhouetteSpecialEffect(float _newFillPercentage)
    {
        float currentFillAmount = loadingIMage.fillAmount;
        float TargetFillAmount = _newFillPercentage;

        //Vector3 EndScaleSize = silhouetteOriginalScale * 1.2f;

        float fillTime = 0.4f;
        float fillAmountStep = (TargetFillAmount - currentFillAmount) / (fillTime * 100f);
        float yieldTime = fillTime / fillAmountStep;

        //Vector3 VectorStep = 2 * (EndScaleSize - silhouetteOriginalScale) / (fillTime * 100f);

        yield return new WaitForSeconds(0.25f);

        while(currentFillAmount < TargetFillAmount)
        {
            currentFillAmount += fillAmountStep;
            loadingIMage.fillAmount = currentFillAmount;

            //skinSilhouette.transform.localScale += VectorStep;

            //if(skinSilhouette.transform.localScale.x >= EndScaleSize.x) VectorStep *= -1;

            yield return null;
        }

        loadingIMage.fillAmount = TargetFillAmount;
        //skinSilhouette.transform.localScale = silhouetteOriginalScale;
    }

    public float GetSkinProgressValue() { return skinProgressValue; }
}
