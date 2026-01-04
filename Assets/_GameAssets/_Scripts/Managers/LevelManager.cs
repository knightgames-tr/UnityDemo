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
        ((PaymentPoint)_standPoints[0]).activatePaymentPoint();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Stand Points

        public List<StandPoint> _standPoints;

        public void onStandPoint(int pointNo){
            _playerController.stopObjectivePointer();
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
                        });
            }

        #endregion

    #endregion

    #region Line Controllers

        public void processLine(int lineNo, NPCController npc){
            
        }

    #endregion
}
