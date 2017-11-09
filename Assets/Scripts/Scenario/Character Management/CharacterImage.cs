using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterImage : MonoBehaviour
{
	Image image;
	CharacterPlacer characterPlacer;

	void Awake()
	{
		image = GetComponent<Image>();
		image.enabled = false;
		transform.parent.GetComponent<CharacterPlacer>();
	}

	public void SetEnabled(bool b)
	{
		image.enabled = b;
	}

	public float GetWidth()
	{
		return image.rectTransform.rect.width;
	}

	public float GetHeight()
	{
		return image.rectTransform.rect.height;
	}

	public void SetLocalPosition(float x, float y)
	{
		var pos = image.rectTransform.localPosition;
		pos.x = x;
		pos.y = y;
		image.rectTransform.localPosition = pos;
	}

	public void SetLocalPosition(Vector3 localPosition)
	{
		image.rectTransform.localPosition = localPosition;
	}

	public void SetSprite(Sprite s)
	{
		image.sprite = s;
	}
}
