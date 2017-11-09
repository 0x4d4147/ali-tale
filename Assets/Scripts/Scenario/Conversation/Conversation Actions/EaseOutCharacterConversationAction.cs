using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseOutCharacterConversationAction : ConversationAction
{
	public string characterAlias;

	public override IEnumerator Execute()
	{
        Debug.LogFormat("Removing character: " + characterAlias);
        CharacterPlacer.instance.RemoveCharacter(characterAlias);
        CharacterPlacer.instance.DisableNameLabelForCharacter(characterAlias);
		yield return null;
	}
}
