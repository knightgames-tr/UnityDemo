using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoneyStackTrigger : MonoBehaviour
{
    PlayerController _playerController;
    public void Init(){
        _playerController = PlayerController.Instance;
    }

    bool _isTriggerActive;
    public void activateTrigger(){
        _isTriggerActive = true;
    }

    bool _triggered;
    public List<Transform> _money;
    float _moveTime = 0.5f;
    void OnTriggerEnter(Collider other) {
        if(!_isTriggerActive || _triggered){
            return;
        }

        if(other.transform.tag == "Player"){
            _triggered = true;
            for(int i=0; i<_money.Count-1;i++){
                _money[i].DOJump(_playerController.transform.position, 3, 1,_moveTime);
                _money[i].DOScale(Vector3.zero,_moveTime*1.25f);
            }
            _money[_money.Count-1].DOJump(_playerController.transform.position, 3, 1,_moveTime);
            _money[_money.Count-1].DOScale(Vector3.zero,_moveTime*1.25f).OnComplete(()=>Destroy(this.gameObject));
        }
    }
}
