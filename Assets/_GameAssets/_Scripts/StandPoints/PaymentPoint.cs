using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using DG.Tweening;

public class PaymentPoint : StandPoint
{
    public override void Start(){
        base.Start();

        _gameManager._scoreUpdatedEvent.AddListener(checkIfCanBeActivated);

        _currentPrice = _pointPrice;
        _priceText.text = _pointPrice+"";
    }

    void Update(){
        if(_isStanding){
            paymentTimer();
        }
    }

    [Header ("Payment Point Settings")]
    public int _pointPrice = 50;
    public TextMeshProUGUI _priceText;
    public Image _iconFiller;
    public Image _moneyIcon;
    public Image _pointIcon;
    public Renderer _arrowMesh;

    float _currentPrice;
    float _maxTimer = 0.01f;
    float _timer;
    void paymentTimer(){
        _timer += Time.deltaTime;

        if(_timer > _maxTimer){
            _timer = 0;

            if(_gameManager.updateMoney(-1)){
                //Update price
                _currentPrice -= 1;

                //Update text and filler
                _priceText.text = _currentPrice+"";
                _iconFiller.fillAmount = (_pointPrice-_currentPrice) / _pointPrice;

                //Check if done
                if(_currentPrice == 0){
                    togglePoint(false);
                }

            }else{
                _isStanding = false;
                _levelManager.offStandPoint(_pointNo);
            }
        }
    }


    float _fadeTime = 0.75f;
    Ease _fadeEase = Ease.Linear;

    public override void togglePoint(bool value)
    {
        base.togglePoint(value);
        if(value){
            _priceText.DOFade(1,_fadeTime).SetEase(_fadeEase);
            _moneyIcon.DOFade(1,_fadeTime).SetEase(_fadeEase);
            _pointIcon.DOFade(1,_fadeTime).SetEase(_fadeEase);
            _iconFiller.DOFade(1,_fadeTime).SetEase(_fadeEase).OnComplete(()=>{
                _arrowMesh.enabled = true;
            });
        }else{
            _isStanding = false;
            _levelManager.offStandPoint(_pointNo);
            _levelManager.donePaymentPoint(_pointNo);
            _arrowMesh.enabled = false;

            transform.DOScale(0,0.5f)
            .OnComplete(()=>{Destroy(gameObject);});
        }
    }

    void checkIfCanBeActivated(){
        if(_gameManager.getCurrentMoney() >= _currentPrice){
            togglePoint(true);

            _gameManager._scoreUpdatedEvent.RemoveListener(checkIfCanBeActivated);
        }
    }
}
