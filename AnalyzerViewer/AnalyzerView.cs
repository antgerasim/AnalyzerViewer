using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace AnalyzerViewer
{

    public abstract class AnalyzerView
    {

        public abstract string Name { get; }

        //public virtual string GetView(TokenStream tokenStream, out int numberOfTokens)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    Token token = tokenStream.Next();

        //    numberOfTokens = 0;

        //    while (token != null)
        //    {
        //        numberOfTokens++;
        //        sb.Append(GetTokenView(token));
        //        token = tokenStream.Next();
        //    }

        //    return sb.ToString();
        //}

        public virtual string GetView(TokenStream tokenStream, out int numberOfAttributes)
        {
            StringBuilder sb = new StringBuilder();
            //instead of CharTermAttribute, use 
            //ITermAttribute termAttr = tokenStream.GetAttribute<ITermAttribute>(); //try addattribute 
            //Attribute termAttr = tokenStream.GetAttribute<Attribute>();
            //Error here. Resolved ITermAttribute is a must!

            ITermAttribute termAttr = tokenStream.GetAttribute<ITermAttribute>(); //try addattribute here

            //Error here

            //http://stackoverflow.com/questions/16274779/get-termattribute-in-tokenstream-lucene-net
            numberOfAttributes = 0;
            tokenStream.Reset(); //try out without
            while (tokenStream.IncrementToken())
            {
                numberOfAttributes++;
                sb.Append(GetAttributeView(termAttr));
                termAttr = tokenStream.GetAttribute<ITermAttribute>();
                //string term = termAttr.Term;
            }
            tokenStream.End();
            tokenStream.Dispose();
            //http://stackoverflow.com/questions/2638200/how-to-get-a-token-from-a-lucene-tokenstream
            return sb.ToString();
        }

        //protected abstract string GetTokenView(Token token);

        protected abstract string GetAttributeView(ITermAttribute termAttribute);

    }

    public class TermAnalyzerView : AnalyzerView
    {

        public override string Name
        {
            get { return "Terms"; }
        }

        //protected override string GetTokenView(Token token) { return "[" + token.TermText() + "]   "; }
        public override string GetView(TokenStream tokenStream, out int numberOfAttributes) { return base.GetView(tokenStream, out numberOfAttributes); }

        protected override string GetAttributeView(ITermAttribute termAttribute) { return "[" + termAttribute.Term + "]   "; }

    }

    /// <summary>
    ///     Not implemented!
    /// </summary>
    public class TermWithOffsetsView : AnalyzerView
    {

        public override string Name
        {
            get { return "Terms With Offsets"; }
        }

        //protected override string GetTokenView(Token token)
        //{

        //    return token.TermText() + "   Start: " + token.StartOffset().ToString().PadLeft(5) + "  End: "
        //           + token.EndOffset().ToString().PadLeft(5) + "\r\n";
        //}

        //protected override string GetAttributeView(TermAttribute termAttribute) { return termAttribute.Term + "   Start: " + Lucene.Net.Analysis.Tokenattributes.IOffsetAttribut}
        protected override string GetAttributeView(ITermAttribute termAttribute) { throw new NotImplementedException(); }

    }

    public class TermFrequencies : AnalyzerView
    {

        public override string Name
        {
            get { return "Term Frequencies"; }
        }

        private Dictionary<string, int> termDictionary = new Dictionary<string, int>();

        public override string GetView(TokenStream tokenStream, out int numberOfAttributes)
        {
            StringBuilder sb = new StringBuilder();

            //Token token = tokenStream.Next();
            ITermAttribute termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            numberOfAttributes = 0;

            while (tokenStream.IncrementToken())
            {
                numberOfAttributes++;

                if (termDictionary.Keys.Contains(termAttribute.Term))
                {
                    termDictionary[termAttribute.Term] = termDictionary[termAttribute.Term] + 1;
                }
                else
                {
                    termDictionary.Add(termAttribute.Term, 1);
                }

                termAttribute = tokenStream.GetAttribute<ITermAttribute>();

                //if (termDictionary.Keys.Contains(token.TermText()))
                //    termDictionary[token.TermText()] = termDictionary[token.TermText()] + 1;
                //else
                //    termDictionary.Add(token.TermText(), 1);

                //token = tokenStream.Next();
            }

            foreach (var item in termDictionary.OrderBy(x => x.Key))
            {
                sb.Append(item.Key + " [" + item.Value + "]   ");
            }

            termDictionary.Clear();

            return sb.ToString();
        }

        protected override string GetAttributeView(ITermAttribute termAttribute) { throw new NotImplementedException(); }

        //protected override string GetTokenView(Token token) { throw new NotImplementedException(); }
    }

}