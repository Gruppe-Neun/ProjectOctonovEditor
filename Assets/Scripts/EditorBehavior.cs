using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EditorBehavior : MonoBehaviour
{
    [SerializeField] private float cameraSensitivity = 90;
    [SerializeField] private float climbSpeed = 4;
    [SerializeField] private float normalMoveSpeed = 10;
    [SerializeField] private float slowMoveFactor = 0.25f;
    [SerializeField] private float fastMoveFactor = 3;
    [SerializeField] private World world;
    [SerializeField] private editorUI ui;

    private Block[,,] clipBoard;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private float LastKeyPressed;
    private Block.BlockType activeBlock;

    void Start() {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        activeBlock = Block.BlockType.DIRT;
    }

    void Update() {
        // Get input from mouse
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        // Limit the y rotation to 90°
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        // Apply rotation
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        // Execute the locomotion of the camera in regular, low or fast speed
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        } else {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        
        // Toggle the lock mode of the cursor
        if (Input.GetKeyDown(KeyCode.End)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        //reset Camera
        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            rotationY = 0;
            rotationX = 0;
            transform.localRotation = new Quaternion(0,0,0,0);
        }


        //rotate chunk
        if (Input.GetKeyDown(KeyCode.X)) {
            World.GetWorldChunk(transform.position).rotateChunk(0);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            World.GetWorldChunk(transform.position).rotateChunk(1);
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            World.GetWorldChunk(transform.position).rotateChunk(2);
        }

        //fill Chunk
        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            World.GetWorldChunk(transform.position).fill(true,activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            World.GetWorldChunk(transform.position).fill(false,activeBlock);
        }

        //edit Chunk
        if (Input.GetKeyDown(KeyCode.Keypad9)) {
            World.GetWorldChunk(transform.position).setOuter(0, activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            World.GetWorldChunk(transform.position).setOuter(1, activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6)) {
            World.GetWorldChunk(transform.position).setOuter(2, activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            World.GetWorldChunk(transform.position).setOuter(3, activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            World.GetWorldChunk(transform.position).setOuter(4, activeBlock);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            World.GetWorldChunk(transform.position).setOuter(5, activeBlock);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            World.GetWorldChunk(transform.position).setChunkType(0, activeBlock);
        }


        //CopyPaste Chunk
        if (Input.GetKeyDown(KeyCode.C)) {
            clipBoard =  World.GetWorldChunk(transform.position).copyChunkData();
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            World.GetWorldChunk(transform.position).pasteChunkData(clipBoard);
        }


        //build single Block
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 10)) {
                Vector3 block = hit.point + (transform.forward * 0.1f);
                World.GetWorldBlock(block).SetType(Block.BlockType.AIR);
                World.GetWorldChunk(block).Redraw();
                World.GetWorldChunk(block).Save();
            }

            
        } else {
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 10)) {
                    Vector3 block = hit.point - (transform.forward * 0.1f);
                    World.GetWorldBlock(block).SetType(activeBlock);
                    World.GetWorldChunk(block).Redraw();
                    World.GetWorldChunk(block).Save();
                }
            }
        }


        //switch blocks

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            ui.setActive(10);
            this.activeBlock = (Block.BlockType)10;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ui.setActive(1);
            this.activeBlock = (Block.BlockType)1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ui.setActive(2);
            this.activeBlock = (Block.BlockType)2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ui.setActive(3);
            this.activeBlock = (Block.BlockType)3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            ui.setActive(4);
            this.activeBlock = (Block.BlockType)4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            ui.setActive(5);
            this.activeBlock = (Block.BlockType)5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            ui.setActive(6);
            this.activeBlock = (Block.BlockType)6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            ui.setActive(7);
            this.activeBlock = (Block.BlockType)7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            ui.setActive(8);
            this.activeBlock = (Block.BlockType)8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            ui.setActive(9);
            this.activeBlock = (Block.BlockType)9;
        }

    }



}
