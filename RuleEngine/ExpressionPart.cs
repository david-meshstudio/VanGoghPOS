using System;
using System.Collections.Generic;
using System.Text;

namespace COM.MeshStudio.Lib.Rule
{
    public class ExpressionPart
    {
        private List<ExpressionPart> parameterList;
        private string operatorstring;
        private string valuestring;
        private double value;
        public List<string> tempExpressionPartList;
        private List<ExpressionVariable> variableList;
        private ExpressionPart fartherPart;

        public ExpressionPart getFartherPart()
        {
            return fartherPart;
        }

        public void setFartherPart(ExpressionPart fartherPart)
        {
            this.fartherPart = fartherPart;
        }

        public List<ExpressionVariable> getVariableValue()
        {
            return variableList;
        }

        public void setVariableValue(List<ExpressionVariable> variableValue)
        {
            this.variableList = variableValue;
        }

        public string getValuestring()
        {
            return valuestring;
        }

        public void setValuestring(string valuestring)
        {
            this.valuestring = valuestring;
        }

        private double getVariableValue(string name)
        {
            double result = 0;
            for (int i = 0; i < variableList.Count; i++)
            {
                ExpressionVariable expressionVariable = variableList[i];
                if (expressionVariable.getName().Equals(name))
                {
                    result = expressionVariable.getValue();
                }
            }
            return result;
        }

        public double getValue(List<ExpressionVariable> variableList)
        {
            this.variableList.AddRange(variableList);
            double result = 0;
            if (operatorstring.Equals(":="))
            {
                try
                {
                    result = Convert.ToDouble(valuestring);
                }
                catch (Exception exp)
                {
                    result = getVariableValue(valuestring);
                }
            }
            else if (operatorstring.Equals("::"))
            {
                result = parameterList[0].getValue(variableList);
            }
            else if (operatorstring.Equals("#"))
            {
                result = parameterList[0].getValue(variableList)
                        + parameterList[1].getValue(variableList);
            }
            else if (operatorstring.Equals("-"))
            {
                result = parameterList[0].getValue(variableList)
                        - parameterList[1].getValue(variableList);
            }
            else if (operatorstring.Equals("@"))
            {
                result = parameterList[0].getValue(variableList)
                        * parameterList[1].getValue(variableList);
            }
            else if (operatorstring.Equals("/"))
            {
                if (parameterList[1].getValue(variableList) != 0)
                {
                    result = parameterList[0].getValue(variableList)
                            / parameterList[1].getValue(variableList);
                }
            }
            else if (operatorstring.Equals("&"))
            {
                result = Math.Pow(parameterList[0].getValue(variableList), parameterList[1].getValue(variableList));
            }
            else if (operatorstring.Equals("SQRT"))
            {
                result = Math.Sqrt(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("LN"))
            {
                result = Math.Log(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("SIN"))
            {
                result = Math.Sin(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("COS"))
            {
                result = Math.Cos(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("TAN"))
            {
                result = Math.Tan(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("COT"))
            {
                result = 1 / Math.Tan(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("SEC"))
            {
                result = 1 / Math.Cos(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("CSC"))
            {
                result = 1 / Math.Sin(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCSIN"))
            {
                result = Math.Asin(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCCOS"))
            {
                result = Math.Acos(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCTAN"))
            {
                result = Math.Atan(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCCOT"))
            {
                result = Math.Atan(1 / parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCSEC"))
            {
                result = Math.Acos(1 / parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ARCCSC"))
            {
                result = Math.Asin(1 / parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("ABS"))
            {
                result = Math.Abs(parameterList[0].getValue(variableList));
            }
            else if (operatorstring.Equals("SUM"))
            {
                result = 0;
                for (int i = 0; i < parameterList.Count; i++)
                {
                    result += parameterList[i].getValue(variableList);
                }
            }
            else if (operatorstring.Equals("COUNT"))
            {
                result = parameterList.Count;
            }
            else if (operatorstring.Equals("MAX"))
            {
                result = parameterList[0].getValue(variableList);
                for (int i = 1; i < parameterList.Count; i++)
                {
                    result = Math.Max(result, parameterList[i].getValue(variableList));
                }
            }
            else if (operatorstring.Equals("MIN"))
            {
                result = parameterList[0].getValue(variableList);
                for (int i = 1; i < parameterList.Count; i++)
                {
                    result = Math.Min(result, parameterList[i].getValue(variableList));
                }
            }
            value = result;
            return value;
        }

        public void setValue(double value)
        {
            this.value = value;
        }

        public List<ExpressionPart> getParameterList()
        {
            return parameterList;
        }

        public string getOperatorstring()
        {
            return operatorstring;
        }

        public void setParameterList(List<ExpressionPart> parameterList)
        {
            this.parameterList = parameterList;
        }

        public void setOperatorstring(string operatorstring)
        {
            this.operatorstring = operatorstring;
        }

        public ExpressionPart()
        {
            parameterList = new List<ExpressionPart>();
            operatorstring = "::";
            value = 0;
            tempExpressionPartList = new List<string>();
            variableList = new List<ExpressionVariable>();
        }

        public void addParameter(ExpressionPart expressionPart)
        {
            parameterList.Add(expressionPart);
            expressionPart.setFartherPart(this);
        }

        public void removeLastParameter()
        {
            parameterList.RemoveAt(parameterList.Count - 1);
        }
    }
}
