using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vangogh.DataObject
{
    public abstract class BusinessEntity : BusinessEntityInterface
    {
        public int ID;
        public string SN, Name;

        public Dictionary<string,string> GetControlParameter()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Text", Name);
            result.Add("MessageName", Name + "," + ID);
            return result;
        }
    }
}
