using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public int _lineNo;
    LevelManager _levelManager;
    void Start()
    {   
        _levelManager = LevelManager.Instance;
        _queue = new List<NPCController>();
    }

    List<NPCController> _queue;

    float _queueOffset = 1f;
    bool _isLineActive = false;
    public void processQueue(){
        if(!_isLineActive){
            return;
        }

        //Remove first item in queue, do related action
        _levelManager.processLine(_lineNo,_queue[0]);
        _queue.RemoveAt(0);

        if(_queue.Count == 0){
            _isLineActive = false;
            return;
        }

        //Move other items of the queue to positions
        for(int i=0;i<_queue.Count;i++){
            _queue[i].moveToPosition(transform.position + transform.forward*i*_queueOffset);
        }
    }

    public void addNPCToLine(NPCController npc){
        _queue.Add(npc);
        _queue[_queue.Count-1].moveToPosition(transform.position + transform.forward*(_queue.Count-1)*_queueOffset);

        if(!_isLineActive){
            _isLineActive = true;
        }
    }
}
