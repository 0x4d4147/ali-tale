using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetingSageOverworldScene : MonoBehaviour
{
    public PlayerOverworldCharacterController playerController;
    public OverworldCharacterController keithController;
    public GameObject keithWalkTarget;

    public float timeBeforeKeithMoves = 0.5f;
    public float waitTimeUnitBackToPlayer = 3f;
    public float waitTimeUntilReactivateInput = 3f;

    void Start()
    {
        StartCoroutine(RunStartSequence());
    }

    IEnumerator RunStartSequence()
    {
        yield return new WaitForSeconds(timeBeforeKeithMoves);

        // Disable player input.
        playerController.SetPlayerInputEnabled(false);
        playerController.SetPlayerCameraEnabled(false);

        // Set camera to follow keith.
        var keithCameraFollow = keithController.gameObject.AddComponent<CameraFollowTarget>();
        keithCameraFollow.followCamera = Camera.main;

        // Walk keith over to off screen a bit.
        keithController.SetMoveTarget(keithWalkTarget.transform.position, 0.4f);

        yield return new WaitForSeconds(waitTimeUnitBackToPlayer);

        // Move camera back to player.
        keithCameraFollow.SetFollowingEnabled(false);
        playerController.SetPlayerCameraEnabled(true);

        yield return new WaitForSeconds(waitTimeUntilReactivateInput);

        // Kill Keith
        Destroy(keithController.gameObject);

        // Reactivate player input.
        playerController.SetPlayerInputEnabled(true);

        // Activate player input.
        yield return null;
    }
}
