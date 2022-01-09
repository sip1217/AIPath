/****
 * Author:ZY
 * Time:2020-6-30
 * Email:2201563@qq.com
 * 
 ****/

using UnityEngine;

public class BlackBord
{
    public static int AIPathStep = 0;

    public static int FirstRowSize = 2;
    public static int RowCount = 9;

    public static int StartLocationIndex = -1;
    public static int TargetLocationIndex = -1;

    public static int BlockCount = 0;
    public static int IMNodeCount = 0;
    public static readonly float Gamma = 0.9f;


    public static BetterList<AINode> Nodes = new BetterList<AINode>();
    public static BetterList<int> SearchRecord = new BetterList<int>();

    public static Color COLOR_TARGET = new Color(0.874f, 0.263f, 0.635f, 0.45f);
    public static Color COLOR_START = new Color(0.314f, 0.314f, 1f, 0.45f);
    public static Color COLOR_PATHNODE = new Color(0f, 1f, 0f, 0.45f);
    public static Color COLOR_BADNODE = new Color(1f, 0f, 0f, 0.45f);
    public static Color COLOR_NORMAL = new Color(0f, 0.569f, 1f, 0.176f);
    public static Color COLOR_IMNODE = new Color(0.2f, 0.2f, 0.2f, 0.45f);


    public static AINode GetNode(int _index) {
        if (_index < 0 || _index >= Nodes.size)
            return null;

        return Nodes[_index];
    }

    public static AINode GetTargetNode()
    {
        if (TargetLocationIndex < 0 || TargetLocationIndex >= Nodes.size)
            return null;

        return Nodes[TargetLocationIndex];
    }

    public static AINode GetNodeForSearching(int _index)
    {
        if (_index < 0 || _index >= Nodes.size)
            return null;

        AINode node = Nodes[_index];
        if (!SearchRecord.Contains(node.LocationIndex))
            SearchRecord.Add(node.LocationIndex);

        return node;
    }

    public static int GetNodesCount() {
        int half = (RowCount >> 1) + RowCount % 2;
        return half * half + (FirstRowSize - 1) * RowCount; 
    }

    public static void SetTargetLocation(int _locationIndex) {
        if (_locationIndex < 0 || _locationIndex >= Nodes.size)
            return;

        if(TargetLocationIndex >= 0)
            Nodes[TargetLocationIndex].ResetNode();
        Nodes[_locationIndex].SetTargetSelf();
        TargetLocationIndex = _locationIndex;
    }

    public static AINode TargetNode
    {
        get
        {
            if (TargetLocationIndex < 0 || TargetLocationIndex >= Nodes.size)
                return null;
            return Nodes[TargetLocationIndex];
        }
    }

    public static BetterList<AINode> GetActiveNodes()
    {
        BetterList<AINode> result = new BetterList<AINode>();
        foreach(var node in Nodes)
        {
            if (node.IsActived)
                result.Add(node);
        }
        return result;
    }

    public static BetterList<int> GetActiveNodeIndexes()
    {
        BetterList<int> result = new BetterList<int>();
        foreach (var node in Nodes)
        {
            if (node.IsActived)
                result.Add(node.LocationIndex);
        }
        return result;
    }
}
