using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpressionIcon
{
	None,
	Worried,
	Alert
}

public class OverworldCharacterController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateAngle = 15f;
    public float slerpBackTime = 0.5f;
    public float wobbleFrequency = 6f;

    public Collider endBoundLeft;
    public Collider endBoundRight;

    protected Rigidbody rb;

    protected bool wasMovingLastFrame = false;
    protected bool isMoving = false;
    protected float movingStartTime;

    protected Quaternion playerInitialRotation;
    protected Coroutine slerpingBackToInitialRotation;

    protected Vector3 characterRelativeXAxis;
    protected float xInputSign = 1f;

    protected float xAmt = 0f;

    protected Vector3 moveTarget;
    protected bool isMoveTargetSet = false;
    protected float moveTargetThreshold = 0.4f;

	protected GameObject expressionIcon;
	protected MeshRenderer expressionIconMeshRenderer;

    protected bool isLeftMovementEnabled = true;
    protected bool isRightMovementEnabled = true;

	[Header("Expression Icon Materials")]
	public Material alertIconMaterial;
	public Material worryIconMaterial;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInitialRotation = rb.rotation;
        characterRelativeXAxis = transform.right;
		expressionIcon = transform.Find("Expression Icon").gameObject;
		expressionIconMeshRenderer = expressionIcon.GetComponent<MeshRenderer>();
    }

    public void Move(float x)
    {
        if (!isLeftMovementEnabled && x < 0f)
            x = 0;
        if (!isRightMovementEnabled && x > 0f)
            x = 0;
        
        xAmt = x;
    }

    public void SetMoveTarget(Vector3 target, float threshold)
    {
        moveTarget = target;
        isMoveTargetSet = true;
        moveTargetThreshold = threshold;
    }

    protected virtual void FixedUpdate()
    {
        xInputSign = Mathf.Sign(xAmt);
        isMoving = !Mathf.Approximately(xAmt, 0f);
        Vector3 moveDelta = characterRelativeXAxis * xAmt * moveSpeed * Time.fixedDeltaTime;
        rb.position = transform.position + moveDelta;
    }

    void Update()
    {
        // If we just started moving.
        if (!wasMovingLastFrame && isMoving)
        {
            // Cancel slerping back if doing that.
            if (slerpingBackToInitialRotation != null)
                StopCoroutine(slerpingBackToInitialRotation);

            // Set the start time here for the sinusoid's base.
            movingStartTime = Time.time;
        }

        // If we just stopped moving.
        if (wasMovingLastFrame && !isMoving)
        {
            // Start slerping back to inital rotation.
            slerpingBackToInitialRotation = StartCoroutine(SlerpBackToInitialRotation());
        }

        // If we are currently moving, do the wobble animation.
        if (isMoving)
        {
            // Rotate around the forward axis.
            float angleOffset = Mathf.Sin(-xInputSign * wobbleFrequency * (Time.time - movingStartTime));
            rb.rotation = playerInitialRotation * Quaternion.AngleAxis(rotateAngle * angleOffset, transform.forward);

            // Make face toward direction of movement.
            Vector3 scl = transform.localScale;
            scl.x = Mathf.Abs(scl.x) * xInputSign;
            transform.localScale = scl;
        }

        // Update the status of this flag.
        wasMovingLastFrame = isMoving;

        // If a move target was set, move to the target.
        if (isMoveTargetSet)
        {
            if (HasReachedMoveTarget())
            {
                isMoveTargetSet = false;
                Move(0f);
            }
            else
            {
                //Debug.LogFormat("OverworldCharacterController:: Distance to move target: " + DistanceToMoveTarget());
                Move(DirectionToMoveTarget());
            }
        }
    }

    public float DistanceToMoveTarget()
    {
        return Vector3.Project(moveTarget - transform.position, characterRelativeXAxis).magnitude;
    }

    public float DirectionToMoveTarget()
    {
        Vector3 delt = Vector3.Project(moveTarget - transform.position, characterRelativeXAxis);
        float signed = Vector3.Dot(delt, characterRelativeXAxis);
        return Mathf.Sign(signed);
    }

    public bool HasReachedMoveTarget()
    {
        return DistanceToMoveTarget() <= moveTargetThreshold;
    }

    public float DistanceToCharacter(OverworldCharacterController character)
    {
        return Vector3.Project(character.transform.position - transform.position, characterRelativeXAxis).magnitude;
    }

    IEnumerator SlerpBackToInitialRotation()
    {
        float startTime = Time.time;
        float elapsedTime;
        Quaternion preSlerpRotation = transform.rotation;
        while ((elapsedTime = Time.time - startTime) < slerpBackTime)
        {
            float ratio = elapsedTime / slerpBackTime;
            transform.rotation = Quaternion.Slerp(preSlerpRotation, playerInitialRotation, ratio);
            yield return null;
        }
        transform.rotation = playerInitialRotation;
    }

	public void ShowIcon(ExpressionIcon icon)
	{
		switch (icon) {
		case ExpressionIcon.None:
			{
				expressionIcon.SetActive(false);
				break;
			}
		case ExpressionIcon.Alert:
			{
				expressionIconMeshRenderer.material = alertIconMaterial;
				expressionIcon.SetActive(true);
				break;
			}
		case ExpressionIcon.Worried:
			{
				expressionIconMeshRenderer.material = worryIconMaterial;
				expressionIcon.SetActive(true);
				break;
			}
		}
	}

    public void OnTriggerEnter(Collider collider)
    {
        if (collider == endBoundLeft)
        {
            isLeftMovementEnabled = false;
        }

        else if (collider == endBoundRight)
        {
            isRightMovementEnabled = false;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider == endBoundLeft)
        {
            isLeftMovementEnabled = true;
        }

        else if (collider == endBoundRight)
        {
            isRightMovementEnabled = true;
        }
    }
}
