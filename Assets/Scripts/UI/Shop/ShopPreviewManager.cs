using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class ShopPreviewManager : MonoBehaviour, ISubscribeUnsubscribe
{
    public static Action ChangeTheModel;
    public static Action<Customizable_SO, bool> ChangeAccessory = delegate (Customizable_SO item, bool deselected) { };
    public static Action<CustomizableItems, bool> AccessoryCameraChange = delegate (CustomizableItems _itemType, bool deselected) { };

    [SerializeField] Transform modelHolder;
    [SerializeField] AccessoruManagement model;
    [SerializeField] List<InCharacterCameraTransitions> characterTransitions;
    [SerializeField] Camera shopCamera;
    public CharacterBuild previewBuild = new CharacterBuild();
    [SerializeField] SaveSystemExperimental SSE;

	private void Start()
    { 
        Subscribe();
    }

    private void OnDestroy() 
    { 
        UnSubscribe();
    }

	public void InitialStart()
    {
        if(SSE == null) SSE = SaveSystemExperimental.GetInstance();

        previewBuild = SSE.ReturnCopyOfBuild( null );

        string name = previewBuild.CustomComponents.First((x) => x.highlight == HighLight.HIGHLIGHTED && x.shopSection == ShopSection.Costume).itemName;

        InstantiateTheModel(name);
    }

    public void UpdateModel()
    {
        previewBuild = new CharacterBuild();

        foreach(var section in SSE.RuntimeList)
        {
            foreach(var itemType in section.Value)
            {
                foreach(var item in itemType.Value)
                {
                    if(item.highlight == HighLight.HIGHLIGHTED)
                    {
                        previewBuild.CustomComponents.Add(item);
                    }
                }
            }
        }

        previewBuild.HairColor = SSE.GetHairColor();

        if(previewBuild.CustomComponents.Any((x) => x.highlight == HighLight.HIGHLIGHTED && x.shopSection == ShopSection.Costume))
            InstantiateTheModel(previewBuild.CustomComponents.First((x) => x.highlight == HighLight.HIGHLIGHTED && x.shopSection == ShopSection.Costume).itemName);
    }
    public void InstantiateTheModel( string name)
    {
        if(model != null)
            Destroy(model.gameObject);

        model = Instantiate( Resources.Load<GameObject>("Costume/"+ name), modelHolder).GetComponent<AccessoruManagement>();
        model.gameObject.SetActive(true);

        UpdateTheModelWithAccessorie();

        ChangePlayerSkinColor(previewBuild.HairColor);
    }
    public void UpdateTheModelWithAccessorie( )
    {
        foreach(var item in previewBuild.CustomComponents)
            if(item.shopSection != ShopSection.Costume)
                model.InstantiateAccessory(item.itemTypeToString, item.skinTypeToString);
    }

   /* public void UpdateAccessory( Customizable_SO item, bool deselected = false )
    {
        Customizable_SO t;

        bool match = false;

        foreach(var buildItem in previewBuild.CustomComponents)
		{
            if(buildItem.itemName == item.itemName && item.shopSection != ShopSection.Accessory) return;

            if(buildItem.itemTypeToString == item.itemTypeToString)
            {
                buildItem.lockInfo.lockStatus = buildItem.lockInfo.priceToUnlock > 0 ? LockStatus.LOCKED : LockStatus.UNLOCKED;

                //SaveSystemExperimental.GetInstance().GetItemFromRuntimeSOList(buildItem.shopSection, buildItem.itemTypeToString, buildItem.itemName).lockInfo.purchaseStatus = buildItem.lockInfo.priceToUnlock > 0 ? CusomizableItemPurchaseStatus.LOCKED : CusomizableItemPurchaseStatus.UNLOCKED;

                match = true;

                break;
            }
		}

        if((match || deselected ))
        {
            if(previewBuild.CustomComponents.Any(x => x.lockInfo.lockStatus != LockStatus.SELECTED && x.lockInfo.lockStatus != LockStatus.SELECTED_AS_WELL))
            { 
                model.RemoveAccessory(item.itemTypeToString);
                previewBuild.CustomComponents.Remove(previewBuild.CustomComponents.First( x => ( x.lockInfo.lockStatus != LockStatus.SELECTED && x.lockInfo.lockStatus != LockStatus.SELECTED_AS_WELL)));
            }

            if(deselected) return;
        }

        previewBuild.CustomComponents.Add(item);

        if(item.shopSection != ShopSection.Costume)
            model.InstantiateAccessory(item.itemTypeToString, item.skinTypeToString);

        ChangePlayerSkinColor(previewBuild.HairColor);
    }
    */

    public void Transition( int _transitionType )
    {
        InShopViewTransitions transType = (InShopViewTransitions)_transitionType;

        InCharacterCameraTransitions NewView = characterTransitions.First((t) => t.transitionType == transType);

        Transform newTrans = NewView.modelTargetTransition;

        AnimateModel(newTrans, NewView);
    }
    public void Transition(CustomizableItems _transitionType, bool selected)
    {
        InCharacterCameraTransitions NewView;

        if(selected)
            NewView = characterTransitions.First((t) => t.accessoryTransitionType == _transitionType);
        else
            NewView = characterTransitions[1];

        Transform newTrans = NewView.modelTargetTransition;

        AnimateModel(newTrans, NewView);
    }
    public void AnimateModel(Transform newTrans, InCharacterCameraTransitions NewView)
    {
        modelHolder.DOScale(newTrans.localScale, NewView.animationTransitionTime).OnComplete(delegate ()
        {
            modelHolder.localScale = newTrans.localScale;
        });

        modelHolder.DOLocalRotate(newTrans.localRotation.eulerAngles, NewView.animationTransitionTime).OnComplete(delegate ()
        {
            modelHolder.localRotation = newTrans.localRotation;
        });

        modelHolder.DOMove(newTrans.position, NewView.animationTransitionTime).OnComplete(delegate ()
        {
            modelHolder.position = newTrans.position;
        });

        shopCamera.DOOrthoSize(NewView.cameraize, NewView.animationTransitionTime).OnComplete(delegate ()
        {
            shopCamera.orthographicSize = NewView.cameraize;
        });
    }

    public void ChangePlayerSkinColor(string hex_value)
    {
        string hex_with_hesh = "#" + hex_value;

        model.ApplyDye(hex_with_hesh);
        previewBuild.HairColor = hex_value;
        SSE.SaveHairColor( hex_value );
    }

    public int SubscribedTimes { get; set; }

    public void Subscribe()
	{
        ++SubscribedTimes;

        //print($"ShopPreviewManager: {SubscribedTimes}");

        ChangeTheModel += UpdateModel;
        AccessoryCameraChange += Transition;
    }

	public void UnSubscribe()
	{
        SubscribedTimes--;

        SubscribedTimes = Mathf.Clamp(SubscribedTimes, 0, 99999);

        ChangeTheModel -= UpdateModel;
        AccessoryCameraChange -= Transition;
    }
}

[System.Serializable]
public struct InCharacterCameraTransitions
{
    [SerializeField] public InShopViewTransitions transitionType;
    [SerializeField] public CustomizableItems accessoryTransitionType;
    public Transform modelTargetTransition;
    public float cameraize;
    public float animationTransitionTime;
}
