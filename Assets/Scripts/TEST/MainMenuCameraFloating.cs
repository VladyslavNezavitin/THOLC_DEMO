using UnityEngine;


public class MainMenuCameraFloating : MonoBehaviour
{
    public float floatingSpeed = 1f;
    public float floatingAmount = 0.1f; 

    private Vector3 originalPosition;
    private Vector3 originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * floatingSpeed) * floatingAmount;
        float offsetY = Mathf.Cos(Time.time * floatingSpeed) * floatingAmount;
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);

        transform.position = originalPosition + offset;
        transform.eulerAngles = originalRotation + offset * 10;
    }
}

