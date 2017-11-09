using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDialogBackgroundConversationAction : ConversationAction
{
	public Sprite spriteToSet;
	public Image dialogImage;

	public override IEnumerator Execute()
	{
		dialogImage = ScenarioManager.instance.dialogBackground;

		Debug.Log("SetDialogBackgroundConversationAction:: Setting background image.");
		dialogImage.sprite = spriteToSet;
		yield break;
	}
}
