using System;
using System.Collections.Generic;
using System.Text;

namespace COM.MeshStudio.Lib.Rule
{
    public class CalculationOperator
    {
        public static char[] legalChars = new char[] { '0', '1', '2', '3', '4',
			'5', '6', '7', '8', '9', '.', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
			'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z', '#', '-', '@', '/', '(', ')', '&',
			'=', '>', '<', '[', ']', ',', '.', '?' };
        public static char[] variableChars = new char[] { '0', '1', '2', '3', '4',
			'5', '6', '7', '8', '9', '.', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
			'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z' };
        public static string[] legalSeperators = new string[] { "(", ")", "," };
        public static string[] legalOperators = new string[] { "&", "@", "/", "#",
			"-" };
        public static string[] legalCompares = new string[] { "=", ">", "<", ">=",
			"<=", "[", "]" };
        public static string[] legalFunctions = new string[] { "SQRT", "LN", "SUM",
			"COUNT", "AVERAGE", "SIN", "COS", "TAN", "COT", "SEC", "CSC",
			"ARCSIN", "ARCCOS", "ARCTAN", "ARCCOT", "ARCSEC", "ARCCSC", "ABS",
			"MAX", "MIN", "SEPERATE" };
        public static string[] legalConstants = new string[] { "CONSTANTPI",
			"CONSTANTE", "CONSTANTI" };
        public static string[] legalInstead = new string[] { "?" };

        public static List<char> getLegalCharList()
        {
            List<char> result = new List<char>();
            for (int i = 0; i < legalChars.Length; i++)
            {
                result.Add(legalChars[i]);
            }
            return result;
        }

        public static bool isInstead(string str)
        {
            bool result = false;
            for (int i = 0; i < legalInstead.Length; i++)
            {
                if (legalInstead[i].Equals(str))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static bool isConstant(string str)
        {
            bool result = false;
            for (int i = 0; i < legalConstants.Length; i++)
            {
                if (legalConstants[i].Equals(str))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static bool isCenterSeperator(char str)
        {
            bool result = false;
            if (legalSeperators[2].Equals(str.ToString()))
            {
                result = true;
            }
            return result;
        }

        public static bool isOperator(char str)
        {
            bool result = false;
            for (int i = 0; i < legalOperators.Length; i++)
            {
                if (legalOperators[i].Equals(str.ToString()))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static bool isFunction(string str)
        {
            bool result = false;
            for (int i = 0; i < legalFunctions.Length; i++)
            {
                if (legalFunctions[i].Equals(str))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static bool isVariableChar(char str)
        {
            bool result = false;
            for (int i = 0; i < variableChars.Length; i++)
            {
                if (variableChars[i] == str)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
