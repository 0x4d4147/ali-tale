using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConversationAction
{
	public abstract IEnumerator Execute();

	public virtual void Command(string command)
	{
	}
}
