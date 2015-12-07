using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class JsonTool
    {
        public static string JSON_Encode_Object(List<object> map)
        {
            string result = "";
            List<object> dictMap = new List<object>();
            foreach(var item in map)
            {
                Type type = item.GetType();
                if(type.IsArray || type.Name.StartsWith("List"))
                {
                    dictMap.Add(changeObject(item));
                }
                else if(type.Name.StartsWith("Dictionary"))
                {

                }
                else if (type.IsPrimitive)
                {
                    dictMap.Add(item);
                }
                else
                {
                    dictMap.Add(changeObject(item));
                }
            }
            result = JSON_Encode(dictMap);
            return result;
        }

        private static object changeObject(object item)
        {
            object result = new Dictionary<string, object>();
            Type type = item.GetType();
            FieldInfo[] pArray = type.GetFields();
            foreach(FieldInfo pi in pArray)
            {
                var pv = pi.GetValue(item);
                Type ptype = pv.GetType();
                if (ptype.IsArray || ptype.Name.StartsWith("List"))
                {
                    List<object> pvList = new List<object>();
                    foreach(object pvi in (List<object>)pv)
                    {
                        pvList.Add(changeObject(pvi));
                    }
                    ((Dictionary<string, object>)result).Add(pi.Name, pvList);
                }
                else if (ptype.Name.StartsWith("Dictionary"))
                {

                }
                else if (ptype.IsPrimitive || ptype.Name.StartsWith("String"))
                {
                    ((Dictionary<string, object>)result).Add(pi.Name, pv);
                }
                else
                {
                    ((Dictionary<string, object>)result).Add(pi.Name, changeObject(pv));
                }

            }
            return result;
        }

        public static string JSON_Encode(List<object> map)
        {
            string result = "[";
            for (int i = 0; i < map.Count; i++)
            {
                var mapItem = map[i];
                if (isList(mapItem))
                {
                    result += JSON_Encode_List(mapItem) + ",";
                }
                else if (isDictionary(mapItem))
                {
                    result += JSON_Encode_Dictionary(mapItem) + ",";
                }
                else
                {
                    result += "\"" + mapItem.ToString() + "\",";
                }
            }
            result = result.Substring(0, result.Length - 1) + "]";
            return result;
        }

        private static string JSON_Encode_List(object list)
        {
            string result = "[";
            for (int i = 0; i < ((List<object>)list).Count; i++)
            {
                var item = ((List<object>)list)[i];
                if (isList(item))
                {
                    result += JSON_Encode_List(item) + ",";
                }
                else if (isDictionary(item))
                {
                    result += JSON_Encode_Dictionary(item) + ",";
                }
                else
                {
                    result += "\"" + item.ToString() + "\",";
                }
            }
            result = result.Substring(0, result.Length - 1) + "]";
            return result;
        }

        private static string JSON_Encode_Dictionary(object dic)
        {
            string result = "{";
            foreach(KeyValuePair<string,object> kv in (Dictionary<string,object>)dic)
            {
                if (isList(kv.Value))
                {
                    result += "\""+kv.Key + "\":" + JSON_Encode_List(kv.Value) + ",";
                }
                else if (isDictionary(kv.Value))
                {
                    result += "\"" + kv.Key + "\":" + JSON_Encode_Dictionary(kv.Value) + ",";
                }
                else
                {
                    result += "\"" + kv.Key + "\":\"" + kv.Value.ToString() + "\",";
                }
            }
            result = result.Substring(0, result.Length - 1) + "}";
            return result;
        }

        public static List<object> JSON_Decode_Object(string jsonString, List<object> referObjectList)
        {
            List<object> result = new List<object>();
            List<object> dictMap = JSON_Decode(jsonString);
            foreach(var item in dictMap)
            {
                Type type = item.GetType();
                if (type.IsArray || type.Name.StartsWith("List"))
                {
                    result.Add(changeObjectReverseList((List<object>)item, referObjectList));
                }
                else if (type.Name.StartsWith("Dictionary"))
                {
                    result.Add(changeObjectReverseDictionary((Dictionary<string, object>)item, referObjectList));
                }
                else if (type.IsPrimitive || type.Name.StartsWith("String"))
                {
                    result.Add(item);
                }
                else
                {

                }
            }
            return result;
        }

        private static List<object> changeObjectReverseList(List<object> itemList, List<object> referObjectList)
        {
            List<object> result = new List<object>();
            foreach (var item in itemList)
            {
                Type itype = item.GetType();
                if (itype.IsArray || itype.Name.StartsWith("List"))
                {
                    result.Add(changeObjectReverseList((List<object>)item, referObjectList));
                }
                else if (itype.Name.StartsWith("Dictionary"))
                {
                    result.Add(changeObjectReverseDictionary((Dictionary<string, object>)item, referObjectList));
                }
                else if (itype.IsPrimitive || itype.Name.StartsWith("String"))
                {
                    result.Add(item);
                }
                else
                {

                }
            }
            return result;
        }

        private static object changeObjectReverseDictionary(Dictionary<string, object> itemDict, List<object> referObjectList)
        {
            var referObject = findReferObject(referObjectList, itemDict);
            bool isDict = referObject == null;
            var result = isDict ? new Dictionary<string, object>() : Activator.CreateInstance(referObject.GetType());
            foreach (KeyValuePair<string, object> kv in itemDict)
            {
                var vObject = kv.Value;
                Type vtype = vObject.GetType();
                object vResult = null;
                if (vtype.IsArray || vtype.Name.StartsWith("List"))
                {
                    vResult = changeObjectReverseList((List<object>)vObject, referObjectList);
                }
                else if (vtype.Name.StartsWith("Dictionary"))
                {
                    vResult = changeObjectReverseDictionary((Dictionary<string, object>)vObject, referObjectList);
                }
                else if (vtype.IsPrimitive || vtype.Name.StartsWith("String"))
                {
                    vResult = vObject;
                }
                else
                {

                }
                if (isDict)
                {
                    ((Dictionary<string, object>)result).Add(kv.Key, vResult);
                }
                else
                {
                    FieldInfo fi = referObject.GetType().GetField(kv.Key);
                    fi.SetValue(result, Convert.ChangeType(vResult, fi.FieldType));
                }
            }
            return result;
        }

        private static object findReferObject(List<object> referObjectList, Dictionary<string, object> itemDict)
        {
            object result = null;
            List<string> keyList = itemDict.Keys.ToList<string>();
            foreach(var item in referObjectList)
            {
                Type itype = item.GetType();
                FieldInfo[] pList = itype.GetFields();
                bool find = true;
                foreach(FieldInfo pi in pList)
                {
                    if(!keyList.Contains<string>(pi.Name))
                    {
                        find = false;
                        break;
                    }
                }
                if(find)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        public static List<object> JSON_Decode(string jsonString)
        {
            List<object> result = new List<object>();
            int type = jsonString.StartsWith("[") ? 0 : 1;
            if (type == 0 && !containSubStruct(jsonString))
            {
                result = getBasicList(new Dictionary<string, object>(), jsonString);
            }
            else
            {
                List<Dictionary<string, object>> dataList = JSON_Decode_Dic(jsonString, type);

                foreach (object data in dataList)
                {
                    result.Add(data);
                }
            }
            return result;
        }

        public static List<Dictionary<string, object>> JSON_Decode_Dic(string jsonString, int type)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            Dictionary<string, object> backDic = new Dictionary<string, object>();
            jsonString = jsonString.Substring(1, jsonString.Length - 2);
            if (!containSubStruct(jsonString))
            {
                if (jsonString.IndexOf(":") == -1)
                {
                    result = null;
                }
                else
                {
                    var value = getBasicDictionary(backDic, jsonString);
                    result.Add(value);
                }
            }
            else
            {
                List<string> jsonStringList = new List<string>();
                Loop_JSON_Decode(jsonStringList, backDic, jsonString, type);
                if (type == 0)
                {
                    foreach(KeyValuePair<string, object> kv in backDic)
                    {
                        result.Insert(0, (Dictionary<string, object>)kv.Value);
                    }
                }
                else if (type == 1)
                {
                    string rootString = jsonStringList[jsonStringList.Count - 1];
                    result.Add(getBasicDictionary(backDic, rootString));
                }
            }
            return result;
        }

        private static void Loop_JSON_Decode(List<string> jsonStringList, Dictionary<string, object> backDic, string jsonString, int type)
        {
            List<string> JSONparts = getJSONparts(jsonString);
            for (int i = 0; i < JSONparts.Count; i++)
            {
                string part = JSONparts[i];
                string key = Guid.NewGuid().ToString() + "-" + DateTime.Now.Ticks.ToString();
                jsonString = jsonString.Replace(part, key);
                for (int j = 0; j < JSONparts.Count; j++)
                {
                    if (i == j) continue;
                    JSONparts[j] = JSONparts[j].Replace(part, key);
                }
                jsonStringList.Add(jsonString);
                if (part.IndexOf(":") == -1)
                {
                    var value = getBasicList(backDic, part);
                    backDic.Add(key, value);
                }
                else
                {
                    var value = getBasicDictionary(backDic, part);
                    backDic.Add(key, value);
                }
            }
        }

        private static void mergeDictionary(Dictionary<string, object> dic1, Dictionary<string, object> dic2)
        {
            foreach (string key in dic2.Keys)
            {
                if (!dic1.ContainsKey(key))
                {
                    dic1.Add(key, dic2[key]);
                }
            }
        }

        private static List<object> getBasicList(Dictionary<string, object> backDic, string jsonString)
        {
            List<object> result = new List<object>();
            jsonString = jsonString.Replace("[", "").Replace("]", "").Replace("\"", "");
            string[] partArr = jsonString.Split(new char[] { ',' });
            foreach (string part in partArr)
            {
                if (backDic.ContainsKey(part))
                {
                    result.Add(backDic[part]);
                }
                else
                {
                    result.Add(part);
                }
            }
            return result;
        }

        private static Dictionary<string, object> getBasicDictionary(Dictionary<string, object> backDic, string jsonString)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            jsonString = jsonString.Replace("{", "").Replace("}", "").Replace("\"", "");
            string[] partArr = jsonString.Split(new char[] { ',' });
            foreach (string part in partArr)
            {
                string[] currItemArr = part.Split(new char[] { ':' });
                string itemKey = currItemArr[0];
                //string a = StaticString.TablenameDictionary[itemKey].ToString();
                string itemValue = part.Replace(itemKey + ":", "");//currItemArr[1];

                if (backDic.ContainsKey(itemValue))
                {
                    result.Add(itemKey, backDic[itemValue]);
                }
                else
                {
                    result.Add(itemKey, itemValue == "null" ? "" : itemValue);
                }
            }
            return result;
        }

        private static List<string> getJSONparts(string jsonString)
        {
            List<string> result = new List<string>();
            List<int> leftBig = getStringIndex(jsonString, "{");
            List<int> leftMid = getStringIndex(jsonString, "[");
            List<int> rightBig = getStringIndex(jsonString, "}");
            List<int> rightMid = getStringIndex(jsonString, "]");
            if (leftBig.Count != rightBig.Count || leftMid.Count != rightMid.Count)
            {
                result.Add(jsonString);
            }
            else
            {
                List<int[]> leftBreakList = getBreakList(leftBig, leftMid);
                leftBreakList.Reverse();
                List<int[]> rightBreakList = getBreakList(rightBig, rightMid);
                for (int i = 0; i < leftBreakList.Count; i++)
                {
                    int start = leftBreakList[i][0];
                    int type = leftBreakList[i][1];
                    for (int j = 0; j < rightBreakList.Count; j++)
                    {
                        int end = rightBreakList[j][0];
                        if (end > start && rightBreakList[j][1] == type)
                        {
                            string part = getSubString(jsonString, start, end);
                            if (!containSameStruct(part, type))
                            {
                                if (type == 0)
                                {
                                    part = "{" + part + "}";
                                }
                                else if (type == 1)
                                {
                                    part = "[" + part + "]";
                                }
                                result.Add(part);
                            }
                        }
                    }
                }
            }
            result = arrangePartList(result);
            return result;
        }

        private static List<string> arrangePartList(List<string> partList)
        {
            List<string> result = new List<string>();
            List<string> lastLayerList = getLastLayerList(ref partList);
            result.AddRange(lastLayerList);
            loopFindMotherLayerList(ref partList, lastLayerList, ref result);
            return result;
        }

        private static void loopFindMotherLayerList(ref List<string> partList, List<string> sonList, ref List<string> result)
        {
            result.AddRange(getMotherLayerList(ref partList, sonList));
            if (partList.Count > 0)
            {
                loopFindMotherLayerList(ref partList, sonList, ref result);
            }
        }

        private static List<string> getLastLayerList(ref List<string> partList)
        {
            List<string> result = new List<string>();
            List<string> partListBack = new List<string>();
            foreach (string part in partList)
            {
                if (!containSubStruct(part))
                {
                    result.Add(part);
                }
                else
                {
                    partListBack.Add(part);
                }
            }
            partList = partListBack;
            return result;
        }

        private static List<string> getMotherLayerList(ref List<string> partList,List<string> sonList)
        {
            List<string> result = new List<string>();
            List<string> partListRemove = new List<string>();
            foreach (string part in partList)
            {
                foreach (string son in sonList)
                {
                    if (part.IndexOf(son) > 0 && !containSubStruct(part.Replace(son, "son")))
                    {
                        result.Add(part);
                        partListRemove.Add(part);
                    }
                }
            }
            foreach (string part in partListRemove)
            {
                partList.Remove(part);
            }
            return result;
        }

        private static bool containSubStruct(string part)
        {
            part = part.Substring(1, part.Length - 1);
            return part.IndexOf("{") >= 0 && part.IndexOf("}") > 0 || part.IndexOf("[") >= 0 && part.IndexOf("]") > 0;
        }

        private static bool containSameStruct(string part, int type)
        {
            bool result = true;
            if (type == 0)
            {
                result = part.IndexOf("{") > 0;
            }
            else
            {
                result = part.IndexOf("[") > 0;
            }
            return result;
        }

        private static bool isList(object obj)
        {
            return obj.GetType().Name.StartsWith("List");
        }

        private static bool isDictionary(object obj)
        {
            return obj.GetType().Name.StartsWith("Dictionary");
        }

        private static string getSubString(string origin, int start, int end)
        {
            return origin.Substring(start + 1, (end - start) - 1);
        }

        private static List<int[]> getBreakList(List<int> bigList, List<int> midList)
        {
            List<int[]> result = new List<int[]>();
            if (bigList.Count == 0)
            {
                foreach (int mid in midList)
                {
                    result.Add(new int[] { mid, 1 });
                }
            }
            else if (midList.Count == 0)
            {
                foreach (int big in bigList)
                {
                    result.Add(new int[] { big, 0 });
                }
            }
            else
            {
                int[] start = new int[] { bigList[0] < midList[0] ? bigList[0] : midList[0], bigList[0] < midList[0] ? 0 : 1 };
                result.Add(start);
                int bigIndex = bigList[0] < midList[0] ? 1 : 0;
                int midIndex = bigList[0] < midList[0] ? 0 : 1;
                int maxSite = Math.Max(bigList[bigList.Count - 1], midList[midList.Count - 1]);
                for (int i = start[0]; i <= maxSite; i++)
                {
                    if (i == bigList[bigIndex > bigList.Count - 1 ? bigList.Count - 1 : bigIndex])
                    {
                        int[] curr = new int[] { i, 0 };
                        bigIndex++;
                        result.Add(curr);
                    }
                    else if (i == midList[midIndex > midList.Count - 1 ? midList.Count - 1 : midIndex])
                    {
                        int[] curr = new int[] { i, 1 };
                        midIndex++;
                        result.Add(curr);
                    }
                }
            }
            return result;
        }

        private static List<int> getStringIndex(string jsonString, string str)
        {
            List<int> result = new List<int>();
            int start = -1;
            do
            {
                int currIndex = jsonString.IndexOf(str, start + 1);
                if (currIndex >= 0)
                {
                    result.Add(currIndex);
                    start = currIndex;
                }
                else
                {
                    break;
                }
            } while (true);
            return result;
        }
    }
}
