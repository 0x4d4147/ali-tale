using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlacer : MonoBehaviour
{
    public static CharacterPlacer instance;

	public struct CharacterMeta
	{
		public CharacterImage charImage;
		public string name;
		public string alias;
	}

	public int referencePixelWidth;
	public int referencePixelHeight;

	RectTransform leftEdgeAnchor;
	RectTransform rightEdgeAnchor;

	RectTransform leftCharacterAnchor;
	RectTransform centerCharacterAnchor;
	RectTransform rightCharacterAnchor;

	int characterImageSize;
	Stack<CharacterMeta> availableCharacterTemplates;
	Dictionary<string, CharacterMeta> usedCharacterTemplates = new Dictionary<string, CharacterMeta>();

    [System.Serializable]
    public struct NamedAnimationCurve
    {
        public string id;
        public AnimationCurve curve;
    }

    public NamedAnimationCurve[] curveDefinitions;
    Dictionary<string, AnimationCurve> curveDict = new Dictionary<string, AnimationCurve>();

	public enum CharacterPosition
	{
		Left,
		Center,
		Right
	}

	void Awake()
	{
        instance = this;

		leftEdgeAnchor = transform.Find("Left Edge Anchor").GetComponent<RectTransform>();
		rightEdgeAnchor = transform.Find("Right Edge Anchor").GetComponent<RectTransform>();

		leftCharacterAnchor = transform.Find("Left Character Anchor").GetComponent<RectTransform>();
		centerCharacterAnchor = transform.Find("Center Character Anchor").GetComponent<RectTransform>();
		rightCharacterAnchor = transform.Find("Right Character Anchor").GetComponent<RectTransform>();

        InitCurveDict();

		characterImageSize = (int)(referencePixelWidth * 0.5f);

		// Collect all the CharacterImage children.
		var chs = GetComponentsInChildren<CharacterImage>();
		availableCharacterTemplates = new Stack<CharacterMeta>(chs.Length);
		for (int i = 0; i < chs.Length; ++i)
		{
			CharacterMeta cm = new CharacterMeta();
			cm.charImage = chs[i];
			availableCharacterTemplates.Push(cm);
		}
	}

    void InitCurveDict()
    {
        for (int i = 0; i < curveDefinitions.Length; ++i)
        {
            curveDict.Add(curveDefinitions[i].id, curveDefinitions[i].curve);
        }
    }

//	void Start()
//	{
//        StartCoroutine(RunAllTests());
//	}

//    IEnumerator RunAllTests()
//    {
//        yield return Test1();
//        //yield return Test2();
//        yield return Test3();
//    }

//    IEnumerator Test1()
//    {
//        yield return new WaitForSeconds(2f);
//        AddCharacter("james", "neutral");
//        SetCharacterOffscreenLeft("james");
//        SetCharacterImageEnabled("james", true);
//        yield return AnimateCharacterTo("james", CharacterPosition.Left, "slow-start", new Vector3(200, 0, 0), 3f);
//        yield return new WaitForSeconds(1f);
//        yield return FlipCharacter("james", "linear", 4f);
//        yield return AnimateCharacterTo("james", CharacterPosition.Right, "linear", new Vector3(-200, 0, 0), 4f);
//        yield return new WaitForSeconds(4f);
//        RemoveCharacter("james");
//    }
//
//    IEnumerator Test2()
//    {
//        yield return new WaitForSeconds(2f);
//        AddCharacter("james", "neutral");
//        SetCharacterOffscreenRight("james");
//        FlipCharacter("james");
//        SetCharacterImageEnabled("james", true);
//        yield return AnimateCharacterTo("james", CharacterPosition.Right, "slow-start", new Vector3(-200, 0, 0), 3f);
//        yield return new WaitForSeconds(1f);
//        FlipCharacter("james");
//        yield return AnimateCharacterTo("james", CharacterPosition.Left, "in-out", new Vector3(-200, 0, 0), 4f);
//        yield return new WaitForSeconds(4f);
//        RemoveCharacter("james");
//    }
//
//    IEnumerator Test3()
//    {
//        AddCharacter("ali", "neutral");
//        SetCharacterPosition("ali", CharacterPosition.Center, new Vector3(216, 0, 0));
//        SetCharacterImageEnabled("ali", true);
//        yield return new WaitForSeconds(3f);
//    }

	public void AddCharacter(string name, string pose)
	{
		AddCharacter(name, pose, name);
	}

	public void AddCharacter(string name, string pose, string alias)
	{
		if (availableCharacterTemplates.Count > 0) {
			var ch = availableCharacterTemplates.Pop();
			ch.name = name;
			ch.alias = alias;
			usedCharacterTemplates.Add(alias, ch);
			LoadPose(alias, pose);
		} else {
			Debug.LogWarningFormat("CharacterPlacer:: Could not add character {0} becase there are no more avaiable templates.", name);
		}
	}

	public void LoadPose(string alias, string pose)
	{
		if (usedCharacterTemplates.ContainsKey(alias))
        {
			var ch = usedCharacterTemplates[alias];
			var posePath = string.Format("Character Poses/{0}/{1}", ch.name, pose);
			ch.charImage.SetSprite(Resources.Load<Sprite>(posePath));
			Debug.Log("CharacterPlacer:: Attempted to load pose at path: " + posePath);
		}
        else
        {
			Debug.LogWarningFormat("CharacterPlacer:: Could not load pose {0} for character {1}, key not in dictionary.", pose, alias);
		}
	}

	public void RemoveCharacter(string alias)
	{
		if (usedCharacterTemplates.ContainsKey(alias))
        {
			var ch = usedCharacterTemplates[alias];
			ch.charImage.SetEnabled(false);
            ResetCharacterScale(alias);
			usedCharacterTemplates.Remove(alias);
            availableCharacterTemplates.Push(ch);
		}
        else
        {
			Debug.LogWarningFormat("CharacterPlacer:: Could not remove character {0}, key not in dictionary.", alias);
		}
	}

    public void ResetCharacterScale(string alias)
    {
        if (usedCharacterTemplates.ContainsKey(alias))
        {
            var charRT = GetCharacterRectTransform(alias);
            var locScale = charRT.localScale;
            if (locScale.x < 0)
            {
                locScale.x *= -1;
            }
            charRT.localScale = locScale;
        }
        else
        {
            Debug.LogWarningFormat("CharacterPlacer:: Could not reset scale for character {0}, key not in dictionary.", alias);
        }
    }

	public void SetCharacterImageEnabled(string alias, bool b)
	{
		if (usedCharacterTemplates.ContainsKey(alias))
        {
			var ch = usedCharacterTemplates[alias];
			ch.charImage.SetEnabled(b);
		}
        else
        {
			Debug.LogWarningFormat("CharacterPlacer:: Could not chage image state for character {0}, key not in dictionary.", alias);
		}
	}

	public void SetCharacterOffscreenLeft(string alias)
	{
		if (usedCharacterTemplates.ContainsKey(alias))
        {
			var ch = usedCharacterTemplates[alias];
			var ci = ch.charImage;
			var leftPos = leftEdgeAnchor.localPosition;
			leftPos.x -= ci.GetWidth() * 0.5f;
			ci.SetLocalPosition(leftPos);
		}
        else
        {
			Debug.LogWarningFormat("CharacterPlacer:: Could not set character {0} offscreen left, key not in dictionary.", alias);
		}
	}

    public void SetCharacterOffscreenRight(string alias)
    {
        if (usedCharacterTemplates.ContainsKey(alias)) {
            var ch = usedCharacterTemplates[alias];
            var ci = ch.charImage;
            var rightPos = rightEdgeAnchor.localPosition;
            rightPos.x += ci.GetWidth() * 0.5f;
            ci.SetLocalPosition(rightPos);
        } else {
            Debug.LogWarningFormat("CharacterPlacer:: Could not set character {0} offscreen right, key not in dictionary.", alias);
        }
    }

    public void SetCharacterPosition(string alias, CharacterPosition position, Vector3 offset)
    {
        if (usedCharacterTemplates.ContainsKey(alias)) {
            var charRT = GetCharacterRectTransform(alias);
            var pos = ToLocalPosition(position) + offset;
            charRT.localPosition = pos;
        } else {
            Debug.LogWarningFormat("CharacterPlacer:: Could not set character {0} position to {1}, alias key not in dictionary.", alias, position.ToString());
        }
    }

    public CharacterImage GetCharacterImage(string alias)
    {
        if (usedCharacterTemplates.ContainsKey(alias)) {
            var ch = usedCharacterTemplates[alias];
            return ch.charImage;
        } else {
            Debug.LogWarningFormat("CharacterPlacer:: Could not get character image for {0}, key not in dictionary.", alias);
            return null;
        }
    }

    public RectTransform GetCharacterRectTransform(string alias)
    {
        if (usedCharacterTemplates.ContainsKey(alias)) {
            var ch = usedCharacterTemplates[alias];
            return ch.charImage.GetComponent<RectTransform>();
        } else {
            Debug.LogWarningFormat("CharacterPlacer:: Could not get character image for {0}, key not in dictionary.", alias);
            return null;
        }
    }

    public IEnumerator AnimateCharacterTo(string alias, CharacterPosition position, string curveName, Vector3 targetPositionOffset, float duration)
	{
        // Get initial values.
        AnimationCurve easeCurve = GetEaseCurve(curveName);
        RectTransform charRT = GetCharacterRectTransform(alias);
        Vector3 targetPosition = ToLocalPosition(position) + targetPositionOffset;
        Vector3 initialPosition = charRT.localPosition;

        // Over the duration, interpolate to target position using the easeCurve.
        float startTime = Time.time;
        float elapsedTime;
        while ((elapsedTime = Time.time - startTime) < duration)
        {
            float progress = elapsedTime / duration;
            float easeValue = easeCurve.Evaluate(progress);
            Vector3 tween = Vector3.Lerp(initialPosition, targetPosition, easeValue);
            charRT.localPosition = tween;
            yield return null;
        }

        // Finish the animation.
        charRT.localPosition = targetPosition;
	}

    public void FlipCharacter(string alias)
    {
        RectTransform charRT = GetCharacterRectTransform(alias);
        if (charRT != null)
        {
            Vector3 scl = charRT.localScale;
            scl.x *= -1;
            charRT.localScale = scl;
        }
        else
        {
            Debug.LogWarningFormat("CharacterPlacer:: Could not flip character {0}, key not in dictionary.", alias);
        }
    }

    public IEnumerator FlipCharacter(string alias, string curveName, float duration)
    {
        // Get initial values.
        AnimationCurve easeCurve = GetEaseCurve(curveName);
        RectTransform charRT = GetCharacterRectTransform(alias);
        Vector3 initialScale = charRT.localScale;
        Vector3 targetScale = charRT.localScale;
        targetScale.x *= -1;

        // Over the duration, interpolate to target position using the easeCurve.
        float startTime = Time.time;
        float elapsedTime;
        while ((elapsedTime = Time.time - startTime) < duration)
        {
            float progress = elapsedTime / duration;
            float easeValue = easeCurve.Evaluate(progress);
            Vector3 tween = Vector3.Lerp(initialScale, targetScale, easeValue);
            charRT.localScale = tween;
            yield return null;
        }

        // Finish the animation.
        charRT.localScale = targetScale;
    }

    AnimationCurve GetEaseCurve(string curveName)
    {
        if (curveDict.ContainsKey(curveName))
        {
            return curveDict[curveName];
        }
        else
        {
            Debug.LogWarningFormat("CharacterPlacer:: Did not find curve with name {0} in curve dictionary, defaulted to Linear curve.", curveName);
            return AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }

    Vector3 ToLocalPosition(CharacterPosition position)
    {
        RectTransform anchor = null;
        switch (position) {
            case CharacterPosition.Left:
                {
                    anchor = leftCharacterAnchor;
                    break;
                }
            case CharacterPosition.Center:
                {
                    anchor = centerCharacterAnchor;
                    break;
                }
            case CharacterPosition.Right:
                {
                    anchor = rightCharacterAnchor;
                    break;
                }
        }
        if (anchor != null)
        {
            return anchor.localPosition;
        }
        else
        {
            Debug.LogWarningFormat("CharacterPlacer:: Error converting CharacterPosition {0} to local position, returning position (0, 0, 0).", position);
            return Vector3.zero;
        }
    }
}
