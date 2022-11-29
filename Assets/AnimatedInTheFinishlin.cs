using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnimatedInTheFinishlin : MonoBehaviour, ISubscribeUnsubscribe
{
    [SerializeField] float[] animationTimeRange;
    [SerializeField] TextMeshProUGUI moneyTextTMP;
    [SerializeField] float scaleMaxSize;

    int playerSuccessRate;

    float pointDistributionTime;
    float animationTime;

    float baseMoneyValue;
    float endMoneyValue;

    bool cutSceneSkipped;

    int fullCycledCount;

	

	public void StartTheAnimation( int successRate, float baseMoney, float finalMoney)
    {
        gameObject.SetActive(true);

        transform.DOKill();

        Subscribe();

        transform.localScale = Vector3.one;

        fullCycledCount = 0;

        playerSuccessRate = successRate;

        cutSceneSkipped = false;

        baseMoneyValue = 0;
        endMoneyValue = finalMoney;

        StopCoroutine(AnimateMoney());

        pointDistributionTime = animationTimeRange[successRate];
        animationTime = playerSuccessRate > 0 ? (pointDistributionTime - 1) / 5f : ( pointDistributionTime - 1 ) / 2f;

        StartCoroutine(AnimateMoney());
    }

    IEnumerator AnimateMoney()
    {
        AnimateTheHolder(animationTime);

        float timeStep = 0.025f;
        float stepCount = pointDistributionTime / timeStep;
        float deltaMoney = (endMoneyValue - baseMoneyValue )/ stepCount ;

        while(baseMoneyValue < endMoneyValue && !cutSceneSkipped)
        {
            baseMoneyValue += deltaMoney;
            moneyTextTMP.text = Mathf.FloorToInt(baseMoneyValue).ToString();

            yield return timeStep;
        }

        transform.DOKill();

        transform.localScale = Vector3.one;

        baseMoneyValue = endMoneyValue;
        moneyTextTMP.text = Mathf.CeilToInt(endMoneyValue).ToString();

        UnSubscribe();
    }

    void AnimateTheHolder(float animationTime)
    {
        transform.DOScale(Vector3.one * scaleMaxSize, animationTime).SetEase(Ease.Linear).OnComplete(delegate ()
        {
            fullCycledCount++;

            if( fullCycledCount <= playerSuccessRate )
            {
                transform.DOScale(Vector3.one, animationTime).SetEase(Ease.Linear).OnComplete(delegate () { AnimateTheHolder(animationTime); });
                return;
            }
            else
            {
                transform.DOScale(Vector3.one, animationTime).SetEase(Ease.Linear);
                return;
			}
        });
    }

    void SkipTheCutscene()
    {
        cutSceneSkipped = true;
    }

    public int SubscribedTimes { get; set; }

    public void Subscribe()
	{
        CustomGameEventList.CutSceneSkipped += SkipTheCutscene;

        ++SubscribedTimes;

        //print( $"AnimatedInTheFinishlin: {SubscribedTimes}" );
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        CustomGameEventList.CutSceneSkipped -= SkipTheCutscene;
    }
}
