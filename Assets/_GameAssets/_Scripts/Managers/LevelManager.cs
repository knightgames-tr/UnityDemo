using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    void Awake(){
        Instance = this;
    }

    PlayerController _playerController;
    void Start()
    {
        _playerController = PlayerController.Instance;

        initLineReadyStates();

        //Activate first payment point
        ((PaymentPoint)_standPoints[0]).activatePaymentPoint();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<NPCController> _NPCs;

    #region Stand Points

        public List<StandPoint> _standPoints;

        public void onStandPoint(int pointNo){
            _playerController.stopObjectivePointer();

            if(pointNo == 1){
                _standPointRoutine = StartCoroutine(processStandPoint1());
            }
        }    
        
        public void offStandPoint(int pointNo){
            _playerController.startObjectivePointer(_standPoints[pointNo].transform);

            if(_standPointRoutine != null){
                StopCoroutine(_standPointRoutine);
            }
        }

        public void donePaymentPoint(int pointNo){
            if(pointNo == 0){
                activatePart2Environment();
            }
        }

        #region Point Specific Actions

            [Header ("Part 2 Objects")]
            public List<Transform> _part2Environment;
            public CinemachineVirtualCamera _part2Camera;
            public List<StairController> _part2Stairs;
            float _part2SpawnTime = 0.5f;
            void activatePart2Environment(){
                _playerController.togglePlayerController(false);
                _playerController.stopObjectivePointer();
                _part2Camera.Priority += 2;

                float delay = 0.75f;
                for(int i=0;i<_part2Environment.Count-1;i++){
                    _part2Environment[i].DOScale(1,_part2SpawnTime).SetDelay(delay);
                    delay += _part2SpawnTime;
                }

                _part2Environment[_part2Environment.Count-1].DOScale(1,_part2SpawnTime)
                    .SetDelay(delay)
                    .OnComplete(()=>{
                        //Toggle back to player camera and show next objective
                        _part2Camera.Priority -= 2;
                        _playerController.togglePlayerController(true);
                        _standPoints[1].togglePoint(true);
                        ((ActionPoint)_standPoints[1]).highLightActionPoint();
                        _playerController.startObjectivePointer(_standPoints[1].transform);

                        //Start stair movement
                        foreach(StairController stair in _part2Stairs){
                            stair.startStairMovement();
                        }
                        
                        foreach(NPCController npc in _NPCs){
                            _lines[0].addNPCToLine(npc);
                        }
                    });
            }

            Coroutine _standPointRoutine;
            float _standPoint1WaitTime = 1f;
            IEnumerator processStandPoint1(){
                while(true){
                    if(!_lines[0]._isLineActive){
                        yield break;
                    }

                    if(_lineReadyStates[0] == false){
                        yield return new WaitForSeconds(_standPoint1WaitTime);
                    }else{
                        _lineReadyNPC[0]._baggage.parent = _playerController.transform;
                        _lineReadyNPC[0]._baggage.DOLocalMove(_playerController.transform.localPosition,1f);
                        yield return new WaitForSeconds(1f);
                        _lines[1].addNPCToLine(_lineReadyNPC[0]);
                        _lines[0].processQueue();
                    }

                }
            }

        #endregion

    #endregion

    #region Line Controllers

        public List<LineController> _lines;
        bool[] _lineReadyStates;
        NPCController[] _lineReadyNPC;
        void initLineReadyStates(){
            _lineReadyStates = new bool[_lines.Count];
            _lineReadyNPC = new NPCController[_lines.Count];
        }

        public void reachedQueuePosition(int queueNo, int lineNo, NPCController npc){
            if(queueNo == 0){
                _lineReadyStates[lineNo] = true;
                _lineReadyNPC[lineNo] = npc;

                //Process stair
                if(lineNo == 1){
                    putNPCToStair(npc.transform);
                }
            }
        }

        #region Line Specific Actions

            void putNPCToStair(Transform npc){
                _part2Stairs[0].addToStairQueue(npc);
                _lines[1].processQueue();
            }

            public void reachedStairTop(NPCController npc){
                _lines[2].addNPCToLine(npc);
            }

        #endregion

    #endregion


}
