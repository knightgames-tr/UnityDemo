using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public int _lineNo;
    LevelManager _levelManager;
    public virtual void Start()
    {   
        _levelManager = LevelManager.Instance;
        _queue = new List<NPCController>();
    }

    List<NPCController> _queue;
    public NPCController getFirstAtQueue(){
        return _queue[0];
    }

    float _queueOffset = 2f;
    public bool _isLineActive{get;private set;} = false;
    public void processQueue(){
        if(!_isLineActive){
            return;
        }

        //Remove first item in queue, do related action
        _queue.RemoveAt(0);

        if(_queue.Count == 0){
            _isLineActive = false;
            return;
        }

        //Move other items of the queue to positions
        for(int i=0;i<_queue.Count;i++){
            _queue[i].moveToPosition(transform.position + transform.forward*i*_queueOffset,i,_lineNo);
        }
    }

    public void addNPCToLine(NPCController npc){
        _queue.Add(npc);
        _queue[_queue.Count-1].moveToPosition(transform.position + transform.forward*(_queue.Count-1)*_queueOffset,_queue.Count-1,_lineNo);

        if(!_isLineActive){
            _isLineActive = true;
        }
    }
}
