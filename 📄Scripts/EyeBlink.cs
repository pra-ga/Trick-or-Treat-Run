using UnityEngine;
using System.Collections;

public class EyeBlink : MonoBehaviour
{
    [SerializeField] private Transform leftEye;   // assign in inspector
    [SerializeField] private Transform rightEye;  // assign in inspector

    [SerializeField] private float openZScale = 0.08f;
    [SerializeField] private float closedZScale = 0f;
    [SerializeField] private float blinkDuration = 0.1f; // time to close/open
    [SerializeField] private Vector2 blinkIntervalRange = new Vector2(2f, 5f); // random wait between blinks

    private void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // wait random time before blinking
            float waitTime = Random.Range(blinkIntervalRange.x, blinkIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            // close eyes smoothly
            yield return StartCoroutine(ScaleEyes(closedZScale));

            // short pause while closed
            yield return new WaitForSeconds(0.1f);

            // open eyes smoothly
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

        // snap to target at the end
        leftEye.localScale = leftTarget;
        rightEye.localScale = rightTarget;
    }
}
