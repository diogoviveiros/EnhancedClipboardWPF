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
using System.Windows.Shapes;

namespace EnhancedClipboardWPF
{

    public class Output
    {
        public string Title { get; set; }
        public string Info { get; set; }
    }

    /// <summary>
    /// Interaction logic for CopilotPrompts.xaml
    /// </summary>
    public partial class CopilotPrompts : Window
    {



        public CopilotPrompts()
        {
            InitializeComponent();



            List<Output> outputs = new List<Output>();

            outputs.Add(new Output
            {
                Title = "Option 1",
                Info = "| Company                        | Contact           | Country  |\r\n|--------------------------------|-------------------|----------|\r\n| Alfreds Futterkiste           | Maria Anders      | Germany  |\r\n| Centro comercial Moctezuma    | Francisco Chang   | Mexico   |\r\n| Ernst Handel                  | Roland Mendel     | Austria  |\r\n| Island Trading                | Helen Bennett     | UK       |\r\n| Laughing Bacchus Winecellars | Yoshi Tannamuri   | Canada   |\r\n| Magazzini Alimentari Riuniti  | Giovanni Rovelli | Italy    |\r\n"
            });

            outputs.Add(new Output
            {
                Title = "Option 2",
                Info = "|      Company                   |      Contact      |  Country |\r\n|:------------------------------:|:-----------------:|:--------:|\r\n| Alfreds Futterkiste            |   Maria Anders    | Germany  |\r\n| Centro comercial Moctezuma     |  Francisco Chang  |  Mexico  |\r\n| Ernst Handel                   |   Roland Mendel   | Austria  |\r\n| Island Trading                 |   Helen Bennett   |    UK    |\r\n| Laughing Bacchus Winecellars  |  Yoshi Tannamuri  |  Canada  |\r\n| Magazzini Alimentari Riuniti   | Giovanni Rovelli |  Italy   |\r\n",
            });

            outputs.Add(new Output
            {
                Title = "Option 3",
                Info = "| Company                        |     Contact      | Country  |\r\n|--------------------------------|:----------------:|----------|\r\n| Alfreds Futterkiste           |  Maria Anders    | Germany  |\r\n| Centro comercial Moctezuma    | Francisco Chang  |  Mexico  |\r\n| Ernst Handel                  |  Roland Mendel   | Austria  |\r\n| Island Trading                |  Helen Bennett   |    UK    |\r\n| Laughing Bacchus Winecellars  | Yoshi Tannamuri  |  Canada  |\r\n| Magazzini Alimentari Riuniti  | Giovanni Rovelli |  Italy   |\r\n",
            });

            outputs.Add(new Output
            {
                Title = "Option 4",
                Info = "|                        Company |          Contact |  Country |\r\n|--------------------------------|------------------|----------|\r\n|            Alfreds Futterkiste |     Maria Anders | Germany  |\r\n| Centro comercial Moctezuma     |  Francisco Chang |  Mexico  |\r\n|                  Ernst Handel  |     Roland Mendel| Austria  |\r\n|                Island Trading  |     Helen Bennett| UK       |\r\n| Laughing Bacchus Winecellars   |  Yoshi Tannamuri |  Canada  |\r\n| Magazzini Alimentari Riuniti   | Giovanni Rovelli |  Italy   |\r\n",
            });



            listView.ItemsSource = outputs;


        }

        private void clipboardCopy_Click(object sender, RoutedEventArgs e)
        {

            if (listView.SelectedItems.Count == 0)
            {
                return; 
            }
            Output foo = (Output)listView.SelectedItems[0];

            Clipboard.SetText(foo.Info);
            this.Close();

        }

        private void saveTemplate_Click(object sender, RoutedEventArgs e)
        {

            if (listView.SelectedItems.Count == 0)
            {
                return;
            }

            Output foo = (Output)listView.SelectedItems[0];

            TemplateItem item = new TemplateItem();
            item.body = foo.Info;
            item.data = foo.Info;
            ClipboardItem clipboardItem = new ClipboardItem();
            clipboardItem.index = 0;
            clipboardItem.clipboardItem = item;
            Trace.WriteLine(clipboardItem.clipboardItem.body);

            ((MainWindow)this.Owner).AddTemplate(null, clipboardItem);
         


            this.Close();

        }

        /// <summary>
        /// Interaction logic to be able to drag the window through the title bar.
        /// </summary>
        void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
