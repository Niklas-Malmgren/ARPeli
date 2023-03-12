using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{

    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;


    Camera arCam;
    GameObject spawnedObject;
    private Vector2 touchStartPosition, touchEndPosition;
    private bool tapped, spawned;

    [SerializeField]
    GameObject goalPrefab;
    private bool goalSpawned;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();

        goalSpawned = false;
        spawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if(Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tapped = false;
            touchStartPosition = Input.GetTouch(0).position;
        }

        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchEndPosition = Input.GetTouch(0).position;
            float x = touchEndPosition.x - touchStartPosition.x;
            float y = touchEndPosition.y - touchStartPosition.y;

            if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0)
            {
                tapped = true;
            }
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray, out hit) && goalSpawned)
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(m_Hits[0].pose.position);
                        tapped = false;
                        spawned = true;
                    }
                }
                else if(Physics.Raycast(ray, out hit) && !goalSpawned)
                {
                    Instantiate(goalPrefab, m_Hits[0].pose.position, Quaternion.identity);
                    goalSpawned = true;
                    tapped = false;
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = m_Hits[0].pose.position;
                tapped = false;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended && spawnedObject != null)
            {
                if (spawnedObject.GetComponent<Renderer>().material.color == Color.white && tapped && spawned)
                {
                    spawned = false;
                }
                else if (spawnedObject.GetComponent<Renderer>().material.color == Color.white && tapped)
                {
                    spawnedObject.GetComponent<Renderer>().material.color = Color.green;
                }
                else if (spawnedObject.GetComponent<Renderer>().material.color == Color.green && tapped)
                {
                    spawnedObject.GetComponent<Renderer>().material.color = Color.blue;
                }
                else if (spawnedObject.GetComponent<Renderer>().material.color == Color.blue && tapped)
                {
                    spawnedObject.GetComponent<Renderer>().material.color = Color.red;
                }
                else if (tapped)
                {
                    spawnedObject.GetComponent<Renderer>().material.color = Color.white;
                }
                spawnedObject = null;
            }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
    }
}
