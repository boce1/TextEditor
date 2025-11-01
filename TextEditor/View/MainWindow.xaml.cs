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
            controller = new TextEditorController(EditorBox, EditorStateLabel, CaretPositionLabel);
            EditorBox.PreviewTextInput += controller.EditorBox_PreviewTextInput;
            EditorBox.PreviewKeyDown += controller.EditorBox_PreviewKeyDown;
            EditorBox.PreviewMouseDown += controller.EditorBox_PreviewMouseDown;
            EditorBox.PreviewMouseMove += controller.EditorBox_PreviewMouseMove;
            EditorBox.PreviewMouseUp += controller.EditorBox_PreviewMouseUp;
            //EditorBox.MouseDown += controller.EditorBox_PreviewMouseDown;
            //EditorBox.MouseMove += controller.EditorBox_PreviewMouseMove;
            //EditorBox.MouseUp += controller.EditorBox_PreviewMouseUp;

            InsertButton.Click += controller.InsertButton_Click;
            ReadOnlyButton.Click += controller.ReadOnlyButton_Click;
            OverwriteButton.Click += controller.OverwriteButton_Click;
        }

        
    }
}