using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    void Awake(){
        Instance = this;
    }

    PaintManager _paintManager;
    PlayerController _playerController;
    void Start()
    {
        _paintManager = PaintManager.Instance;
        _playerController = PlayerController.Instance;

        initLineReadyStates();

        //Activate first payment point
        _standPoints[0].togglePoint(true);
        _baggages = new List<Transform>();
        
    }

    public List<NPCController> _npcs;

    #region Stand Points

        public List<StandPoint> _standPoints;

        public void onStandPoint(int pointNo){
            _playerController.stopObjectivePointer();

            if(pointNo == 0){
                _playerController.toggleMoneyParticles(true);
            }else if(pointNo == 1){
                StartCoroutine(processStandPoint1());
            }else if(pointNo == 2){
                StartCoroutine(processStandPoint2());
            }else if(pointNo == 3){
                StartCoroutine(processStandPoint3());
            }else if(pointNo == 4){
                StartCoroutine(processStandPoint4());
            }else if(pointNo == 5){
                _playerController.toggleMoneyParticles(true);
            }
        }    
        
        public void offStandPoint(int pointNo){
            _playerController.toggleMoneyParticles(false);
            _playerController.startObjectivePointer(_standPoints[pointNo].transform);
        }

        public void donePaymentPoint(int pointNo){
            if(pointNo == 0){
                _playerController.toggleMoneyParticles(false);
                activatePart2Environment();
            }else if(pointNo == 5){
                _playerController.toggleMoneyParticles(false);
                StartCoroutine(processStandPoint5());
            }
        }

        bool _standWait = false;

        #region Point Specific Actions

            [Separator]
            [Header ("Part 2 Objects")]
            public List<Transform> _part2Environment;
            public CinemachineVirtualCamera _part2Camera;
            public List<StairController> _part2Stairs;
            float _part2SpawnTime = 0.4f;
            void activatePart2Environment(){
                _playerController.togglePlayerController(false);
                _playerController.stopObjectivePointer();
                _part2Camera.Priority += 2;

                float delay = _part2SpawnTime;
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
                        
                        foreach(NPCController npc in _npcs){
                            _lines[0].addNPCToLine(npc);
                        }
                    });
            }

            [Separator]
            public Transform _baggagePlayerParent;
            public List<Transform> _baggages;
            float _baggageOffset = 0.4f;
            float _standPoint1WaitTime = 0.1f;
            IEnumerator processStandPoint1(){
                while(true){
                    if(_standWait){
                        yield break;
                    }

                    if(!_lines[0]._isLineActive){
                        yield break;
                    }

                    if(!_standPoints[1].getIsStanding()){
                        yield break;
                    }

                    if(_lineReadyStates[0] == false){
                        yield return new WaitForSeconds(_standPoint1WaitTime);
                    }else{

                        //Put baggage under player transform
                        Transform baggage = _lineReadyNPC[0]._baggage;

                        _baggages.Add(baggage);
                        baggage.parent = null;
                        baggage.DOJump(_baggagePlayerParent.transform.position*0.95f+new Vector3(0,_baggages.IndexOf(baggage)*_baggageOffset,0),2f,1,0.4f).OnComplete(()=>{
                            baggage.DOMoveInTargetLocalSpace(_baggagePlayerParent.transform,new Vector3(0,_baggages.IndexOf(baggage)*_baggageOffset,0),0.1f).OnComplete(()=>{
                                baggage.parent = _baggagePlayerParent.transform;
                                baggage.DOLocalRotate(new Vector3(0,0,90),0.5f);
                            });
                        });

                        _lineReadyNPC[0].toggleBaggageCarry(false);

                        _standWait = true;
                        yield return new WaitForSeconds(0.3f);
                        _standWait = false;

                        _playerController.toggleBaggageCarry(true);
                        _lines[1].addNPCToLine(_lineReadyNPC[0]);
                        _lineReadyStates[0] = false;
                        _lines[0].processQueue();
                    }

                }
            }

            [Separator]
            public Transform _baggagePutPosition;
            float _standPoint2WaitTime = 0.3f;
            int _currentBaggageIndex;
            IEnumerator processStandPoint2(){
                while(true){
                    if(_standWait){
                        yield break;
                    }

                    if(!_standPoints[2].getIsStanding()){
                        yield break;
                    }

                    if(_baggagePlayerParent.childCount < 1){
                        //Line completed
                        _currentBaggageIndex = _baggages.Count-1;
                        _playerController.toggleBaggageCarry(false);
                        _standPoints[2].togglePoint(false);
                        _standPoints[3].togglePoint(true);
                        _playerController.startObjectivePointer(_standPoints[3].transform);
                        yield break;
                    }

                    _baggages[_currentBaggageIndex].parent = null;
                    _baggages[_currentBaggageIndex].DOJump(_baggagePutPosition.position+new Vector3(0,_baggageOffset*(_baggages.Count-1-_currentBaggageIndex),0),3,1,_standPoint2WaitTime);
                    _baggages[_currentBaggageIndex].DOLocalRotate(_baggagePutPosition.eulerAngles,_standPoint2WaitTime);
                    _currentBaggageIndex--;
                    
                    _standWait = true;
                    yield return new WaitForSeconds(_standPoint2WaitTime);
                    _standWait = false;
                }
            }

            [Separator]
            float _standPoint3WaitTime = 0.6f;
            public Transform _baggageJumpPosition;
            public Transform _jumpPadObject;
            public Transform _truckBaggagePosition;
            IEnumerator processStandPoint3(){
                while(true){
                    if(_standWait){
                        yield break;
                    }

                    if(!_standPoints[3].getIsStanding()){
                        yield break;
                    }

                    if(_currentBaggageIndex < 0){
                        //Line completed
                        _standPoints[3].togglePoint(false);
                        _standPoints[4].togglePoint(true);
                        _playerController.startObjectivePointer(_part2Stairs[0].highLightArrow());
                        moveTruckAway();

                        foreach(StairController stair in _part2Stairs){
                            stair.toggleForPlayer(true);
                        }
                        yield break;
                    }

                    //Move upper baggages one step down
                    for(int i=_currentBaggageIndex-1;i>-1;i--){
                        _baggages[i].DOMove(_baggages[i+1].position,_standPoint3WaitTime/3);
                    }

                    //Move current baggage to the truck
                    Sequence baggageSequence = DOTween.Sequence();
                    baggageSequence.Append(_baggages[_currentBaggageIndex].DOMove(_baggageJumpPosition.position,_standPoint3WaitTime/3))
                    .Append(_baggages[_currentBaggageIndex].DOJump(_jumpPadObject.position,1,1,_standPoint3WaitTime/3))
                    .Append(_baggages[_currentBaggageIndex].DOJump(_truckBaggagePosition.position+new Vector3(0,_baggageOffset*(_baggages.Count-1-_currentBaggageIndex),0),3,1,(_standPoint3WaitTime/3)*2))
                    .OnComplete(()=>{
                            _baggages[_currentBaggageIndex].parent = _truckBaggagePosition;
                            _currentBaggageIndex--;
                    });

                    //Move jumppad up and down
                    _jumpPadObject.DOMove(_jumpPadObject.position+new Vector3(0,2,0),_standPoint3WaitTime/3).SetDelay((_standPoint3WaitTime/3)*2).OnComplete(()=>{
                        _jumpPadObject.DOMove(_jumpPadObject.position-new Vector3(0,2,0),_standPoint3WaitTime/3);
                    });

                    _standWait = true;
                    yield return new WaitForSeconds(_standPoint3WaitTime*(5/3f));
                    _standWait = false;
                }
            }

            public Transform _truck;
            float _truckMoveAmount = -30f;
            void moveTruckAway(){
                _truck.DOLocalMoveZ(_truck.position.z+_truckMoveAmount,2f).OnComplete(()=>{
                    _truck.DOLocalMoveZ(_truck.position.z-_truckMoveAmount,2f);
                    int baggageCount = _baggages.Count;
                    for(int i=0;i<baggageCount;i++){
                        Destroy(_baggages[i].gameObject);
                    }
                });
            }

            [Separator]
            public Transform _plane;
            public GameObject _moneyPrefab;
            public List<Transform> _moneyPutPlaces;
            int _moneyPutCount;
            float _currentMoneyPutOffset;
            float _moneyPutOffset = 0.12f;
            float _standPoint4WaitTime = 0.1f;
            IEnumerator processStandPoint4(){
                while(true){
                    if(_standWait){
                        yield break;
                    }

                    if(!_lines[2]._isLineActive){
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

                        _standWait = true;
                        yield return new WaitForSeconds(1f);
                        _standWait = false;
                    }

                }
            }

            [Separator]
            public CinemachineVirtualCamera _paintCamera;
            IEnumerator processStandPoint5(){
                _playerController.togglePlayerController(false);
                _playerController.stopObjectivePointer();
                _paintCamera.Priority += 2;
                yield return new WaitForSeconds(Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time);
                _paintManager.activatePaintManager();
            }

        #endregion

    #endregion

    #region Line Controllers

        [Separator]
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
                    reachedAirplane(npc);
                }
            }
        }

        public void lineCompleted(int lineNo){
            if(lineNo == 0){
                _currentBaggageIndex = _baggages.Count-1;
                _standPoints[1].togglePoint(false);
                _standPoints[2].togglePoint(true);
                _playerController.startObjectivePointer(_standPoints[2].transform);
            }else if(lineNo == 2){
                _standPoints[4].togglePoint(false);
                _playerController.startObjectivePointer(_standPoints[5].transform);
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

            public void playerReachedStairTop(){
                _part2Stairs[0].dehighLightArrow();
                _playerController.startObjectivePointer(_standPoints[4].transform);
            }

            int _passengerCounter;
            public TextMeshProUGUI _passengerCounterText;
            public Transform _airplaneTransform;
            public Transform _airplaneDonePosition;
            void reachedAirplane(NPCController npc){
                Destroy(npc.gameObject);
                _lines[3].processQueue();

                _passengerCounterText.text = ++_passengerCounter+"/"+_npcs.Count;
                if(_passengerCounter == _npcs.Count){
                    _airplaneTransform.DOMove(_airplaneDonePosition.position,1f);
                }
            }

        #endregion

    #endregion


}
