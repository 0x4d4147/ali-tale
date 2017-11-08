using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacterPoseConversationAction : ConversationAction
{
	public string characterName;
	public string characterPose;

	public override IEnumerator Execute()
	{
		CharacterManager.instance.LoadPoseForExistingCharacter(characterName, characterPose);
		Debug.LogFormat("ChangeCharacterPoseConversationAction:: Attempted to load pose {0} for {1}.", characterPose, characterName);
		yield break;
	}
}
