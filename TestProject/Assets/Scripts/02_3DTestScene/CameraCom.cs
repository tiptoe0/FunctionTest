using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCom : MonoBehaviour
{
    //오브젝트 관련 변수
    public Camera cam;
    public Transform transform_gizmo;
    Transform transform_rot;
    Transform transform_cam;

    Matrix4x4 matrix_current = Matrix4x4.identity;
    Vector3 position_startRot;
    Matrix4x4 matrix_startRot;

    //커서 좌표 관련 변수
    Vector3 position_startPan;
    Vector3 position_currPan;
    float currentDistance;
    float xPanInput, yPanInput;
    float xPan, yPan = 0.0f;

    //(Speed) 상수
    class ConstString
    {
        public static float camRotSpeed = 0.2f;
        public static float camPanSpeed = 0.005f;
        public static float camZoomSpeed = 0.5f;

        public static float camPanYRange = 1f;
        public static float camPanXRange = 2f;

        public static float cursorMouseWheel = 1f;
        public static float camZoomMinRange = 1f;
        public static float camZoomMaxRange = 5f;
    }

    void Start()
    {
        transform_rot = this.transform;
        transform_cam = cam.transform;

        //TODO
        //1. reset button
        //2. rotate 화면 중앙
        //3. zoom 할때 커서 위치
        //4. gizmo update
        //0-1. Touch event
        //0-2. 3D event
    }

    void Update()
    {
        RotateCamera();
        PanCamera();
        ZoomCamera();
    }

    void RotateCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            position_startRot = Input.mousePosition;
            matrix_startRot = matrix_current;
        }
        else if (Input.GetMouseButton(1))
        {
            //Input값 가져옴
            Vector3 inputPos = Input.mousePosition - position_startRot;
            Vector3 normal = inputPos.normalized;

            //Input값을 기반으로 angle & axis 계산
            float angle = inputPos.magnitude * ConstString.camRotSpeed;
            Vector3 axis = new Vector3(-normal.y, -normal.x, 0f);

            //행렬로 계산
            Matrix4x4 rotMtx = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
            matrix_current = rotMtx * matrix_startRot;

            //계산된 행렬로 Camera & Gizmo Transform 세팅
            SetRotation(matrix_current);
        }
    }

    void SetRotation(Matrix4x4 matrix)
    {
        Matrix4x4 camMtx = matrix.inverse;
        transform_rot.localRotation = camMtx.rotation;
        transform_gizmo.localRotation = matrix.rotation;
    }

    void PanCamera()
    {
        if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.B)))
        {
            position_startPan = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.B))
        {
            //Input값 가져옴
            position_currPan = Input.mousePosition - position_startPan;
            xPanInput = position_currPan.x;
            yPanInput = position_currPan.y;
            xPan = xPanInput * ConstString.camPanSpeed;
            yPan = -1 * yPanInput * ConstString.camPanSpeed;

            //Camera Pan Pos 설정
            Vector3 prev = transform_cam.position;
            transform_cam.position += transform_cam.right * xPan * -1;
            transform_cam.position += transform_cam.up * yPan;

            //Pan 범위 체크
            if (Mathf.Abs(transform_cam.localPosition.y) > ConstString.camPanYRange | Mathf.Abs(transform_cam.localPosition.x) > ConstString.camPanXRange)
            {
                transform_cam.position = prev;
            }

            //Input값 업데이트
            position_startPan = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2) || Input.GetKeyUp(KeyCode.B))
        {
        }
    }

    void ZoomCamera()
    {
        float wheelDistance = Input.GetAxis("Mouse ScrollWheel") * ConstString.cursorMouseWheel;
        if (wheelDistance != 0)
        {
            //Zoom값 설정
            currentDistance = wheelDistance;
            currentDistance *= ConstString.camZoomSpeed;

            //Camera Zoom Pos 설정
            Vector3 prev = transform_cam.position;
            transform_cam.position += transform_cam.forward * currentDistance;

            //Zoom 범위 체크
            if (transform_cam.localPosition.z < ConstString.camZoomMinRange || transform_cam.localPosition.z > ConstString.camZoomMaxRange)
            {
                transform_cam.position = prev;
            }
        }
    }  
}