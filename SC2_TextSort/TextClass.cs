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
        private int useCount;
        private int useCountTemp;
        private string zh_CN;
        private string en_US;
        private int firstTextLineNumber = -1;
        string originString;
        bool haveEN_US;

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
            zh_CN = textLine.Substring(idLength + 1);
            en_US = "";
            originString = textLine;
            haveEN_US = false;
        }

        public void WriteTxt(StreamWriter txtWriter)
        {
            txtWriter.WriteLine(id + "=" + en_US);
        }
                        
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
                if (textIndex == 1)
                {
                    csvWriter.WriteField(lineNumber);
                    csvWriter.WriteField(galaxyCodeLine);
                }
                else
                {
                    csvWriter.WriteField("");
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(textIndex);
                textIndex++;
                csvWriter.WriteField(select.Id);
                List<TextInStringTxt> exists = list.Where(r => r.FirstTextLineNumber != -1 && r.ZH_CN == select.ZH_CN && r != select).ToList();
                if (exists.Count == 0)
                {
                    csvWriter.WriteField(select.UseCountTemp);
                    if (select.UseCountTemp == 1)
                    {
                        select.FirstTextLineNumber = writeLine;
                        csvWriter.WriteField(writeLine);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(select.ZH_CN);
                    }
                    else
                    {
                        csvWriter.WriteField(select.FirstTextLineNumber);
                        csvWriter.WriteField(select.ZH_CN);
                        csvWriter.WriteField(select.Id);
                    }
                }
                else
                {
                    csvWriter.WriteField(select.UseCountTemp);
                    csvWriter.WriteField(exists.First().FirstTextLineNumber);
                    csvWriter.WriteField(select.ZH_CN);
                    csvWriter.WriteField(exists.First().Id);
                }
                select.UseCountTemp++;
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
