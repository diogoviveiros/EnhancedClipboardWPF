using System.Windows.Markup;
using mshtml;

namespace EnhancedClipboardWPF.Core
{
    public static class Format
    {
        public static HTMLDocument? doc;

        public static void bold()
        {

            if (doc != null)
            {
                doc.execCommand("Bold", false, null);


            }
        }

        public static void Italic()
        {
            if (doc != null)
            {
                doc.execCommand("Italic", false, null);

            }
        }

        public static void Underline()
        {
            if (doc != null)
            {
                doc.execCommand("Underline", false, null);
            }
        }

        public static void JustifyLeft()
        {
            if (doc != null)
            {
                doc.execCommand("JustifyLeft", false, null);
            }
        }

        public static void JustifyCenter()
        {
            if (doc != null)
            {
                doc.execCommand("JustifyCenter", false, null);
            }
        }

        public static void JustifyRight()
        {
            if (doc != null)
            {
                doc.execCommand("JustifyRight", false, null);
            }
        }

        public static void JustifyFull()
        {
            if (doc != null)
            {
                doc.execCommand("JustifyFull", false, null);
            }
        }


        public static void InsertOrderedList()
        {
            if (doc != null)
            {
                doc.execCommand("InsertOrderedList", false, null);
            }
        }

        public static void InsertUnorderedList()
        {
            if (doc != null)
            {
                doc.execCommand("InsertUnorderedList", false, null);
            }
        }

        public static void Outdent()
        {
            if (doc != null)
            {
                doc.execCommand("Outdent", false, null);
            }
        }

        public static void Indent()
        {
            if (doc != null)
            {
                doc.execCommand("Indent", false, null);
                doc.ToString();
                
            }
        }

        public static void InsertTemplate()
        {
            if(doc != null)
            {
                IHTMLSelectionObject currentSelection = doc.selection;

                if (currentSelection != null)
                {


                    IHTMLTxtRange? range = currentSelection.createRange() as IHTMLTxtRange;
                    string selection = "";

                    if (range != null)
                    {
                        selection = range.text;

                        range.pasteHTML("[-->" + selection + "<--]");
                    }


                }
            }
            
        }

        public static void LaunchCopilotWindow()
        {
            Copilot c1 = new Copilot(doc.body.innerText, doc.body.innerHTML);

            c1.Show();
            
        }

    }
}
