using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CreateCardAsset : Editor
{
    public enum ExcelTitleEnum
    {
        Name,
        EnglishName,
        ID,
        Type,
        Description,
    }
    [MenuItem("Tools/CreateCard/CreateCardSOFromExcel")]
    public static void CreateCardSOFromExcel()
    {
        //Excel的文件路径
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");

        //读取Excel的内容数据
        using var package = new ExcelPackage(new FileInfo(excelPath));
        Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");

        //sheet1创建卡牌SO资源
        CreateCardSOAsset(package.Workbook.Worksheets[1]);
        ////sheet2创建卡牌组合SO资源
        //CreateCardInteractSOAsset(package.Workbook.Worksheets[1]);
    }

    private static void CreateCardSOAsset(ExcelWorksheet worksheet)
    {
        int startRow = 2, startCol = 1;
        //将读取的Excel数据赋值给相应的可序列化字段
        for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
        {
            CardSO data = ScriptableObject.CreateInstance<CardSO>();
            //将读取的Excel数据赋值给相应的属性
            data.cardName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
            data.cardEnglishName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.EnglishName].Text; // Assuming English name is the same as card name
            data.cardID = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ID].Text);
            data.cardType = (E_CardType)System.Enum.Parse(typeof(E_CardType), worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text);
            data.cardDescription = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
            //创建资源文件
            string savePath = $"Assets/Resources_moved/SO/CardSO/{data.cardType.ToString()}/{data.cardName}.asset";
            //确保目录存在
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            AssetDatabase.CreateAsset(data, savePath);
            Debug.Log("Created asset: " + savePath);
        }
    }







    [MenuItem("Tools/CreateCard/CreateCardPrefabs")]
    public static void CreateCardPrefabs()
    {
        //Excel的文件路径
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");

        //读取Excel的内容数据
        using var package = new ExcelPackage(new FileInfo(excelPath));
        Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");

        CreateCardPrefab(package.Workbook.Worksheets[1]);
    }
    

    private static void CreateCardPrefab(ExcelWorksheet worksheet)
    {
        int startRow = 2, startCol = 1;
        //将读取的Excel数据赋值给相应的可序列化字段
        for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
        {
            CardSO data = ScriptableObject.CreateInstance<CardSO>();
            //将读取的Excel数据赋值给相应的属性
            data.cardName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
            data.cardEnglishName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.EnglishName].Text; // Assuming English name is the same as card name
            data.cardID = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ID].Text);
            data.cardType = (E_CardType)System.Enum.Parse(typeof(E_CardType), worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text);
            data.cardDescription = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
            //创建资源文件
            string savePath = $"Assets/Resources_moved/SO/CardSO/{data.cardType.ToString()}/{data.cardName}.asset";
            //确保目录存在
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            AssetDatabase.CreateAsset(data, savePath);

            CreateCardPrefabWithSO(data);
            Debug.Log("Created asset: " + savePath);
        }
    }
    private static void CreateCardPrefabWithSO(CardSO cardSO)
    {
        
        string savePath 
            = $"Assets/Resources_moved/Prefabs/InteractableObj/Card/{cardSO.cardType.ToString()}/HandCard_{cardSO.cardEnglishName}.prefab";
        //确保目录存在
        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        //创建预制体
        GameObject cardPrefab = new GameObject(cardSO.cardName);
        switch (cardSO.cardType)
        {
            case E_CardType.E_Entity:
                AddComponentIfFound(cardPrefab, $"HandCard_En_{cardSO.cardEnglishName}");
                AddComponentIfFound(cardPrefab, $"TableCard_En_{cardSO.cardEnglishName}");
                cardPrefab.AddComponent<HandCardVisual>();
                cardPrefab.AddComponent<TableCardVisual>();
                
                HandCardVisual En_handCardVisual = cardPrefab.GetComponent<HandCardVisual>();
                TableCardVisual En_tableCardVisual = cardPrefab.GetComponent<TableCardVisual>();

                cardPrefab.GetComponent<HandCardBase>().cardSO = cardSO;
                cardPrefab.GetComponent<TableCardBase>().cardSO = cardSO;

                break;
            case E_CardType.E_Modificatory:
                AddComponentIfFound(cardPrefab, $"HandCard_Mo_{cardSO.cardEnglishName}");
                AddComponentIfFound(cardPrefab, $"TableCard_Mo_{cardSO.cardEnglishName}");

                cardPrefab.AddComponent<HandCardVisual>();
                cardPrefab.AddComponent<TableCardVisual>();

                cardPrefab.GetComponent<HandCardBase>().cardSO = cardSO;
                cardPrefab.GetComponent<TableCardBase>().cardSO = cardSO;
                break;
            case E_CardType.E_Behavior:
                AddComponentIfFound(cardPrefab, $"HandCard_Be_{cardSO.cardEnglishName}");

                cardPrefab.AddComponent<HandCardVisual>();

                cardPrefab.GetComponent<HandCardBase>().cardSO = cardSO;
                break;
            case E_CardType.E_Condition:
                AddComponentIfFound(cardPrefab, $"HandCard_Co_{cardSO.cardEnglishName}");
                AddComponentIfFound(cardPrefab, $"TableCard_Co_{cardSO.cardEnglishName}");

                cardPrefab.AddComponent<HandCardVisual>();
                cardPrefab.AddComponent<TableCardVisual>();

                cardPrefab.GetComponent<HandCardBase>().cardSO = cardSO;
                cardPrefab.GetComponent<TableCardBase>().cardSO = cardSO;
                break;
            default:
                Debug.LogError("Unknown card type: " + cardSO.cardType);
                GameObject.DestroyImmediate(cardPrefab);
                return;
        }

        if (cardSO.cardType == E_CardType.E_Behavior)
        {
            HandCardBase handCardBase = cardPrefab.GetComponent<HandCardBase>();
            handCardBase.cardSO = cardSO;
            HandCardVisual handCardVisual = cardPrefab.GetComponent<HandCardVisual>();
            handCardVisual.curveParameters 
                = AddressablesMgr.Instance.LoadAsset<CurveParameters>("CurveParameters");
        }
        else
        {
            HandCardBase handCardBase = cardPrefab.GetComponent<HandCardBase>();
            TableCardBase tableCardBase = cardPrefab.GetComponent<TableCardBase>();
            handCardBase.cardSO = cardSO;
            tableCardBase.cardSO = cardSO;
            HandCardVisual handCardVisual = cardPrefab.GetComponent<HandCardVisual>();
            TableCardVisual tableCardVisual = cardPrefab.GetComponent<TableCardVisual>();
            handCardVisual.curveParameters
                = AddressablesMgr.Instance.LoadAsset<CurveParameters>("CurveParameters");

            tableCardBase.enabled = false;
            tableCardVisual.enabled = false;
        }

        RectTransform rectTransform= cardPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200,300);
        rectTransform.eulerAngles = new Vector3(0, 90, 0);

        cardPrefab.AddComponent<SortingGroup>();
        cardPrefab.AddComponent<CanvasRenderer>();

        cardPrefab.AddComponent<PoolObj>().maxNum = 30;

        //设置CardView为子对象
        GameObject cardViewPrefab = AddressablesMgr.Instance.LoadAsset<GameObject>("CardView");
        if (cardViewPrefab != null)
        {
            GameObject instantiatedCardView 
                = (GameObject)PrefabUtility.InstantiatePrefab(cardViewPrefab, cardPrefab.transform);
            CardView theCardView = instantiatedCardView.GetComponent<CardView>();
            theCardView.cardSO = cardSO;

        }
        else
        {
            Debug.LogError("CardView prefab not found in Addressables. Please check the asset name and ensure it is included in Addressables.");
            GameObject.DestroyImmediate(cardPrefab);
            return;
        }



            //保存预制体
            PrefabUtility.SaveAsPrefabAsset(cardPrefab, savePath);
        Debug.Log("Created prefab: " + savePath);
        //删除临时创建的游戏对象
        GameObject.DestroyImmediate(cardPrefab);
        AddressablesMgr.Instance.Release<GameObject>("CardView");
        AddressablesMgr.Instance.Release<CurveParameters>("CurveParameters");



    }
    // 辅助方法：在所有已加载的程序集中查找并添加组件
    private static void AddComponentIfFound(GameObject go, string componentName)
    {
        System.Type componentType = GetTypeInAllAssemblies(componentName);

        if (componentType != null)
        {
            go.AddComponent(componentType);
            Debug.Log($"Successfully added component: '{componentName}'");
        }
        else
        {
            Debug.LogError($"Component '{componentName}' not found in any loaded assembly. Please check your script name and namespace.");
        }
    }

    // 辅助方法：在所有已加载的程序集中查找类型
    private static System.Type GetTypeInAllAssemblies(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            // 尝试获取类型，忽略大小写
            var type = assembly.GetType(typeName, false, true);
            if (type != null)
            {
                return type;
            }
        }
        return null;
    }



}
