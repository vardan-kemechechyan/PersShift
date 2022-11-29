using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AccessoruManagement : MonoBehaviour
{
	[SerializeField] List<TransformAccessoryPair> AllAccessorySkeletonTransforms;
	[SerializeField] SkinnedMeshRenderer FaceMaterial;
	[SerializeField] Dictionary<string, GameObject> InstantiatedAccessories = new Dictionary<string, GameObject>();

	List<(string, string )> AccessoriesToWear = new List<(string _itemType, string skinType)>();

	public void LoadAccessory(List<(string, string)> _accessoriesToWear)
	{
		AccessoriesToWear = _accessoriesToWear;

		foreach(var acc in AccessoriesToWear)
			InstantiateAccessory(acc.Item1, acc.Item2);
	}

	public void InstantiateAccessory( string _itemType, string skinType)
	{
		string itemType = _itemType.ToString();

		if(_itemType != CustomizableItems.Face.ToString())
		{
			var acc = Resources.Load<GameObject>(itemType + "/" + itemType + "_" + skinType);
			acc.SetActive(true);

			if(InstantiatedAccessories.ContainsKey(_itemType.ToString()))
			{
				DestroyImmediate(InstantiatedAccessories[_itemType.ToString()]);
				InstantiatedAccessories.Remove(_itemType.ToString());
			}

			InstantiatedAccessories.Add(_itemType.ToString(), Instantiate(acc, AllAccessorySkeletonTransforms.First((x) => x.accessoryType.ToString() == _itemType).accessoryHolder));
		}
		else
		{
			ApplyMakeUp(Resources.Load<Material>(itemType + "/" + itemType + "_" + skinType));
		}
	}

	public void RemoveAccessory( string _itemType )
	{
		if(InstantiatedAccessories.ContainsKey(_itemType))
		{
			DestroyImmediate(InstantiatedAccessories[_itemType]);
			InstantiatedAccessories.Remove(_itemType);
		}
	}

	public void ApplyMakeUp( Material _newFace )
	{
		Material[] intMaterials = new Material[FaceMaterial.materials.Length];
		for(int i = 0; i < intMaterials.Length; i++)
		{
			string n = FaceMaterial.materials[i].name.Split('_')[0];

			if(n == "Head" || n == "Face")
				intMaterials[i] = _newFace;
			else
				intMaterials[i] = FaceMaterial.materials[i];
		}

		FaceMaterial.materials = intMaterials;
	}

	public void ApplyDye( string _hexValue )
	{
		Color color;

		if(ColorUtility.TryParseHtmlString(_hexValue, out color))
		{
			GameObject Hair = null;

			InstantiatedAccessories.TryGetValue(CustomizableItems.Hairstyle.ToString(), out Hair); 

			if(Hair != null )
				Hair.GetComponent<MeshRenderer>().material.color = color;
		}
	}
}

[System.Serializable]
public struct TransformAccessoryPair
{
	public CustomizableItems accessoryType;
	public Transform accessoryHolder;
}
