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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for the document object tree root.
    /// </summary>
    internal sealed class RootConverter : BaseConverter
    {
#pragma warning disable 169
        private static readonly log4net.ILog Log =
#pragma warning restore 169
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets the type of the corresponding expression (ExpressionType.Root).
        /// </summary>
        public override ExpressionType ExprType
        {
            get { return ExpressionType.Root; }
        }

        private Dictionary<string, LatexExpression> _documentInfo;

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var bld = new StringBuilder();
            bld.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
            bld.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1 plus MathML 2.0//EN\" ");
            bld.Append("\"http://www.w3.org/Math/DTD/mathml2/xhtml-math11-f.dtd\">\n");
            bld.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xml:lang=\"");
            bld.Append(expr.Customization.Localization);
            bld.Append("\">\n");

            ReadDocumentInfo(expr.Expressions[0]);
            WriteHead(expr, bld);
            bld.Append("<body>\n");
            MakeTitle(bld);

            var bodyBuilder = expr.Customization.PlacementOfTableOfContents == TableOfContentsPlacement.Top 
                ? new StringBuilder() : bld;
            // Convert the {document} block
            LatexExpression documentExpression = expr.FindDocument();
            if (documentExpression != null)
            {
                try
                {
                    bodyBuilder.Append(documentExpression.Convert());
                }
                // ReSharper disable RedundantCatchClause
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
#if DEBUG
                    throw;
#else
                Log.Error("Failed to convert the document block", e);
#endif
                }
                // ReSharper restore RedundantCatchClause
            }
            if (expr.Customization.PlacementOfTableOfContents != TableOfContentsPlacement.None)
            {
                MakeTableOfContents(bld, expr.Customization.SectionContents, expr.Customization.TableOfContentsTitle,
                    expr.Customization.TocSectionFormat, expr.Customization.TocSubsectionFormat);
            }

            if (expr.Customization.PlacementOfTableOfContents != TableOfContentsPlacement.Bottom)
            {
                bld.Append(bodyBuilder.ToString());
            }
            if (expr.Customization.Bibliography != null && expr.Customization.Bibliography.Count > 0)
            {
                MakeBibliography(bld, expr.Customization.BibliographyTitle, 
                    expr.Customization.BibliographyRecordFormat, expr.Customization.Bibliography);
            }
            bld.Append("</body>\n</html>");
            return bld.ToString();
        }

        /// <summary>
        /// Writes the attached bibliography.
        /// </summary>
        /// <param name="bld">The StringBuilder instance to write to.</param>
        /// <param name="title">The title of the block.</param>
        /// <param name="recordFormat">The format of each bibliography record.</param>
        /// <param name="data">The bibliography data.</param>
        private static void MakeBibliography(StringBuilder bld, string title, string recordFormat, Dictionary<string, BibtexItem> data)
        {
            bld.Append("<h2 class=\"bib_header\">" + title + "</h2>\n");
            bld.Append("<div class=\"bibliography\">\n");
            foreach (var pair in data)
            {
                var item = pair.Value;
                string author = "";
                List<LatexExpression> buffer;
                if (item.Values.TryGetValue("author", out buffer))
                {
                    author = SequenceConverter.ConvertOutline(buffer, buffer[0].Customization);
                }
                string rtitle = "";
                if (item.Values.TryGetValue("title", out buffer))
                {
                    rtitle = SequenceConverter.ConvertOutline(buffer, buffer[0].Customization);
                }
                string extra = "";
                foreach (var field in item.Values)
                {
                    if (field.Key != "title" && field.Key != "author")
                    {
                        extra += SequenceConverter.ConvertOutline(field.Value, field.Value[0].Customization);
                        extra += ", ";
                    }
                }
                if (extra[extra.Length - 1] == ' ')
                {
                    extra = extra.Substring(0, extra.Length - 2);
                }
                bld.Append(String.Format(recordFormat, "bib" + item.Number, item.Number, author, rtitle, extra));
                bld.Append("<br />\n");
            }
            bld.Append("</div><br />");
        }

        /// <summary>
        /// Writes the table of contents.
        /// </summary>
        /// <param name="bld">The StringBuilder instance to write to.</param>
        /// <param name="sectionContents">The TOC data.</param>
        /// <param name="tableOfContentsTitle">The TOC title.</param>
        /// <param name="tocSectionFormat">The TOC section format string for StringFormat.</param>
        /// <param name="tocSubsectionFormat">The TOC subsection format string for StringFormat.</param>
        private static void MakeTableOfContents(StringBuilder bld, IDictionary<SectionType, 
            List<SectionContentsValue>> sectionContents, string tableOfContentsTitle,
            string tocSectionFormat, string tocSubsectionFormat)
        {
            bld.Append("<h2 class=\"toc_header\">" + tableOfContentsTitle + "</h2>\n");
            bld.Append("<div class=\"toc\">\n");
            var sects = sectionContents[SectionType.Numbered];
            for (int i = 0; i < sects.Count; i++)
            {
                var res = String.Format(tocSectionFormat, i + 1, "ns" + i, sects[i].Title);
                bld.Append(res + "<br />\n");
                for (int j = 0; j < sects[i].Subsections[SectionType.Numbered].Count; j++)
                {
                    res = String.Format(tocSubsectionFormat, i + 1, j + 1,
                        "ns" + i + "nss" + j,
                        sects[i].Subsections[SectionType.Numbered][j]);
                    bld.Append(res + "<br />\n");
                }
                for (int j = 0; j < sects[i].Subsections[SectionType.Unnumbered].Count; j++)
                {
                    res = String.Format(tocSubsectionFormat, i + 1, j + 1,
                        "ns" + i + "nnss" + j,
                        sects[i].Subsections[SectionType.Unnumbered][j]);
                    bld.Append(res + "<br />\n");
                }
            }
            sects = sectionContents[SectionType.Unnumbered];
            for (int i = 0; i < sects.Count; i++)
            {
                var res = String.Format(tocSectionFormat, i + 1, "nns" + i, sects[i].Title);
                bld.Append(res + "<br />\n");
                for (int j = 0; j < sects[i].Subsections[SectionType.Numbered].Count; j++)
                {
                    res = String.Format(tocSubsectionFormat, i + 1, j + 1,
                        "nns" + i + "nss" + j,
                        sects[i].Subsections[SectionType.Numbered][j]);
                    bld.Append(res + "<br />\n");
                }
                for (int j = 0; j < sects[i].Subsections[SectionType.Unnumbered].Count; j++)
                {
                    res = String.Format(tocSubsectionFormat, i + 1, j + 1,
                        "nns" + i + "nnss" + j,
                        sects[i].Subsections[SectionType.Unnumbered][j]);
                    bld.Append(res + "<br />\n");
                }
            }
            bld.Append("</div><br />\n");
        } 

        /// <summary>
        /// Retrieves the necessary document information.
        /// </summary>
        /// <param name="head">The expression sequence before the {document} block.</param>
        private void ReadDocumentInfo(IEnumerable<LatexExpression> head)
        {
            if (head == null)
            {
                throw new ArgumentNullException("head");
            }            
            _documentInfo = new Dictionary<string, LatexExpression>();
            foreach (var expr in head)
            {
                if (expr.ExprType == ExpressionType.Command)
                {
                    foreach (var name in LatexParser.InfoNames)
                    {
                        if (expr.Name == name)
                        {                            
                            _documentInfo.Add(name, expr);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the &lt;head&gt; tag.
        /// </summary>
        /// <param name="expr">The expression which is being converted.</param>
        /// <param name="bld">The output StringBuilder instance.</param>
        private void WriteHead(LatexExpression expr, StringBuilder bld)
        {
            bld.Append("<head>\n");
            var path = Path.GetDirectoryName(expr.Customization.OutputPath);
            if (!String.IsNullOrEmpty(path))
            {
                bld.Append("<base href=\"file:///" + expr.Customization.OutputPath.Replace('\\', '/') + "\" />\n");
            }
            bld.Append("<title>" + (_documentInfo.ContainsKey("title") ?
                AsPlainText(_documentInfo["title"]) : expr.Customization.Localization == "ru"? "Без названия" : "Untitled") + "</title>\n");
            bld.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /> \n");
            bld.Append("<meta name=\"description\" content=\"Latex document converted to XHTML 1.1 + MathML 2.0\" />\n");
            LatexExpression authorExpr;
            if (_documentInfo.TryGetValue("author", out authorExpr))
            {
                bld.Append("<meta name=\"author\" content=\"" + AsPlainText(authorExpr) + "\" />\n");
            }
            bld.Append("<meta name=\"generator\" content=\"Latex2MathML, © Markovtsev Vadim, 2010\" />\n");
            bld.Append("<meta name=\"date\" content=\"" + DateTime.Now + "\" />\n");
            bld.Append("<meta name=\"src\" content=\"" + Path.GetFileName(expr.Customization.SourcePath) + "\" />\n");
            bld.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />\n");
            bld.Append("<link rel=\"icon\" type=\"image/png\" href=\"favicon.png\" />");
            bld.Append("</head>\n");
        }

        /// <summary>
        /// Tries to display the expression as the plain text chunk.
        /// </summary>
        /// <param name="expr">The expression to display.</param>
        /// <returns>The string representing the plain text chunk.</returns>
        private static string AsPlainText(LatexExpression expr)
        {
            if (expr.Expressions != null)
            {
                string raw = SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization);
                raw += '<';
                var match = System.Text.RegularExpressions.Regex.Match(raw, @"(?<=(<[^<>]*>\s*)*)[^<>]+(?=\s*<)");
                return match.Value;
            }
            return expr.Customization.Localization == "ru" ? "Без названия" : "Untitled";
        }

        /// <summary>
        /// Writes the replacement of \maketitle.
        /// </summary>
        /// <param name="bld">The output StringBuilder instance.</param>
        private void MakeTitle(StringBuilder bld)
        {
            LatexExpression expr;
            if (_documentInfo.TryGetValue("title", out expr))
            {
                bld.Append("<h1 class=\"title\">" + SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization) + "</h1>\n");
            }
            if (_documentInfo.TryGetValue("author", out expr))
            {
                bld.Append("<h2 class=\"author\">" + SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization) + "</h2>\n");
            }
            if (_documentInfo.TryGetValue("date", out expr))
            {
                bld.Append("<h2 class=\"date\">" + SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization) + "</h2>\n");
            }
        }
    }
}
