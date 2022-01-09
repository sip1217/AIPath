/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

using UnityEngine;

public class AIObserver
{
    private BetterList<int> calOnceList = new BetterList<int>();

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

    //计算理想状态价值分布，理想收敛系数gamma = 0.9 [0.9 ~ 0.99] 
    //public void CalculateStateValueDistribution(AINode _node, AINode _lastNode)
    //{
    //    float gamma = BlackBord.Gamma;

    //    Debug.LogFormat("node{0}.stateValue =========>> {1}", _node.LocationIndex, _node.StateValue);
    //    foreach (var act in _node.actions)
    //    {
    //        if (_lastNode != null && _lastNode.actions.Contains(act))
    //            continue;

    //        if (n.IsOptimalAction(node))
    //            if (!calOnceList.Contains(act))
    //            {
    //                AINode n = BlackBord.GetNode(act);
    //                calOnceList.Add(_node.LocationIndex);
    //                n.StateValue = _node.Reward + gamma * _node.StateValue;
    //                CalculateStateValueDistribution(n, _node);
    //            }
    //    }

    //    foreach (var act in node.actions)
    //    {
    //        if (!calOnceList.Contains(act))
    //        {
    //            calOnceList.Add(act);
    //            CalculateStateValueDistribution(act);
    //        }
    //    }
    //}

    public void CalculateStateValueDistribution(int _targetIndex)
    {
        float gamma = BlackBord.Gamma;
        BetterList<int> allLeft = BlackBord.GetActiveNodeIndexes();

        int sum = BlackBord.GetNodesCount();
        int i = 0;
        while (allLeft.size > 0)
        {
            int index = (_targetIndex + i) % sum;
            AINode node = BlackBord.GetNode(index);

            if (!node.IsActived || node.StateValue == -1) {
                i++;
                continue;
            } 

            foreach (var act in node.actions)
            {
                AINode n = BlackBord.GetNode(act);

                float nsv = node.Reward + gamma * node.StateValue;
                if (nsv > n.StateValue && n.Reward != 1)
                    n.StateValue = nsv;
            }
            allLeft.Remove(index);
            i++;
        }
    }


    public void CheckStateValue() {
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
