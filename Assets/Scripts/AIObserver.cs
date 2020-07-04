/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

using UnityEngine;

public class AIObserver
{


    private static AIObserver instance;
    public static AIObserver Instance {
        get {
            if (instance == null)
                instance = new AIObserver();

            return instance;
        }
    }

    public AIObserver() {
        InitGrid();
    }

    public void InitGrid() {
        int nodeCount = BlackBord.GetNodesCount();

        for (int i = 0; i < nodeCount; i++) {
            BlackBord.Nodes.Add(new AINode(i));
        }
    }

    public void SetNodeActive(int _locationIndex, bool _isActived) {
        if (_locationIndex >= 0 && _locationIndex < BlackBord.Nodes.size)
            BlackBord.GetNode(_locationIndex).IsActived = _isActived;
    }

    public void ResetNodes() {
        int nodeCount = BlackBord.Nodes.size;

        for (int i = 0; i < nodeCount; i++)
        {
            BlackBord.GetNode(i).ResetNode();
        }
    }

    public void ResetNodesActions()
    {
        int nodeCount = BlackBord.Nodes.size;

        for (int i = 0; i < nodeCount; i++)
        {
            BlackBord.GetNode(i).ResetActions();
        }
    }

    //计算理想状态价值分布，理想回报参数gamma = 0.9 [0.9 ~ 0.99] 值越小计算效率越高，值越大长期回报越大
    public void CalculateStateValueDistribution(int _state) {
        AINode node = BlackBord.GetNode(_state);

        float gamma = BlackBord.Gamma;

        BetterList<int> actions = node.actions;
        for (int i = 0, imax = actions.size; i < imax; i++) {
            AINode n = BlackBord.GetNode(actions[i]);
            if (node.StateValue > n.StateValue)
            {
                n.StateValue = n.Reward + gamma * node.StateValue;
                CalculateStateValueDistribution(actions[i]);
            }
        }
    }


    public void CheckStateValue() {
        BetterList<AINode> nodes = BlackBord.Nodes;
        CalculateStateValueDistribution(BlackBord.TargetLocationIndex); 
    }


    private BetterList<int> GetPassableActions(AINode _node) {
        BetterList<int> passableList = new BetterList<int>();

        for (int i = 0, imax = _node.actions.size; i < imax; i++) {
            int a = _node.actions[i];
            if (!BlackBord.SearchRecord.Contains(a))
                passableList.Add(a);
        }

        return passableList;
    }

    public BetterList<int> GetOptimalActionList() {
        BetterList<int> oal = new BetterList<int>();
        BlackBord.SearchRecord.Clear();
        BlackBord.SearchRecord.Add(BlackBord.StartLocationIndex);

        int ptr = BlackBord.GetNode(BlackBord.StartLocationIndex).GetOptimalAction();
        while (ptr != -1) {
            oal.Add(ptr);
            ptr = BlackBord.GetNode(ptr).GetOptimalAction();
        }

        BlackBord.SearchRecord.Clear();
        return oal;
    }

    public float GetMaxValue(BetterList<float> _values, ref int _maxIndex) {
        float value = _values[0];
        for (int i = 0, imax = _values.size; i < imax; i++) {
            if (value < _values[i])
            {
                value = _values[i];
                _maxIndex = i;
            }
        }
        return value;
    } 
}
