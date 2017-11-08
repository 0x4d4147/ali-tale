using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacterConversationAction : ConversationAction
{
	public string characterName = "";

	public override IEnumerator Execute()
	{
		Debug.LogFormat("SwitchCharacterConversationAction: TODO make character {0} active.", characterName);
		yield break;
	}
}
