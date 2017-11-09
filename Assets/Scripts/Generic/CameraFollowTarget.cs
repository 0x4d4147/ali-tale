using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Camera followCamera;
    Vector3 playerToCameraOffset;

    bool isFollowing = true;

    void Start()
    {
        playerToCameraOffset = followCamera.transform.position - transform.position;
    }

    void FixedUpdate()
    {
        if (isFollowing)
            followCamera.transform.position = Vector3.Lerp(followCamera.transform.position, transform.position + playerToCameraOffset, Time.fixedDeltaTime);
    }

    public void SetFollowingEnabled(bool b)
    {
        isFollowing = b;
    }
}
