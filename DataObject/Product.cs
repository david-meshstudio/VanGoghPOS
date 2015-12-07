using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vangogh.DataObject
{
    public class Product : BusinessEntity
    {
        public Product()
        {
            this.ID = 0;
            this.SN = "";
            this.Name = "";
        }
    }
}
