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
	
	
	public class RuleConstructor {
		public static List<RuleItem> ConstructRuleList(
				List<SVOClause> svoClauseList) {
			List<RuleItem> result = new List<RuleItem>();
			RuleSVOExplainer svoExplainer = new RuleSVOExplainer();
			for (int i = 0; i < svoClauseList.Count; i++) {
				SVOClause svoClause = svoClauseList[i];
				String ruleName = svoClause.ruleName;
				RuleItem ruleItem = GetOrAddRuleItemFromList(result, ruleName);
				if (svoClause.rulePart.Equals(RuleEnum.RuleParts.CONDITION)) {
					ruleItem.SetRuleCondition(new RuleCondition(svoClause,
							svoExplainer));
				} else if (svoClause.rulePart.Equals(RuleEnum.RuleParts.CONSTRAINT)) {
					ruleItem.SetRuleConstraint(new RuleConstraint(svoClause,
							svoExplainer));
				} else if (svoClause.rulePart
						.Equals(RuleEnum.RuleParts.CONSEQUENCE)) {
					ruleItem.SetRuleConsequence(new RuleConsequence(svoClause,
							svoExplainer));
				}
			}
			return result;
		}
	
		private static RuleItem GetOrAddRuleItemFromList(
				List<RuleItem> ruleItemList, String ruleName) {
			for (int i = 0; i < ruleItemList.Count; i++) {
				if (ruleItemList[i].GetRuleName().Equals(ruleName)) {
					return ruleItemList[i];
				}
			}
			RuleItem newRuleItem = new RuleItem(ruleName);
			ILOG.J2CsMapping.Collections.Collections.Add(ruleItemList,newRuleItem);
			return newRuleItem;
		}
	}
}
