using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyVisualizationScipt : MonoBehaviour
{
	MoneyManagementSystem money_Manager;
	[SerializeField] TextMeshProUGUI moneyText;

	private void Start()
	{
		money_Manager = GameManager.GetInstance().GetComponent<MoneyManagementSystem>();
		UpdateMoneyCounter(money_Manager.AddMoneyUI(this));
	}

	public void UpdateMoneyCounter(float money) { moneyText.text = money.ToString(); }
}
