using UnityEngine;
using UnityEngine.InputSystem;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;
    private Rigidbody rb;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("Settings")]
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    [SerializeField] InputActionReference leftHandStick;
    [SerializeField] InputActionReference buttonA;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (buttonA.action.IsPressed())
            Dash();

        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        Transform forwardT = playerCam;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + Vector3.up * dashUpwardForce;


        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }


    private Vector3 GetDirection(Transform forwardT)
    {
        Vector2 moveInput = leftHandStick.action.ReadValue<Vector2>();

        Vector3 direction = new Vector3();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        Quaternion angleQ = Quaternion.Euler(0, angle, 0);

        Quaternion yRot = Quaternion.Euler(0, forwardT.rotation.eulerAngles.y, 0);
        Vector3 forwardRot = yRot * Vector3.forward;

        direction = angleQ * forwardRot;

        return direction;
    }
}