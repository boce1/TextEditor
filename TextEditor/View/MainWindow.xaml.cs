using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextEditor.Controler;
using TextEditor.Memento;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextEditorController controller;
        public MainWindow()
        {
            InitializeComponent();
            controller = new TextEditorController(EditorBox, EditorStateLabel, CaretPositionLabel, FilePathLabel);
            EditorBox.PreviewTextInput += controller.EditorBox_PreviewTextInput;
            EditorBox.PreviewKeyDown += controller.EditorBox_PreviewKeyDown;
            EditorBox.PreviewMouseDown += controller.EditorBox_PreviewMouseDown;
            EditorBox.PreviewMouseMove += controller.EditorBox_PreviewMouseMove;
            EditorBox.PreviewMouseUp += controller.EditorBox_PreviewMouseUp;

            OpenFileMenu.Click += controller.OpenFileMenu_Click;
            SaveFileMenu.Click += controller.SaveFileMenu_Click;
            SaveAsFileMenu.Click += controller.SaveAsFileMenu_Click;

            CopyMenu.Click += controller.CopyMenu_Click;
            PasteMenu.Click += controller.PasteMenu_Click;
            CutMenu.Click += controller.CutMenu_Click;

            InsertMenu.Click += controller.InsertMenu_Click; 
            ReadOnlyMenu.Click += controller.ReadOnlyMenu_Click; 
            OverwriteMenu.Click += controller.OverwriteMenu_Click;
        }

        
    }
}