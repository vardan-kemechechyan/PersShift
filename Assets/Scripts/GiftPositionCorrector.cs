using Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftPositionCorrector : MonoBehaviour
{
    [SerializeField] List<TransformType> transforms;

    SaveSystemExperimental sse;

    public void PositionTheItem( Customizable_SO openedItem )
    {
        if(sse == null) sse = SaveSystemExperimental.GetInstance();

        string itemName = openedItem.shopSection.ToString();

        foreach(var holder in transforms)
		{ 
            if(holder.template.childCount != 0)
                Destroy(holder.template.GetChild(0).gameObject);

            holder.template.gameObject.SetActive(false);

            if(holder.item.ToString() == openedItem.itemTypeToString)
            {
                var go = Instantiate(Resources.Load<GameObject>(openedItem.itemTypeToString + "/" + openedItem.itemName), holder.template);
                holder.template.gameObject.SetActive(true);
            }
		}

        if(itemName == ShopSection.Costume.ToString())
        {
            CharacterBuild previewBuild = sse.ReturnCopyOfBuild(openedItem);

            previewBuild.CustomComponents.RemoveAll(x => x.shopSection != ShopSection.Hairstyle);

            previewBuild.CustomComponents.Add(openedItem);

            transforms[9].template.gameObject.SetActive(true);

            var model = Instantiate(Resources.Load<GameObject>(itemName + "/" + openedItem.itemName), transforms[9].template).GetComponent<AccessoruManagement>(); ;

            foreach(var item in previewBuild.CustomComponents)
                if(item.shopSection != ShopSection.Costume)
                    model.InstantiateAccessory(item.itemTypeToString, item.skinTypeToString);

            string hex_with_hesh = "#" + previewBuild.HairColor;

            model.ApplyDye(hex_with_hesh);

            model.gameObject.SetActive(true);

        }
        else if(itemName == ShopSection.Face.ToString())
        {

		}
    }
}

[System.Serializable]
public class TransformType
{
    public CustomizableItems item;
    public Transform template;
}
