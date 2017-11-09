using UnityEngine;

public class FollowOverworldCharacterController : MonoBehaviour
{
    public float minDistance;
    public float stopThreshold;
    public OverworldCharacterController targetCharacter;
    OverworldCharacterController followerCharacter;

    public bool debug = false;

    void Awake()
    {
        followerCharacter = GetComponent<OverworldCharacterController>();
    }

    void Update()
    {
        if (targetCharacter.DistanceToCharacter(followerCharacter) > minDistance)
        {
            followerCharacter.SetMoveTarget(targetCharacter.transform.position, stopThreshold);
        }
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * minDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.up * -0.1f, (transform.position + transform.up * -0.1f) + transform.right * stopThreshold);
        }
    }
}
