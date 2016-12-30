using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using CsvHelper;

namespace SC2_TextSort
{
    
    /// <summary>
    /// 文本列表中的字符串
    /// </summary>
    public class TextInStringTxt
    {
        private int index;
        private string id;
        private string type;
        private int useCount;
        private int useCountTemp;
        private string zh_CN;
        private string en_US;
        private string patch_CN;
        private int firstTextLineNumber = -1;
        bool inGalaxy;
        string originString;
        bool haveEN_US;
        bool updataCN;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textIndex">文本行数</param>
        /// <param name="textLine">文本内容</param>
        public TextInStringTxt(int textIndex, string textLine)
        {
            index = textIndex;
            useCount = 1;
            useCountTemp = 1;
            int idLength = textLine.IndexOf('=');
            id = textLine.Substring(0, idLength);
            type = id.Substring(0, id.IndexOf('/'));
            zh_CN = textLine.Substring(idLength + 1);
            en_US = "";
            patch_CN = "";
            originString = textLine;
            haveEN_US = false;
            inGalaxy = false;
            updataCN = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id_text">id</param>
        /// <param name="cn_text">中文文本</param>
        /// <param name="en_text">英文文本</param>
        public TextInStringTxt(string id_text, string cn_text, string en_text)
        {
            index = 0;
            useCount = 1;
            useCountTemp = 1;
            id = id_text;
            type = id.Substring(0, id.IndexOf('/'));
            zh_CN = cn_text;
            en_US = en_text;
            patch_CN = "";
            originString = "";
            haveEN_US = true;
            updataCN = false;
        }

        /// <summary>
        /// 生成Txt文件
        /// </summary>
        /// <param name="txtWriter">写入流</param>
        /// <param name="keepUntranslate">保留未翻译内容</param>
        /// <param name="showKeepId">未翻译内容附加ID关键字</param>
        /// <remarks>未翻译内容关键字在句首，以[]标记。</remarks>
        public void WriteTxt(StreamWriter txtWriter, bool keepUntranslate, bool showKeepId)
        {
            if (keepUntranslate)
            {
                if (en_US != "")
                {
                    txtWriter.WriteLine(id + "=" + en_US);
                }
                else
                {
                    if (showKeepId)
                    {
                        txtWriter.WriteLine(id + "=" + "[" + id.Substring(id.LastIndexOf('/') + 1) + "]" + zh_CN);
                    }
                    else
                    {
                        txtWriter.WriteLine(id + "=" + zh_CN);
                    }
                }
            }
            else
            {
                txtWriter.WriteLine(id + "=" + en_US);
            }
        }

        /// <summary>
        /// 生成Txt文件
        /// </summary>
        /// <param name="txtWriter">写入流</param>
        public void WriteTxt(StreamWriter txtWriter)
        {
            txtWriter.WriteLine(id + "=" + zh_CN);
        }

        /// <summary>
        /// 写入CSV文件
        /// </summary>
        /// <param name="csvWriter"></param>
        public void CsvWrite(CsvWriter csvWriter, List<TextInStringTxt> list, ref int writeLine)
        {
            //位于Galaxy的文本已经单独写入
            if (inGalaxy) return;

            //TextId
            csvWriter.WriteField(id);
            //TextType
            csvWriter.WriteField(type);
            //InGalaxy
            csvWriter.WriteField("NotInGalaxy");
            //GalaxyLine
            csvWriter.WriteField(-1);
            //GalaxyTextIndex
            csvWriter.WriteField(-1);
            //UpdataCN
            if (updataCN)
            {
                csvWriter.WriteField("UpdataCN");
            }
            else
            {
                csvWriter.WriteField("NotUpdataCN");
            }
            //FirstLineNumberInCsv, TextRepeatCount, RepeatTextInZH-CN, PatchTextInZH-CN, NoRepeatZH-CN
            List<TextInStringTxt> exists = list.Where(r => r.FirstTextLineNumber != -1 && r.ZH_CN == zh_CN && r != this).ToList();
            if (exists.Count == 0)
            {
                if (useCountTemp == 1)
                {
                    firstTextLineNumber = writeLine;
                    csvWriter.WriteField(writeLine);
                    csvWriter.WriteField(useCountTemp);
                    csvWriter.WriteField("");
                    csvWriter.WriteField(patch_CN);
                    csvWriter.WriteField(zh_CN);
                }
                else
                {
                    csvWriter.WriteField(firstTextLineNumber);
                    csvWriter.WriteField(useCountTemp);
                    csvWriter.WriteField(zh_CN);
                    csvWriter.WriteField(patch_CN);
                    csvWriter.WriteField(id);
                }
            }
            else
            {
                csvWriter.WriteField(useCountTemp);
                csvWriter.WriteField(exists.First().FirstTextLineNumber);
                csvWriter.WriteField(zh_CN);
                csvWriter.WriteField(patch_CN);
                csvWriter.WriteField(exists.First().Id);
            }
            useCountTemp++;
            //EN-US
            csvWriter.WriteField(en_US);
            csvWriter.NextRecord();
            writeLine++;
        }


        #region 属性

        /// <summary>
        /// 文本在原始文件中的序号
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        /// <summary>
        /// 文本ID
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        /// <summary>
        /// 文本类型
        /// </summary>
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// 使用计数
        /// </summary>
        public int UseCount
        {
            get
            {
                return useCount;
            }

            set
            {
                useCount = value;
            }
        }

        /// <summary>
        /// 使用计数临时
        /// </summary>
        public int UseCountTemp
        {
            get
            {
                return useCountTemp;
            }

            set
            {
                useCountTemp = value;
            }
        }

        /// <summary>
        /// 文本中文内容
        /// </summary>
        public string ZH_CN
        {
            get
            {
                return zh_CN;
            }

            set
            {
                zh_CN = value;
            }
        }

        /// <summary>
        /// 英文文本
        /// </summary>
        public string EN_US
        {
            get
            {
                return en_US;
            }

            set
            {
                en_US = value;
            }
        }

        /// <summary>
        /// 旧版本中文
        /// </summary>
        public string Patch_CN
        {
            get
            {
                return patch_CN;
            }

            set
            {
                patch_CN = value;
            }
        }

        /// <summary>
        /// 文本第一次出现的行号
        /// </summary>
        public int FirstTextLineNumber
        {
            get
            {
                return firstTextLineNumber;
            }

            set
            {
                firstTextLineNumber = value;
            }
        }

        /// <summary>
        /// 原始文本内容
        /// </summary>
        public string OriginString
        {
            get
            {
                return originString;
            }

            set
            {
                originString = value;
            }
        }

        /// <summary>
        /// 已经写入EN_US
        /// </summary>
        public bool HaveEN_US
        {
            get
            {
                return haveEN_US;
            }

            set
            {
                haveEN_US = value;
            }
        }

        /// <summary>
        /// 在Galaxy中存在
        /// </summary>
        public bool InGalaxy
        {
            get
            {
                return inGalaxy;
            }

            set
            {
                inGalaxy = value;
            }
        }

        /// <summary>
        /// 使用补丁时中文被刷新
        /// </summary>
        public bool UpdataCN
        {
            get
            {
                return updataCN;
            }

            set
            {
                updataCN = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// 每行Galaxy代码中包含的Text
    /// </summary>
    public class TextInGalaxyCodeLine
    {
        public static Regex REGULAREXPRESSIONS_STRINGEXTERNAL = new Regex("(?<=StringExternal\\(\")[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*(?=\"\\))");
        int lineNumber;
        string galaxyCodeLine;
        List<TextInStringTxt> textList;

        /// <summary>
        /// 构造函数
        /// </summary>
        private TextInGalaxyCodeLine()
        {
            textList = new List<TextInStringTxt>();
        }

        /// <summary>
        /// 获取每行Galaxy脚本的文本
        /// </summary>
        /// <param name="index">galaxy脚本行号</param>
        /// <param name="galaxyCode"></param>
        /// <returns>一行galaxy脚本包含的Text</returns>
        public static TextInGalaxyCodeLine GetTextListByGalaxyLine(int number, string galaxyCode, List<TextInStringTxt> textList)
        {
            MatchCollection match = REGULAREXPRESSIONS_STRINGEXTERNAL.Matches(galaxyCode);
            if (match.Count == 0) return null;
            TextInGalaxyCodeLine newCodeLine = new TextInGalaxyCodeLine();
            newCodeLine.galaxyCodeLine = galaxyCode;
            newCodeLine.lineNumber = number;
            foreach (var select in match)
            {
                List<TextInStringTxt> selectItems = textList.Where(r => r.Id == select.ToString()).ToList();
                TextInStringTxt selectItem;
                if (selectItems.Count == 0)
                {
                    selectItem = new TextInStringTxt(-1, select.ToString() + "=");
                }
                else
                {
                    selectItem = selectItems.First();
                }
                selectItem.UseCount++;
                newCodeLine.TextList.Add(selectItem);
            }
            return newCodeLine;
        }

        /// <summary>
        /// 写入CSV文件
        /// </summary>
        /// <param name="csvWriter"></param>
        public void CsvWrite(CsvWriter csvWriter, List<TextInStringTxt> list, ref int writeLine)
        {
            int textIndex = 1;
            foreach (TextInStringTxt select in textList)
            {
                //TextId
                csvWriter.WriteField(select.Id);
                //TextType
                csvWriter.WriteField(select.Type);
                //InGalaxy
                if (select.InGalaxy)
                {
                    csvWriter.WriteField("InGalaxy");
                }
                else
                {
                    csvWriter.WriteField("NotInGalaxy");
                }
                //GalaxyLine
                csvWriter.WriteField(lineNumber);
                //GalaxyTextIndex
                csvWriter.WriteField(textIndex);
                textIndex++;
                //UpdataCN
                if (select.UpdataCN)
                {
                    csvWriter.WriteField("UpdataCN");
                }
                else
                {
                    csvWriter.WriteField("NotUpdataCN");
                }
                //FirstLineNumberInCsv, TextRepeatCount, RepeatTextInZH-CN, PatchTextInZH-CN, NoRepeatZH-CN
                List<TextInStringTxt> exists = list.Where(r => r.FirstTextLineNumber != -1 && r.ZH_CN == select.ZH_CN && r != select).ToList();
                if (exists.Count == 0)
                {
                    if (select.UseCountTemp == 1)
                    {
                        select.FirstTextLineNumber = writeLine;
                        csvWriter.WriteField(writeLine);
                        csvWriter.WriteField(select.UseCountTemp);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(select.Patch_CN);
                        csvWriter.WriteField(select.ZH_CN);
                    }
                    else
                    {
                        csvWriter.WriteField(select.FirstTextLineNumber);
                        csvWriter.WriteField(select.UseCountTemp);
                        csvWriter.WriteField(select.ZH_CN);
                        csvWriter.WriteField(select.Patch_CN);
                        csvWriter.WriteField(select.Id);
                    }
                }
                else
                {
                    csvWriter.WriteField(select.UseCountTemp);
                    csvWriter.WriteField(exists.First().FirstTextLineNumber);
                    csvWriter.WriteField(select.ZH_CN);
                    csvWriter.WriteField(select.Patch_CN);
                    csvWriter.WriteField(exists.First().Id);
                }
                select.UseCountTemp++;
                //EN-US
                csvWriter.WriteField(select.EN_US);
                csvWriter.NextRecord();
                writeLine++;
            }
        }

        /// <summary>
        /// 在Galaxy文件中的行号
        /// </summary>
        public int LineNumber
        {
            get
            {
                return lineNumber;
            }

            set
            {
                lineNumber = value;
            }
        }

        /// <summary>
        /// 此行Galaxy脚步内容
        /// </summary>
        public string GalaxyCodeLine
        {
            get
            {
                return galaxyCodeLine;
            }

            set
            {
                galaxyCodeLine = value;
            }
        }

        /// <summary>
        /// Galaxy语句中的Text列表
        /// </summary>
        public List<TextInStringTxt> TextList
        {
            get
            {
                return textList;
            }

            set
            {
                textList = value;
            }
        }
    }
}
