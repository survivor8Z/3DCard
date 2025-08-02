//using OfficeOpenXml;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEditor;
//using UnityEngine;

//public class Plant : ScriptableObject
//{
//    public string PlantName; //植物名称
//    public Sprite Icon; //植物图标
//    public string IconPath; //植物图标资源路径
//    public int Price; //植物价格
//    public float ColdTime; //植物冷却时间
//    public string Description; //植物描述
//}

//public class CreatDataAsset : Editor
//{
//    private enum ExcelTitleEnum
//    {
//        Name,//名字
//        Icon,//图片
//        IconPath,//图片资源路径
//        Price,//价格
//        ColdTime,//冷却时间
//        Description//描述
//    }


//    [MenuItem("Tools/CreateAssetFromExcel")]
//    static void CreateAssetFromExcel()
//    {
//        //Excel的文件路径
//        string excelPath = Path.Combine(Application.dataPath, "Editor/Datas.xlsx");

//        //读取Excel的内容数据
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

//    //    //将读取的Excel数据赋值给相应的可序列化字段
//    //    List<SerializableData> list = new();
//    //    for (int i = startRow; i < worksheet.Dimension.Rows; i++)
//    //    {
//    //        SerializableData data = new SerializableData();
//    //        //将读取的Excel数据赋值给相应的属性
//    //        data.PlantName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
//    //        data.Price = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Price].Text);
//    //        data.Description = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;
//    //        data.IconPath = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.IconPath].Text;

//    //        list.Add(data);
//    //    }

//    //    //将可序列化数据转为BinaryData并保存到目标路径
//    //    string savePath = Application.dataPath + "/Resources/BinaryDataAsset/data.dat";
//    //    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//    //        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

//    //    using FileStream fs = new(savePath, FileMode.Create);
//    //    BinaryFormatter binaryFormatter = new BinaryFormatter();
//    //    binaryFormatter.Serialize(fs, list);
//    //    Debug.Log("生成BinaryData");
//    //}

//    //private static void CreateJsonDataAsset(ExcelWorksheet worksheet)
//    //{
//    //    int startRow = 2, startCol = 1;

//    //    //将读取的Excel数据赋值给相应的可序列化字段
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

//    //    //将可序列化数据转为JsonData并保存到目标路径
//    //    string savePath = Application.dataPath + "/Resources/JsonDataAsset/data.json";
//    //    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//    //        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
//    //    File.WriteAllText(savePath, JsonConvert.SerializeObject(list));
//    //    Debug.Log("生成JsonData");
//    //}

//    private static void CreateScriptObjectAsset(ExcelWorksheet worksheet)
//    {
//        //(开始坐标[1,1]\有效数据坐标[2,1])
//        int startRow = 2, startCol = 1;
//        for (int i = startRow; i < worksheet.Dimension.Rows; i++)
//        {
//            //创建ScriptObject对象
//            Plant plant = ScriptableObject.CreateInstance<Plant>();

//            //将读取的Excel数据赋值给相应的属性
//            plant.PlantName = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Name].Text;
//            plant.Price = int.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Price].Text);
//            plant.Description = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.Description].Text;

//            string iconPath = worksheet.Cells[i, startCol + (int)ExcelTitleEnum.IconPath].Text;
//            plant.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);

//            plant.ColdTime = float.Parse(worksheet.Cells[i, startCol + (int)ExcelTitleEnum.ColdTime].Text);

//            //获取文件名
//            string fileName = Path.GetFileNameWithoutExtension(iconPath);

//            //保证保存路径文件夹存在
//            string savePath = $"Assets/Resources/ScriptObjectAsset/{fileName}.asset";
//            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
//                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

//            //将已赋值的ScriptObject保存到目标路径下
//            AssetDatabase.CreateAsset(plant, savePath);

//        }
//        AssetDatabase.SaveAssets();
//        Debug.Log("生成ScriptObject");
//    }

//}
