using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : MonoBehaviour
{
	public GameObject buttonPrefab;
	Image backgroundFadeImage;
	GameObject childVerticalLayoutGroup;

	List<OptionButton> buttons = new List<OptionButton>();

	public bool isActive = false;

	public static OptionPanel instance;

	void Awake()
	{
		instance = this;
		backgroundFadeImage = GetComponent<Image>();
		childVerticalLayoutGroup = transform.GetChild(0).gameObject;
	}

	public void ClearOptions()
	{
		for (int i = 0; i < buttons.Count; ++i) {
			Destroy(buttons[i].gameObject);
		}
		buttons.Clear();
	}

	public void AddOption(string label, string nextConversationId, Conversation parentConversation)
	{
		Debug.Log("OptionPanel:: Adding option " + label);
		GameObject btnInst = Instantiate<GameObject>(buttonPrefab, childVerticalLayoutGroup.transform, false);
		OptionButton button = btnInst.GetComponent<OptionButton>();
		button.optionPanel = this;
		button.SetLabel(label);
		button.nextConversation = nextConversationId;
		button.parentConversation = parentConversation;
		buttons.Add(button);
	}

	public void SetActive(bool b)
	{
		isActive = b;
		backgroundFadeImage.enabled = b;
		childVerticalLayoutGroup.SetActive(b);
	}
}
