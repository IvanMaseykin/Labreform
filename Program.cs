using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Labreform
{
    class Program
    {
        static void Main(string[] args)
        {
            string valuesFilePath = "Args/tests.json"; 
            string testsFilePath = "Args/values.json"; 
            string reportFilePath = "Args/report.json"; 

            if (File.Exists(valuesFilePath) && File.Exists(testsFilePath))
            {
                Dictionary<int, string> valuesData = LoadValuesData(valuesFilePath);
                TestStructure testsData = LoadTestsData(testsFilePath);

                FillValues(testsData, valuesData);

                SaveData(reportFilePath, testsData);

                Console.WriteLine("Отчет успешно сформирован: " + reportFilePath);
            }
            else
            {
                Console.WriteLine("Ошибка: Указанные файлы не существуют.");
            }
        }

        static Dictionary<int, string> LoadValuesData(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string jsonString = sr.ReadToEnd();
                var jsonData = JsonSerializer.Deserialize<ValuesData>(jsonString);
                Dictionary<int, string> valuesDict = new Dictionary<int, string>();
                foreach (var item in jsonData.Values)
                {
                    valuesDict[item.Id] = item.Value;
                }
                return valuesDict;
            }
        }

        static TestStructure LoadTestsData(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string jsonString = sr.ReadToEnd();
                return JsonSerializer.Deserialize<TestStructure>(jsonString);
            }
        }

        static void FillValues(TestStructure testsData, Dictionary<int, string> valuesData)
        {
            if (valuesData.ContainsKey(testsData.Id))
            {
                testsData.Value = valuesData[testsData.Id];
            }
            if (testsData.Values != null)
            {
                foreach (var child in testsData.Values)
                {
                    FillValues(child, valuesData);
                }
            }
        }

        static void SaveData(string filePath, TestStructure data)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                sw.Write(jsonString);
            }
        }

        class ValuesData
        {
            public List<ValueItem> Values { get; set; }
        }

        class ValueItem
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        class TestStructure
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
            public List<TestStructure> Values { get; set; }
        }
    }
}
