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

    // Dumb hacks.
    public Vector3 leftOffset = new Vector3(200, 0, 0);
    public Vector3 rightOffset = new Vector3(-200, 0, 0);

	public override IEnumerator Execute()
	{
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

        placer.SetCharacterImageEnabled(characterName, true);

        yield return placer.AnimateCharacterTo(characterName, ToCharacterPosition(rootPosition), "in-out", GetStandardOffset(rootPosition), 0.9f);
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
        }
        // Bad default, but whatever.
        return CharacterPlacer.CharacterPosition.Center;
    }

    // Dumb hack...
    public Vector3 GetStandardOffset(CharacterManager.RootPosition pos)
    {
        switch (rootPosition)
        {
            case CharacterManager.RootPosition.Left:
                {
                    return leftOffset;
                }
            case CharacterManager.RootPosition.Right:
                {
                    return rightOffset;
                }
        }

        return Vector3.zero;
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
