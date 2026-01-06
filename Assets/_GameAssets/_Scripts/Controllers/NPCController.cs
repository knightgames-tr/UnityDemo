using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    LevelManager _levelManager;
    void Start()
    {
        _levelManager = LevelManager.Instance;

        _npcAgent = GetComponent<NavMeshAgent>();

        //Play Idle Anim 
        _currentAnimState = AnimStates.Idle;
        _npcAnimator.Play(AnimStates.Idle.ToString());        
        toggleBaggageCarry(true);
        _toggleBag = 1;
    }

    void Update(){
        if(_npcAgentState){
            checkReachedDestination();
        }
        
        animationController();
    }

    #region Navmesh Agent

        NavMeshAgent _npcAgent;
        bool _npcAgentState = true;

        public void toggleNPCAgent(bool value){
            _npcAgent.enabled = value;
            _npcAgentState = value;
        }

        bool _movement = false;

        void checkReachedDestination(){
            if (!_npcAgent.pathPending){
                if (_npcAgent.remainingDistance <= _npcAgent.stoppingDistance){
                    if (!_npcAgent.hasPath || _npcAgent.velocity.sqrMagnitude == 0f){

                        if(_arrived == false){
                            reachedDestination();
                        }
                        
                    }
                }
            }
        }

        bool _arrived = true;
        
        void reachedDestination(){
            _arrived = true;
            _movement = false;

            reachedQueuePosition();

            rotateToReachedPoint();

        }

        Vector3 _reachedPoint;
        public void setReachedPoint(Vector3 reachedPoint){
            _reachedPoint = reachedPoint;
        }

        public void rotateToReachedPoint(){
            if(_reachedPoint != null){
                transform.rotation = Quaternion.LookRotation(_reachedPoint);
            }
        }

        
        Vector3 getPointOnNavmesh(Vector3 point){
            NavMeshHit navHit;
            NavMesh.SamplePosition(point, out navHit, 1 , NavMesh.AllAreas);
            return navHit.position;
        }

    #endregion

    #region Animation
        
        enum AnimStates{
            Idle,
            Run
        }
        AnimStates _currentAnimState;
        public Animator _npcAnimator;
        bool _isBaggageOn;
        float _toggleBag;
        float _toggleBagSpeed=3f;
        void animationController(){
            //Set run idle states
            if(_currentAnimState != AnimStates.Run && _npcAgent.velocity.magnitude >= 0.01f){
                _currentAnimState = AnimStates.Run;
                _npcAnimator.Play(AnimStates.Run.ToString());
            }else if(_currentAnimState != AnimStates.Idle && _npcAgent.velocity.magnitude < 0.01f){
                _currentAnimState = AnimStates.Idle;
                _npcAnimator.Play(AnimStates.Idle.ToString());
            }

            //Toggle baggage carry
            if(_isBaggageOn && _toggleBag < 1){
                _toggleBag += Time.deltaTime*_toggleBagSpeed;
                if(_toggleBag >= 1){
                    _toggleBag = 1;
                }
                _npcAnimator.SetLayerWeight(1,_toggleBag);
            }else if(!_isBaggageOn && _toggleBag > 0){
                _toggleBag -= Time.deltaTime*_toggleBagSpeed;
                if(_toggleBag <= 0){
                    _toggleBag = 0;
                }
                _npcAnimator.SetLayerWeight(1,_toggleBag);
            }
        }
        
        public void toggleBaggageCarry(bool value){
            _isBaggageOn = value;
        }

    #endregion

    public Transform _baggage;

    int _currentQueueNo = -1;
    int _currentLineNo = -1;
    public void moveToPosition(Vector3 targetPosition, int queueNo, int lineNo){
        toggleNPCAgent(true);
        _npcAgent.SetDestination(getPointOnNavmesh(targetPosition));
        setReachedPoint(targetPosition);

        _arrived = false;
        _movement = true;

        _currentQueueNo = queueNo;
        _currentLineNo = lineNo;
    }

    public void reachedQueuePosition(){
        _levelManager.reachedQueuePosition(_currentQueueNo, _currentLineNo, this);
    }
    public void reachedStairTop(){
        _levelManager.reachedStairTop(this);
    }
}
