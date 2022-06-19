using System.IO;
using System;
using PascalABCCompiler.Errors;

namespace PasCompiler
{
    public class Helper {
        public static string CreateTempPas(string code) {
            var myfilename = Path.GetTempFileName();
            myfilename = Path.ChangeExtension(myfilename, "pas");
            StreamWriter sw = new StreamWriter(myfilename);
            sw.Write(code);
            return myfilename;
        }
        public static string EnhanceErrorMsg(Object err0)
        {
            var err = err0 as LocatedError;
            var msg = err.ToString();
            string res = "";
            var ind1 = msg.IndexOf('(');
            var ind2 = msg.IndexOf(')');
            var pos = "";
            if (ind1 > -1 && ind2 > -1) {
                pos = msg.Substring(ind1, ind2 + 1 - ind1);
            }
            if (ind2 > -1 && ind2 < msg.Length) {
                ind2 = msg.IndexOf(':', ind2);
                if (ind2 > -1 && ind2 < msg.Length - 1)
                    ind2 = msg.IndexOf(':', ind2 + 1);
                res = msg.Substring(ind2 + 1, msg.Length - ind2-1).Trim(' ');
                if (pos != "")
                    res = pos + ": " + res;
            }
            if (err0 is SemanticError)
            {
                SemanticError semErr = err0 as SemanticError;
                pos = "(" + semErr.Location.begin_line_num + "," + semErr.Location.begin_column_num + ")";
                res = pos + ": " + msg;
            }
            return res;
        }
    }
}
