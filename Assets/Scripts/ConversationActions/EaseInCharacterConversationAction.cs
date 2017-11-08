using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EaseInCharacterConversationAction : ConversationAction
{
	public AnimationCurve easeInCurve;
	public float easeInSpeed;
	public string characterName;
	public CharacterManager.RootPosition rootPosition;
	public string startPose;

	public override IEnumerator Execute()
	{
		CharacterManager.instance.LoadCharacter(rootPosition, characterName, startPose);
        CharacterPlacer placer = CharacterPlacer.instance;
        placer.AddCharacter(characterName, startPose);

        switch (rootPosition)
        {
            case CharacterManager.RootPosition.Left:
                {
                    placer.SetCharacterOffscreenLeft(characterName);
                    break;
                }
            case CharacterManager.RootPosition.Right:
                {
                    placer.SetCharacterOffscreenRight(characterName);
                    break;
                }
            case CharacterManager.RootPosition.Center:
                {
                    break;
                }
        }

//        placer.SetCharacterImageEnabled(characterName);

        yield return placer.AnimateCharacterTo(characterName, ToCharacterPosition(rootPosition), "linear", new Vector3(0, 0, 0), 1f);
	}

    public CharacterPlacer.CharacterPosition ToCharacterPosition(CharacterManager.RootPosition pos)
    {
        switch (rootPosition)
        {
            case CharacterManager.RootPosition.Left:
                {
                    return CharacterPlacer.CharacterPosition.Left;
                }
            case CharacterManager.RootPosition.Right:
                {
                    return CharacterPlacer.CharacterPosition.Right;
                }
            case CharacterManager.RootPosition.Center:
                {
                    // TODO.
                    break;
                }
        }
        return CharacterPlacer.CharacterPosition.Left;
    }

	public void SetRootPosition(string position)
	{
		switch (position) {
		case "left":
			{
				rootPosition = CharacterManager.RootPosition.Left;
				break;
			}
		case "center":
			{
				rootPosition = CharacterManager.RootPosition.Center;
				break;
			}
		case "right":
			{
				rootPosition = CharacterManager.RootPosition.Right;
				break;
			}
		default:
			{
				Debug.LogFormat("EaseInCharacterConversationAction:: did not understand root position {0}", position);
				break;
			}
		}
	}
}
