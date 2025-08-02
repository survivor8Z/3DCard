//using OfficeOpenXml;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEditor;
//using UnityEngine;

//public class Plant : ScriptableObject
//{
//    public string PlantName; //ֲ������
//    public Sprite Icon; //ֲ��ͼ��
//    public string IconPath; //ֲ��ͼ����Դ·��
//    public int Price; //ֲ��۸�
//    public float ColdTime; //ֲ����ȴʱ��
//    public string Description; //ֲ������
//}

//public class CreatDataAsset : Editor
//{
//    private enum ExcelTitleEnum
//    {
//        Name,//����
//        Icon,//ͼƬ
//        IconPath,//ͼƬ��Դ·��
//        Price,//�۸�
//        ColdTime,//��ȴʱ��
//        Description//����
//    }


//    [MenuItem("Tools/CreateAssetFromExcel")]
//    static void CreateAssetFromExcel()
//    {
//        //Excel���ļ�·��
//        string excelPath = Path.Combine(Application.dataPath, "Editor/Datas.xlsx");

//        //��ȡExcel����������
//        using var package = new ExcelPackage(new FileInfo(excelPath));

//        foreach (var worksheet in package.Workbook.Worksheets)
//        {
//            //CreateBinaryDataAsset(worksheet);

//            //CreateJsonDataAsset(worksheet);

//            CreateScriptObjectAsset(worksheet);
//        }
//    }

//    //private static void CreateBinaryDataAsset(ExcelWorksheet worksheet)
//    //{
//    //    int startRow = 2, startCol = 1;

//    //    //����ȡ��Excel���ݸ�ֵ����Ӧ�Ŀ����л��ֶ�
//    //    List<SerializableData> list = new();
//    //    for (int i = startRow; i < worksheet.Dimension.Rows; i++)
//    //    {
//    //        SerializableData data = new SerializableData();
//    //        //����ȡ��Excel���ݸ�ֵ����Ӧ������
//    //        data.PlantName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
//    //        data.Price = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Price].Text);
//    //        data.Description = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
//    //        data.IconPath = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.IconPath].Text;

//    //        list.Add(data);
//    //    }

//    //    //�������л�����תΪBinaryData�����浽Ŀ��·��
//    //    string savePath = Application.dataPath + "/Resources/BinaryDataAsset/data.dat";
//    //    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//    //        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

//    //    using FileStream fs = new(savePath, FileMode.Create);
//    //    BinaryFormatter binaryFormatter = new BinaryFormatter();
//    //    binaryFormatter.Serialize(fs, list);
//    //    Debug.Log("����BinaryData");
//    //}

//    //private static void CreateJsonDataAsset(ExcelWorksheet worksheet)
//    //{
//    //    int startRow = 2, startCol = 1;

//    //    //����ȡ��Excel���ݸ�ֵ����Ӧ�Ŀ����л��ֶ�
//    //    List<SerializableData> list = new();
//    //    for (int i = startRow; i < worksheet.Dimension.Rows; i++)
//    //    {
//    //        SerializableData data = new SerializableData();
//    //        data.PlantName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
//    //        data.Price = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Price].Text);
//    //        data.Description = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
//    //        data.IconPath = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.IconPath].Text;

//    //        list.Add(data);
//    //    }

//    //    //�������л�����תΪJsonData�����浽Ŀ��·��
//    //    string savePath = Application.dataPath + "/Resources/JsonDataAsset/data.json";
//    //    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//    //        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
//    //    File.WriteAllText(savePath, JsonConvert.SerializeObject(list));
//    //    Debug.Log("����JsonData");
//    //}

//    private static void CreateScriptObjectAsset(ExcelWorksheet worksheet)
//    {
//        //(��ʼ����[1,1]\��Ч��������[2,1])
//        int startRow = 2, startCol = 1;
//        for (int i = startRow; i < worksheet.Dimension.Rows; i++)
//        {
//            //����ScriptObject����
//            Plant plant = ScriptableObject.CreateInstance<Plant>();

//            //����ȡ��Excel���ݸ�ֵ����Ӧ������
//            plant.PlantName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
//            plant.Price = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Price].Text);
//            plant.Description = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;

//            string iconPath = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.IconPath].Text;
//            plant.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);

//            plant.ColdTime = float.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ColdTime].Text);

//            //��ȡ�ļ���
//            string fileName = Path.GetFileNameWithoutExtension(iconPath);

//            //��֤����·���ļ��д���
//            string savePath = $"Assets/Resources/ScriptObjectAsset/{fileName}.asset";
//            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

//            //���Ѹ�ֵ��ScriptObject���浽Ŀ��·����
//            AssetDatabase.CreateAsset(plant, savePath);

//        }
//        AssetDatabase.SaveAssets();
//        Debug.Log("����ScriptObject");
//    }

//}
