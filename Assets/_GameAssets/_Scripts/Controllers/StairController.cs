using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StairController : MonoBehaviour
{
    public int _lineNo;
    PlayerController _playerController;
    LevelManager _levelManager;
    void Start()
    {
        _playerController = PlayerController.Instance;
        _levelManager = LevelManager.Instance;

        _stairQueue = new List<Transform>();
        initStepPositions();
    }

    public Collider _stairPlayerCollider;
    public void toggleForPlayer(bool value){
        _stairPlayerCollider.enabled = value;
    }

    public Transform _startStep;
    public Transform _endStep;
    public Transform _donePosition;
    List<Transform> _steps;
    int _stepCount = 17;
    void initStepPositions(){
        _steps = new List<Transform>();

        //Change parent scale to calculate spawn positions
        transform.parent.localScale = Vector3.one;
        transform.localScale = Vector3.one;

        for(int i=0;i<_stepCount;i++){
            Vector3 spawnPosition = Vector3.Lerp(_startStep.position,_endStep.position,((float)i)/_stepCount);
            GameObject newStep = Instantiate(_startStep.gameObject,spawnPosition,Quaternion.identity);
            newStep.transform.parent = transform;
            _steps.Add(newStep.transform);
        }

        transform.parent.localScale = Vector3.zero;
        transform.localScale = Vector3.zero;
    }

    float _stepSpeed = 0.2f;
    public void startStairMovement(){
        for(int i=0;i<_steps.Count-1;i++){
            Vector3 movePosition = Vector3.Lerp(_startStep.position,_endStep.position,(i+1f)/_stepCount);
            _steps[i].DOMove(movePosition,_stepSpeed).SetEase(Ease.Linear).SetLoops(-1);
        }
            
        _steps[_steps.Count-1].DOMove(_endStep.position,_stepSpeed).SetEase(Ease.Linear).SetLoops(-1)
        .OnStepComplete(()=>{stepReached();});
    }

    List<Transform> _stairQueue;
    public void addToStairQueue(Transform item){
        _stairQueue.Add(item);

        if(item.tag == "Player"){
            _playerController.togglePlayerController(false);
            _playerController.stopObjectivePointer();
        }
    }
    void stepReached(){
        //Move each item to upper step
        for(int i=_steps.Count-1;i>-1;i--){
            if(_steps[i].childCount > 0){
                if(i == _stepCount-1){
                    //Stair completed for item,move to next pos
                    Transform item = _steps[i].GetChild(0);
                    item.parent = null;
                    item.transform.position = _donePosition.position;

                    if(item.tag == "Player"){
                        _playerController.togglePlayerController(true);
                        _playerController.startObjectivePointer();
                        _levelManager.playerReachedStairTop();
                    }else if(item.tag == "NPC"){
                        item.GetComponent<NPCController>().reachedStairTop();
                    }
                }else if(i == 0){
                    _steps[i].GetChild(0).parent = _steps[i+1];
                    _steps[i+1].GetChild(0).localPosition = Vector3.zero;    
                    Vector3 dir = _startStep.right;
                    dir.y = 0f;

                    Transform item = _steps[i].GetChild(0);
                    if(item.tag == "Player"){
                        _steps[i+1].GetComponent<PlayerController>()._playerModel.DORotateQuaternion(Quaternion.LookRotation(dir), 0.1f);
                    }else{
                        _steps[i+1].GetChild(0).DORotateQuaternion(Quaternion.LookRotation(dir), 0.1f);
                    }
                }else{
                    _steps[i].GetChild(0).parent = _steps[i+1];
                    _steps[i+1].GetChild(0).localPosition = Vector3.zero;

                }
            }
        }

        //Add new item from queue
        if(_stairQueue.Count > 0){
            //Toggle navmesh agent if npc
            if(_stairQueue[0].tag == "NPC"){
                _stairQueue[0].GetComponent<NPCController>().toggleNPCAgent(false);
            }
            _stairQueue[0].transform.parent = _steps[0];
            _stairQueue[0].transform.localPosition = Vector3.zero;
            _stairQueue.RemoveAt(0);
        }
    }

    public Renderer _arrow;
    float _highLightTime = 1f;
    public Transform highLightArrow(){
        _arrow.material.DOColor(Color.green,_highLightTime);
        return _arrow.transform;
    }

    public void dehighLightArrow(){
        _arrow.material.DOColor(Color.white,_highLightTime);
    }
}
