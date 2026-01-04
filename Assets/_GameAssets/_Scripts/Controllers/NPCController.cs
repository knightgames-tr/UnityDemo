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
        toggleNPCAgent(false);
    }

    void Update(){
        if(_npcAgentState){
            checkReachedDestination();
        }
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
            toggleNPCAgent(false);

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
