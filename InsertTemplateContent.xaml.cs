using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace EnhancedClipboardWPF
{

    /// <summary>
    /// Interaction logic for InsertTemplateContent.xaml
    /// </summary>
    /// 
    internal static class ClipboardFormats
    {
        static readonly string HEADER =
            "Version:0.9\r\n" +
            "StartHTML:{0:0000000000}\r\n" +
            "EndHTML:{1:0000000000}\r\n" +
            "StartFragment:{2:0000000000}\r\n" +
            "EndFragment:{3:0000000000}\r\n";

        static readonly string HTML_START =
            "<html>\r\n" +
            "<body>\r\n" +
            "<!--StartFragment-->";

        static readonly string HTML_END =
            "<!--EndFragment-->\r\n" +
            "</body>\r\n" +
            "</html>";

        public static string ConvertHtmlToClipboardData(string html)
        {
            var encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            var data = Array.Empty<byte>();

            var header = encoding.GetBytes(String.Format(HEADER, 0, 1, 2, 3));
            data = data.Concat(header).ToArray();

            var startHtml = data.Length;
            data = data.Concat(encoding.GetBytes(HTML_START)).ToArray();

            var startFragment = data.Length;
            data = data.Concat(encoding.GetBytes(html)).ToArray();

            var endFragment = data.Length;
            data = data.Concat(encoding.GetBytes(HTML_END)).ToArray();

            var endHtml = data.Length;

            var newHeader = encoding.GetBytes(
                String.Format(HEADER, startHtml, endHtml, startFragment, endFragment));
            if (newHeader.Length != startHtml)
            {
                throw new InvalidOperationException(nameof(ConvertHtmlToClipboardData));
            }

            Array.Copy(newHeader, data, length: startHtml);
            return encoding.GetString(data);
        }
    }

    public partial class InsertTemplateContent : Window
    {
        private int index;
        private ObservableCollection<TemplateInputs> templates;
        private String data;

        public InsertTemplateContent()
        {
            InitializeComponent();
        }

        public InsertTemplateContent(ObservableCollection<TemplateInputs> templates, int index, String data)
        {
            InitializeComponent();

                       
            TemplatesView.ItemsSource = templates;
            this.templates = templates;
            this.index = index;
            this.data = data;
            

            
        }

        public class listInputs
        {
            public string label { get; set; }
            public string input { get; set; }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {

           
            foreach (var template in templates)
            {

                Trace.WriteLine("Replacing " + template.inputName + " with " + template.input);
                data = data.Replace("[--&gt;" + template.inputName + "&lt;--]", template.input);
            } 

            if (data.IndexOf("<html") < 0)
            {
                data = data.Substring(data.IndexOf("<HTML"));
            }
            else
            {
                data = data.Substring(data.IndexOf("<html"));
            }

            


            ((MainWindow)this.Owner).setDataToClipboard(index, ClipboardFormats.ConvertHtmlToClipboardData(data));
            this.Close();
            
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
           
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
    }
}
