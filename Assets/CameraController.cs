using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public Transform trayModeReference;

    private Vector3 offset;
    private Vector3 startPosition;
    private Quaternion startRotation;
    

    // Start is called before the first frame update
    void Start()
    {
        startPosition = player.position;
        startRotation = transform.rotation;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 0 = normal follow
        // 1 = tray follow
        // 2 = fall mode

        if (GameStateManager.GetGameState() == 0)
        {
            Vector3 targetPosition = new Vector3(player.position.x, startPosition.y, player.position.z) + offset;
            transform.rotation = startRotation;
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (GameStateManager.GetGameState() == 1)
        {
            transform.position = trayModeReference.position;
            transform.rotation = trayModeReference.rotation;
        }
        else if (GameStateManager.GetGameState() == 2)
        {

        }
    }
}

