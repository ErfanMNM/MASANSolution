using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#region ECharts Option Classes

public class LabelOption
{
    public bool show { get; set; }
    public string position { get; set; }
    public int distance { get; set; }
    public string align { get; set; }
    public string verticalAlign { get; set; }
    public int rotate { get; set; }
    public string formatter { get; set; }
    public int fontSize { get; set; }
    public string fontFamily { get; set; }
    public Dictionary<string, object> rich { get; set; }
}

public class AxisPointer
{
    public string type { get; set; }
}

public class Tooltip
{
    public string trigger { get; set; }
    public AxisPointer axisPointer { get; set; }
}

public class Feature
{
    public Mark mark { get; set; }
    public DataView dataView { get; set; }
    public MagicType magicType { get; set; }
    public Restore restore { get; set; }
    public SaveAsImage saveAsImage { get; set; }
}

public class Mark { public bool show { get; set; } }
public class DataView { public bool show { get; set; } public bool readOnly { get; set; } }
public class MagicType { public bool show { get; set; } public List<string> type { get; set; } }
public class Restore { public bool show { get; set; } }
public class SaveAsImage { public bool show { get; set; } }

public class Toolbox
{
    public bool show { get; set; }
    public string orient { get; set; }
    public string left { get; set; }
    public string top { get; set; }
    public Feature feature { get; set; }
}

public class XAxis
{
    public string type { get; set; }
    public AxisTick axisTick { get; set; }
    public List<string> data { get; set; }
}

public class AxisTick
{
    public bool show { get; set; }
}

public class YAxis
{
    public string type { get; set; }
}

public class Emphasis
{
    public string focus { get; set; }
}

public class Series
{
    public string name { get; set; }
    public string type { get; set; }
    public int? barGap { get; set; }
    public LabelOption label { get; set; }
    public Emphasis emphasis { get; set; }
    public List<int> data { get; set; }
}

public class EChartsOption
{
    public Tooltip tooltip { get; set; }
    public Toolbox toolbox { get; set; }
    public List<XAxis> xAxis { get; set; }
    public List<YAxis> yAxis { get; set; }
    public List<Series> series { get; set; }
}

#endregion

public static class EChartsInjector
{
    public static void InjectEChartsOption(string inputHtmlPath, EChartsOption optionObject, string outputHtmlPath = null)
    {
        if (!File.Exists(inputHtmlPath))
            throw new FileNotFoundException("Không tìm thấy file template HTML", inputHtmlPath);

        string html = File.ReadAllText(inputHtmlPath);

        string json = JsonConvert.SerializeObject(optionObject, Formatting.Indented);

        string pattern = @"option\s*=\s*\{[\s\S]*?\};";
        string replacement = $"option = {json};";

        string newHtml = Regex.Replace(html, pattern, replacement);

        File.WriteAllText(outputHtmlPath ?? inputHtmlPath, newHtml);

        Console.WriteLine($"✅ Đã inject ECharts option vào: {(outputHtmlPath ?? inputHtmlPath)}");
    }
}

