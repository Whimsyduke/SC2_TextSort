using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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

        public TextInStringTxt(int textIndex, string textLine)
        {
            index = textIndex;
            useCount = 1;
            useCountTemp = 1;
            int idLength = textLine.IndexOf('=');
            id = textLine.Substring(0, idLength);
            zh_CN = textLine.Substring(idLength + 1);
            en_US = "";
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
            foreach (var select in match)
            {
                TextInStringTxt selectItem = textList.Where(r => r.Id == select.ToString()).First();
                selectItem.UseCount++;
                newCodeLine.TextList.Add(selectItem);
            }
            return newCodeLine;
        }

        /// <summary>
        /// 写入CSV文件
        /// </summary>
        /// <param name="csvWriter"></param>
        public void CsvWrite(CsvWriter csvWriter)
        {
            csvWriter.WriteField(lineNumber);
            csvWriter.WriteField(galaxyCodeLine);
            csvWriter.NextRecord();
            csvWriter.WriteField("ID:");
            foreach (TextInStringTxt select in textList)
            {
                csvWriter.WriteField(select.Id);
            }
            csvWriter.NextRecord();
            csvWriter.WriteField("USE COUNT:");
            foreach (TextInStringTxt select in textList)
            {
                csvWriter.WriteField(select.UseCountTemp);
                select.UseCountTemp++;
            }
            csvWriter.NextRecord();
            csvWriter.WriteField("ZH_CN:");
            foreach (TextInStringTxt select in textList)
            {
                csvWriter.WriteField(select.ZH_CN);
            }
            csvWriter.NextRecord();
            csvWriter.WriteField("EN_US:");
            foreach (TextInStringTxt select in textList)
            {
                csvWriter.WriteField(select.EN_US);
            }
            csvWriter.NextRecord();
        }

        public static TextInGalaxyCodeLine CsvRead(CsvReader csvReader, List<TextInStringTxt> textList)
        {
            TextInGalaxyCodeLine newCodeLine = new TextInGalaxyCodeLine();
            return newCodeLine;
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
