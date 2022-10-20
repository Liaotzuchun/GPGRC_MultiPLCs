using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Models;

public class StreamReaderIni
{
    /// <summary>
    /// ini檔案 Section列表
    /// </summary>
    public Dictionary<string, IniElementConstruct> Sections { get; set; }

    /// <summary> 使用Stream Builder建立空Ini檔案</summary>
    public StreamReaderIni()
    {
        Sections = new Dictionary<string, IniElementConstruct>();
    }

    /// <summary> 使用Stream Builder建立Ini檔案</summary>
    /// <param name="Path"> 檔案位置</param>
    public StreamReaderIni(string Path)
    {
        DecodingIni(Path, Encoding.ASCII);
    }

    /// <summary>
    /// 增加Ini的Section
    /// </summary>
    /// <param name="SectionName"> 新增的Section名字</param>
    /// <returns>新增的Section Item</returns>
    public IniElementConstruct AddIniSection(string SectionName)
    {
        var Tmp = new IniElementConstruct();
        Sections.Add(SectionName, Tmp);
        return Tmp;
    }

    /// <summary> 使用Stream Builder建立Ini檔案</summary>
    /// <param name="Path"> 檔案位置</param>
    /// /// <param name="Encoding"> 解析編碼</param>
    public StreamReaderIni(string Path, Encoding Encoding)
    {
        DecodingIni(Path, Encoding);
    }

    /// <summary>
    /// 為Ini檔案解碼
    /// </summary>
    /// <param name="Path">檔案位置</param>
    /// <param name="Encoding">解碼方式</param>
    public void DecodingIni(string Path, Encoding Encoding)
    {
        if (!File.Exists(Path))
        {
            return;
        }

        using var sr = new StreamReader(Path, Encoding);
        Sections = new Dictionary<string, IniElementConstruct>();

        IniElementConstruct iniElementConstruct = null;
        while (sr.ReadLine() is { } s)
        {
            //string s = sr.ReadLine();
            //空值或空字串判斷
            if (s == "")
            {
                continue;
            }

            //以;或是#開頭作註解的判斷
            if (Regex.Match(s, @"^(;|#).*$").Success)
            {
                continue;
            }

            //讀取[Section]
            if (Regex.Match(s, @"^\[.*\]").Success)
            {
                iniElementConstruct = AddIniSection(s.Trim().Substring(1, s.Length - 2));
                //Sections.Add(s.Trim().Substring(1, s.Length - 2), iniElementConstruct);
                continue;
            }

            //如果Section存在，才去判斷Key
            if (iniElementConstruct != null)
            {
                var KeyValue = s.Split('=');
                iniElementConstruct.ItemElements.Add(KeyValue[0].Trim(), KeyValue[1].Trim());
            }
        }
    }

    /// <summary> 輸出Ini </summary>
    public Task EncodindIni(string Path)
    {
        return EncodindIni(Path, new UTF8Encoding(false));
    }

    /// <summary> 輸出Ini </summary>
    public async Task EncodindIni(string Path, Encoding Encoding)
    {
        var sb = new StringBuilder();

        foreach (var section in Sections)
        {
            sb.AppendLine($"[{section.Key}]");

            foreach (var item in section.Value.ItemElements)
            {
                sb.AppendLine($"{item.Key} = {item.Value}");
            }
        }

        using var outputFile = new StreamWriter(Path, false, Encoding);
        await outputFile.WriteAsync(sb.ToString());
    }
}

/// <summary> Ini章節結構</summary>
public class IniElementConstruct
{
    /// <summary>
    /// ItemElement 的集合
    /// </summary>
    public Dictionary<string, string> ItemElements { get; set; }

    /// <summary> Ini章節結構</summary>
    public IniElementConstruct()
    {
        ItemElements = new Dictionary<string, string>();
    }

    /// <summary>
    /// 增加一元素
    /// </summary>
    /// <param name="Key">Key</param>
    /// <param name="Value">值</param>
    public void AddElement(string Key, string Value)
    {
        ItemElements.Add(Key, Value);
    }
}