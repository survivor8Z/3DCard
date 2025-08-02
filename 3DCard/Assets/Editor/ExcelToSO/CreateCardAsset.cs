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
        //Excel���ļ�·��
        string excelPath = Path.Combine(Application.dataPath, "Editor/ExcelToSO/���ñ�.xlsx");

        //��ȡExcel����������
        using var package = new ExcelPackage(new FileInfo(excelPath));
        Debug.Log("Loaded Excel package: " + package.Workbook.Worksheets.Count + " worksheets found.");

        //sheet1��������SO��Դ
        CreateCardSOAsset(package.Workbook.Worksheets[0]);
        ////sheet2�����������SO��Դ
        //CreateCardInteractSOAsset(package.Workbook.Worksheets[1]);
    }
    private static void CreateCardSOAsset(ExcelWorksheet worksheet)
    {
        int startRow = 2, startCol = 1;
        //����ȡ��Excel���ݸ�ֵ����Ӧ�Ŀ����л��ֶ�
        for (int i = startRow; i <= worksheet.Dimension.Rows; i++)
        {
            CardSO data = ScriptableObject.CreateInstance<CardSO>();
            //����ȡ��Excel���ݸ�ֵ����Ӧ������
            data.cardName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
            data.cardID = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ID].Text);
            data.cardType = (E_CardType)System.Enum.Parse(typeof(E_CardType), worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Type].Text);
            data.cardDescription = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
            //������Դ�ļ�
            string savePath = $"Assets/Resources/SO/CardSO/{data.cardType.ToString()}/{data.cardName}.asset";
            //ȷ��Ŀ¼����
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
