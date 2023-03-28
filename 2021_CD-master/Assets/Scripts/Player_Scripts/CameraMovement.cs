using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow; // ���󰡾��� ������Ʈ ����
    public float followSpeed = 10f;   //������Ʈ �ӵ�
    public float sensitivity = 100f;   //���콺 ����
    public float clampAngle = 70f;    //���콺 ���Ʒ� ����

    private float rotX;
    private float rotY;

    public Transform realCamera;    
    public Vector3 dirNormalied;    //����
    public Vector3 finalDir;        //��������
    public float minDistance;       //ī�޶�� ĳ���Ͱ� �ּ� �Ÿ�
    public float maxDistance;       //ī�޶�� ĳ���Ͱ� �ִ� �Ÿ�
    public float finalDistance;
    public float smoothness = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotX = transform.localRotation.eulerAngles.y;

        dirNormalied = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    private void LateUpdate() {
        transform.position = Vector3.MoveTowards(transform.position,
            objectTofollow.position, followSpeed * Time.deltaTime);
        finalDir = transform.TransformPoint(dirNormalied * maxDistance);

        RaycastHit hit;

        if(Physics.Linecast(transform.position, finalDir, out hit)) {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        } else {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, 
            dirNormalied * finalDistance, Time.deltaTime * smoothness);
    }
}
