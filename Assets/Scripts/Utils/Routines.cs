using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public static class Routines
{
    public static IEnumerator RigidbodyFallRoutine(Rigidbody rb)
    {
        rb.isKinematic = false;

        float timer = 0f;

        while (timer < 3f)
        {
            if (rb.velocity.magnitude > 0.01f || rb.angularVelocity.magnitude > 0.01f)
                timer = 0f;

            timer += Time.deltaTime;
            yield return null;
        }

        rb.isKinematic = true;
    }

    public static IEnumerator TransformRotationRoutine(Transform transform, Axis axis, float angle, float duration)
    {
        Quaternion initialRotation = transform.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.AngleAxis(angle, axis.ToVector());

        float t = 0f;
        float currentVelocity = 0f;
        float smoothTime = duration / 3.33f;

        while (t < 0.99f)
        {
            t = Mathf.SmoothDamp(t, 1f, ref currentVelocity, smoothTime);
            transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            yield return null;
        }

        transform.localRotation = targetRotation;
    }
}