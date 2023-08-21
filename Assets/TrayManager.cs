using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayManager : MonoBehaviour
{
    private GameObject previousHitObj; // To keep track of the previously hit object
    private GameObject finalHitObj;

    // Update is called once per frame
    void Update()
    {
        // This is the "clicking" state
        if (GameStateManager.GetGameState() == 0)
        {
            // Casting a ray from camera to mouse 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            GameObject currentHitObj = null; // Store the currently hit object

            if (Physics.Raycast(ray, out hit, 100))
            {
                currentHitObj = hit.collider.gameObject;

                // If the object is a pickup it has an outline!
                if (currentHitObj.CompareTag("Pickup"))
                {
                    currentHitObj.transform.GetComponent<Outline>().enabled = true;

                    // Checking for a click
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Enabling the "tray place mode"
                        GameStateManager.SetGameState(1);
                        finalHitObj = currentHitObj;

                        // Setting the glass to ignore raycast
                        finalHitObj.layer = 2;
                    }
                }
            }

            // Setting the previous hit's outline to false
            if (previousHitObj != null && previousHitObj.GetInstanceID() != currentHitObj.GetInstanceID() && previousHitObj.CompareTag("Pickup"))
            {
                previousHitObj.transform.GetComponent<Outline>().enabled = false;
            }

            // updating
            previousHitObj = currentHitObj;
        }

        // This is the dropping state
        if (GameStateManager.GetGameState() == 1)
        {
            // Cast a ray from the camera to the tray (hey that rhymes)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.name == "Tray")
                {
                    Vector3 newPosition = hit.point + Vector3.up * 1.03f;
                    finalHitObj.transform.position = Vector3.Lerp(finalHitObj.transform.position, newPosition, Time.deltaTime * 5f);

                    // If the user clicks, enable the rigidbody
                    if (Input.GetMouseButtonUp(0))
                    {
                        finalHitObj.GetComponent<Rigidbody>().isKinematic = false;
                        finalHitObj.transform.parent = transform;
                        StartCoroutine(WaitSetGameState(1f, 0, finalHitObj.GetComponent<Rigidbody>()));
                    }
                }
            }
        }
    }

    IEnumerator WaitSetGameState(float time, int state, Rigidbody rb)
    {
        yield return new WaitForSeconds(time);
        rb.isKinematic = true;
        GameStateManager.SetGameState(state);
    }
}
