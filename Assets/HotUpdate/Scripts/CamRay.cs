using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamRay : MonoBehaviour
{
    public Transform player;
    public GameObject openDoorText;
    // Start is called before the first frame update
    void Start()
    {
		openDoorText.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	private void FixedUpdate()
	{
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, ~(1 << 3)))
        {
            if (hit.collider.tag == "Door")
            {
                if (Vector3.Distance(player.position, hit.collider.transform.position) <= 5)
                {
                    Door door = hit.collider.GetComponent<Door>();
                    openDoorText.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        door.PlayDoorAnimation();
                    }
                }
                else
                {
					openDoorText.SetActive(false);
				}
            }
            else
            {
				openDoorText.SetActive(false);
			}
        }
        else
        {
			openDoorText.SetActive(false);
		}
	}
}
