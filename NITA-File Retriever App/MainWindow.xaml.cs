using System.Windows;
using System.Windows.Controls;



namespace PO_FileRetrieverUI
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string systemLang = Thread.CurrentThread.CurrentCulture.ToString(); // ex. of output: en-CA en-US fr-CA fr-FR
            Properties.Settings.Default["Lang"] = systemLang[..systemLang.IndexOf('-')].ToUpper();
            Properties.Settings.Default.Save();
            GenFnct.SetLanguage((string)Properties.Settings.Default["Lang"]);
        }


        /// <summary>
        /// Launches the app by displaying the acknowledgements window first and making users agree to the terms and conditions before allowing access to the File Retriever UI
        /// </summary>
        private void ClickAcknowledgements(object sender, RoutedEventArgs e)
        {
            // if users agree to the terms and conditions, acknowledgements window closes and UI window appears
            if (sender == btn_yes)
            {
                AppWindow RetrieverApp = new AppWindow();
                try { RetrieverApp.Show(); }
                catch { }
            }
            // if users exit/disagree with the terms and conditions, the entire app closes
            this.Close();
        }

        private void btn_language_click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["Lang"] = (string)btn_language.Content;
            Properties.Settings.Default.Save();
            GenFnct.SetLanguage((string)Properties.Settings.Default["Lang"]);
        }

    }
}