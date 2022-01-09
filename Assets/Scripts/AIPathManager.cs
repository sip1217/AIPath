/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

//using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIPathManager : MonoBehaviour
{
    private static readonly string MSG_RESTART = "请在网格中任意点击节点，将其设置为障碍节点！\n完成后点击‘确认完成’进入下一步！";
    private static readonly string MSG_SETTARGET = "请任意选择一个节点作为目的地！\n确认后点击‘确认目标节点’进入下一步";
    private static readonly string MSG_SETSTART = "请选择目标节点以外的任意节点作为始发点，\n\n确认后点击‘获取最佳路线’可浏览结果！";
    private static readonly string MSG_RESETSTART = "已获取当前最优路线！可点击‘重新设置始发点’继续体验，或点击‘重新开始’重新布局场景！";
    private static readonly string MSG_BADSTART = "无法将目的地改为始发点，请重新选择始发点！";
    private static readonly string MSG_NOPATH = "当前地形无法找到连通始发点与终点的可达路径，很遗憾请重新来过吧！";
    private static readonly string MSG_GETPATH = "点击‘生成路线’获取当前始发点至终点的最优路线！";

    public static AIPathManager Instance;
    AIObserver observer;

    public RectTransform dataUIPart;
    private Text[] dataList;

    private BetterList<int> optimalPath;

    public Transform agentsPart;
    private AgentCell[] agentList;

    private int lastTarget = -1;
    private int lastStart = -1;

    private bool showingDebugData = false;

    public Button nextStepBtn;
    public Button restartBtn;
    private Text nextStepBtnText;
    public Button trainingDebugBtn;

    public Text messageLabel;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dataList = dataUIPart.GetComponentsInChildren<Text>();
        agentList = agentsPart.GetComponentsInChildren<AgentCell>();
        nextStepBtnText = nextStepBtn.GetComponentInChildren<Text>();
        RestartSearching();
    }

    public void RestartSearching() {
        lastStart = -1;
        BlackBord.StartLocationIndex = -1;
        lastTarget = 1;
        BlackBord.TargetLocationIndex = -1;
        BlackBord.AIPathStep = 0;
        BlackBord.BlockCount = 0;

        nextStepBtn.interactable = true;
        trainingDebugBtn.interactable = false;
        AIObserver.Instance.ResetNodes();
        messageLabel.text = MSG_RESTART;
        nextStepBtnText.text = ">> 确认完成 <<";
        for (int i = 0, imax = dataList.Length; i < imax; i++)
        {
            dataList[i].text = string.Empty;
            agentList[i].ResetSelf();
        }
    }

    public void AgentSelected(int _index) {
        switch (BlackBord.AIPathStep)
        {
            case 0:
                SetBlock(_index);
                break;

            case 1:
                ResetTarget(_index);
                break;

            case 2:
                ResetStart(_index);
                break;
        }
    }

    private void SetBlock(int _index)
    {
        bool isBlock = agentList[_index].SetAgentAsBlock();
        BlackBord.GetNode(_index).ResetNode().IsActived = !isBlock;

        if (isBlock) {
            BlackBord.BlockCount++;
            dataList[_index].text = "X";
        }
        else
        {
            BlackBord.BlockCount--;
            dataList[_index].text = string.Empty;
        }
    }

    private void RemoveStart() {
        if (lastStart >= 0)
        {
            agentList[lastStart].SetAgentAsStart(false);
            dataList[lastTarget].text = string.Empty;
            SetOptimalPath(false);
        }
    }

    private void ResetStart(int _index)
    {
        if (_index == BlackBord.TargetLocationIndex) {
            StartCoroutine(ShowMessage(MSG_BADSTART, MSG_SETSTART));
            return;
        }

        if (lastStart != _index)
        {
            if (lastStart >= 0)
            {
                agentList[lastStart].SetAgentAsStart(false);
                dataList[lastTarget].text = string.Empty;
            }

            agentList[_index].SetAgentAsStart(true); 
            dataList[_index].text = "Start";
            BlackBord.StartLocationIndex = _index;
            BlackBord.GetNode(_index).ResetNode().ResetActions();
            lastStart = _index;

            nextStepBtn.interactable = true;
        }
    }

    private void SetOptimalPath(bool _isSelected)
    {
        if (_isSelected && !optimalPath.Contains(BlackBord.TargetLocationIndex))
        {
            messageLabel.text = MSG_NOPATH;
            return;
        }

        int imax = optimalPath.size - 1;
        //Debug.Log(" optimalPath.size ===========>>  " + optimalPath.size);
        for (int i = 0; i < imax; i++) {
            agentList[optimalPath[i]].SetAgentAsPathNode(_isSelected);
        }
    }

    private void ResetTarget(int _index) {
        if (lastTarget != _index)
        {
            if (lastTarget >= 0)
            {
                agentList[lastTarget].SetAgentAsTarget(false);
                dataList[lastTarget].text = string.Empty;
                BlackBord.GetNode(lastTarget).ResetNode();
            }
            agentList[_index].SetAgentAsTarget(true);
            dataList[_index].text = "Target";
            BlackBord.SetTargetLocation(_index);
            lastTarget = _index;
        }

        if (BlackBord.TargetLocationIndex >= 0 && !nextStepBtn.interactable)
            nextStepBtn.interactable = true;
    }

    public void NextStep() {
        switch (BlackBord.AIPathStep)
        {
            case 0:
                messageLabel.text = MSG_SETTARGET;
                nextStepBtn.interactable = false;
                nextStepBtnText.text = ">> 确认目标 <<";
                BlackBord.AIPathStep = 1;
                break;

            case 1:
                AIObserver.Instance.ResetNodesActions();
                AIObserver.Instance.CheckStateValue();
                trainingDebugBtn.interactable = true;
                messageLabel.text = MSG_SETSTART;
                nextStepBtn.interactable = false;
                nextStepBtnText.text = ">> 获取最佳路线 <<";
                BlackBord.AIPathStep = 2;
                break;

            case 2:
                optimalPath = AIObserver.Instance.GetOptimalActionList();
                messageLabel.text = MSG_RESETSTART;
                SetOptimalPath(true);
                nextStepBtnText.text = ">> 重设始发节点 <<";
                nextStepBtn.interactable = false;
                BlackBord.AIPathStep = 4;
                break;

            case 3:
                RemoveStart();
                messageLabel.text = MSG_GETPATH;
                nextStepBtnText.text = ">> 获取路线 <<";
                nextStepBtn.interactable = false;
                BlackBord.AIPathStep = 2;
                break;
        }
    }

    public void ShowDebugData() {
        if (!showingDebugData)
        {
            for (int i = 0, imax = dataList.Length; i < imax; i++)
            {
                dataList[i].text = BlackBord.GetNode(i).StateValue.ToString("F5");
            }
            showingDebugData = true;
        }
        else
        {
            for (int i = 0, imax = dataList.Length; i < imax; i++)
            {
                if (i == BlackBord.StartLocationIndex)
                    dataList[i].text = "Start";
                else if (i == BlackBord.TargetLocationIndex)
                    dataList[i].text = "Target";
                else if (!BlackBord.GetNode(i).IsActived)
                    dataList[i].text = "X";
                else
                    dataList[i].text = string.Empty;
            }

            showingDebugData = false;
        }
    }

    private IEnumerator ShowMessage(string _from, string _to) {
        messageLabel.text = _from;
        yield return new WaitForSeconds(2f);
        messageLabel.text = _to;
    }
}
