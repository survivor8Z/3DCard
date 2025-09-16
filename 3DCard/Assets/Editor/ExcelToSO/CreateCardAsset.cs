using OfficeOpenXml;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

public class CreateCardAsset : Editor
{
    // Excel标题枚举，用于更清晰地访问列数据
    public enum ExcelTitleEnum
    {
        Name,
        EnglishName,
        ID,
        Type,
        Values,
        Description,
    }

    // 模板文件路径
    static string handCardTemplatePath = "Assets/Editor/ExcelToSO/CardCodeTemplate/HandCardTemplate.txt";
    static string tableCardTemplatePath = "Assets/Editor/ExcelToSO/CardCodeTemplate/TableCardTemplate.txt";
    //----------------------------------------------------
    // 第一步: 生成所有卡牌SO资源
    //----------------------------------------------------
    [MenuItem("Tools/CardGeneration/1. Generate All CardSOs")]
    public static void GenerateAllCardSOs()
    {
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");
        if (!File.Exists(excelPath))
        {
            Debug.LogError("Excel file not found at path: " + excelPath);
            return;
        }

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");
            CreateCardSOAsset(package.Workbook.Worksheets[1]);
        }
        AssetDatabase.Refresh();
        Debug.Log("Finished generating all CardSO assets.");
    }

    private static void CreateCardSOAsset(ExcelWorksheet worksheet)
    {
        int startRow = 2, startCol = 1;
        for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
        {
            CardSO data = ScriptableObject.CreateInstance<CardSO>();
            data.cardName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
            data.cardEnglishName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.EnglishName].Text;
            data.cardID = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ID].Text);
            data.cardType = (E_CardType)System.Enum.Parse(typeof(E_CardType), worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text);

            string valuesStr = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Values].Text;
            data.values = ParseValues(valuesStr, data.cardName);

            data.cardDescription = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;

            string savePath = $"Assets/Resources_moved/SO/CardSO/{data.cardType.ToString()}/{data.cardName}.asset";
            EnsureDirectoryExists(savePath);
            AssetDatabase.CreateAsset(data, savePath);
        }
    }

    //----------------------------------------------------
    // 第二步: 生成所有卡牌脚本
    //----------------------------------------------------
    [MenuItem("Tools/CardGeneration/2. Generate All Card Scripts")]
    public static void GenerateAllCardScriptsFromExcel()
    {
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");
        if (!File.Exists(excelPath))
        {
            Debug.LogError("Excel file not found at path: " + excelPath);
            return;
        }

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            int startRow = 2, startCol = 1;
            for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
            {
                //这个已经是把E_给去掉了
                string cardTypeStr = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text.Substring(2);
                string cardEnglishName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.EnglishName].Text;
                GenerateScript(cardTypeStr, cardEnglishName);
            }
        }
        Debug.Log("Finished generating all card scripts. Please wait for compilation to complete before proceeding.");
    }

    private static void GenerateScript(string cardTypeStr, string cardEnglishName)
    {
        string typeAbbreviation = GetCardTypeAbbreviation(cardTypeStr);
        string handCardCodeTemplate = File.ReadAllText(handCardTemplatePath);
        string tableCardCodeTemplate = File.ReadAllText(tableCardTemplatePath);

        handCardCodeTemplate = handCardCodeTemplate
            .Replace("{CardType}", cardTypeStr)
            .Replace("{CardTypeFirAndSec}", typeAbbreviation)
            .Replace("{CardEnglishName}", cardEnglishName);

        tableCardCodeTemplate = tableCardCodeTemplate
            .Replace("{CardType}", cardTypeStr)
            .Replace("{CardTypeFirAndSec}", typeAbbreviation)
            .Replace("{CardEnglishName}", cardEnglishName);

        string handCardSavePath = $"Assets/Scripts/Card/HandCard/{cardTypeStr}Card/HandCard_{typeAbbreviation}_{cardEnglishName}.cs";
        string tableCardSavePath = $"Assets/Scripts/Card/TableCard/{cardTypeStr}Card/TableCard_{typeAbbreviation}_{cardEnglishName}.cs";

        EnsureDirectoryExists(handCardSavePath);
        EnsureDirectoryExists(tableCardSavePath);

        if (!File.Exists(handCardSavePath))
        {
            File.WriteAllText(handCardSavePath, handCardCodeTemplate);
            Debug.Log($"Successfully created: {handCardSavePath}");
        }
        else
        {
            Debug.LogWarning($"Skipping existing file: {handCardSavePath}");
        }

        if (!File.Exists(tableCardSavePath))
        {
            File.WriteAllText(tableCardSavePath, tableCardCodeTemplate);
            Debug.Log($"Successfully created: {tableCardSavePath}");
        }
        else
        {
            Debug.LogWarning($"Skipping existing file: {tableCardSavePath}");
        }

        AssetDatabase.Refresh();
    }

    //----------------------------------------------------
    // 第三步: 根据SO创建所有预制体
    //----------------------------------------------------
    [MenuItem("Tools/CardGeneration/3. Generate All Card Prefabs")]
    public static void GenerateAllCardPrefabsFromSOs()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardSO", new string[] { "Assets/Resources_moved/SO/CardSO" });
        if (guids.Length == 0)
        {
            Debug.LogError("No CardSO assets found. Please run Step 1 first.");
            return;
        }

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CardSO cardSO = AssetDatabase.LoadAssetAtPath<CardSO>(assetPath);
            if (cardSO != null)
            {
                CreateCardPrefabFromSO(cardSO);
            }
        }
        Debug.Log("Finished generating all card prefabs.");
    }

    private static void CreateCardPrefabFromSO(CardSO cardSO)
    {
        string cardTypeStrE = cardSO.cardType.ToString();
        string cardTypeAbbreviation = GetCardTypeAbbreviation(cardTypeStrE);
        string savePath = $"Assets/Resources_moved/Prefabs/InteractableObj/Card/{cardTypeStrE}/HandCard_{cardSO.cardEnglishName}.prefab";

        EnsureDirectoryExists(savePath);

        GameObject cardPrefab = new GameObject(cardSO.cardName);

        // 添加卡牌逻辑脚本
        string handComponentName = $"HandCard_{cardTypeAbbreviation}_{cardSO.cardEnglishName}";
        string tableComponentName = $"TableCard_{cardTypeAbbreviation}_{cardSO.cardEnglishName}";
        AddComponentSafely(cardPrefab, handComponentName);
        AddComponentSafely(cardPrefab, tableComponentName);

        // 添加其他通用组件
        AddComponentSafely(cardPrefab, "HandCardVisual");
        AddComponentSafely(cardPrefab, "TableCardVisual");
        cardPrefab.AddComponent<RectTransform>().sizeDelta = new Vector2(200, 300);
        cardPrefab.AddComponent<CanvasGroup>();
        cardPrefab.AddComponent<SortingGroup>();
        cardPrefab.AddComponent<CanvasRenderer>();
        cardPrefab.AddComponent<PoolObj>().maxNum = 30;

        // 绑定SO和视觉组件
        HandCardBase handCardBase = cardPrefab.GetComponent<HandCardBase>();
        TableCardBase tableCardBase = cardPrefab.GetComponent<TableCardBase>();
        HandCardVisual handCardVisual = cardPrefab.GetComponent<HandCardVisual>();
        TableCardVisual tableCardVisual = cardPrefab.GetComponent<TableCardVisual>();

        if (handCardBase != null) handCardBase.cardSO = cardSO;
        if (tableCardBase != null) tableCardBase.cardSO = cardSO;
        if (handCardVisual != null) handCardVisual.curveParameters = AddressablesMgr.Instance.LoadAsset<CurveParameters>("CurveParameters");
        if (tableCardVisual != null) tableCardVisual.enabled = false;
        if (tableCardBase != null) tableCardBase.enabled = false;

        // 添加 CardView 子对象
        GameObject cardViewPrefab = AddressablesMgr.Instance.LoadAsset<GameObject>("CardView");
        if (cardViewPrefab != null)
        {
            GameObject instantiatedCardView = (GameObject)PrefabUtility.InstantiatePrefab(cardViewPrefab, cardPrefab.transform);
            CardView theCardView = instantiatedCardView.GetComponent<CardView>();
            if (theCardView != null)
            {
                theCardView.cardSO = cardSO;
                theCardView.title.text = cardSO.cardName;
                theCardView.description.text = cardSO.cardDescription;
            }
        }
        else
        {
            Debug.LogError("CardView prefab not found in Addressables. Skipping prefab creation for " + cardSO.cardName);
            GameObject.DestroyImmediate(cardPrefab);
            return;
        }

        // 保存并清理
        PrefabUtility.SaveAsPrefabAsset(cardPrefab, savePath);
        Debug.Log("Created prefab: " + savePath);
        GameObject.DestroyImmediate(cardPrefab);
    }
    //----------------------------------------------------
    // 第四步: 根据另一个表格生成所有组合规则SO
    //----------------------------------------------------
    public enum ExcelCombinationTitleEnum
    {
        Combination,
        Result,
        CombinationId,
        Priority,
        Explanation
    }
    [MenuItem("Tools/CardGeneration/4. Generate Individual Combination Rules")]
    public static void GenerateIndividualCombinationRules()
    {
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");
        if (!File.Exists(excelPath))
        {
            Debug.LogError("Excel file not found at path: " + excelPath);
            return;
        }

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[2];
            if (worksheet == null)
            {
                Debug.LogError("Combination rules worksheet not found.");
                return;
            }

            CreateIndividualRuleAssets(worksheet);
        }
        AssetDatabase.Refresh();
        Debug.Log("Finished generating all individual CardCombinationRule assets.");
    }

    private static void CreateIndividualRuleAssets(ExcelWorksheet worksheet)
    {
        TotalCombinationRule totalCombinationRule = ScriptableObject.CreateInstance<TotalCombinationRule>();
        int startRow = 2, startCol = 1;
        for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
        {
            string combinationStr = worksheet.Cells[i, startCol + (int)ExcelCombinationTitleEnum.Combination].Text;
            string resultCardEnglishName = worksheet.Cells[i, startCol + (int)ExcelCombinationTitleEnum.Result].Text;
            string combinationIdStr = worksheet.Cells[i, startCol + (int)ExcelCombinationTitleEnum.CombinationId].Text;
            string priorityStr = worksheet.Cells[i, startCol + (int)ExcelCombinationTitleEnum.Priority].Text;

            if (string.IsNullOrEmpty(combinationStr) || string.IsNullOrEmpty(resultCardEnglishName))
            {
                Debug.LogWarning($"Skipping row {i} due to missing combination or result data.");
                continue;
            }

            List<CardSO> requiredCards = ParseCombinationString(combinationStr);
            if (requiredCards == null)
            {
                Debug.LogError($"Failed to parse combination string '{combinationStr}' at row {i}. Skipping.");
                continue;
            }

            CardSO resultCardSO = AssetDatabase.LoadAssetAtPath<CardSO>(
                $"Assets/Resources_moved/SO/CardSO/E_Entity/{resultCardEnglishName}.asset"
            );
            if (resultCardSO == null)
            {
                Debug.LogError($"Result card SO '{resultCardEnglishName}' not found. Did you run Step 1? Skipping row {i}.");
                continue;
            }

            // 创建 CardCombinationRule SO
            CardCombinationRule newRule = ScriptableObject.CreateInstance<CardCombinationRule>();
            newRule.requiredCards = requiredCards;
            newRule.resultCard = resultCardSO;
            newRule.id = int.TryParse(combinationIdStr, out int idValue) ? idValue : 0;
            newRule.priority = int.TryParse(priorityStr, out int priorityValue) ? priorityValue : 0;

            // 保存为单独的 SO 文件
            string fileName = $"{resultCardEnglishName}Rule_{newRule.id}";
            string savePath = $"Assets/Resources_moved/SO/CardCombinationRule/{fileName}.asset";
            EnsureDirectoryExists(savePath);
            AssetDatabase.CreateAsset(newRule, savePath);
            Debug.Log($"Created combination rule: {savePath}");
            // 添加到总规则中
            totalCombinationRule.totalCombinationRuleList.Add(newRule);
            totalCombinationRule.totalCombinationRuleDic.Add(newRule.id, newRule);
        }
        // 保存总规则 SO
        string totalSavePath = "Assets/Resources_moved/SO/CardCombinationRule/TotalCombinationRule/TotalCombinationRule.asset";
        EnsureDirectoryExists(totalSavePath);
        AssetDatabase.CreateAsset(totalCombinationRule, totalSavePath);
        Debug.Log($"Created total combination rule asset: {totalSavePath}");
    }

    // 辅助方法：解析组合字符串，返回 CardSO 列表
    private static List<CardSO> ParseCombinationString(string combinationStr)
    {
        List<CardSO> requiredCards = new List<CardSO>();
        string[] cardEntries = combinationStr.Split(',');

        foreach (string entry in cardEntries)
        {
            string[] parts = entry.Split('_');
            if (parts.Length != 2)
            {
                Debug.LogError($"Invalid combination format: {entry}");
                return null;
            }

            string cardEnglishName = parts[0].Trim();
            if (!int.TryParse(parts[1].Trim(), out int count))
            {
                Debug.LogError($"Invalid count in combination format: {entry}");
                return null;
            }

            // 查找对应的 CardSO 资产
            CardSO cardSO = AssetDatabase.LoadAssetAtPath<CardSO>(
                $"Assets/Resources_moved/SO/CardSO/E_Entity/{cardEnglishName}.asset"
            );
            if (cardSO == null)
            {
                Debug.LogError($"Required card SO '{cardEnglishName}' not found. Did you run Step 1?");
                return null;
            }

            // 根据数量添加到列表中
            for (int i = 0; i < count; i++)
            {
                requiredCards.Add(cardSO);
            }
        }
        return requiredCards;
    }

    //----------------------------------------------------
    // 辅助方法
    //----------------------------------------------------
    private static string GetCardTypeAbbreviation(string cardTypeStr)
    {
        if (cardTypeStr.StartsWith("E_"))//这个主要是防止错误
        {
            return cardTypeStr.Substring(2, 2);
        }
        else
        {
            return cardTypeStr.Substring(0, 2);
        }
    }
    private static void AddComponentSafely(GameObject go, string componentName)
    {
        System.Type componentType = GetTypeInAllAssemblies(componentName);
        if (componentType != null)
        {
            go.AddComponent(componentType);
            Debug.Log($"Successfully added component: '{componentName}'");
        }
        else
        {
            Debug.LogError($"Component '{componentName}' not found. Please ensure the script exists and has been compiled.");
        }
    }

    private static System.Type GetTypeInAllAssemblies(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName, false, true);
            if (type != null) return type;
        }
        return null;
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static List<int> ParseValues(string valuesStr, string cardName)
    {
        List<int> values = new List<int>();
        if (!string.IsNullOrEmpty(valuesStr))
        {
            string[] valuesArr = valuesStr.Split(',');
            foreach (string value in valuesArr)
            {
                if (int.TryParse(value, out int intValue))
                {
                    values.Add(intValue);
                }
                else
                {
                    Debug.LogWarning($"Invalid integer value '{value}' in Values for card '{cardName}'. Skipping.");
                }
            }
        }
        return values;
    }
}