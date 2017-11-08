using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WriteParagraphConversationAction : ConversationAction
{
	public string message;
	public string speakingCharacter;
	public float timeBetweenCharacters = 0.01f;

	public TMP_Text dialogTextBox;

	bool hasNextBeenPressed = false;
    bool isStillWriting = true;

	public override IEnumerator Execute()
	{
		dialogTextBox = ScenarioManager.instance.dialogText;

		// Clear text box.
		dialogTextBox.text = "[" + speakingCharacter + "]     ";

		// Do for all characters.
		int	 messageIndex = 0;
		while (messageIndex < message.Length) {
			
			// Wait until time to write character.
			float startTime = Time.time;
			while (Time.time - startTime < timeBetweenCharacters) {
				yield return null;
			}

			// Write character.
			dialogTextBox.text += message[messageIndex];

			// Go to next character.
			++messageIndex;
		}

        isStillWriting = false;

		// Wait for player to press next.
		while (!hasNextBeenPressed) {
			yield return null;
		}
	}

	public override void Command(string cmd)
	{
        if (isStillWriting) return;
		if (cmd == "continue") hasNextBeenPressed = true;
	}
}
