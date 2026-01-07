using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ImageAnimation : MonoBehaviour
{
    void Start()
    {
        if(_positionAnimation){
            positionAnimation();
        }
        if(_rotationAnimation){
            rotationAnimation();
        }
        if(_scaleAnimation){
            scaleAnimation();
        }
    }

    [Header ("Position Animation")]
    public bool _positionAnimation = true;
    public float _posChangeValue = 25f;
    public float _posChangeTime = 1.5f;
    void positionAnimation(){
        //Set first position
        float posChangeHalf = _posChangeValue * -0.5f;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + posChangeHalf, gameObject.transform.localPosition.z);

        Vector3 animPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + _posChangeValue, gameObject.transform.localPosition.z);
        gameObject.transform.DOLocalMove(animPos,_posChangeTime).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }


    [Header ("Rotation Animation")]
    public bool _rotationAnimation = true;
    public float _rotChangeValue = 2.5f;
    public float _rotChangeTime = 2.5f;
    void rotationAnimation(){
        //Set first rotation
        float rotChangeHalf = _rotChangeValue * -0.5f;
        gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z + rotChangeHalf);

        Vector3 animRot = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z + _rotChangeValue);
        gameObject.transform.DOLocalRotate(animRot,_rotChangeTime).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }


    [Header ("Scale Animation")]
    public bool _scaleAnimation = true;
    public float _scaleChangeValue = 0.01f;
    public float _scaleChangeTime = 2f;
    void scaleAnimation(){
        //Set first position
        float scaleChangeHalf = _scaleChangeValue * -0.5f;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + scaleChangeHalf, gameObject.transform.localScale.y + scaleChangeHalf, gameObject.transform.localScale.z);

        Vector3 animScale = new Vector3(gameObject.transform.localScale.x + _scaleChangeValue, gameObject.transform.localScale.y + _scaleChangeValue, gameObject.transform.localScale.z);
        gameObject.transform.DOScale(animScale,_scaleChangeTime).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }
    

}
