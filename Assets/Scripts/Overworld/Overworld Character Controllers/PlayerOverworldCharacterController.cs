using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOverworldCharacterController : OverworldCharacterController
{
    public Camera followCamera;

    CameraFollowTarget cameraFollowTarget;
    bool playerInputIsEnabled = true;

	protected override void Awake()
	{
        base.Awake();
        cameraFollowTarget = gameObject.AddComponent<CameraFollowTarget>();
        cameraFollowTarget.followCamera = followCamera;
	}

    protected override void FixedUpdate()
    {
        if (playerInputIsEnabled)
            Move(Input.GetAxis("Horizontal"));
        
        base.FixedUpdate();
    }

    public void SetPlayerInputEnabled(bool b)
    {
        playerInputIsEnabled = b;
    }

    public void SetPlayerCameraEnabled(bool b)
    {
        cameraFollowTarget.SetFollowingEnabled(b);
    }
}
