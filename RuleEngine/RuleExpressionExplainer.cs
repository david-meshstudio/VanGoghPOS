// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12-2-1 ����4:09    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace COM.MeshStudio.Lib.Rule
{
	public class RuleExpressionExplainer : ExpressionExplainer {
		public RuleExpressionExplainer() {
			this.variableList = new List<ExpressionVariable>();
		}
	
		private List<ExpressionVariable> variableList;
	
		public List<ExpressionVariable> GetVariableList() {
			return variableList;
		}
	
		public void SetVariableList(List<ExpressionVariable> variableList_0) {
			this.variableList = variableList_0;
		}
	
		public double GetExpressionValue() {
			return GetExpressionValue(variableList);
		}
	
		public double GetExpressionValue(List<ExpressionVariable> variableList_0) {
			double result = 0;
			result = this.GetExpressionMeaning().GetValue(variableList_0);
			return result;
		}
	
		public void SetVariable(String variableName, double variableValue) {
			variableName = variableName.ToUpper();
			for (int i = 0; i < variableList.Count; i++) {
				if (variableList[i].GetName().Equals(variableName)) {
					variableList[i].SetValue(variableValue);
					return;
				}
			}
			ILOG.J2CsMapping.Collections.Collections.Add(variableList,new ExpressionVariable(variableName, variableValue));
		}
	}
}
