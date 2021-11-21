using UnityEngine;

public class CameraJerk : MonoBehaviour
{
    [SerializeField] private float yAmp = 2f, xAmp = 2f;

    Vector3 temp;

    private void Update()
    {
        temp.x = (xAmp * Mathf.Sin(Time.time));
        temp.y = (yAmp * Mathf.Cos(Time.time));

        transform.localPosition = temp;
    }
}
