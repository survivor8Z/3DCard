using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateCardAsset : Editor
{
    public enum ExcelTitleEnum
    {
        Name,
        ID,
        Type,
        Description,
    }
    [MenuItem("Tools/CreateAssetFromExcel")]
    public static void CreateAssetFromExcel()
    {
        //Excel的文件路径
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/配置表.xlsx");

        //读取Excel的内容数据
        using var package = new ExcelPackage(new FileInfo(excelPath));
        Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");

        //sheet1创建卡牌SO资源
        CreateCardSOAsset(package.Workbook.Worksheets[0]);
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
            data.cardID = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ID].Text);
            data.cardType = (E_CardType)System.Enum.Parse(typeof(E_CardType), worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text);
            data.cardDescription = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
            //创建资源文件
            string savePath = $"Assets/Resources/SO/CardSO/{data.cardType.ToString()}/{data.cardName}.asset";
            //确保目录存在
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            AssetDatabase.CreateAsset(data, savePath);
            Debug.Log("Created asset: " + savePath);
        }
    }

    private static void CreateCardInteractSOAsset(ExcelWorksheet worksheet)
    {

    }
}
