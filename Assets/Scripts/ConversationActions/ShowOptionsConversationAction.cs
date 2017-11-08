using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOptionsConversationAction : ConversationAction
{
	public OptionPanel optionPanel;

	List<Option> options = new List<Option>();

	struct Option
	{
		public string label;
		public string nextConversationId;
		public Conversation parentConversation;
	}

	public void AddOption(string label, string nextConversationId, Conversation parentConversation)
	{
		Option o = new Option();
		o.label = label;
		o.nextConversationId = nextConversationId;
		o.parentConversation = parentConversation;
		options.Add(o);
	}

	public override IEnumerator Execute()
	{
		Debug.LogFormat("ShowOptionsConversationAction:: Executing options.");
		OptionPanel.instance.ClearOptions();

		for (int i = 0; i < options.Count; ++i) {
			OptionPanel.instance.AddOption(options[i].label, options[i].nextConversationId, options[i].parentConversation);
		}

		OptionPanel.instance.SetActive(true);

		while (OptionPanel.instance.isActive) {
			yield return null;
		}

		Debug.LogFormat("ShowOptionsConversationAction:: done.");
	}
}
