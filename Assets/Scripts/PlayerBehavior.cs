using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    [SerializeField] private float CameraSensitivity = 90f;

    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private float SlowMoveFactor = 0.25f;
    [SerializeField] private float FastMoveFactor = 3f;
    [SerializeField] private float AirControlFactor = 0.1f;

    [SerializeField] private float JumpSpeed = 5f;
    [SerializeField] private float Gravity = 20f;

    private float rotationY = 0.0f;
    private float rotationX = 0.0f;

    private Camera headCamera;
    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;
    private float headShake = 0f;

    // Start is called before the first frame update
    void Start()
    {
        headCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //rotate head
        rotationX += Input.GetAxis("Mouse X") * CameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * CameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        headCamera.transform.localRotation = Quaternion.AngleAxis(rotationY, Vector3.left) * Quaternion.AngleAxis(Input.GetAxis("Horizontal"),Vector3.back);






        //move the Player
        if (controller.isGrounded) {

            float forward = Input.GetAxis("Vertical")*MoveSpeed*Time.deltaTime, strafe = Input.GetAxis("Horizontal")*MoveSpeed*Time.deltaTime;


            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                forward *= FastMoveFactor;
            } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                forward *= SlowMoveFactor;
            }



            //head shake effect
            headShake += forward / MoveSpeed * 10;
            if (headShake > Mathf.PI * 4) {
                headShake -= Mathf.PI * 4;
            }
            Vector3 headPos = new Vector3(0, 0.5f + Mathf.Sin(headShake) * 0.1f, 0);
            headCamera.transform.localPosition = headPos;



            moveDirection = transform.forward * forward + transform.right * strafe;
            //jump
            if (Input.GetButton("Jump")) {
                moveDirection.y = JumpSpeed;
            }


        } else {


            Vector3 inputDirection = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal") ;

            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                inputDirection *= (MoveSpeed * FastMoveFactor) * Time.deltaTime * AirControlFactor;
            } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                inputDirection *= (MoveSpeed * SlowMoveFactor) * Time.deltaTime * AirControlFactor;
            } else {
                inputDirection *= MoveSpeed * Time.deltaTime * AirControlFactor;
            }
            moveDirection += inputDirection;
            Mathf.Clamp(moveDirection.x, -(MoveSpeed * FastMoveFactor) * Time.deltaTime, (MoveSpeed * FastMoveFactor) * Time.deltaTime);
            Mathf.Clamp(moveDirection.z, -(MoveSpeed * FastMoveFactor) * Time.deltaTime, (MoveSpeed * FastMoveFactor) * Time.deltaTime);


    
            //apply gravity
            moveDirection.y -= (Gravity * Time.deltaTime * Time.deltaTime);
        }
        controller.Move(moveDirection);



        



        /*Change Field of View
        Debug.Log(movement.magnitude);
        float fieldOfView = 90 + movement.magnitude - 9;
        Mathf.Clamp(fieldOfView, 90, 120);

        headCamera.fieldOfView = fieldOfView;
        */





        //release the mouse
        if (Input.GetKeyDown(KeyCode.End)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


}
