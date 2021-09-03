using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] Transform Target; // to follow the player
     Camera mainCamera;
     Player player;

    [Header("Camera Attributes")]
    [SerializeField] float FollowSpeed = 4f;
    Vector3 originalPos;
    private Transform camTransform;
    [SerializeField] float cameraZoom;
    [SerializeField] bool cameraFollowing;
    [SerializeField] float yAxisOffset;

    [Header("Camera shake features")] // for getting hit, dashes, jumps, interactions with world
    [SerializeField] float shakeDuration = 0f;  //duration of the shake
    [SerializeField] float shakeAmount = 0.5f;    // Amplitude of the shake. A larger value shakes the camera harder.
    [SerializeField] float decreaseFactor = 1.0f;

    private void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
        mainCamera = FindObjectOfType<Camera>();
    }
    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        player = FindObjectOfType<Player>();

        if(cameraFollowing)
        {
           Vector3 newPosition = Target.position;
           newPosition.z = -10;
           newPosition.y = newPosition.y + yAxisOffset;
           transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
        }
        



        if (Input.GetKey(KeyCode.Z)) //to zoom in and out of the game, for testing purposes
        {
            mainCamera.orthographicSize = mainCamera.orthographicSize + cameraZoom * Time.deltaTime;
        }

        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void ShakeCamera()
    {
        originalPos = camTransform.localPosition;
        shakeDuration = 0.5f;
    }

    public void ChangeCamera(float number) // to trigger a camera change in certain situations
    {
        mainCamera.orthographicSize = mainCamera.orthographicSize + number;
    }
}
