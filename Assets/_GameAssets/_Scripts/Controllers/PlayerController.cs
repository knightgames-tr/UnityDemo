using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    void Awake(){
        Instance = this;
    }
    void Start(){
        _characterController = GetComponent<CharacterController>();
    }

    void Update(){
        if(_playerControllerActive){
            movePlayer();
        }

        if(_isPointing){
            updateObjectiveArrow();
        }
    }

    public Joystick _joystick;
    CharacterController _characterController;

    bool _playerControllerActive = true;
    public void togglePlayerController(bool value){
        _playerControllerActive = value;
        _joystick.enabled = value;
    }

    
    public Transform _playerModel;
    public Animator _playerAnimator;

    #region Movement
        float _moveSpeed = 5f;
        float _rotateSpeed = 3f;
        void movePlayer(){
            //Move player
            Vector3 move = transform.right * _joystick.Horizontal + transform.forward * _joystick.Vertical;
            if(move.magnitude < 0.01){
                return;
            }
            _characterController.Move(move * _moveSpeed * Time.deltaTime);

            //Rotate player to move vector, using slerp
            Quaternion targetRot = Quaternion.LookRotation(move);
            _playerModel.rotation = Quaternion.Slerp(_playerModel.rotation,targetRot,_rotateSpeed * Time.deltaTime);
        }
    #endregion

    #region Objective Pointer

        public Transform _objectivePointerArrow;
        Transform _objective;
        bool _isPointing = false;
        public void startObjectivePointer(Transform objective=null)
        {
            if(objective != null){
                _objective = objective;
            }
            _objectivePointerArrow.gameObject.SetActive(true);
            _isPointing = true;
        }

        void updateObjectiveArrow(){
            _objectivePointerArrow.LookAt(_objective.position);
        }

        public void stopObjectivePointer(){
            _objectivePointerArrow.gameObject.SetActive(false);
            _isPointing = false;
        }

    #endregion
}
