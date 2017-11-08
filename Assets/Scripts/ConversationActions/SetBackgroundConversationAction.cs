using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBackgroundConversationAction : ConversationAction
{
	public Sprite spriteToSet;
	public Image backgroundImage;

	public override IEnumerator Execute()
	{
		backgroundImage = ScenarioManager.instance.backgroundImage;

		Debug.Log("SetBackgroundConversationAction:: Setting background image.");
		backgroundImage.sprite = spriteToSet;
		yield break;
	}
}
