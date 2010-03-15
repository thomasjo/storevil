using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Utility;

namespace StorEvil.InPlace
{
    public class ImplementationHelper
    {
        public string Suggest(string s)
        {
            var hasTable = (s.Contains("\r\n|"));
            if (hasTable)
            {
                s = s.Until("\r\n");
            }
            var pieces = s.Split().Where(p => p.Trim() != "");

            var argTypes = new List<string>();
            var argNames = new List<string>();

            var method = pieces.First();
            int index = 1;

            foreach (var piece in pieces.Skip(1))
            {
                bool isLastPiece = (++index == pieces.Count());
                if (IsInteger(piece))
                {
                    var argName = "arg" + argTypes.Count;
                    if (!isLastPiece)
                        method += "_" + argName;
                    argTypes.Add("int");
                     argNames.Add(argName);
                }
                else if (IsQuotedString(piece))
                {
                    var argName = "arg" + argTypes.Count;
                    if (!isLastPiece)
                        method += "_" + argName;

                    argTypes.Add("string");
                    argNames.Add(argName);
                }
                else if (IsOutlineParameter(piece))
                {
                    var argName = piece.Substring(1,piece.Length-2);

                    if (!isLastPiece)
                        method += "_" + argName;
                    argTypes.Add("string");
                    argNames.Add(argName);
                }
                else
                {
                    method += "_" + piece;
                }
            }

            string argText = "";

            for (int i = 0; i < argTypes.Count; i++)
            {
                argText += ", " + argTypes[i] + " " + argNames[i];
            }

            if (hasTable)
                argText += ", string[][] tableData";

            argText += " ";
            var code = "public void " + method + "(" + argText.Substring(1).Trim() + ")\r\n{\r\n    StorEvil.ScenarioStatus.Pending(); \r\n}";

            return "// " + s + "\r\n" + code;
        }

        private bool IsOutlineParameter(string piece)
        {
            return piece.StartsWith("<") && piece.EndsWith(">");
        }

        private bool IsQuotedString(string piece)
        {
            return piece.StartsWith("\"") && piece.EndsWith("\"");
        }

        private bool IsInteger(string piece)
        {
            return piece.All(c => char.IsDigit(c));
        }
    }
}