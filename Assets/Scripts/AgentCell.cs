/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

using UnityEngine;
using DG.Tweening;

public class AgentCell : MonoBehaviour
{
    private MeshRenderer selfRenderer;
    private Material selfMater;
    public int agentIndex = 0;

    private Transform selfTrans;

    private bool isBadCell = false;
    private bool isTweening = false;

    void Start()
    {
        selfTrans = GetComponent<Transform>();
        agentIndex = int.Parse(name.Substring(6));
        selfRenderer = GetComponent<MeshRenderer>();
        selfMater = GetComponent<MeshRenderer>().materials[1];
    }

    private void OnMouseUpAsButton()
    {
        if (!isTweening && AIPathManager.Instance != null)
            AIPathManager.Instance.AgentSelected(agentIndex);
    }

    public void SetAgentAsTarget(bool _isTarget) {
        if (_isTarget)
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.up * 180f, 0.5f).OnComplete(() => 
            { selfMater.color = BlackBord.COLOR_TARGET; isTweening = false; });
        }
        else
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() => 
            { selfMater.color = BlackBord.COLOR_NORMAL; isTweening = false; });
        }
    }

    public void SetAgentAsStart(bool _isStart)
    {
        if (_isStart)
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.up * 180f, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_START; isTweening = false; });
        }
        else
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_NORMAL; isTweening = false; });
        }
    }

    public void SetAgentAsIMNode(bool _isIMNode)
    {
        if (_isIMNode)
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.up * 180f, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_IMNODE; isTweening = false; });
        }
        else
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_NORMAL; isTweening = false; });
        }
    }    

    public void SetAgentAsPathNode(bool _isOptNode)
    {
        if (_isOptNode)
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.up * 180f, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_PATHNODE; isTweening = false; });
        }
        else
        {
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.zero, 0.2f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_NORMAL; isTweening = false; });
        }
    }

    public bool SetAgentAsBlock()
    {
        if (!isBadCell)
        {
            isBadCell = true;
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.up * 180f, 0.5f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_BADNODE; isTweening = false; });
        }
        else
        {
            isBadCell = false;
            isTweening = true;
            selfTrans.DOLocalRotate(Vector3.zero, 0.2f).OnComplete(() =>
            { selfMater.color = BlackBord.COLOR_NORMAL; isTweening = false; });
        }

        return isBadCell;
    }

    public void ResetSelf() {
        selfMater.color = BlackBord.COLOR_NORMAL;
        selfTrans.DOLocalRotate(Vector3.zero, 0.2f);
        isBadCell = false;
        isTweening = false;
    }
}
