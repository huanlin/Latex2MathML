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

namespace Latex2MathML
{
    /// <summary>
    /// The proxy class between a command and the corresponding converter.
    /// </summary>
    internal class CommandConverter : NamedConverter
    {
        /// <summary>
        /// The hash table of command converters.
        /// </summary>
        public static readonly IDictionary<int, CommandConverter> CommandConverters = new Dictionary<int, CommandConverter>
        {
            #region Initialization
            {(new SectionConverter()).GetHashCode(), new SectionConverter()},
            {(new NumberedSectionConverter()).GetHashCode(), new NumberedSectionConverter()},
            {(new SubsectionConverter()).GetHashCode(), new SubsectionConverter()},
            {(new NumberedSubsectionConverter()).GetHashCode(), new NumberedSubsectionConverter()},
            {(new FracCommandConverter()).GetHashCode(), new FracCommandConverter()},
			{(new DfracCommandConverter()).GetHashCode(), new DfracCommandConverter()},
			{(new TfracCommandConverter()).GetHashCode(), new TfracCommandConverter()},
            {(new SqrtCommandConverter()).GetHashCode(), new SqrtCommandConverter()},            
            {(new GraphicsConverter()).GetHashCode(), new GraphicsConverter()},
            {(new ThanksConverter()).GetHashCode(), new ThanksConverter()},
            {(new IEEEAuthorAConverter()).GetHashCode(), new IEEEAuthorAConverter()},
            {(new IEEEAuthorNConverter()).GetHashCode(), new IEEEAuthorNConverter()},
            {(new ItemCommandConverter()).GetHashCode(), new ItemCommandConverter()},

            {(new OverlineAccentConverter()).GetHashCode(), new OverlineAccentConverter()},
            {(new HatAccentConverter()).GetHashCode(), new HatAccentConverter()},
            {(new WidehatAccentConverter()).GetHashCode(), new WidehatAccentConverter()},
            {(new CheckAccentConverter()).GetHashCode(), new CheckAccentConverter()},
            {(new TildeAccentConverter()).GetHashCode(), new TildeAccentConverter()},
            {(new WidetildeAccentConverter()).GetHashCode(), new WidetildeAccentConverter()},
            {(new AcuteAccentConverter()).GetHashCode(), new AcuteAccentConverter()},
            {(new GraveAccentConverter()).GetHashCode(), new GraveAccentConverter()},
            {(new DotAccentConverter()).GetHashCode(), new DotAccentConverter()},
            {(new DdotAccentConverter()).GetHashCode(), new DdotAccentConverter()},
            {(new BreveAccentConverter()).GetHashCode(), new BreveAccentConverter()},
            {(new BarAccentConverter()).GetHashCode(), new BarAccentConverter()},
            {(new VecAccentConverter()).GetHashCode(), new VecAccentConverter()},
            {(new HatTextAccentConverter()).GetHashCode(), new HatTextAccentConverter()},
            {(new CheckTextAccentConverter()).GetHashCode(), new CheckTextAccentConverter()},
            {(new TildeTextAccentConverter()).GetHashCode(), new TildeTextAccentConverter()},
            {(new AcuteTextAccentConverter()).GetHashCode(), new AcuteTextAccentConverter()},
            {(new GraveTextAccentConverter()).GetHashCode(), new GraveTextAccentConverter()},
            {(new DotTextAccentConverter()).GetHashCode(), new DotTextAccentConverter()},
            {(new DdotTextAccentConverter()).GetHashCode(), new DdotTextAccentConverter()},
            {(new BreveTextAccentConverter()).GetHashCode(), new BreveTextAccentConverter()},
            {(new BarTextAccentConverter()).GetHashCode(), new BarTextAccentConverter()},

			{(new TextSize_tiny_Converter()).GetHashCode(), new TextSize_tiny_Converter()},
			{(new TextSize_scriptsize_Converter()).GetHashCode(), new TextSize_scriptsize_Converter()},
			{(new TextSize_footnotesize_Converter()).GetHashCode(), new TextSize_footnotesize_Converter()},
			{(new TextSize_small_Converter()).GetHashCode(), new TextSize_small_Converter()},
			{(new TextSize_normalsize_Converter()).GetHashCode(), new TextSize_normalsize_Converter()},
			{(new TextSize_large_Converter()).GetHashCode(), new TextSize_large_Converter()},
			{(new TextSize_Large_Converter()).GetHashCode(), new TextSize_Large_Converter()},
			{(new TextSize_LARGE_Converter()).GetHashCode(), new TextSize_LARGE_Converter()},
			{(new TextSize_huge_Converter()).GetHashCode(), new TextSize_huge_Converter()},
			{(new TextSize_Huge_Converter()).GetHashCode(), new TextSize_Huge_Converter()},
			
			{(new TextitStyleConverter()).GetHashCode(), new TextitStyleConverter()},
			{(new TextbfStyleConverter()).GetHashCode(), new TextbfStyleConverter()},
			{(new EmphStyleConverter()).GetHashCode(), new EmphStyleConverter()},
            {(new UnderlineStyleConverter()).GetHashCode(), new UnderlineStyleConverter()},
            
            {(new LabelCommandConverter()).GetHashCode(), new LabelCommandConverter()},
            {(new RefCommandConverter()).GetHashCode(), new RefCommandConverter()},
            {(new HyperrefCommandConverter()).GetHashCode(), new HyperrefCommandConverter()},
            {(new UrlCommandConverter()).GetHashCode(), new UrlCommandConverter()},
            {(new HrefCommandConverter()).GetHashCode(), new HrefCommandConverter()},

            {(new StateCommandConverter()).GetHashCode(), new StateCommandConverter()},
            {(new StatexCommandConverter()).GetHashCode(), new StatexCommandConverter()},
            {(new ProcedureCommandConverter()).GetHashCode(), new ProcedureCommandConverter()},
            {(new EndProcedureCommandConverter()).GetHashCode(), new EndProcedureCommandConverter()},
            {(new MathcalCommandConverter()).GetHashCode(), new MathcalCommandConverter()},
            
            //{(new STATECommandConverter()).GetHashCode(), new STATECommandConverter()},
            {(new CiteCommandConverter()).GetHashCode(), new CiteCommandConverter()},
            {(new FootnoteCommandConverter()).GetHashCode(), new FootnoteCommandConverter()},            
            #endregion
        };

        public static readonly Dictionary<string, string> CommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"\\", "<br />"},
            {"footnoterule", "<br />"},
            {"alpha", "&#x3B1;<!-- &alpha; -->"},
            {"beta", "&#x3B2;<!-- &beta; -->"},
            {"gamma", "&#x3B3;<!-- &gamma; -->"},
            {"delta", "&#x3B4;<!-- &delta; -->"},
            {"epsilon", "&#x3B5;<!-- &epsilon; -->"},
            {"varepsilon", "&#x3B5;<!-- &epsilon; -->"},
            {"zeta", "&#x3B6;<!-- &zeta; -->"},
            {"eta", "&#x3B7;<!-- &eta; -->"},
            {"theta", "&#x3B8;<!-- &theta; -->"},
            {"iota", "&#x3B9;<!-- &iota; -->"},
            {"kappa", "&#x3BA;<!-- &kappa; -->"},
            {"lambda", "&#x3BB;<!-- &lambda; -->"},
            {"mu", "&#x3BC;<!-- &mu; -->"},
            {"nu", "&#x3BD;<!-- &nu; -->"},
            {"xi", "&#x3BE;<!-- &xi; -->"},
            {"omicron", "&#x3BF;<!-- &omicron; -->"},
            {"pi", "&#x3C0;<!-- &pi; -->"},
            {"rho", "&#x3C1;<!-- &rho; -->"},            
            {"sigma", "&#x3C3;<!-- &sigma; -->"},
            {"tau", "&#x3C4;<!-- &tau; -->"},
            {"upsilon", "&#x3C5;<!-- &upsilon; -->"},
            {"phi", "&#x3C6;<!-- &phi; -->"},
            {"chi", "&#x3C7;<!-- &chi; -->"},
            {"psi", "&#x3C8;<!-- &psi; -->"},
            {"omega", "&#x3C9;<!-- &omega; -->"},
            {"dag", "&#x2020;<!-- &dagger; -->"},
            {"ddag", "&#x2021;<!-- &Dagger; -->"},
            {"pounds", "&#x00A3;<!-- &pound; -->"},
            {"textsterling", "&#x00A3;<!-- &pound; -->"},
            {"euro", "&#x20AC;<!-- &euro; -->"},
            {"EUR", "&#x20AC;<!-- &euro; -->"},
            {"S", "&#x00A7;<!-- &sect; -->"},
            {"hline", "<hr />"},
            {"vert", "|"},
            {"~", "~"},
            #endregion
        };

        public static readonly Dictionary<string, string> MathFunctionsCommandConstants = new Dictionary<string, string>
        {          
            #region Initialization
            {"sin", "<mi>sin</mi>\n"},
            {"cos", "<mi>cos</mi>\n"},
            {"tan", "<mi>tan</mi>\n"},
            {"sec", "<mi>sec</mi>\n"},
            {"csc", "<mi>csc</mi>\n"},
            {"cot", "<mi>cot</mi>\n"},
            {"sinh", "<mi>sinh</mi>\n"},
            {"cosh", "<mi>cosh</mi>\n"},
            {"tanh", "<mi>tanh</mi>\n"},
            {"sech", "<mi>sech</mi>\n"},
            {"csch", "<mi>csch</mi>\n"},
            {"coth", "<mi>coth</mi>\n"},
            {"arcsin", "<mi>arcsin</mi>\n"},
            {"arccos", "<mi>arccos</mi>\n"},
            {"arctan", "<mi>arctan</mi>\n"},
            {"arccosh", "<mi>arccosh</mi>\n"},
            {"arccot", "<mi>arccot</mi>\n"},
            {"arccoth", "<mi>arccoth</mi>\n"},
            {"arccsc", "<mi>arccsc</mi>\n"},
            {"arccsch", "<mi>arccsch</mi>\n"},
            {"arcsec", "<mi>arcsec</mi>\n"},
            {"arcsech", "<mi>arcsech</mi>\n"},
            {"arcsinh", "<mi>arcsinh</mi>\n"},
            {"arctanh", "<mi>arctanh</mi>\n"},             
            #endregion
        };

        public static readonly Dictionary<string, string> MathFunctionsScriptCommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"int", "<mo>&#x222B;<!-- &int; --></mo>\n"},                                  
            {"iint", "<mo>&#x222C;<!-- iint --></mo>\n"},
            {"iiint", "<mo>&#x222D;<!-- iiint --></mo>\n"},
            {"oint", "<mo>&#x222E;<!-- oint --></mo>\n"}, 
            {"oiint", "<mo>&#x222F;<!-- oiint --></mo>\n"},
            {"oiiint", "<mo>&#x2230;<!-- oiiint --></mo>\n"},
            {"ointclockwise", "<mo>&#x2232;<!-- ointclockwise --></mo>\n"},
            {"ointctrclockwise", "<mo>&#x2233;<!-- ointctrclockwise --></mo>\n"},   

            {"lim", "<mo>lim</mo>\n"},
            {"sup", "<mo>sup</mo>\n"},
            {"inf", "<mo>inf</mo>\n"},
            {"min", "<mo>min</mo>\n"},
            {"max", "<mo>max</mo>\n"},
            {"ker", "<mo>ker</mo>\n"},
            {"sum", "<mo>&#x2211;<!-- &sum; --></mo>\n"}
            #endregion
        };

        public static readonly Dictionary<string, string> MathCommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"displaystyle", ""},
                      
            {"neq", "<mi>&#x2260;<!-- &ne; --></mi>\n"},
            {"equiv", "<mi>&#x2261;<!-- equiv --></mi>\n"},
            {"pm", "<mi>&#xB1;<!-- pm --></mi>\n"},
            {"mp", "<mi>&#x2213;<!-- mp --></mi>\n"},
            {"sim", "<mi>&#x223C;<!-- &sim; --></mi>\n"},
            {"approx", "<mi>&#x2248;<!-- &asymp; --></mi>\n"},
            {"cap", "<mi>&#x2229;<!-- &cap; --></mi>\n"},
            {"cup", "<mi>&#x2230;<!-- &cup; --></mi>\n"},                      
            {"in", "<mi>&#x2208;<!-- &isin; --></mi>\n"},
            {"notin", "<mi>&#x2209;<!-- &notin; --></mi>\n"},
            {"ni", "<mi>&#x220B;<!-- &ni; --></mi>\n"},
            {"forall", "<mi>&#x2200;<!-- &forall; --></mi>\n"},
            {"infty", "<mi>&#x221E;<!-- &infty; --></mi>\n"},
            {"exists", "<mi>&#x2203;<!-- &exist; --></mi>\n"},
            {"nexists", "<mi>&#x2204;<!-- \nexists --></mi>\n"},
            {"to", "<mo>&#x2192;<!-- &rarr; --></mo>\n"},
            {"quad", "<mspace width=\"2em\"/>"},
            {"qquad", "<mspace width=\"4em\"/>"},
            {";", "<mspace width=\"1em\"/>"},
            {":", "<mspace width=\"1em\"/>"},

            {"Leftrightarrow", "<mi>&#x21D4;<!-- &hArr; --></mi>\n"},
            {"Updownarrow", "<mi>&#x21D5;<!-- \\Updownarrow --></mi>\n"},
            {"Downarrow", "<mi>&#x21D3;<!-- &dArr; --></mi>\n"},
            {"Rightarrow", "<mi>&#x21D2;<!-- &rArr; --></mi>\n"},
            {"Uparrow", "<mi>&#x21D1;<!-- &uArr; --></mi>\n"},
            {"Leftarrow", "<mi>&#x21D0;<!-- &lArr; --></mi>\n"},
            {"nRightarrow", "<mi>&#x21CF;<!-- \\nRightarrow --></mi>\n"},
            {"nLeftrightarrow", "<mi>&#x21CE;<!-- \\nLeftrightarrow --></mi>\n"},
            {"nLeftarrow", "<mi>&#x21CD;<!-- \\nLeftarrow --></mi>\n"},
            {"rightleftharpoons", "<mi>&#x21CC;<!-- \\rightleftharpoons --></mi>\n"},
            {"leftrightharpoons", "<mi>&#x21CB;<!-- \\leftrightharpoons --></mi>\n"},
            {"downdownarrows", "<mi>&#x21CA;<!-- \\downdownarrows --></mi>\n"},
            {"rightrightarrows", "<mi>&#x21C9;<!-- \\rightrightarrows --></mi>\n"},
            {"upuparrows", "<mi>&#x21C8;<!-- \\upuparrows --></mi>\n"},
            {"leftleftarrows", "<mi>&#x21C7;<!-- \\leftleftarrows --></mi>\n"},
            {"leftrightarrows", "<mi>&#x21C6;<!-- \\leftrightarrows --></mi>\n"},
            {"updownarrows", "<mi>&#x21C5;<!-- \\updownarrows --></mi>\n"},
            {"rightleftarrows", "<mi>&#x21C4;<!-- \\rightleftarrows --></mi>\n"},
            {"downharpoonleft", "<mi>&#x21C3;<!-- \\downharpoonleft --></mi>\n"},
            {"downharpoonright", "<mi>&#x21C2;<!-- \\downharpoonright --></mi>\n"},
            {"rightharpoondown", "<mi>&#x21C1;<!-- \\rightharpoondown --></mi>\n"},
            {"rightharpoonup", "<mi>&#x21C0;<!-- \\rightharpoonup --></mi>\n"},
            {"upharpoonleft", "<mi>&#x21BF;<!-- \\upharpoonleft --></mi>\n"},
            {"upharpoonright", "<mi>&#x21BE;<!-- \\upharpoonright --></mi>\n"},
            {"leftharpoondown", "<mi>&#x21BD;<!-- \\leftharpoondown --></mi>\n"},
            {"leftharpoonup", "<mi>&#x21BC;<!-- \\leftharpoonup --></mi>\n"},
            {"circlearrowright", "<mi>&#x21BB;<!-- \\circlearrowright --></mi>\n"},
            {"circlearrowleft", "<mi>&#x21BA;<!-- \\circlearrowleft --></mi>\n"},
            {"curvearrowright", "<mi>&#x21B7;<!-- \\curvearrowright --></mi>\n"},
            {"curvearrowleft", "<mi>&#x21B6;<!-- \\curvearrowleft --></mi>\n"},
            {"Rsh", "<mi>&#x21B1;<!-- \\Rsh --></mi>\n"},
            {"Lsh", "<mi>&#x21B0;<!-- \\Lsh --></mi>\n"},
            {"looparrowright", "<mi>&#x21AC;<!-- \\looparrowright --></mi>\n"},
            {"looparrowleft", "<mi>&#x21AB;<!-- \\looparrowleft --></mi>\n"},
            {"rightarrowtail", "<mi>&#x21A3;<!-- \\rightarrowtail --></mi>\n"},
            {"leftarrowtail", "<mi>&#x21A2;<!-- \\leftarrowtail --></mi>\n"},
            {"twoheaddownarrow", "<mi>&#x21A1;<!-- \\twoheaddownarrow --></mi>\n"},
            {"twoheadrightarrow", "<mi>&#x21A0;<!-- \\twoheadrightarrow --></mi>\n"},
            {"twoheaduparrow", "<mi>&#x219F;<!-- \\twoheaduparrow --></mi>\n"},
            {"twoheadleftarrow", "<mi>&#x219E;<!-- \\twoheadleftarrow --></mi>\n"},
            {"swarrow", "<mi>&#x2199;<!-- \\swarrow --></mi>\n"},
            {"searrow", "<mi>&#x2198;<!-- \\searrow --></mi>\n"},
            {"nearrow", "<mi>&#x2197;<!-- \\nearrow --></mi>\n"},
            {"nwarrow", "<mi>&#x2196;<!-- \\nwarrow --></mi>\n"},
            {"updownarrow", "<mi>&#x2195;<!-- \\updownarrow --></mi>\n"},
            {"leftrightarrow", "<mi>&#x2194;<!-- \\leftrightarrow --></mi>\n"},
            {"downarrow", "<mi>&#x2193;<!-- &darr; --></mi>\n"},
            {"rightarrow", "<mi>&#x2192;<!-- &rarr; --></mi>\n"},
            {"uparrow", "<mi>&#x2191;<!-- &uarr; --></mi>\n"},
            {"leftarrow", "<mi>&#x2190;<!-- &larr; --></mi>\n"},

            {"varnothing", "<mi>&#x2205;<!-- &empty; --></mi>\n"},
            {"triangle", "<mi>&#x2206;<!-- \\triangle --></mi>\n"},
            {"nabla", "<mi>&#x2207;<!-- &nabla; --></mi>\n"},
            {"triangledown", "<mi>&#x2207;<!-- &nabla; --></mi>\n"},
            {"blacksquare", "<mi>&#x220E;<!-- \\blacksquare --></mi>\n"},
            {"partial", "<mi>&#x2202;<!-- &part; --></mi>\n"},
            {"dashrightarrow", "<mi>&#x21E2;<!-- \\dashrightarrow --></mi>\n"},
            {"dashleftarrow", "<mi>&#x21E0;<!-- \\dashleftarrow --></mi>\n"},
            
            {"left", ""},
            {"right", ""},
            {"[", "<mo>[</mo>\n"},
            {"]", "<mo>]</mo>\n"},
            {"{", "<mo>{</mo>\n"},
            {"}", "<mo>}</mo>\n"},
            {"|", "<mo>|</mo>\n"}
            #endregion
        };

        /// <summary>
        /// Gets an empty string. This property must be overriden by all the inheritors.
        /// </summary>
        public override string Name
        {
            get { return ""; }
        }

        /// <summary>
        /// Returns true if the block cancels a math environment; otherwise, false.
        /// </summary>
        /// <returns></returns>
        public virtual bool CancelsMathMode()
        {
            return false;
        }

        public static bool CancelsMathMode(string name)
        {
            foreach (var commandConverter in CommandConverters)
            {
                if (commandConverter.Value.Name == name)
                {
                    return commandConverter.Value.CancelsMathMode();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public virtual int ExpectedBranchesCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the type of the corresponding expression (that is, ExpressionType.Command).
        /// </summary>
        public override ExpressionType ExprType
        {
            get
            {
                return ExpressionType.Command;
            }
        }

        /// <summary>
        /// Gets the Expressions[0][0] string or an empty string if no child expressions exist.
        /// This property can be overriden by an inheritor.
        /// </summary>
        public virtual string FirstValue
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the hash code of this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ExprType.GetHashCode() ^ FirstValue.GetHashCode();
        }

        /// <summary>
        /// Converts the value of this instance to a System.String.
        /// </summary>
        /// <returns>The System.String instance.</returns>
        public override string ToString()
        {
            return string.Format("[{0}] {1} ({2})", ExprType, Name, FirstValue);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the specified System.Object is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var conv = obj as CommandConverter;
            if (conv == null)
            {
                return false;
            }
            return Name == conv.Name && ExprType == conv.ExprType && FirstValue == conv.FirstValue;
        }

        /// <summary>
        /// Searches in a predefined conversion table and returns the converted result or null.
        /// </summary>
        /// <param name="table">The conversion table to search in.</param>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The converted result or null.</returns>
        private static string SearchInTables(IDictionary<string, string> table, LatexExpression expr)
        {
            string constant;
            if (table.TryGetValue(expr.Name, out constant))
            {
                string children = "";
                if (expr.Expressions != null && expr.Options != null && expr.Options.AsExpressions != null)
                {
                    for (int i = 0; i < expr.Options.AsExpressions.Count; i++)
                    {
                        children += expr.Options.AsExpressions[i].Convert();
                    }
                }
                if (expr.Expressions != null)
                {
                    for (int i = 0; i < expr.Expressions.Count; i++)
                    {
                        for (int j = 0; j < expr.Expressions[i].Count; j++)
                        {
                            children += expr.Expressions[i][j].Convert();
                        }
                    }
                }
                return constant + children;
            }
            return null;
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            CommandConverter converter;
            if (CommandConverters.TryGetValue(expr.GetCommandConverterHashCode(), out converter))
            {
                if (converter.ExpectedBranchesCount > 0 && (expr.Expressions == null || expr.Expressions.Count < converter.ExpectedBranchesCount))
                {
                    return "<!-- Unexpected format in command \\" + converter.Name + " -->";
                }
                var result = converter.Convert(expr);
                // Make sure that {} blocks which were attached to the command by mistake will be converted, too.
                // Goddamn ancient Latex
                if (expr.Expressions != null && expr.Expressions.Count > converter.ExpectedBranchesCount)
                {
                    for (int i = converter.ExpectedBranchesCount; i < expr.Expressions.Count; i++)
                    {
                        result += SequenceConverter.ConvertOutline(expr.Expressions[i], expr.Customization);
                    }
                }
                return result;
            }
            string constant;
            if (CommandConstants.TryGetValue(expr.Name, out constant))
            {
                if (expr.MathMode)
                {
                    return "<mi>" + constant + "</mi>";
                }
                return constant;
            }
            if ((constant = SearchInTables(MathCommandConstants, expr)) != null)
            {
                return constant;
            }
            if ((constant = SearchInTables(MathFunctionsCommandConstants, expr)) != null)
            {
                return constant;
            }
            if ((constant = SearchInTables(MathFunctionsScriptCommandConstants, expr)) != null)
            {
                return constant;
            }            
            return "<!-- \\" + LatexStringToXmlString(expr.Name) + " -->\n";
        }
    }
}
