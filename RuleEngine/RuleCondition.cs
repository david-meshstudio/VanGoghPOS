// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12-2-1 ����4:09    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace COM.MeshStudio.Lib.Rule
{	
	public class RuleCondition : BasicRulePart {
		private bool conditionResult;
	
		public RuleCondition() : base() {
			this.conditionResult = false;
		}
	
		public RuleCondition(SVOClause svoClause, RuleSVOExplainer svoExplainer) {
			this.conditionResult = false;
			this.SetSvoClause(svoClause);
			this.SetSvoExplainer(svoExplainer);
		}
	
		public bool ExplainCondition(Object parameterObjectSub,
				Object[] methodParametersSub, Object parameterObjectVer,
				Object[] methodParametersVer, Object parameterObjectObj,
				Object[] methodParametersObj) {
			try {
				conditionResult = (bool)(((Boolean)base.ExplainMe(parameterObjectSub,methodParametersSub,parameterObjectVer,methodParametersVer,parameterObjectObj,methodParametersObj)));
			} catch (Exception e) {
				// TODO Auto-generated catch block
				Console.Error.WriteLine(e.StackTrace);
			}
			return conditionResult;
		}
	
		public bool IsConditionResult() {
			return conditionResult;
		}
	
		public void SetConditionResult(bool conditionResult_0) {
			this.conditionResult = conditionResult_0;
		}
	
	}
}