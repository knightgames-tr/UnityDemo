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

        //Play Idle Anim 
        _currentAnimState = AnimStates.Idle;
        _playerAnimator.Play(AnimStates.Idle.ToString());   
    }

    void Update(){
        movePlayer();

        if(_isPointing){
            updateObjectiveArrow();
        }
        
        animationController();
    }

    public Joystick _joystick;
    CharacterController _characterController;

    bool _playerControllerActive = true;
    public void togglePlayerController(bool value){
        _playerControllerActive = value;

        if(value){
            _joystick.enabled = value;
            _joystick.OnPointerUp(null);
        }else{
            _joystick.OnPointerUp(null);
            _joystick.enabled = value;
        }
    }

    
    public Transform _playerModel;

    #region Movement
        float _moveSpeed = 8f;
        float _rotateSpeed = 10f;
        Vector3 _movementVector;
        void movePlayer(){
            if(!_playerControllerActive){
                _movementVector = Vector3.zero;
                return;
            }

            //Move player
            _movementVector = transform.right * _joystick.Horizontal + transform.forward * _joystick.Vertical;
            if(_movementVector.magnitude < 0.01){
                return;
            }
            _characterController.Move(_movementVector * _moveSpeed * Time.deltaTime);

            //Rotate player to move vector, using slerp
            Quaternion targetRot = Quaternion.LookRotation(_movementVector);
            _playerModel.rotation = Quaternion.Slerp(_playerModel.rotation,targetRot,_rotateSpeed * Time.deltaTime);
        }
    #endregion

    #region Animation
        
        enum AnimStates{
            Idle,
            Run
        }
        AnimStates _currentAnimState;
        public Animator _playerAnimator;
        bool _isBaggageOn;
        float _toggleBag;
        float _toggleBagSpeed=1f;
        void animationController(){
            //Set run idle states
            if(_currentAnimState != AnimStates.Run && _movementVector.magnitude >= 0.01f){
                _currentAnimState = AnimStates.Run;
                _playerAnimator.Play(AnimStates.Run.ToString());
            }else if(_currentAnimState != AnimStates.Idle && _movementVector.magnitude < 0.01f){
                _currentAnimState = AnimStates.Idle;
                _playerAnimator.Play(AnimStates.Idle.ToString());
            }

            //Toggle baggage carry
            if(_isBaggageOn && _toggleBag < 1){
                _toggleBag += Time.deltaTime*_toggleBagSpeed;
                if(_toggleBag >= 1){
                    _toggleBag = 1;
                }
                _playerAnimator.SetLayerWeight(1,_toggleBag);
            }else if(!_isBaggageOn && _toggleBag > 0){
                _toggleBag -= Time.deltaTime*_toggleBagSpeed;
                if(_toggleBag <= 0){
                    _toggleBag = 0;
                }
                _playerAnimator.SetLayerWeight(1,_toggleBag);
            }
        }
        
        public void toggleBaggageCarry(bool value){
            _isBaggageOn = value;
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
