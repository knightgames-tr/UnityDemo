using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using MyBox;
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
        _standPoints[0].togglePoint(true);
        
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
                StartCoroutine(processStandPoint1());
            }else if(pointNo == 2){
                StartCoroutine(processStandPoint2());
            }else if(pointNo == 3){
                StartCoroutine(processStandPoint3());
            }else if(pointNo == 4){
                StartCoroutine(processStandPoint4());
            }
        }    
        
        public void offStandPoint(int pointNo){
            _playerController.startObjectivePointer(_standPoints[pointNo].transform);
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
            float _part2SpawnTime = 0.2f;
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

            [Separator]
            public Transform _baggageParent;
            float _standPoint1WaitTime = 0.1f;
            IEnumerator processStandPoint1(){
                while(true){
                    if(!_lines[0]._isLineActive){
                        yield break;
                    }

                    if(!_standPoints[1].getIsStanding()){
                        yield break;
                    }

                    if(_lineReadyStates[0] == false){
                        yield return new WaitForSeconds(_standPoint1WaitTime);
                    }else{
                        _lineReadyNPC[0]._baggage.parent = null;
                        _lineReadyNPC[0]._baggage.DOMoveInTargetLocalSpace(_baggageParent.transform,Vector3.zero,1f).OnComplete(()=>{
                            _lineReadyNPC[0]._baggage.parent = _baggageParent.transform;
                        });
                        _lineReadyNPC[0].toggleBaggageCarry(false);
                        yield return new WaitForSeconds(1f);
                        _playerController.toggleBaggageCarry(true);
                        _lines[1].addNPCToLine(_lineReadyNPC[0]);
                        _lineReadyStates[0] = false;
                        _lines[0].processQueue();
                    }

                }
            }

            public Transform _baggagePut;
            float _standPoint2WaitTime = 0.3f;
            float _childCount;
            IEnumerator processStandPoint2(){
                while(true){
                    if(!_standPoints[2].getIsStanding()){
                        yield break;
                    }

                    if(_baggageParent.childCount < 1){
                        //Line completed
                        _playerController.toggleBaggageCarry(false);
                        _standPoints[2].togglePoint(false);
                        _standPoints[3].togglePoint(true);
                        _playerController.startObjectivePointer(_standPoints[3].transform);
                        _childCount = 0;
                        yield break;
                    }

                    Transform child = _baggageParent.GetChild(0);
                    child.parent = _baggagePut.transform;
                    child.DOLocalMove(new Vector3(0,_childCount,0),_standPoint2WaitTime);
                    _childCount++;
                    yield return new WaitForSeconds(_standPoint2WaitTime);
                }
            }

            float _standPoint3WaitTime = 0.3f;
            public Transform _truck;
            IEnumerator processStandPoint3(){
                while(true){
                    if(!_standPoints[3].getIsStanding()){
                        yield break;
                    }

                    if(_baggagePut.childCount < 1){
                        //Line completed
                        _standPoints[3].togglePoint(false);
                        _standPoints[4].togglePoint(true);
                        _playerController.startObjectivePointer(_standPoints[3].transform);

                        foreach(StairController stair in _part2Stairs){
                            stair.toggleForPlayer(true);
                        }
                        yield break;
                    }

                    Transform child = _baggagePut.GetChild(0);
                    child.parent = _truck.transform;
                    child.DOLocalMove(new Vector3(0,_childCount,0),_standPoint3WaitTime);
                    _childCount++;
                    yield return new WaitForSeconds(_standPoint2WaitTime);
                }
            }

            public Transform _plane;
            public GameObject _moneyPrefab;
            public List<Transform> _moneyPutPlaces;
            int _moneyPutCount;
            float _currentMoneyPutOffset;
            float _moneyPutOffset = 0.12f;
            float _standPoint4WaitTime = 0.1f;
            IEnumerator processStandPoint4(){
                while(true){
                    if(!_lines[2]._isLineActive){
                        _standPoints[5].togglePoint(true);
                        yield break;
                    }

                    if(!_standPoints[4].getIsStanding()){
                        yield break;
                    }

                    if(_lineReadyStates[2] == false){
                        yield return new WaitForSeconds(_standPoint4WaitTime);
                    }else{
                        //Create money objects and move them to pos
                        Transform moneyObject = Instantiate(_moneyPrefab,_lines[2].transform.position,Quaternion.identity).transform;
                        moneyObject.GetComponent<MoneyStackTrigger>().Init();
                        moneyObject.DOMove(_moneyPutPlaces[_moneyPutCount].position+new Vector3(0,_currentMoneyPutOffset,0),1f)
                        .OnComplete(()=>{moneyObject.GetComponent<MoneyStackTrigger>().activateTrigger();});
                        if(++_moneyPutCount == _moneyPutPlaces.Count){
                            _moneyPutCount = 0;
                            _currentMoneyPutOffset += _moneyPutOffset;
                        }

                        _lines[3].addNPCToLine(_lineReadyNPC[2]);
                        _lineReadyStates[2] = false;
                        _lines[2].processQueue();
                        yield return new WaitForSeconds(1f);
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
                }else if(lineNo == 3){
                    Destroy(npc.gameObject);
                    _lines[3].processQueue();
                }
            }
        }

        public void lineCompleted(int lineNo){
            if(lineNo == 0){
                _standPoints[1].togglePoint(false);
                _standPoints[2].togglePoint(true);
                _playerController.startObjectivePointer(_standPoints[2].transform);
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
