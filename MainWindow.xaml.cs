using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;
using DataObject = System.Windows.Forms.DataObject;
using IDataObject = System.Windows.Forms.IDataObject;
using ListViewItem = System.Windows.Controls.ListViewItem;
using ModifierKeys = EnhancedClipboardWPF.Core.ModifierKeys;
using Window = System.Windows.Window;
using EnhancedClipboardWPF.Core;


//ToDo: Add list for images, links

namespace EnhancedClipboardWPF
{

    /// <summary>
    /// Item Class for Files
    /// </summary>
    /// 
    public class FileItem
    {
        public TemplateItem? fileItem { get; set; }
        public int index { get; set; }
    }


    /// <summary>
    /// Item Class for Text
    /// </summary>
    /// 
    public class ClipboardItem
    {
        public TemplateItem? clipboardItem { get; set; }
        public int index { get; set; }
    }

    /// <summary>
    /// Item Class for Templates in general
    /// </summary>
    /// 
    public class TemplateItem : INotifyPropertyChanged
    {

        public string? body { get; set; }

        public ContentType type { get; set; }
        public string? data { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;


        public void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    /// <summary>
    /// Item Class for Inputs, important to establish for MVVM connection to add template inputs (items that are wrapped with [-->x<--] in text
    /// </summary>
    /// 
    public class TemplateInputs
    {
        public string inputName { get; set; }
        public string input { get; set; }

        public TemplateInputs(string inputName, string input)
        {
            this.inputName = inputName;
            this.input = input;
        }

    }


    /// <summary>
    /// Enum for whether a file is Text (Plain, HTML, or RTF), a File, an Image...
    /// </summary>
    /// 
    public enum ContentType : uint
    {
        HTML = 1,
        RTF = 2,
        Text = 3,
        File = 4,
        Image = 5
    }


    /// <summary>
    /// Enum for determining what data file we are dealing with, mainly to show display different images
    /// </summary>
    /// 
    public enum DataType : uint
    {
        Text = 1,
        TXT = 2,
        Word = 3,
        Excel = 4,
        PowerPoint = 5,
        Media = 6,
        Folder = 7,
        Other = 8
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        HotKeyManager hotKey = new HotKeyManager();
        InputManager inputManager = new InputManager();
        List<ClipboardItem> clipboardList = new List<ClipboardItem>();
        List<FileItem> fileList = new List<FileItem>();
        ObservableCollection<TemplateItem?> clipboardItems = new ObservableCollection<TemplateItem?>();
        List<TemplateItem?> fileItems = new List<TemplateItem?>();

        public MainWindow()
        {

            InitializeComponent();


            //Load already saved list from JSON

            LoadTemplates();



            // register the event that is fired after the key press.
            hotKey.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(HotKeyManager_KeyPressed);
            // register the Control + Alt + C and Control + Alt + V combinations as hot keys.
            hotKey.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt,
                Keys.C);
            hotKey.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Alt,
                Keys.V);

            
            clipboardItems = new ObservableCollection<TemplateItem?>(clipboardList.ConvertAll(x => x.clipboardItem));
            //fileItems = fileList.ConvertAll(x => x.fileItem);


            //Set the Items Source to clipboard Items
            SavedClipboardList.ItemsSource = clipboardItems;
        }


        /// <summary>
        /// Fetch the list of Text, Files already stored. 
        /// </summary>
        public void LoadTemplates()
        {

            fileList = JsonConvert.DeserializeObject<List<FileItem>>(File.ReadAllText(Application.StartupPath + @"\Resources\FileItems.json"));

            clipboardList = JsonConvert.DeserializeObject<List<ClipboardItem>>(File.ReadAllText(Application.StartupPath + @"\Resources\ClipboardItems.json"));


        }


        /// <summary>
        /// Adding a new item and notifying the ListView to update. 
        /// </summary>
        public void AddTemplate(FileItem? fileItem, ClipboardItem? clipboardItem)
        {
            if (fileItem != null)
            {

                fileList.Add(fileItem);
                File.WriteAllText(Application.StartupPath + @"\Resources\FileItems.json", JsonConvert.SerializeObject(fileList));

            }
            else if (clipboardItem != null)
            {

                clipboardList.Add(clipboardItem);

                //Informs listViewItem to refresh and update.
                clipboardItems.Add(clipboardItem.clipboardItem);

                if(clipboardItem.clipboardItem != null)
                {
                    clipboardItem.clipboardItem.NotifyPropertyChanged("body");
                }
               
            }


        }

        /// <summary>
        /// Save the templates by rewriting the files
        /// </summary>
        public void SaveTemplate()
        {
            File.WriteAllText(Application.StartupPath + @"\Resources\FileItems.json", JsonConvert.SerializeObject(fileList));
            File.WriteAllText(Application.StartupPath + @"\Resources\ClipboardItems.json", JsonConvert.SerializeObject(clipboardList));
        }


        /// <summary>
        /// Checks if a key was pressed and determines what to do next. Ctrl+Alt+C will add a template. Ctrl+Alt+V will open/hide the MainWindow.
        /// </summary>
        void HotKeyManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {

            // show the keys pressed in a label.
            if ((e.Key == Keys.C) && (e.Modifier == (ModifierKeys.Alt | ModifierKeys.Control)))
            {



                //Clear current data in clipboard and wait a few ms to ensure there's no race conditions
                //Clipboard.Clear();

                //Copy data to clipboard
                inputManager.sendCopyCommand();

                Thread.Sleep(600);
                //Fetch data Check what DataFormat it is


                StringCollection files = new StringCollection();
                TemplateItem item = new TemplateItem();

                IDataObject iData = Clipboard.GetDataObject();

                if (Clipboard.ContainsFileDropList())
                {
                    string fileText = "";
                    files = Clipboard.GetFileDropList();

                    foreach (String obj in files)
                    {
                        fileText += obj + "\n";
                    }

                    item.body = fileText;
                    item.data = fileText;
                    item.type = ContentType.File;

                    FileItem fileItem = new FileItem();

                    fileItem.index = fileList.Count;
                    fileItem.fileItem = item;


                    AddTemplate(fileItem, null);


                }
                else if (Clipboard.ContainsData(DataFormats.Html))
                {

                    item.body = Clipboard.GetText();
                    item.data = Clipboard.GetData(DataFormats.Html).ToString();
                    item.type = ContentType.HTML;

                    ClipboardItem clipboardItem = new ClipboardItem();
                    clipboardItem.index = clipboardList.Count;
                    clipboardItem.clipboardItem = item;

                    AddTemplate(null, clipboardItem);
                }
                else if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    item.body = Clipboard.GetText();
                    item.data = Clipboard.GetData(DataFormats.Rtf).ToString();
                    item.type = ContentType.RTF;

                    ClipboardItem clipboardItem = new ClipboardItem();
                    clipboardItem.index = clipboardList.Count;
                    clipboardItem.clipboardItem = item;

                    AddTemplate(null, clipboardItem);



                }
                else if (Clipboard.ContainsData(DataFormats.Text))
                {
                    item.body = Clipboard.GetText();
                    item.data = Clipboard.GetText();
                    item.type = ContentType.RTF;

                    ClipboardItem clipboardItem = new ClipboardItem();
                    clipboardItem.index = clipboardList.Count;
                    clipboardItem.clipboardItem = item;

                    AddTemplate(null, clipboardItem);
                }
                else
                {
                    //TODO: Put old data back, do nothing else.
                }





            }
            else if (e.Key == Keys.V)
            {

                if (this.Visibility != Visibility.Visible)
                {
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }

            }
        }



        /// <summary>
        /// Sets data to clipboard after the item in the ListView was selected. 
        /// </summary>
        private void SavedClipboardList_ItemClick(object sender, RoutedEventArgs e)
        {

            if (e.OriginalSource is Button || (e.OriginalSource is DependencyObject dependencyObject
        && FindParent<Button>(dependencyObject) != null))
            {
                return;
            }



            var item = sender as ListViewItem;
       
            if (item != null)
            {


                var content = item.Content;
                int index = SavedClipboardList.Items.IndexOf(content);
                

                if (clipboardList[index].clipboardItem.type == ContentType.File)
                {
                    string[] elements = clipboardList[index].clipboardItem.data.Split("\n");
                    StringCollection files = new StringCollection();
                    files.AddRange(elements);
                    Clipboard.SetFileDropList(files);
                }
                else
                {

                    ObservableCollection<TemplateInputs> templateInputs = new ObservableCollection<TemplateInputs>();
                    string data = clipboardList[index].clipboardItem.data;


                    //Removes line breaks so that the next step can appropriately find the substrings we want
                    data = Regex.Replace(data, @"\t|\n|\r", "");


                    //Find substrings 
                    string pattern = string.Format(
                        "{0}({1}){2}", Regex.Escape("[--&gt;"), ".+?", Regex.Escape("&lt;--]"));

                    foreach (Match m in Regex.Matches(data, pattern))
                    {
                        templateInputs.Add(new TemplateInputs(m.Groups[1].Value, ""));

                                                   
                    }

                    if(templateInputs.Count != 0)
                    {
                        
                        InsertTemplateContent templateWindow = new InsertTemplateContent(templateInputs, index, data);
                        templateWindow.Show();
                        templateWindow.Owner = this;
                    }
                    else
                    {
                        setDataToClipboard(index, data);
                    }


                }



                if (this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous)))
                {

                    inputManager.SendPasteCommand();

                }

                this.Visibility = Visibility.Collapsed;






            }


        }


        /// <summary>
        /// Packs the selected data to a DataObject and injects that into the Clipboard
        /// </summary>
        public void setDataToClipboard(int index, string data)
        {

            Trace.WriteLine(data);
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Html, data);
            dataObject.SetData(DataFormats.Text, clipboardList[index].clipboardItem.body);
            dataObject.SetData(DataFormats.UnicodeText, clipboardList[index].clipboardItem.body);
            Clipboard.SetDataObject(dataObject);
        }


        /// <summary>
        /// Sets the item source to text clipboard items 
        /// </summary>
        private void ChangeToTextTemplates(object sender, RoutedEventArgs e)
        {
            SavedClipboardList.ItemsSource = clipboardItems;

        }

        /// <summary>
        /// Sets the item source to file clipboard items 
        /// </summary>
        private void ChangeToFileTemplates(object sender, RoutedEventArgs e)
        {
            SavedClipboardList.ItemsSource = fileItems;
        }


        /// <summary>
        /// Interaction logic to be able to drag the window through the title bar.
        /// </summary>
        void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }


        /// <summary>
        /// Interaction logic to be able to hide the window when the X is clicked.
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Interaction logic for when the X is clicked on a ListViewItem. Removes the item from the list, notifies the ListView that a change has been made, and updates the appropriate file. 
        /// </summary>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TemplateItem item = button.DataContext as TemplateItem;

            int index = clipboardItems.IndexOf(item);

            clipboardList.RemoveAt(index);

            clipboardItems.Remove(item);
            item.NotifyPropertyChanged("body");


            SaveTemplate();

        }


        /// <summary>
        /// Interaction logic for when the up arrow is clicked on a ListViewItem. Lowers the index and increases the index of the item right above it. Notifies the ListView that a change has been made, and updates the appropriate file. 
        /// </summary>
        private void RaiseListPriority(object sender, RoutedEventArgs e)
        {


            Button? button = sender as Button;

            TemplateItem? item = button.DataContext as TemplateItem;

            int index = clipboardItems.IndexOf(item);


            //Do nothing if the item is already at the top. 
            if (index == 0)
            {
                return;
            }

            ClipboardItem save = clipboardList[index - 1];

            clipboardList[index - 1] = clipboardList[index];

            clipboardList[index] = save;

            TemplateItem saveList = clipboardItems[index - 1];

            clipboardItems[index - 1] = clipboardItems[index];
            clipboardItems[index] = saveList;

            item.NotifyPropertyChanged("body");

            SaveTemplate();




        }


        /// <summary>
        /// Interaction logic for when the down arrow is clicked on a ListViewItem. Increases the index and decreases the index of the item right below it. Notifies the ListView that a change has been made, and updates the appropriate file. 
        /// </summary>
        private void LowerListPriority(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TemplateItem item = button.DataContext as TemplateItem;

            int index = clipboardItems.IndexOf(item);


            //Do nothing if this is already the last item
            if (index == clipboardItems.Count - 1)
            {
                return;
            }

            ClipboardItem save = clipboardList[index + 1];

            clipboardList[index + 1] = clipboardList[index];

            clipboardList[index] = save;

            TemplateItem saveList = clipboardItems[index + 1];

            clipboardItems[index + 1] = clipboardItems[index];
            clipboardItems[index] = saveList;

            item.NotifyPropertyChanged("body");

            SaveTemplate();

        }


        /// <summary>
        /// Interaction logic for when the "..." button is clicked on a ListViewItem. Parses the data appropriately so it can be read by the HTML viewer and then opens the WebEditor window passing the appropriate information.
        /// </summary>
        private void OpenEditWindow(object sender, RoutedEventArgs e)
        {

            Button button = sender as Button;
            TemplateItem item = button.DataContext as TemplateItem;

            int index = clipboardItems.IndexOf(item);
            string html = clipboardItems[index].data;
            Trace.WriteLine(html);

            Trace.WriteLine(html.IndexOf("<html"));
            Trace.WriteLine(html.IndexOf("<HTML"));

            if(html == "" || html == null)
            {

            }else if (html.IndexOf("<html") < 0)
            {
                html = html.Substring(html.IndexOf("<HTML"));
            }
            else
            {
                html = html.Substring(html.IndexOf("<html"));
            }


            WebEditor w1 = new WebEditor(html, index);

            w1.Show();
            w1.Owner = this;

        }


        /// <summary>
        /// Finds the correct item that is being pressed in the ListViewItem. This avoids the ListViewItem being selected when one of the buttons (i.e. Delete, up, down, Open Edit View) is pressed.
        /// </summary>
        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null)
                return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }


        /// <summary>
        /// Updates the ListViewItem after edits have been made to it, notifies the ListView that changes have been made and saves it to the appropriate file. 
        /// </summary>
        public void UpdateItem(int index)
        {

           

            var data = Clipboard.GetData(DataFormats.Html).ToString();
           
            clipboardItems[index].data = data;



            clipboardItems[index].body = Clipboard.GetText();
            clipboardItems[index].NotifyPropertyChanged("body");

            SaveTemplate();
        }


    }


}
