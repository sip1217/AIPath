﻿/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

public class AINode
{
    #region Properties
    private bool isActived = true;
    public bool IsActived {
        get { return isActived; }
        set { isActived = value;
        }
    }

    private float reward = -1f;
    public float Reward {
        get { return reward; }
        private set { reward = value; }
    }

    private float stateValue;
    public float StateValue {
        get { return stateValue; }
        set { stateValue = value; }
    }

    public BetterList<int> actions = new BetterList<int>();
    //public BetterList<int> nextState = new BetterList<int>();

    private int optimalAction = 0;
    public int OptimalAction {
        get { return optimalAction; }
        set { optimalAction = value; }
    }

    private int locationIndex;
    public int LocationIndex {
        get { return locationIndex; }
    }

    private int locationX = 0;
    private int locationY = 0;
    private int locationRowSize = 0;
    #endregion

    //---------------------------------------------------

    public AINode(int _locationIndex, bool _isActived = true) {
        locationIndex = _locationIndex;
        isActived = _isActived;
        reward = -1;
        stateValue = -1f * BlackBord.Nodes.size;
    }

    //---------------------------------------------------

    #region Private Functions
    private void CheckSlefLocation() {    
        CheckLocation(locationIndex, ref locationRowSize, ref locationX, ref locationY);
    }

    private void CheckLocation(int _locationIndex, ref int _locationRowSize, ref int _locationX, ref int _locationY) {
        int frs = BlackBord.FirstRowSize;
        int rc = BlackBord.RowCount;
        int half = rc >> 1;

        int i = 0;
        int curRowSize = frs;
        while (_locationIndex >= curRowSize)
        {
            _locationIndex -= curRowSize;

            i++;
            if (i <= half)
                curRowSize = frs + i;
            else
                curRowSize = frs + (rc - i - 1);
        }

        _locationRowSize = curRowSize;
        _locationX = _locationIndex;
        _locationY = i;
    }
    #endregion

    #region Public Functions

    public AINode ResetNode() {
        stateValue = -1f * BlackBord.Nodes.size;
        isActived = true;
        Reward = -1;
        actions.Clear();

        return this;
    }

    public void ResetActions() {
        if (!isActived) return;

        actions.Clear();
        int _locationIndex = locationIndex;
        CheckSlefLocation();

        int selfRS = locationRowSize;
        int selfLX = locationX;
        int selfLY = locationY;

        int ly = 0;
        int lx = 0;
        int rs = 0;

        int frs = BlackBord.FirstRowSize;
        int rc = BlackBord.RowCount;
        int nc = BlackBord.Nodes.size;
        int half = rc >> 1;

        if (selfLY > 0)
        {
            int up = _locationIndex - (selfLY <= half ? selfRS : selfRS + 1);
            if (up >= 0)
            {
                CheckLocation(up, ref rs, ref lx, ref ly);
                if (ly == selfLY - 1 && BlackBord.GetNode(up).IsActived)
                    actions.Add(up);
            }

            CheckLocation(++up, ref rs, ref lx, ref ly);
            if (ly == selfLY - 1 && BlackBord.GetNode(up).IsActived)
                actions.Add(up);
        }

        if (selfLX > 0 && BlackBord.GetNode(_locationIndex - 1).IsActived)
            actions.Add(_locationIndex - 1);

        if (selfLX < selfRS - 1 && BlackBord.GetNode(_locationIndex + 1).IsActived)
            actions.Add(_locationIndex + 1);

        if (selfLY < rc - 1)
        {
            int down = _locationIndex + (selfLY < half ? selfRS + 1 : selfRS);
            if (down < nc)
            {
                CheckLocation(down, ref rs, ref lx, ref ly);
                if (ly == selfLY + 1 && BlackBord.GetNode(down).IsActived)
                    actions.Add(down);
            }

            CheckLocation(--down, ref rs, ref lx, ref ly);
            if (ly == selfLY + 1 && BlackBord.GetNode(down).IsActived)
                actions.Add(down);
        }

        //UnityEngine.Debug.LogFormat("Node[{0}]'s ResetActions result is ====================================", locationIndex);
        //for (int i = 0, imax = actions.size; i < imax; i++) {
        //    UnityEngine.Debug.LogFormat("Node[{0}].actions[{1}] is ====> <{2}>", locationIndex, i, actions[i]);
        //}
    }

    public int GetOptimalAction() {
        int oa = -1;
        if (reward < 0 && actions.size > 0)
        {
            float maxV = StateValue;

            for (int i = 0, imax = actions.size; i < imax; i++) {
                int ai = actions[i];

                if (BlackBord.SearchRecord.Contains(ai))
                    continue;

                float v = BlackBord.GetNode(ai).StateValue;
                BlackBord.SearchRecord.Add(ai);
                if (maxV < v) {
                    maxV = v;
                    oa = ai;
                }
            }
        }
        return oa;
    }

    public float GetActionReward(int _locationIndex) {
        float ar = -1f;
        int i = actions.IndexOf(_locationIndex);
        if (i >= 0)
        {
            AINode node = BlackBord.GetNode(i);
            ar = node.Reward;
        }
        return ar;
    }


    public void SetTargetSelf() {
        //actions.Clear();
        IsActived = true;
        Reward = 0;
        stateValue = 0f;
    }

    #endregion
}
