using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation
{
	public string nextConversation = "";

	List<ConversationAction> actionList = new List<ConversationAction>();
	ConversationAction currentAction;

	public IEnumerator Execute()
	{
		// Execute each action sequentially.
		for (int i = 0; i < actionList.Count; ++i) {
			currentAction = actionList[i];
			Debug.LogFormat("Conversation:: executing action " + i);
			yield return currentAction.Execute();
			Debug.LogFormat("Conversation:: ended action " + i);
		}
	}

	public void AddAction(ConversationAction act)
	{
		actionList.Add(act);
	}

	public void Command(string cmd)
	{
		currentAction.Command(cmd);
	}
}

