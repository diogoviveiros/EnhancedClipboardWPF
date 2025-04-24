using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace EnhancedClipboardWPF
{
    /// <summary>
    /// Interaction logic for Copilot.xaml
    /// </summary>
    public partial class Copilot : Window
    {
        string plainText;
        string data;

        public class dataInput
        {
            string plainText;
            string data;

        }

        public Copilot()
        {
            InitializeComponent();

         
        }

        public Copilot(string plainText, string data)
        {
            InitializeComponent();

            
            this.plainText = plainText;
            this.data = data;

            content.Text = plainText;

        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Generate Outputs, put it in a new window, and assign MainWindow as its owner so that it can send command back (mostly in case they want to save it as a template)
        /// </summary>
        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            CopilotPrompts cp1 = new CopilotPrompts();

            cp1.Owner = ((MainWindow)this.Owner);
            cp1.Show();
            this.Hide();
        }

    }

       
}
