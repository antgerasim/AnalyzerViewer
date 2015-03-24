using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace AnalyzerViewer
{

    public partial class MainForm : Form
    {

        public BindingList<AnalyzerInfo> AnalyzerInfoList = new BindingList<AnalyzerInfo>();
        public BindingList<AnalyzerView> AnalyzerViewsList = new BindingList<AnalyzerView>();
        //Without Bindinglist, result in tbOutputText loose their original sort and display in alphabetical order  
        //public List<AnalyzerInfo> AnalyzerInfoList = new List<AnalyzerInfo>();
        //public List<AnalyzerView> AnalyzerViewsList = new List<AnalyzerView>();

        public MainForm()
        {
            InitializeComponent();

            AnalyzerInfoList.Add(new AnalyzerInfo("Keyword Analyzer", "\"Tokenizes\" the entire stream as a single token.",
                new KeywordAnalyzer()));
            AnalyzerInfoList.Add(new AnalyzerInfo("Whitespace Analyzer", "An Analyzer that uses WhitespaceTokenizer.",
                new WhitespaceAnalyzer()));
            AnalyzerInfoList.Add(new AnalyzerInfo("Stop Analyzer", "Filters LetterTokenizer with LowerCaseFilter and StopFilter.",
                new StopAnalyzer(Version.LUCENE_30)));
            AnalyzerInfoList.Add(new AnalyzerInfo("Simple Analyzer", "An Analyzer that filters LetterTokenizer with LowerCaseFilter.",
                new SimpleAnalyzer()));
            AnalyzerInfoList.Add(new AnalyzerInfo("Standard Analyzer",
                "Filters StandardTokenizer with StandardFilter, LowerCaseFilter and StopFilter, using a list of English stop words.",
                new StandardAnalyzer(Version.LUCENE_30)));
            AnalyzerInfoList.Add(new AnalyzerInfo("Russian Analyzer",
                "RussianLetterTokenizer, RussianLowerCaseFilter, StopFilter (custom stop list), RussianStemFilter.",
                new RussianAnalyzer(Version.LUCENE_30)));
            AnalyzerInfoList.Add(new AnalyzerInfo("Russian Snowball Analyzer",
                "StandardTokenizer, StandardFilter, LowerCaseFilter, [StopFilter] SnowballFilter.",
                new SnowballAnalyzer(Version.LUCENE_30, "Russian")));

            AnalyzerViewsList.Add(new TermAnalyzerView());
            //AnalyzerViewsList.Add(new TermWithOffsetsView());
            AnalyzerViewsList.Add(new TermFrequencies());

            tbDescription.DataBindings.Add(new Binding("Text", AnalyzerInfoList, "Description"));

            cbAnalysers.DisplayMember = "Name";
            cbAnalysers.ValueMember = "LuceneAnalyzer";
            cbAnalysers.DataSource = AnalyzerInfoList;

            cbViews.DisplayMember = "Name";
            cbViews.DataSource = AnalyzerViewsList;

            cbAnalysers.SelectedIndex = 0;
            cbViews.SelectedIndex = 0;

            cbAnalysers.SelectedValueChanged += new EventHandler(cbAnalysers_SelectedValueChanged);
            cbViews.SelectedValueChanged += new EventHandler(cbViews_SelectedValueChanged);
            tbSourceText.TextChanged += new EventHandler(tbSourceText_TextChanged);

            tbSourceText.Text = "The quick brown fox jumped over the lazy dog.";
            AnalyzeText();
        }

        private void cbViews_SelectedValueChanged(object sender, EventArgs e) { AnalyzeText(); }

        private void tbSourceText_TextChanged(object sender, EventArgs e) { AnalyzeText(); }

        private void cbAnalysers_SelectedValueChanged(object sender, EventArgs e) { AnalyzeText(); }

        public void AnalyzeText()
        {
            Analyzer analyzer = cbAnalysers.SelectedValue as Analyzer;

            int termCounter = 0;

            if (analyzer != null)
            {
                StringBuilder sb = new StringBuilder();

                AnalyzerView view = cbViews.SelectedValue as AnalyzerView;

                StringReader stringReader = new StringReader(tbSourceText.Text);

                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);

                tbOutputText.Text = view.GetView(tokenStream, out termCounter).Trim();
            }

            lblStats.Text = string.Format("Total of {0} Term(s) Found.", termCounter);
        }

        private void Form1_Load(object sender, EventArgs e) { }

    }

}