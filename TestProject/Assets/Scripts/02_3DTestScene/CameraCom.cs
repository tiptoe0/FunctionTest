using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCom : MonoBehaviour
{
    //������Ʈ ���� ����
    public Camera cam;
    public Transform transform_gizmo;
    Transform transform_rot;
    Transform transform_cam;

    Matrix4x4 matrix_current = Matrix4x4.identity;
    Vector3 position_startRot;
    Matrix4x4 matrix_startRot;

    //Ŀ�� ��ǥ ���� ����
    Vector3 position_startPan;
    Vector3 position_currPan;
    float currentDistance;
    float xPanInput, yPanInput;
    float xPan, yPan = 0.0f;

    //(Speed) ���
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
        //2. rotate ȭ�� �߾�
        //3. zoom �Ҷ� Ŀ�� ��ġ
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
            //Input�� ������
            Vector3 inputPos = Input.mousePosition - position_startRot;
            Vector3 normal = inputPos.normalized;

            //Input���� ������� angle & axis ���
            float angle = inputPos.magnitude * ConstString.camRotSpeed;
            Vector3 axis = new Vector3(-normal.y, -normal.x, 0f);

            //��ķ� ���
            Matrix4x4 rotMtx = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
            matrix_current = rotMtx * matrix_startRot;

            //���� ��ķ� Camera & Gizmo Transform ����
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
            //Input�� ������
            position_currPan = Input.mousePosition - position_startPan;
            xPanInput = position_currPan.x;
            yPanInput = position_currPan.y;
            xPan = xPanInput * ConstString.camPanSpeed;
            yPan = -1 * yPanInput * ConstString.camPanSpeed;

            //Camera Pan Pos ����
            Vector3 prev = transform_cam.position;
            transform_cam.position += transform_cam.right * xPan * -1;
            transform_cam.position += transform_cam.up * yPan;

            //Pan ���� üũ
            if (Mathf.Abs(transform_cam.localPosition.y) > ConstString.camPanYRange | Mathf.Abs(transform_cam.localPosition.x) > ConstString.camPanXRange)
            {
                transform_cam.position = prev;
            }

            //Input�� ������Ʈ
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
            //Zoom�� ����
            currentDistance = wheelDistance;
            currentDistance *= ConstString.camZoomSpeed;

            //Camera Zoom Pos ����
            Vector3 prev = transform_cam.position;
            transform_cam.position += transform_cam.forward * currentDistance;

            //Zoom ���� üũ
            if (transform_cam.localPosition.z < ConstString.camZoomMinRange || transform_cam.localPosition.z > ConstString.camZoomMaxRange)
            {
                transform_cam.position = prev;
            }
        }
    }  
}