using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterPosition = CharacterPlacer.CharacterPosition;

public class EaseInCharacterConversationAction : ConversationAction
{
	public AnimationCurve easeInCurve;
	public float easeInSpeed;
	public string characterName;
    public string characterAlias;
    public CharacterPosition rootPosition;
	public string startPose;

    // Dumb hacks.
    public Vector3 leftOffset = new Vector3(200, 0, 0);
    public Vector3 rightOffset = new Vector3(-200, 0, 0);

	public override IEnumerator Execute()
	{
        CharacterPlacer placer = CharacterPlacer.instance;
        placer.AddCharacter(characterName, startPose, characterAlias);
        SetCharacterOffscreen();
        placer.SetCharacterImageEnabled(characterAlias, true);
        placer.SetNameLabelForCharacter(characterAlias, rootPosition);
        yield return placer.AnimateCharacterTo(characterAlias, rootPosition, "in-out", GetDefaultOffsetForPosition(), 0.9f);
	}

    void SetCharacterOffscreen()
    {
        CharacterPlacer placer = CharacterPlacer.instance;

        switch (rootPosition)
        {
            case CharacterPosition.Left:
                {
                    placer.SetCharacterOffscreenLeft(characterAlias);
                    break;
                }
            case CharacterPosition.Right:
                {
                    placer.SetCharacterOffscreenRight(characterAlias);
                    break;
                }
        }
    }

    // Dumb hack...
    Vector3 GetDefaultOffsetForPosition()
    {
        switch (rootPosition)
        {
            case CharacterPosition.Left:
                {
                    return leftOffset;
                }
            case CharacterPosition.Right:
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
                    rootPosition = CharacterPosition.Left;
    				break;
    			}
    		case "center":
    			{
                    rootPosition = CharacterPosition.Center;
    				break;
    			}
    		case "right":
    			{
                    rootPosition = CharacterPosition.Right;
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
