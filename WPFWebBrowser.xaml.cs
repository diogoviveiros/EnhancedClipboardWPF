using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnhancedClipboardWPF.Core;
using mshtml;
using static System.Windows.Forms.DataFormats;
using Format = EnhancedClipboardWPF.Core.Format;

namespace EnhancedClipboardWPF
{
    /// <summary>
    /// Interaction logic for WPFWebBrowser.xaml
    /// </summary>
    public partial class WPFWebBrowser : UserControl
    {
        public HTMLDocument? doc;
        public WebBrowser? webBrowser;

        public WPFWebBrowser()
        {
            InitializeComponent();
        }

        //Fixes HTML encoding to be shown properly in an MSHTML Webbrowser view. Otherwise, characters like Â get displayed randomly where line breaks should be
        public string FixHtml(string HTML)
        {
            StringBuilder sb = new StringBuilder();
            char[] s = HTML.ToCharArray();
            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    sb.Append("&#" + Convert.ToInt32(c) + ";");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public void newWb(string html)
        {
            if (webBrowser != null)
            {
                webBrowser.LoadCompleted -= completed;
                webBrowser.Dispose();
                gridwebBrowser.Children.Remove(webBrowser);
            }

            if (doc != null)
            {
                doc.clear();
            }

            webBrowser = new WebBrowser();
            webBrowser.LoadCompleted += completed;
            gridwebBrowser.Children.Add(webBrowser);

            Script.HideScriptErrors(webBrowser, true);

            if(html!= null)
            {
               webBrowser.NavigateToString(FixHtml(html));
            }
            
            if(webBrowser!= null)
            {
                doc = webBrowser.Document as HTMLDocument;

                if (doc != null){

                    doc.designMode = "On";
                    Format.doc = doc;
                }
                
            }
            

            

        }

        private void completed(object sender, NavigationEventArgs e)
        {

            if (webBrowser != null)
            {
                doc = webBrowser.Document as HTMLDocument;
                if (doc != null)
                {
                    doc.designMode = "On";
                }
            }
           
        }


    }
}
