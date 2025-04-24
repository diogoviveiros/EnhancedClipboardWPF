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

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {

           
            foreach (var template in templates)
            {

                Trace.WriteLine("Replacing " + template.inputName + " with " + template.input);
                data = data.Replace("[--&gt;" + template.inputName + "&lt;--]", template.input);
            } 
            ((MainWindow)this.Owner).setDataToClipboard(data);
            this.Close();
            
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
           
        }
        
    }
}
