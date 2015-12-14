using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class PrintTemplateExplainer
    {
        private string PrintTemplateStrings;
        private List<string[]> bodyOrderConfigs;
        private List<string[]> orderColumnValue;

        public PrintTemplateExplainer(string tempstring)
        {
            PrintTemplateStrings = tempstring;
            bodyOrderConfigs = new List<string[]>();
            orderColumnValue = new List<string[]>();
        }

        public string[] GenerateContent(ReportClass reportClass)
        {
            string[] tempArray = getTemplate(PrintTemplateStrings);

            List<string> list = getString(tempArray, reportClass);

            string[] result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = list[i];
            }
            return result;
        }

        private string[] getTemplate(string content)
        {
            string[] allstring = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return allstring;
        }

        private List<string> getString(string[] tempArray, ReportClass reportClass)
        {
            //存放号码和tabnum
            List<string> objectNum = new List<string>();
            //存放菜单
            List<string> menus = new List<string>();
            string mark = "";
            //对齐方式
            string marklength = "";
            string markVariableName = "";
            string alignStyle = "";

            bool beginText = false;

            //循环解析
            for (int i = 0; i < tempArray.Length; i++)
            {
                string findstring = tempArray[i];


                if (findstring.IndexOf("variable") != -1)
                {
                    string findstring1 = findstring.Replace("<variable ", "");
                    findstring1 = findstring1.Replace(">", ",");
                    string[] result = findstring1.Split(new string[] { "," }, StringSplitOptions.None);
                    mark = result[0];
                    marklength = result[2];
                    markVariableName = result[1];
                    alignStyle = result[3];
                    objectNum.Add(mark + ":" + marklength + ":" + markVariableName + ":" + alignStyle);
                }
                else if (findstring.IndexOf("variable") > 0)
                {
                    //MessageBox.Show("该模板无效");
                }
                else if (findstring.Contains("<Text>"))
                {
                    beginText = true;
                }
                else if (findstring.Contains("</Text>"))
                {
                    beginText = false;
                }
                else
                {
                    if (beginText)
                    {
                        menus.Add(findstring);
                    }
                }
            }
            return getTextList(objectNum, menus, reportClass);
        }

        private int calculateStringLength(string str)
        {
            int result = 0;
            char[] strCharArray = str.ToCharArray(0, str.Length);
            for (int i = 0; i < strCharArray.GetLength(0); i++)
            {
                if (Convert.ToInt32(strCharArray[i]) > 255)
                {
                    result += 2;
                }
                else
                {
                    result++;
                }
            }
            return result;
        }

        private List<int> recordLineStringLength(string str)
        {
            char[] strCharArray = str.ToCharArray(0, str.Length);
            List<int> charLength = new List<int>();
            for (int i = 0; i < strCharArray.GetLength(0); i++)
            {
                if (Convert.ToInt32(strCharArray[i]) > 255)
                {
                    charLength.Add(2);
                }
                else
                {
                    charLength.Add(1);
                }
            }
            return charLength;
        }

        private string replaceReportRow(string originalRow, string variableStr, string variableValue, int variableLength)
        {
            string result = "";
            string variableStr1 = variableStr.PadRight(variableLength, ' ');
            if (originalRow.IndexOf(variableStr1) >= 0)
            {
                result = originalRow.Replace(variableStr1, variableValue);
            }
            else
            {
                string variableStr2 = variableStr;
                if (originalRow.IndexOf(variableStr2) >= 0)
                {
                    result = originalRow.Replace(variableStr, variableValue);
                }
            }
            return result;
        }

        private List<string> getOrderConfig(string bodyID)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < bodyOrderConfigs.Count; i++)
            {
                if (bodyOrderConfigs[i][0].Equals(bodyID))
                {
                    result.AddRange(bodyOrderConfigs[i]);
                }
            }
            return result;
        }

        private List<string> getTextList(List<string> objectNum, List<string> menus, ReportClass reportClass)
        {
            List<string> list = new List<string>();
            int length = objectNum.Count;
            int menuLength = menus.Count;
            for (int i = 0; i < menuLength; i++)
            {
                //循环截取出号码
                for (int j = 0; j < length; j++)
                {
                    string tempString = objectNum[j];
                    string[] temp = tempString.Split(new string[] { ":" }, StringSplitOptions.None);
                    string variableStr = "<" + temp[0] + ">";
                    string variableName = temp[2];
                    string markMaxLength = temp[1];
                    string markAlignStyle = temp[3];
                    if (menus[i].IndexOf(variableStr) != -1)
                    {
                        string variableValue = mappingVariable(variableName, reportClass);
                        int len = calculateStringLength(variableValue);
                        int variableLength = Convert.ToInt32(markMaxLength);
                        if (len < variableLength)
                        {
                            if (markAlignStyle.ToLower().Equals("left"))
                            {
                                variableValue = variableValue.PadRight(variableLength - (len - variableValue.Length), ' ');
                            }
                            else if (markAlignStyle.ToLower().Equals("right"))
                            {
                                variableValue = variableValue.PadLeft(variableLength - (len - variableValue.Length), ' ');
                            }
                            else if (markAlignStyle.ToLower().Equals("center"))
                            {
                                string variableValue1 = variableValue.PadRight(variableValue.Length + (variableLength - (len - variableValue.Length) - variableValue.Length) / 2, ' ');
                                variableValue = variableValue1.PadLeft(variableLength - (len - variableValue.Length), ' ');
                            }
                        }
                        else if (len > variableLength)
                        {
                            if (len == variableValue.Length)
                            {
                                variableValue = variableValue.Substring(0, variableLength);
                            }
                            else
                            {
                                variableValue = variableValue.Substring(0, variableLength - (len - variableValue.Length));
                            }
                        }
                        menus[i] = replaceReportRow(menus[i], variableStr, variableValue, variableLength);
                    }
                }
                if (menus[i].IndexOf("<Order>") != -1)
                {
                    string orderConfigString = menus[i];
                    orderConfigString = orderConfigString.Replace("<Order>", "");
                    string[] orderConfigs = orderConfigString.Split(new char[] { ',' });
                    bodyOrderConfigs.Add(orderConfigs);
                }
                else if (menus[i].IndexOf("</For>") != -1)
                {

                }
                else if (menus[i].IndexOf("<For>") != -1)
                {
                    string bodyID = menus[i].Replace("<For>", "").Replace("<", "").Replace(">", "");
                    i++;
                    string[] duixiang = menus[i].ToString().Split(new string[] { ">" }, StringSplitOptions.None);
                    ReportClass.ReportBodyGrid reportBodyGrid = reportClass.getBodyGrid(bodyID);
                    List<string> currentBodyContent = new List<string>();
                    List<string> orderConfig = getOrderConfig(bodyID);
                    bool needOrder = false;
                    if (orderConfig.Count > 0)
                    {
                        needOrder = true;
                    }
                    currentBodyContent = new List<string>();
                    for (int m = 0; m < reportBodyGrid.bodyGridValue.Count; m++)
                    {
                        string loopItem = menus[i];
                        for (int k = 0; k < length; k++)
                        {
                            string tempString = objectNum[k];
                            string[] temp = tempString.Split(new string[] { ":" }, StringSplitOptions.None);
                            string variableStr = "<" + temp[0] + ">";
                            string variableName = temp[2];
                            string markMaxLength = temp[1];
                            string markAlignStyle = temp[3];
                            string variableValue = "";
                            if (menus[i].IndexOf(variableStr) != -1)
                            {
                                variableValue = mappingVariable(variableName, reportBodyGrid, m);
                                int len = calculateStringLength(variableValue);
                                int variableLength = Convert.ToInt32(markMaxLength);
                                if (len < variableLength)
                                {
                                    if (markAlignStyle.ToLower().Equals("left"))
                                    {
                                        variableValue = variableValue.PadRight(variableLength - (len - variableValue.Length), ' ');
                                    }
                                    else if (markAlignStyle.ToLower().Equals("right"))
                                    {
                                        variableValue = variableValue.PadLeft(variableLength - (len - variableValue.Length), ' ');
                                    }
                                    else if (markAlignStyle.ToLower().Equals("center"))
                                    {
                                        string variableValue1 = variableValue.PadRight(variableValue.Length + (variableLength - (len - variableValue.Length) - variableValue.Length) / 2, ' ');
                                        variableValue = variableValue1.PadLeft(variableLength - (len - variableValue.Length), ' ');
                                    }
                                }
                                else if (len > variableLength)
                                {

                                    List<int> currentLineLength = recordLineStringLength(variableValue);
                                    int charNumber = 0;
                                    int lineLength = 0;
                                    for (int j = 0; j < currentLineLength.Count; j++)
                                    {
                                        lineLength += currentLineLength[j];
                                        if (lineLength <= variableLength)
                                        {
                                            charNumber++;
                                        }
                                        else
                                        {
                                            lineLength -= currentLineLength[j];
                                            break;
                                        }
                                    }
                                    string vvm = variableValue.Substring(0, charNumber);
                                    string variableValue1 = vvm + "\r\n";
                                    string vvmn = variableValue.Replace(vvm, "");
                                    variableValue = variableValue1 + vvmn.PadRight(variableLength - (len - lineLength - vvmn.Length), ' ');

                                    //if (len == variableValue.Length)
                                    //{
                                    //    variableValue = variableValue.Substring(0, variableLength);
                                    //}
                                    //else
                                    //{
                                    //    variableValue = variableValue.Substring(0, variableLength - (len - variableValue.Length));
                                    //}
                                }
                                loopItem = replaceReportRow(loopItem, variableStr, variableValue, variableLength);
                            }
                            if (needOrder)
                            {
                                if (variableName.Equals(orderConfig[1]))
                                {
                                    string[] currentOrderColumnValue = new string[] { variableValue, currentBodyContent.Count.ToString() };
                                    orderColumnValue.Add(currentOrderColumnValue);
                                }
                            }
                        }
                        loopItem += "\r\n";
                        //list.Add(loopItem);
                        currentBodyContent.Add(loopItem);
                    }
                    if (needOrder)
                    {
                        list.AddRange(orderBodyContent(currentBodyContent, orderConfig[2]));
                    }
                    else
                    {
                        list.AddRange(currentBodyContent);
                    }
                }
                else
                {
                    if (menus[i].EndsWith("\r\n"))
                    {

                    }
                    else if (menus[i].EndsWith("\r\n\n"))
                    {
                        menus[i] = menus[i].Replace("\r\n\n", "\r\n");
                    }
                    else
                    {
                        menus[i] += "\r\n";
                    }
                    list.Add(menus[i]);
                }
            }
            return list;
        }

        private string mappingVariable(string variableName, ReportClass reportClass)
        {
            string result = "";
            result = reportClass.reportText.getVariableValue(variableName).Replace(".00", "");
            return result;
        }

        private string mappingVariable(string variableName, ReportClass.ReportBodyGrid reportBodyGrid, int index)
        {
            string result = "";
            result = reportBodyGrid.getBodyGridValue(index, variableName).Replace(".00", "");
            return result;
        }

        private List<string> orderBodyContent(List<string> currentBodyContent, string orderMethod)
        {
            List<string> result = new List<string>();
            int compI = 0;
            if (orderMethod.ToUpper().Equals("DESC"))
            {
                compI = -1;
            }
            else if (orderMethod.ToUpper().Equals("ASC"))
            {
                compI = 1;
            }
            for (int i = 0; i < orderColumnValue.Count - 1; i++)
            {
                string v1 = orderColumnValue[i][0];
                string v2 = orderColumnValue[i + 1][0];
                try
                {
                    double d1 = Convert.ToDouble(v1);
                    double d2 = Convert.ToDouble(v2);

                    if (d1 * compI > d2 * compI)
                    {
                        orderColumnValue.Reverse(i, 2);
                        i = -1;
                    }
                    else
                    {

                    }
                }
                catch (Exception exp)
                {
                    if (string.Compare(v1, v2) * compI > 0)
                    {
                        orderColumnValue.Reverse(i, 2);
                        i = -1;
                    }
                    else
                    {

                    }
                }
            }
            for (int i = 0; i < currentBodyContent.Count; i++)
            {
                result.Add(currentBodyContent[Convert.ToInt32(orderColumnValue[i][1])]);
            }
            return result;
        }
    }

    public class ReportClass
    {
        public string ID, Name;
        public ReportText reportText;
        public List<ReportBodyGrid> reportBodyGridList;

        public ReportClass()
        {
            ID = "";
            Name = "";
            reportText = new ReportText();
            reportBodyGridList = new List<ReportBodyGrid>();
        }

        public void addBodyGrid(ReportBodyGrid reportBodyGrid)
        {
            reportBodyGridList.Add(reportBodyGrid);
        }

        public ReportBodyGrid getBodyGrid(string reportBodyID)
        {
            ReportBodyGrid result = new ReportBodyGrid();
            for (int i = 0; i < reportBodyGridList.Count; i++)
            {
                if (reportBodyGridList[i].ID.Equals(reportBodyID))
                {
                    result = reportBodyGridList[i];
                    break;
                }
            }
            return result;
        }

        public string getBodyGridValue(string reportBodyID, int rowIndex, string columnName)
        {
            string result = "";
            result = getBodyGrid(reportBodyID).getBodyGridValue(rowIndex, columnName);
            return result;
        }

        public class ReportVariable
        {
            public string ID, Name, Value;

            public ReportVariable(string ID, string Name, string Value)
            {
                this.ID = ID;
                this.Name = Name;
                this.Value = Value;
            }
        }

        public class ReportText
        {
            public List<ReportVariable> TextVariables;

            public ReportText()
            {
                TextVariables = new List<ReportVariable>();
            }

            public string getVariableValue(string variableName)
            {
                string result = "";
                foreach (ReportVariable rv in TextVariables)
                {
                    if (rv.Name.ToLower().Equals(variableName.ToLower()))
                    {
                        result = rv.Value.ToString();
                    }
                }
                return result;
            }

            public void addTextVariable(List<string> paras)
            {
                addTextVariable(paras[0], paras[1], paras[2]);
            }

            public void addTextVariable(string ID, string Name, string Value)
            {
                TextVariables.Add(new ReportVariable(ID, Name, Value));
            }
        }

        public class ReportBodyGrid
        {
            public string ID;
            public List<string> columnList;
            public List<List<string>> bodyGridValue;

            public ReportBodyGrid()
            {
                ID = "";
                columnList = new List<string>();
                bodyGridValue = new List<List<string>>();
            }

            public ReportBodyGrid(string ID)
            {
                this.ID = ID;
                columnList = new List<string>();
                bodyGridValue = new List<List<string>>();
            }

            public string getBodyGridValue(int rowIndex, int columnIndex)
            {
                string result = "";
                try
                {
                    result = bodyGridValue[rowIndex][columnIndex];
                }
                catch (Exception exp)
                {

                }
                return result;
            }

            public string getBodyGridValue(int rowIndex, string columnName)
            {
                int columnIndex = columnList.IndexOf(columnName);
                return getBodyGridValue(rowIndex, columnIndex);
            }

            public void defineColumns(List<string> columnList)
            {
                if (bodyGridValue.Count == 0)
                {
                    this.columnList = columnList;
                }
            }

            public void defineColumns(string[] columnList)
            {
                List<string> columnArray = new List<string>();
                for (int i = 0; i < columnList.GetLength(0); i++)
                {
                    columnArray.Add(columnList[i]);
                }
                defineColumns(columnArray);
            }

            public void addRow(List<string> newRow)
            {
                if (newRow.Count == columnList.Count)
                {
                    bodyGridValue.Add(newRow);
                }
            }

            public void addRow(string[] newRow)
            {
                List<string> rowArray = new List<string>();
                for (int i = 0; i < newRow.GetLength(0); i++)
                {
                    rowArray.Add(newRow[i]);
                }
                addRow(rowArray);
            }
        }

        public class ReportLogo
        {

        }
    }
}
