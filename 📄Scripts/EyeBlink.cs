using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour
{
    [Header("Eye Transforms")]
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private Transform leftEyeBall;
    [SerializeField] private Transform rightEyeBall;

    [Header("Blink Settings")]
    [SerializeField] private float openZScale = 0.08f;
    [SerializeField] private float closedZScale = 0f;
    [SerializeField] private float blinkDuration = 0.1f;
    [SerializeField] private Vector2 blinkIntervalRange = new Vector2(2f, 5f);

    [Header("Eye Movement Settings")]
    [SerializeField] private float eyeMoveRadius = 0.03f; // how far eyes can move from center
    [SerializeField] private float eyeMoveSpeed = 1.5f;   // how quickly eyes move to target
    [SerializeField] private Vector2 lookIntervalRange = new Vector2(1f, 3f); // random delay between look changes

    private Vector3 leftEyeBallDefaultPos;
    private Vector3 rightEyeBallDefaultPos;
    private Vector3 leftTargetOffset;
    //private Vector3 rightTargetOffset;

    private void Start()
    {
        leftEyeBallDefaultPos = leftEye.localPosition;
        rightEyeBallDefaultPos = rightEye.localPosition;

        StartCoroutine(BlinkRoutine());
        //StartCoroutine(RandomEyeMovementRoutine());
    }

    // --- Blinking ---
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(blinkIntervalRange.x, blinkIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(ScaleEyes(closedZScale));
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(ScaleEyes(openZScale));
        }
    }

    private IEnumerator ScaleEyes(float targetZ)
    {
        float elapsed = 0f;
        Vector3 leftStart = leftEye.localScale;
        Vector3 rightStart = rightEye.localScale;

        Vector3 leftTarget = new Vector3(leftStart.x, leftStart.y, targetZ);
        Vector3 rightTarget = new Vector3(rightStart.x, rightStart.y, targetZ);

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blinkDuration;

            leftEye.localScale = Vector3.Lerp(leftStart, leftTarget, t);
            rightEye.localScale = Vector3.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        leftEye.localScale = leftTarget;
        rightEye.localScale = rightTarget;
    }

    // --- Random Eye Movement ---
    private IEnumerator RandomEyeMovementRoutine()
    {
        while (true)
        {
            // pick a random offset within a circle
            leftTargetOffset = Random.insideUnitCircle * eyeMoveRadius;
            //rightTargetOffset = Random.insideUnitCircle * eyeMoveRadius;

            float lookTime = Random.Range(lookIntervalRange.x, lookIntervalRange.y);
            float elapsed = 0f;

            while (elapsed < lookTime)
            {
                elapsed += Time.deltaTime;

                // smooth lerp towards current target
                leftEyeBall.localPosition = Vector3.Lerp(
                    leftEyeBall.localPosition,
                    leftEyeBallDefaultPos + new Vector3(leftTargetOffset.x, leftTargetOffset.y, 0),
                    Time.deltaTime * eyeMoveSpeed
                );

                rightEyeBall.localPosition = Vector3.Lerp(
                    rightEyeBall.localPosition,
                    rightEyeBallDefaultPos + new Vector3(leftTargetOffset.x, leftTargetOffset.y, 0),
                    Time.deltaTime * eyeMoveSpeed
                );

                yield return null;
            }

            // pick new target after interval
        }
    }
}
