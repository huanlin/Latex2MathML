/*  
    This file is part of Latex2MathML.

    Latex2MathML is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Latex2MathML is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Latex2MathML.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Latex2MathML
{
    /// <summary>
    /// Represents a single Bibtex record.
    /// </summary>
    internal class BibtexItem
    {
        /// <summary>
        /// Gets the Bibtex type of this item.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the name of this item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the cite number of this item.
        /// </summary>
        public int Number { get; private set; }

        private readonly Dictionary<string, List<LatexExpression>> _values;

        /// <summary>
        /// Gets the data associated with this item.
        /// </summary>
        public Dictionary<string, List<LatexExpression>> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Initializes a new instance of the BibtexItem class.
        /// </summary>
        /// <param name="type">the Bibtex type of the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="number">The cite number of the item.</param>
        /// <param name="values">The data associated with the item.</param>
        /// <param name="customization"></param>
        public BibtexItem(string type, string name, int number, 
            Dictionary<string, string> values, LatexToMathMLConverter customization)
        {
            Type = type;
            Name = name;
            Number = number;
            _values = new Dictionary<string, List<LatexExpression>>(values.Count);           
            foreach (var pair in values)
            {
                var root = LatexExpression.CreateRoot(new StringReader(pair.Value), customization);
                _values.Add(pair.Key, root.Expressions[0]);
            }
        }
    } 

    internal sealed class BibtexParser
    {
        private StringReader _rdr;
        private Dictionary<string, BibtexItem> _records;
        private LatexToMathMLConverter _customization;

        /// <summary>
        /// Gets the raw Bibtex records.
        /// </summary>
        public Dictionary<string, BibtexItem> Records
        {
            get
            {
                if (_records == null)
                {
                    Parse();
                }
                return _records;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BibtexParser class.
        /// </summary>
        /// <param name="rdr"></param>
        public BibtexParser(StringReader rdr, LatexToMathMLConverter customization)
        {
            _rdr = rdr;
            _customization = customization;
        }

        private void Parse()
        {
            _records = new Dictionary<string, BibtexItem>();
            string str;
            int index = 1;
            bool dummy;
            str = LatexExpression.ReadToNotEmptyString(_rdr.ReadLine(), _rdr, out dummy);
            do
            {                
                switch (str[0])
                {
                    case '@':
                        var type = Regex.Match(str.Substring(1), @"[^\s{]+").Value;
                        str = str.Substring(1 + type.Length);
                        type = type.ToLower();
                        var values = LatexExpression.ParseBraces(ref str, _rdr, "{", "}", out dummy);
                        if (type == "comment") break;
                        int pos = values.IndexOf(',');
                        var name = values.Substring(0, pos).Trim();
                        values = values.Substring(pos + 1);
                        var valuesDictionary = ParseValues(values);
                        var item = new BibtexItem(type, name, index++, valuesDictionary, _customization);
                        _records.Add(name, item);
                        break;
                    default:
                        return;
                }
                str = LatexExpression.ReadToNotEmptyString(str, _rdr, out dummy);
            }
            while (str != null);            
        }

        private static Dictionary<string, string> ParseValues(string values)
        {
			var result = new Dictionary<string, string>();
			int i = 0;
			while (i < values.Length)
			{
                for (; i < values.Length && char.IsWhiteSpace(values[i]); i++) ;
				if (i == values.Length) break;
				int begin = i;
                for (; i < values.Length && values[i] != '='; i++) ;
				if (i == values.Length) break;
				var tagName = values.Substring(begin, i - begin).TrimEnd().ToLower();
				i++;
                for (; i < values.Length && char.IsWhiteSpace(values[i]); i++) ;
				if (i == values.Length) break;
				string tagValue;
				begin = i + 1;
				switch (values[i])
				{				
				case '{':
					i++;
					for (int stack = 1; stack > 0; i++)	
					{
						switch(values[i])
						{
						case '{':
							stack++;
							break;
						case '}':
							stack--;
							break;
						default:
							continue;
						}
					}
					break;
				case '"':
					i++;
					for (;; i++)	
					{
						if (values[i] == '"' && values[i - 1] != '\\')
						{
							break;	
						}
					}
					break;
				default:
					if (char.IsDigit(values[i]))
					{
						i++;
						for (; char.IsDigit(values[i]); i++);
					}
					break;
				} 			
				tagValue = values.Substring(begin, i++ - begin - 1);
				result.Add(tagName, tagValue);
			}
			return result;
        }         
    }

    /// <summary>
    /// The converter class for \cite.
    /// </summary>
    internal sealed class CiteCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "cite";
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            var keys = expr.Expressions[0][0].Name.Split(',');
            BibtexItem item;
            var buffer = "[";
            foreach (var key in keys)
            {
                if (expr.Customization.Bibliography.TryGetValue(key.Trim(), out item))
                {
                    buffer += string.Format(expr.Customization.CiteFormat, "bib" + item.Number, item.Number);
                    buffer += ", ";
                }
            }
            if (buffer[buffer.Length - 1] == ' ')
            {
                buffer = buffer.Substring(0, buffer.Length - 2);
            }
            buffer += "]";
            return buffer;
        }
    }
}
