using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace StarshipTheory.PatchInjector.Controls
{
    /// <summary>
    /// Interaction logic for FileTextbox.xaml
    /// </summary>
    public partial class FileTextbox : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<FileTextboxChangedEventArgs> TextChanged;

        private String[] _Filenames;

        public String[] Filenames
        {
            get { return _Filenames; }
            set
            {
                _Filenames = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filenames"));
            }
        }

        [Category("Common"), DefaultValue("")]
        public String Filename
        {
            get
            {
                return _Filenames == null || _Filenames.Length == 0 ? null : _Filenames.FirstOrDefault();
            }
            set
            {
                if (_Filenames == null || _Filenames.Length < 1)
                    _Filenames = new String[1];

                _Filenames[0] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
            }
        }

        private String _Title = "";

        [Category("Common"), DefaultValue("")]
        public String Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private bool _MultiSelect = false;

        [Category("Common"), DefaultValue(false)]
        public bool MultiSelect
        {
            get { return _MultiSelect; }
            set
            {
                _MultiSelect = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiSelect"));
            }
        }

        private String _DefaultExt = "";

        [Category("Common"), DefaultValue("")]
        public String DefaultExt
        {
            get { return _DefaultExt; }
            set
            {
                _DefaultExt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DefaultExt"));
            }
        }

        private String _Filter = "";

        [Category("Common"), DefaultValue("")]
        public String Filter
        {
            get { return _Filter; }
            set
            {
                _Filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filter"));
            }
        }

        private int _FilterIndex = 0;

        [Category("Common"), DefaultValue(0)]
        public int FilterIndex
        {
            get { return _FilterIndex; }
            set
            {
                _FilterIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilterIndex"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FileTextbox()
        {
            InitializeComponent();
        }

        public FileTextbox(String Title, String Filename) : this()
        {
            this.Title = Title;
            this.Filename = Filename;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog ofd = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = ofd.CheckPathExists = true;
            ofd.DefaultExt = DefaultExt;
            ofd.FileName = Filename;
            ofd.Filter = Filter;
            ofd.FilterIndex = FilterIndex;
            ofd.Multiselect = MultiSelect;
            ofd.RestoreDirectory = true;
            ofd.Title = Title;

            bool? Res = ofd.ShowDialog();
            if(Res.HasValue && Res.Value)
            {
                Filename = ofd.FileName;
                Filenames = ofd.FileNames;
                Filter = ofd.Filter;
                FilterIndex = ofd.FilterIndex;

                TextChanged?.Invoke(this, new FileTextboxChangedEventArgs(Filename, Filenames));
            }
        }

        private void txtFile_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtFile_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Filenames = new string[] { txtFile.Text };
            TextChanged?.Invoke(this, new FileTextboxChangedEventArgs(Filename, Filenames));
        }
    }

    public class FileTextboxChangedEventArgs : EventArgs
    {
        public String Filename { get; private set; }
        public String[] Filenames { get; private set; }

        public FileTextboxChangedEventArgs(String Filename, String[] Filenames)
        {
            this.Filename = Filename;
            this.Filenames = Filenames;
        }
    }
}
