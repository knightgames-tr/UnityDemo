using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StairController : MonoBehaviour
{
    PlayerController _playerController;
    LevelManager _levelManager;
    void Start()
    {
        _playerController = PlayerController.Instance;
        _levelManager = LevelManager.Instance;
        initStepPositions();
    }

    void Update()
    {
        
    }

    public Transform _startStep;
    public Transform _endStep;
    public Transform _donePosition;
    List<Transform> _steps;
    int _stepCount = 10;
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

    float _stepSpeed = 0.7f;
    public void startStairMovement(){
        for(int i=0;i<_steps.Count;i++){
            Vector3 movePosition = Vector3.Lerp(_startStep.position,_endStep.position,(i+1f)/_stepCount);
            _steps[i].DOMove(movePosition,_stepSpeed).SetEase(Ease.Linear).SetLoops(-1);
        }
            
        _steps[_steps.Count-1].DOMove(_endStep.position,_stepSpeed).SetEase(Ease.Linear).SetLoops(-1)
        .OnComplete(()=>{stepReached();});
    }

    void stepReached(){
        for(int i=0;i<_steps.Count;i++){
            if(_steps[i].childCount > 0){
                if(i == _stepCount-1){
                    //Stair completed for character,move to next pos
                    Transform character = _steps[i].GetChild(0);
                    character.parent = null;
                    character.transform.position = _donePosition.position;
                    if(character.tag == "Player"){
                        _playerController.togglePlayerController(true);
                        _playerController.startObjectivePointer();
                    }else if(character.tag == "NPC"){
                        character.GetComponent<NPCController>().
                    }
                }else{
                    _steps[i].GetChild(0).parent = _steps[i+1];
                }
            }
        }
    }
}
