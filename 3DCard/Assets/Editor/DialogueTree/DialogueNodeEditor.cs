using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class DialogueNodeEditor : Editor
{
    public enum ExcelTitleEnum
    {
        Type,
        Name,
        Content,
        ConditionCheck,
        ExitWay,
        ID,
        SubnodeID,
    }

    [MenuItem("Tools/CreateDialogueSOFromExcel")]
    public static void CreateDialogueSOFromExcel()
    {
        // Excel的文件路径
        string excelPath = Path.Combine(Application.dataPath, "Editor/DialogueTree/文本配置.xlsx");

        // 确保输出目录存在
        string outputPath = "Assets/Resources_moved/SO/DialogueTree/Boss";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // 读取Excel的内容数据
        using var package = new ExcelPackage(new FileInfo(excelPath));
        Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");

        // 创建对话节点SO资源
        CreateDialogueNodeSOAsset(package.Workbook.Worksheets[1]);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Dialogue nodes created successfully!");
    }

    private static void CreateDialogueNodeSOAsset(ExcelWorksheet worksheet)
    {
        int startRow = 2; // 数据从第2行开始
        int startCol = 1; // 数据从第1列开始

        Dictionary<string, DialogueNodeBase> nodeDict = new Dictionary<string, DialogueNodeBase>();
        Dictionary<string, List<string>> childRelationships = new Dictionary<string, List<string>>();

        // 第一步：创建所有节点
        for (int row = startRow; row <= worksheet.Dimension.Rows; row++)
        {
            string nodeTypeStr = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.Type].Text.Trim();
            string nodeName = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.Name].Text.Trim();
            string nodeId = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.ID].Text.Trim();
            string subnodeIds = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.SubnodeID].Text.Trim();

            // 检查 nodeTypeStr 是否为空，如果是则跳过这一行
            if (string.IsNullOrWhiteSpace(nodeTypeStr))
            {
                Debug.LogWarning($"Skipping row {row} because node type is empty.");
                continue;
            }

            E_DialogueNodeType nodeType;
            try
            {
                // 使用 Enum.TryParse 代替 Enum.Parse，它更安全，不会抛出异常
                if (!System.Enum.TryParse(nodeTypeStr, out nodeType))
                {
                    // 如果转换失败，打印警告并跳过这一行
                    Debug.LogError($"Invalid node type '{nodeTypeStr}' found for ID '{nodeId}' on row {row}. Skipping this row.");
                    continue;
                }
            }
            catch (System.Exception ex)
            {
                // 捕获 Enum.Parse 可能抛出的其他异常
                Debug.LogError($"Error parsing enum on row {row} for ID '{nodeId}'. Exception: {ex.Message}");
                continue;
            }

            // 尝试创建节点
            DialogueNodeBase node = CreateNodeByType(nodeType, nodeName, worksheet, row, startCol);

            if (node != null)
            {
                node.name = nodeName;
                string assetPath = Path.Combine("Assets/Resources_moved/SO/DialogueTree/Boss", $"{nodeId}.asset");
                AssetDatabase.CreateAsset(node, assetPath);
                nodeDict.Add(nodeId, node);

                if (!string.IsNullOrEmpty(subnodeIds))
                {
                    childRelationships[nodeId] = subnodeIds.Split(',').Select(id => id.Trim()).ToList();
                }

                Debug.Log($"Created {nodeType} node: {assetPath}");
            }
            else
            {
                // 如果 CreateNodeByType 返回 null，说明创建失败
                Debug.LogError($"Failed to create node with ID '{nodeId}' and Type '{nodeTypeStr}' on row {row}. Please check the data in your Excel file for this row.");
            }
        }

        // 第二步：建立节点间的引用关系
        foreach (var relationship in childRelationships)
        {
            // 检查父节点是否存在
            if (nodeDict.TryGetValue(relationship.Key, out var parentNode))
            {
                // 如果父节点不存在，打印错误日志并跳过
                if (parentNode == null)
                {
                    Debug.LogError($"Parent node with ID '{relationship.Key}' was not created properly. Skipping child relationship setup.");
                    continue;
                }

                foreach (var childId in relationship.Value)
                {
                    // 检查子节点是否存在
                    if (nodeDict.TryGetValue(childId, out var childNode))
                    {
                        // 如果子节点不存在，打印错误日志并跳过
                        if (childNode == null)
                        {
                            Debug.LogError($"Child node with ID '{childId}' for parent '{relationship.Key}' was not found. Skipping.");
                            continue;
                        }

                        AddChildNode(parentNode, childNode);
                        EditorUtility.SetDirty(parentNode);
                    }
                    else
                    {
                        // 这种情况表明 Excel 中的 subnodeID 引用了不存在的 ID
                        Debug.LogError($"Could not find child node with ID '{childId}' for parent '{relationship.Key}'. Please check your Excel file.");
                    }
                }
            }
            else
            {
                // 这种情况表明 Excel 中的父节点 ID 引用了不存在的节点
                Debug.LogError($"Could not find parent node with ID '{relationship.Key}'. Please check your Excel file.");
            }
        }
        // 找到根节点（没有被任何节点作为子节点的节点）
        DialogueNodeBase rootNode = nodeDict.Values.FirstOrDefault(n =>
            !nodeDict.Values.Any(other =>
                (other is DSequenceNode seq && seq.childrenNodes.Contains(n)) ||
                (other is DSelectorNode sel && sel.childrenNodes.Contains(n)) ||
                (other is DConditionNode cond && cond.childNode == n)
            )
        );

        if (rootNode != null)
        {
            int y = 0;
            ArrangeTree(rootNode, 0, ref y);
        }
        else
        {
            Debug.LogWarning("未找到根节点，无法自动排列树结构。");
        }
    }

    private static DialogueNodeBase CreateNodeByType(E_DialogueNodeType nodeType, string nodeName,
                                                   ExcelWorksheet worksheet, int row, int startCol)
    {
        switch (nodeType)
        {
            case E_DialogueNodeType.E_Sequence:
                return ScriptableObject.CreateInstance<DSequenceNode>();

            case E_DialogueNodeType.E_Selector:
                return ScriptableObject.CreateInstance<DSelectorNode>();

            case E_DialogueNodeType.E_Condition:
                var conditionNode = ScriptableObject.CreateInstance<DConditionNode>();
                conditionNode.conditionCheckName = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.ConditionCheck].Text.Trim();
                return conditionNode;

            case E_DialogueNodeType.E_Action:
                var actionNode = ScriptableObject.CreateInstance<DActionNode>();

                actionNode.dialogueText = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.Content].Text;

                actionNode.conditionCheckName = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.ConditionCheck].Text.Trim();

                string exitWayStr = worksheet.Cells[row, startCol + (int)ExcelTitleEnum.ExitWay].Text;
                List<string> exitParams = exitWayStr.Split(',').Select(s => s.Trim()).ToList();

                if (exitParams[0] == "E_Wait")
                {
                    actionNode.exitType = E_DialogueActionExitType.E_Wait;
                    actionNode.waitTime = float.Parse(exitParams[1]);
                }
                else
                {
                    actionNode.exitType = (E_DialogueActionExitType)System.Enum.Parse(
                        typeof(E_DialogueActionExitType), exitParams[0]);
                }

                return actionNode;

            default:
                Debug.LogError($"Unknown node type: {nodeType}");
                return null;
        }
    }

    private static void AddChildNode(DialogueNodeBase parent, DialogueNodeBase child)
    {
        if (parent is DSequenceNode sequenceNode)
        {
            if (!sequenceNode.childrenNodes.Contains(child))
            {
                sequenceNode.childrenNodes.Add(child);
            }
        }
        else if (parent is DSelectorNode selectorNode)
        {
            if (!selectorNode.childrenNodes.Contains(child))
            {
                selectorNode.childrenNodes.Add(child);
            }
        }
        else if (parent is DConditionNode conditionNode)
        {
            conditionNode.childNode = child;
        }
        var parentOutput = parent.GetOutputPort("nextNode");
        var childInput = child.GetInputPort("parentNode");
        if (parentOutput != null && childInput != null && !parentOutput.IsConnectedTo(childInput))
        {
            parentOutput.Connect(childInput);
            EditorUtility.SetDirty(parent);
            EditorUtility.SetDirty(child);
        }
    }

    private static void ArrangeTree(DialogueNodeBase node, int depth, ref int y, int xSpacing = 350, int ySpacing = 180, HashSet<DialogueNodeBase> visited = null)
    {
        if (node == null) return;
        if (visited == null) visited = new HashSet<DialogueNodeBase>();
        if (visited.Contains(node)) return;
        visited.Add(node);

        // 设置当前节点位置
        node.position = new Vector2(depth * xSpacing, y);
        EditorUtility.SetDirty(node);

        // 递归排列子节点
        if (node is DSequenceNode seq)
        {
            foreach (var child in seq.childrenNodes)
            {
                y += ySpacing;
                ArrangeTree(child, depth + 1, ref y, xSpacing, ySpacing, visited);
            }
        }
        else if (node is DSelectorNode sel)
        {
            foreach (var child in sel.childrenNodes)
            {
                y += ySpacing;
                ArrangeTree(child, depth + 1, ref y, xSpacing, ySpacing, visited);
            }
        }
        else if (node is DConditionNode cond && cond.childNode != null)
        {
            y += ySpacing;
            ArrangeTree(cond.childNode, depth + 1, ref y, xSpacing, ySpacing, visited);
        }
        // 叶子节点（如 DActionNode）无需递归
    }
}