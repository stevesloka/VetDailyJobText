using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LData
{
    public class Constants
    {
        static public string AppConnectionString { get { return System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString; } }
        static public DateTime NullDate { get { return new DateTime(1900, 01, 01); } }
    }
}
