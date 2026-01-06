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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Money

        public UnityEvent _scoreUpdatedEvent;
        int _currentMoney = 50;
        public TextMeshProUGUI _moneyText;

        public bool updateMoney(int changeAmount){
            if(_currentMoney+changeAmount < 0){
                return false;
            }
            
            _scoreUpdatedEvent.Invoke();

            _currentMoney += changeAmount;
            _moneyText.text = _currentMoney+"";

            return true;
        }

        public int getCurrentMoney(){
            return _currentMoney;
        }


    #endregion
}
