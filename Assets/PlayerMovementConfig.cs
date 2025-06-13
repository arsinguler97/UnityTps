using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementConfig", menuName = "Game Configs/Movement Config")]
public class PlayerMovementConfig : ScriptableObject
{
    public float targetMoveSpeed = 5f;
    public float runSpeed = 10f;

    public float accelerationRate = 10f;
    public float decelerationRate = 15f;

    public float baseJumpForce = 8f;
    public float maxJumpHoldTime = 0.3f;

    public float gravityMultiplier = 2f;
    public float airControlFactor = 0.5f;

    public float lookSensitivity = 2f;

    public float groundCheckDistance = 0.2f;
    public float groundCheckHeight = 0.05f;
}