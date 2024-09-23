using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "DialogGraph")]
public class StoryGraph : NodeGraph
{
    public DialogAssets GetAssets()
    {
        List<Node> assetNodes = nodes.FindAll((x) =>
        {
            return x.GetType() == typeof(AssetNode);
        });

        if (assetNodes.Count==1)
        {
            if (assetNodes[0]!=null)
            {
                AssetNode temp = assetNodes[0] as AssetNode;
                return temp.dialogAssets;
            }
        }
        
        Debug.LogError("没有创建asset节点或者创建了多个");
        return null;
    }
}
