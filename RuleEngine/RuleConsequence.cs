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
	public class RuleConsequence : BasicRulePart {
		private Object consequenceResult;
	
		public RuleConsequence() : base() {
		}
	
		public RuleConsequence(SVOClause svoClause, RuleSVOExplainer svoExplainer) {
			this.SetSvoClause(svoClause);
			this.SetSvoExplainer(svoExplainer);
		}
	
		public Object ExplainConsequence(Object parameterObjectSub,
				Object[] methodParametersSub, Object parameterObjectVer,
				Object[] methodParametersVer, Object parameterObjectObj,
				Object[] methodParametersObj) {
			consequenceResult = base.ExplainMe(parameterObjectSub,methodParametersSub,parameterObjectVer,methodParametersVer,parameterObjectObj,methodParametersObj);
			return consequenceResult;
		}
	}
}