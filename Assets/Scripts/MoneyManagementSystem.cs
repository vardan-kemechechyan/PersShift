using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManagementSystem : MonoBehaviour
{
	[SerializeField] float money = 0;

	[SerializeField] List<MoneyVisualizationScipt> AllMoneyUI = new List<MoneyVisualizationScipt>();

	SaveSystemExperimental sse;
	public float Money
	{
		get => money;
		set
		{
			money = value;

			foreach(var mny_UI in AllMoneyUI)
				mny_UI?.UpdateMoneyCounter(money);
		}
	}

	private void Start()
	{
		if(sse == null) sse = SaveSystemExperimental.GetInstance();

		Money = sse.GetMoney();
	}

	public float AddMoneyUI(MoneyVisualizationScipt _moneyUI) { AllMoneyUI.Add(_moneyUI); return Money; }
}
