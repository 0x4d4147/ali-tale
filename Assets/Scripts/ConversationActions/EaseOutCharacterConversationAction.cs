using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseOutCharacterConversationAction : ConversationAction
{
	public string characterName;

	public override IEnumerator Execute()
	{
		Debug.LogFormat("Removing character: " + characterName);
        CharacterPlacer.instance.RemoveCharacter(characterName);

		// TODO Position off screen.
		// TODO Ease in character.
		yield return null;
	}
}
