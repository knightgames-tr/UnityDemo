using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake(){
        Instance = this;
    }

    #region Money

        public UnityEvent _scoreUpdatedEvent;
        int _currentMoney = 50;
        public GameObject _moneyUI;
        public TextMeshProUGUI _moneyText;

        public bool updateMoney(int changeAmount){
            if(_currentMoney+changeAmount < 0){
                return false;
            }

            _currentMoney += changeAmount;
            _moneyText.text = _currentMoney+"";

            _scoreUpdatedEvent.Invoke();

            return true;
        }

        public int getCurrentMoney(){
            return _currentMoney;
        }

        public void deactivateMoneyUI(){
            _moneyUI.SetActive(false);
        }


    #endregion
}
