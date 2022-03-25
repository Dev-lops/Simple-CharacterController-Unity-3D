using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineMachinePOVExtension : CinemachineExtension
{
    protected InputManager input;
    protected GameManager manager;
    protected float mouseY, mouseX;
    protected override void Awake() {
        input = InputManager.inst;
        manager = GameManager.inst;
        mouseY = transform.localRotation.eulerAngles.x;
        mouseX = transform.localRotation.eulerAngles.y;
        base.Awake();
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase VCB, CinemachineCore.Stage stage, ref CameraState state, float value){
        if(VCB.Follow){
            if(stage == CinemachineCore.Stage.Aim){
                mouseX += input.MouseDeltaInput().x * manager.mouseSensiblity * Time.deltaTime; 
                mouseY += input.MouseDeltaInput().y * manager.mouseSensiblity * Time.deltaTime;
                mouseY = Mathf.Clamp(mouseY,-90,90);
                state.RawOrientation = Quaternion.Euler(-mouseY,mouseX,0);
            }
        }
    }
}
