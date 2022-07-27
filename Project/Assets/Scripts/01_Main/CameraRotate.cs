using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public bool ableControl;

    float angleX, angleY;
    float targetAngleX, targetAngleY;

    float speed = 10f;

    GameObject currentSelectObject;

    void Update()
    {
        if (ableControl)
        {
            if (GetComponent<Animator>() != null) Destroy(GetComponent<Animator>());
            targetAngleX -= Input.GetAxis("Mouse Y");
            targetAngleY += Input.GetAxis("Mouse X");

            targetAngleX = Mathf.Clamp(targetAngleX, -20, 20);
            targetAngleY = Mathf.Clamp(targetAngleY, -75, 25);

            angleX = Mathf.Lerp(angleX, targetAngleX, Time.deltaTime * speed);
            angleY = Mathf.Lerp(angleY, targetAngleY, Time.deltaTime * speed);
            transform.rotation = Quaternion.Euler(angleX, angleY, 0);

            if (Physics.Raycast(GetComponent<Camera>().ViewportPointToRay(Vector3.one * .5f), out RaycastHit hit, 1 << LayerMask.NameToLayer("SelectionObject")))
            {
                if (currentSelectObject != null)
                {
                    if (hit.collider.gameObject != currentSelectObject)
                    {
                        currentSelectObject.GetComponent<MeshRenderer>().material.color = Color.white;
                        currentSelectObject = hit.collider.gameObject;
                        currentSelectObject.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
                else
                {
                    currentSelectObject = hit.collider.gameObject;
                    currentSelectObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
            else
            {
                if (currentSelectObject != null)
                {
                    currentSelectObject.GetComponent<MeshRenderer>().material.color = Color.white;
                    currentSelectObject = null;
                }
            }
        }
        else
        {
            targetAngleX = transform.eulerAngles.x;
            targetAngleY = transform.eulerAngles.y;
            angleX = transform.eulerAngles.x;
            angleY = transform.eulerAngles.y;
        }
    }
}
