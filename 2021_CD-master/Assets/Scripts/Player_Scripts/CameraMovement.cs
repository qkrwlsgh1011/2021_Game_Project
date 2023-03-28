using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow; // 따라가야할 오브젝트 정보
    public float followSpeed = 10f;   //오브젝트 속도
    public float sensitivity = 100f;   //마우스 감도
    public float clampAngle = 70f;    //마우스 위아래 제한

    private float rotX;
    private float rotY;

    public Transform realCamera;    
    public Vector3 dirNormalied;    //방향
    public Vector3 finalDir;        //최종방향
    public float minDistance;       //카메라와 캐릭터간 최소 거리
    public float maxDistance;       //카메라와 캐릭터간 최대 거리
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
