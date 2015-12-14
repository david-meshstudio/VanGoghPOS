using System;
using System.Collections.Generic;
using System.Text;

namespace COM.MeshStudio.Lib.Rule
{
    public class ExpressionExplainer
    {
        private int layerNumber;
        private ExpressionPart expressionMeaning = new ExpressionPart();
        private string errorMessage;
        private List<string> tempPart1, tempPart2, tempPart3,
                tempPart4;
        private List<int> tempPartIndexList;
        private bool isSecondRound = false;

        public List<string> getTempPart1()
        {
            return tempPart1;
        }

        public void setTempPart1(List<string> tempPart)
        {
            this.tempPart1 = tempPart;
        }

        public List<string> getTempPart2()
        {
            return tempPart2;
        }

        public void setTempPart2(List<string> tempPart2)
        {
            this.tempPart2 = tempPart2;
        }

        public List<string> getTempPart3()
        {
            return tempPart3;
        }

        public List<string> getTempPart4()
        {
            return tempPart4;
        }

        public void setTempPart3(List<string> tempPart3)
        {
            this.tempPart3 = tempPart3;
        }

        public void setTempPart4(List<string> tempPart4)
        {
            this.tempPart4 = tempPart4;
        }

        public int getLayerNumber()
        {
            return layerNumber;
        }

        public ExpressionPart getExpressionMeaning()
        {
            return expressionMeaning;
        }

        public string explain(string text)
        {
            expressionMeaning = new ExpressionPart();
            tempPart1 = new List<string>();
            tempPart2 = new List<string>();
            tempPart3 = new List<string>();
            tempPart4 = new List<string>();
            tempPartIndexList = new List<int>();
            string result = "";
            text = standardize(text);
            if (!checkLegality(text))
            {
                result = errorMessage;
            }
            else
            {
                analyzeExpression(text);
            }
            return result;
        }

        private string standardize(string text)
        {
            string result = "";
            text = text.Replace('+', '#');
            text = text.Replace('*', '@');
            text = text.Replace('^', '&');
            result = text.ToUpper();
            result.Trim();
            result = "(" + result + ")";
            return result;
        }

        private bool checkLegality(string text)
        {
            bool result = true;
            char[] textInCharList = text.ToCharArray();
            List<char> legalCharList = CalculationOperator
                    .getLegalCharList();
            if (result)
            {
                for (int i = 0; i < textInCharList.Length; i++)
                {
                    if (!legalCharList.Contains(textInCharList[i]))
                    {
                        result = false;
                        errorMessage = "Contain ilegal charater!";
                        break;
                    }
                }
            }
            if (result)
            {
                int leftSep = 0;
                int rightSep = 0;
                for (int i = 0; i < textInCharList.Length; i++)
                {
                    if (textInCharList[i] == CalculationOperator.legalSeperators[0]
                            .ToCharArray()[0])
                    {
                        leftSep++;
                    }
                    else if (textInCharList[i] == CalculationOperator.legalSeperators[1]
                          .ToCharArray()[0])
                    {
                        rightSep++;
                    }
                }
                if (leftSep != rightSep)
                {
                    errorMessage = "Bracket number not equivalent!";
                    result = false;
                }
            }

            return result;
        }

        private void analyzeExpression(string text)
        {
            analyzeExpressionBracket(text);
            isSecondRound = true;
            tempPart3.AddRange(tempPart1);
            tempPart4.AddRange(tempPart2);
            tempPart2.Clear();
            int tempPartLength = tempPart3.Count;
            for (int i = 0; i < tempPartLength; i++)
            {
                tempPartIndexList.Add(i);
            }
            for (int i = 0; i < tempPartLength; i++)
            {
                string texti = tempPart3[i];
                texti = addBrackets(texti);
                analyzeExpressionBracket(texti);
                tempPartIndexList[i] = tempPart1.Count - 1;
                for (int j = tempPartIndexList.Count; j < tempPart1.Count; j++)
                {
                    tempPartIndexList.Add(j);
                }
                // tempPart4.addAll(tempPart2);
                // tempPart2.clear();
            }
            tempPart3.Clear();
            tempPart3.AddRange(tempPart1);
            tempPart4.Clear();
            tempPart4.AddRange(tempPart2);
            // int tempPartLength = tempPart4.size();
            // for (int i = 0; i < tempPartLength; i++) {
            // string texti = tempPart4.get(i);
            // texti = addBrackets(texti);
            // analyzeExpressionBracket(texti);
            // tempPart4.addAll(tempPart2);
            // tempPart2.clear();
            // }
            // tempPart3.addAll(tempPart1);
            if (tempPart2.Count == 0)
            {
                tempPart2.Add(text);
            }
            analyzeExpressionPart();
        }

        private void analyzeExpressionBracket(string text)
        {
            string seperatestringLeftPart = CalculationOperator.legalSeperators[0];
            string seperatestringRightPart = CalculationOperator.legalSeperators[1];
            int rpi = text.IndexOf(seperatestringRightPart);
            if (rpi > 0)
            {
                string text0 = text.Substring(0, rpi);
                int lpi = text0.LastIndexOf(seperatestringLeftPart);
                string text1 = text0.Substring(0, lpi);
                string text2 = text0.Substring(lpi + 1);
                string text3 = text.Substring(rpi + 1);
                tempPart1.Add(text2);
                string lastWord = getOneWordBackward(text1);
                if (text1.Equals(""))
                {

                }
                else if (CalculationOperator.isFunction(lastWord))
                {
                    if (text1.StartsWith(lastWord))
                    {
                        text1 = "";
                    }
                    else
                    {
                        text1 = text1.Split(new string[] { lastWord }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    int index = tempPart1.Count;
                    text2 = lastWord + "(" + "TEMPPART" + index + ")";
                    tempPart1.Add(text2);
                }
                text0 = text1 + "TEMPPART" + tempPart1.Count.ToString()
                        + text3;
                analyzeExpressionBracketLoop(seperatestringLeftPart,
                        seperatestringRightPart, text0);
                tempPart2.Add(text0);
            }
            else
            {

            }
        }

        private void analyzeExpressionBracketLoop(
                string seperatestringLeftPart, string seperatestringRightPart,
                string text)
        {
            int rpi = text.IndexOf(seperatestringRightPart);
            if (rpi > 0)
            {
                string text0 = text.Substring(0, rpi);
                int lpi = text0.LastIndexOf(seperatestringLeftPart);
                string text1 = text0.Substring(0, lpi);
                string text2 = text0.Substring(lpi + 1);
                string text3 = text.Substring(rpi + 1);
                tempPart1.Add(text2);
                string lastWord = getOneWordBackward(text1);
                if (text1.Equals(""))
                {

                }
                else if (CalculationOperator.isFunction(lastWord))
                {
                    if (text1.StartsWith(lastWord))
                    {
                        text1 = "";
                    }
                    else
                    {
                        text1 = text1.Split(new string[] { lastWord }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    int index = tempPart1.Count;
                    text2 = lastWord + "(" + "TEMPPART" + index + ")";
                    tempPart1.Add(text2);
                }
                text0 = text1 + "TEMPPART" + tempPart1.Count.ToString()
                        + text3;
                analyzeExpressionBracketLoop(seperatestringLeftPart,
                        seperatestringRightPart, text0);
                tempPart2.Add(text0);
            }
            else
            {

            }
        }

        private string getOneWordBackward(string originStr)
        {
            string result = "";
            if (originStr.Length == 0)
            {

            }
            else
            {
                char[] originChars = originStr.ToCharArray();
                int index = originStr.Length - 1;
                while (CalculationOperator.isVariableChar(originChars[index]))
                {
                    result = originChars[index].ToString() + result;
                    index--;
                    if (index < 0)
                        break;
                }
            }
            return result;
        }

        private void analyzeExpressionPart()
        {
            ExpressionPart epf = new ExpressionPart();
            epf = expressionMeaning;
            string currentPart = tempPart1[tempPart1.Count - 1];
            bool justFunction = false;
            if (currentPart.IndexOf("(") > 0)
            {
                string funcName = currentPart
                        .Substring(0, currentPart.IndexOf("("));
                currentPart = currentPart.Substring(currentPart.IndexOf("(") + 1);
                currentPart = currentPart.Substring(0, currentPart.Length - 1);
                epf.setOperatorstring(funcName);
                justFunction = true;
            }
            List<string> itemList = splictPartByOperator(currentPart);
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                if (currentItem.StartsWith("TEMPPART"))
                {
                    ExpressionPart epc = new ExpressionPart();
                    epf.addParameter(epc);
                    if (justFunction)
                    {
                        epc.setOperatorstring(epf.getOperatorstring());
                        epf.setOperatorstring("::");
                        justFunction = false;
                    }
                    epf = epc;
                    string tempPartIndexstring = currentItem.Split(new string[] { "TEMPPART" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    int index = Convert.ToInt32(tempPartIndexstring) - 1;
                    string tempPartstring = "";
                    if (isSecondRound)
                    {
                        tempPartstring = tempPart1[tempPartIndexList[index]];
                    }
                    else
                    {
                        tempPartstring = tempPart1[index];
                    }
                    analyzeExpressionPartLoop(tempPartstring, epf);
                    epf = epf.getFartherPart();
                }
                else if (CalculationOperator
                      .isOperator(currentItem.ToCharArray()[0]))
                {
                    epf.setOperatorstring(currentItem);
                    if (i < itemList.Count - 2)
                    {
                        ExpressionPart epc = new ExpressionPart();
                        epf.addParameter(epc);
                        epf = epc;
                    }
                }
                else if (CalculationOperator.isCenterSeperator(currentItem
                      .ToCharArray()[0]))
                {
                    epf.setOperatorstring(epf.getFartherPart().getOperatorstring());
                    if (i < itemList.Count - 2)
                    {
                        ExpressionPart epc = new ExpressionPart();
                        epf.addParameter(epc);
                        epf = epc;
                    }
                }
                else
                {
                    ExpressionPart epc = new ExpressionPart();
                    epc.setOperatorstring(":=");
                    epc.setValuestring(currentItem);
                    epf.addParameter(epc);
                }
            }
        }

        private void analyzeExpressionPartLoop(string text,
                ExpressionPart root)
        {
            ExpressionPart epf = new ExpressionPart();
            epf = root;
            string currentPart = text;
            bool justFunction = false;
            if (currentPart.IndexOf("(") > 0)
            {
                string funcName = currentPart
                        .Substring(0, currentPart.IndexOf("("));
                currentPart = currentPart.Substring(currentPart.IndexOf("(") + 1);
                currentPart = currentPart.Substring(0, currentPart.Length - 1);
                epf.setOperatorstring(funcName);
                justFunction = true;
            }
            List<string> itemList = splictPartByOperator(currentPart);
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                if (currentItem.StartsWith("TEMPPART"))
                {
                    ExpressionPart epc = new ExpressionPart();
                    epf.addParameter(epc);
                    if (justFunction)
                    {
                        epc.setOperatorstring(epf.getOperatorstring());
                        epf.setOperatorstring("::");
                        justFunction = false;
                    }
                    epf = epc;
                    string tempPartIndexstring = currentItem.Split(new string[] { "TEMPPART" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    int index = Convert.ToInt32(tempPartIndexstring) - 1;
                    string tempPartstring = "";
                    if (isSecondRound)
                    {
                        tempPartstring = tempPart1[tempPartIndexList[index]];
                    }
                    else
                    {
                        tempPartstring = tempPart1[index];
                    }
                    analyzeExpressionPartLoop(tempPartstring, epf);
                    epf = epf.getFartherPart();
                }
                else if (CalculationOperator
                      .isOperator(currentItem.ToCharArray()[0]))
                {
                    epf.setOperatorstring(currentItem);
                    if (i < itemList.Count - 2)
                    {
                        ExpressionPart epc = new ExpressionPart();
                        epf.addParameter(epc);
                        epf = epc;
                    }
                }
                else if (CalculationOperator.isCenterSeperator(currentItem
                      .ToCharArray()[0]))
                {
                    epf.setOperatorstring(epf.getFartherPart().getOperatorstring());
                    if (i < itemList.Count - 2)
                    {
                        ExpressionPart epc = new ExpressionPart();
                        epf.addParameter(epc);
                        epf = epc;
                    }
                }
                else
                {
                    ExpressionPart epc = new ExpressionPart();
                    epc.setOperatorstring(":=");
                    epc.setValuestring(currentItem);
                    epf.addParameter(epc);
                }
            }
        }

        private List<string> splictPart(string text)
        {
            text = standardize(text);
            List<string> result = new List<string>();
            result.Add(text);
            for (int i = 0; i < CalculationOperator.legalOperators.Length; i++)
            {
                List<string> temp = splictPartBystring(result,
                        CalculationOperator.legalOperators[i]);
                result.Clear();
                result.AddRange(temp);
            }
            return result;
        }

        private List<string> splictPartByOperator(string text)
        {
            List<string> result = new List<string>();
            result.Add(text);
            List<string> temp;
            for (int i = 0; i < CalculationOperator.legalOperators.Length; i++)
            {
                temp = splictPartBystring(result,
                        CalculationOperator.legalOperators[i]);
                result.Clear();
                result.AddRange(temp);
            }
            temp = splictPartBystring(result,
                    CalculationOperator.legalSeperators[2]);
            result.Clear();
            result.AddRange(temp);
            return result;
        }

        private List<string> splictPartBystring(List<string> text,
                string operatorStr)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < text.Count; i++)
            {
                string[] parts = text[i].Split(new string[] { operatorStr }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < parts.Length; j++)
                {
                    result.Add(parts[j]);
                    if (j < parts.Length - 1)
                    {
                        result.Add(operatorStr);
                    }
                }
            }
            return result;
        }

        private string addBrackets(string text)
        {
            text = standardize(text);
            // System.out.println(text);
            return addBrackets(splictPartByOperator(text));
        }

        private string addBrackets(List<string> itemList)
        {
            string result = "";
            List<int> indexList = new List<int>();
            int[] index2tempResultIndex = new int[itemList.Count];
            List<List<int>> relatedIndexList = new List<List<int>>();
            for (int i = 0; i < itemList.Count; i++)
            {
                List<int> list = new List<int>();
                relatedIndexList.Add(list);
            }
            List<int> tier1Index = getTier1Index(itemList);
            List<string> tempResultList = new List<string>();
            for (int i = 0; i < tier1Index.Count; i++)
            {
                List<int> updateIndex = new List<int>();
                int s1 = tier1Index[i] - 1;
                int s2 = tier1Index[i];
                int s3 = tier1Index[i] + 1;
                updateIndex.Add(s1);
                updateIndex.Add(s2);
                updateIndex.Add(s3);
                string part1 = itemList[s1];
                if (indexList.Contains(s1))
                {
                    part1 = tempResultList[index2tempResultIndex[s1]];
                    List<int> relatedIndex = relatedIndexList[s1];
                    updateIndex.AddRange(relatedIndex);
                }
                string part2 = itemList[s2];
                string part3 = itemList[s3];
                if (indexList.Contains(s3))
                {
                    part3 = tempResultList[index2tempResultIndex[s3]];
                    List<int> relatedIndex = relatedIndexList[s3];
                    updateIndex.AddRange(relatedIndex);
                }
                string tempResult = "(" + part1 + part2 + part3 + ")";
                tempResultList.Add(tempResult);
                indexList.Add(s1);
                indexList.Add(s2);
                indexList.Add(s3);
                for (int j = 0; j < updateIndex.Count; j++)
                {
                    relatedIndexList[updateIndex[j]] = updateIndex;
                    index2tempResultIndex[updateIndex[j]] = tempResultList.Count - 1;
                }
            }
            List<int> tier2Index = getTier2Index(itemList);
            for (int i = 0; i < tier2Index.Count; i++)
            {
                List<int> updateIndex = new List<int>();
                int s1 = tier2Index[i] - 1;
                int s2 = tier2Index[i];
                int s3 = tier2Index[i] + 1;
                updateIndex.Add(s1);
                updateIndex.Add(s2);
                updateIndex.Add(s3);
                string part1 = itemList[s1];
                if (indexList.Contains(s1))
                {
                    part1 = tempResultList[index2tempResultIndex[s1]];
                    List<int> relatedIndex = relatedIndexList[s1];
                    updateIndex.AddRange(relatedIndex);
                }
                string part2 = itemList[s2];
                string part3 = itemList[s3];
                if (indexList.Contains(s3))
                {
                    part3 = tempResultList[index2tempResultIndex[s3]];
                    List<int> relatedIndex = relatedIndexList[s3];
                    updateIndex.AddRange(relatedIndex);
                }
                string tempResult = "(" + part1 + part2 + part3 + ")";
                tempResultList.Add(tempResult);
                indexList.Add(s1);
                indexList.Add(s2);
                indexList.Add(s3);
                for (int j = 0; j < updateIndex.Count; j++)
                {
                    relatedIndexList[updateIndex[j]] = updateIndex;
                    index2tempResultIndex[updateIndex[j]] = tempResultList.Count - 1;
                }
            }
            List<int> tier3Index = getTier3Index(itemList);
            for (int i = 0; i < tier3Index.Count; i++)
            {
                List<int> updateIndex = new List<int>();
                int s1 = tier3Index[i] - 1;
                int s2 = tier3Index[i];
                int s3 = tier3Index[i] + 1;
                updateIndex.Add(s1);
                updateIndex.Add(s2);
                updateIndex.Add(s3);
                string part1 = itemList[s1];
                if (indexList.Contains(s1))
                {
                    part1 = tempResultList[index2tempResultIndex[s1]];
                    List<int> relatedIndex = relatedIndexList[s1];
                    updateIndex.AddRange(relatedIndex);
                }
                string part2 = itemList[s2];
                string part3 = itemList[s3];
                if (indexList.Contains(s3))
                {
                    part3 = tempResultList[index2tempResultIndex[s3]];
                    List<int> relatedIndex = relatedIndexList[s3];
                    updateIndex.AddRange(relatedIndex);
                }
                string tempResult = "(" + part1 + part2 + part3 + ")";
                tempResultList.Add(tempResult);
                indexList.Add(s1);
                indexList.Add(s2);
                indexList.Add(s3);
                for (int j = 0; j < updateIndex.Count; j++)
                {
                    relatedIndexList[updateIndex[j]] = updateIndex;
                    index2tempResultIndex[updateIndex[j]] = tempResultList.Count - 1;
                }
            }
            List<int> tier4Index = getTier4Index(itemList);
            for (int i = 0; i < tier4Index.Count; i++)
            {
                List<int> updateIndex = new List<int>();
                int s1 = tier4Index[i] - 1;
                int s2 = tier4Index[i];
                int s3 = tier4Index[i] + 1;
                updateIndex.Add(s1);
                updateIndex.Add(s2);
                updateIndex.Add(s3);
                string part1 = itemList[s1];
                if (indexList.Contains(s1))
                {
                    part1 = tempResultList[index2tempResultIndex[s1]];
                    List<int> relatedIndex = relatedIndexList[s1];
                    updateIndex.AddRange(relatedIndex);
                }
                string part2 = itemList[s2];
                string part3 = itemList[s3];
                if (indexList.Contains(s3))
                {
                    part3 = tempResultList[index2tempResultIndex[s3]];
                    List<int> relatedIndex = relatedIndexList[s3];
                    updateIndex.AddRange(relatedIndex);
                }
                string tempResult = "(" + part1 + part2 + part3 + ")";
                tempResultList.Add(tempResult);
                indexList.Add(s1);
                indexList.Add(s2);
                indexList.Add(s3);
                for (int j = 0; j < updateIndex.Count; j++)
                {
                    relatedIndexList[updateIndex[j]] = updateIndex;
                    index2tempResultIndex[updateIndex[j]] = tempResultList.Count - 1;
                }
            }
            if (tempResultList.Count > 0)
            {
                result = tempResultList[tempResultList.Count - 1];
                // while (result.startsWith("(") & result.endsWith(")")) {
                // result = result.substring(1, result.length() - 1);
                // }
                result = result.Substring(1, result.Length - 1);
            }
            else
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    result += itemList[i];
                }
            }
            return result;
        }

        private List<int> getTier1Index(List<string> itemList)
        {
            List<int> result = new List<int>();
            string[] targets = new string[] { CalculationOperator.legalOperators[0] };
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                for (int j = 0; j < targets.Length; j++)
                {
                    if (currentItem.Equals(targets[j]))
                    {
                        result.Add(i);
                        break;
                    }
                }
            }
            return result;
        }

        private List<int> getTier2Index(List<string> itemList)
        {
            List<int> result = new List<int>();
            string[] targets = new string[] {
				CalculationOperator.legalOperators[1],
				CalculationOperator.legalOperators[2] };
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                for (int j = 0; j < targets.Length; j++)
                {
                    if (currentItem.Equals(targets[j]))
                    {
                        result.Add(i);
                        break;
                    }
                }
            }
            return result;
        }

        private List<int> getTier3Index(List<string> itemList)
        {
            List<int> result = new List<int>();
            string[] targets = new string[] {
				CalculationOperator.legalOperators[3],
				CalculationOperator.legalOperators[4] };
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                for (int j = 0; j < targets.Length; j++)
                {
                    if (currentItem.Equals(targets[j]))
                    {
                        result.Add(i);
                        break;
                    }
                }
            }
            return result;
        }

        private List<int> getTier4Index(List<string> itemList)
        {
            List<int> result = new List<int>();
            string[] targets = new string[] { CalculationOperator.legalSeperators[2] };
            for (int i = 0; i < itemList.Count; i++)
            {
                string currentItem = itemList[i];
                for (int j = 0; j < targets.Length; j++)
                {
                    if (currentItem.Equals(targets[j]))
                    {
                        result.Add(i);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
