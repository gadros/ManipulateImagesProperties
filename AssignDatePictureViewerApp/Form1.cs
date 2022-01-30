using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImageFilePropertiesQueryAndEdit;
using System.Globalization;

namespace AssignDatePictureViewerApp
{
    public partial class Form1 : Form
    {
        string _currnetFolder;
        List<string> _imagesIterator;
        int _imageIndex;
        private int _numOfImages;
        private FileInfo _imageFileInfo;
        private ImageProperties _currentImageProperties;
        readonly CultureInfo _hebCultureInfo = new CultureInfo("he-IL");
        private DateTime _lastSetDate;
        private bool _firstTimeSelectFolderDone;
        private string _lastImage;

        public Form1()
        {
            InitializeComponent();
            _currnetFolder = Properties.Settings.Default.LastFolder;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SelectFolderBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = string.IsNullOrEmpty(_currnetFolder) ? @"c:\bugs\pics" : _currnetFolder;

            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                var selectFiles = "*.jpg,*.jpeg,*.png,*.bmp".Split(',').SelectMany(ext =>
                    (System.IO.Directory.EnumerateFiles(folderBrowserDialog1.SelectedPath, ext,
                        System.IO.SearchOption.TopDirectoryOnly)));
                string[] selectedFiles = selectFiles as string[] ?? selectFiles.ToArray();
                if (selectedFiles.Any())
                {
                    IEnumerable<string> imageFiles = selectedFiles
                        .OrderBy(fileName => fileName, StringComparer.OrdinalIgnoreCase).ToHashSet();
                    _imagesIterator = new List<string>(imageFiles);
                    _imageIndex = 0;
                    _numOfImages = imageFiles.Count();
                    _lastSetDate = DateTime.MinValue;

                    TryMoveToLastLoadedImage();
                    LoadImage();

                    SaveLastFolder();
                }

                _firstTimeSelectFolderDone = true;
            }
        }

        private void SaveLastFolder()
        {
            _currnetFolder = folderBrowserDialog1.SelectedPath;
            Properties.Settings.Default.LastFolder = _currnetFolder;
            Properties.Settings.Default.Save();
        }

        private void TryMoveToLastLoadedImage()
        {
            if (!_firstTimeSelectFolderDone && !string.IsNullOrEmpty(_currnetFolder))
            {
                // TODO: move to _lastImage
            }
        }

        private void SetMaskedDate(DateTime date)
        {
            string day = date.Day.ToString("00");
            string month = date.Month.ToString("00");
            string year = date.Year.ToString("0000");
            ImageDateMskTxtBox.Text = string.Concat(day, "/", month,"/", year);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            if (_imageIndex + 1 < _numOfImages)
            {
                _imageIndex++;
                LoadImage();
            }
        }

        private void LoadImage()
        {
            string currentImageName = Path.Combine(_currnetFolder, _imagesIterator[_imageIndex]);
            using (FileStream stream = new FileStream(currentImageName, FileMode.Open, FileAccess.Read))
                // get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                // make a new Bitmap object the owner of the MemoryStream
                pictureBox1.Image = new Bitmap(memoryStream);
            }

            _imageFileInfo = new FileInfo(currentImageName);
            _currentImageProperties = new ImageProperties(currentImageName, _imageFileInfo, false);

            ImageDateMskTxtBox.ForeColor = Color.Black;

            DateTime takenOrModified = _currentImageProperties.DateTaken;
            if (takenOrModified == DateTime.MinValue)
            {
                takenOrModified = _currentImageProperties.DateModified;
                ImageDateMskTxtBox.ForeColor = Color.Red;
            }

            if (takenOrModified != DateTime.MinValue)
            {
                if (_lastSetDate != DateTime.MinValue)
                {
                    if (takenOrModified.Year == _lastSetDate.Year) //i assumed that if it's a sibling image and the years agrees then i can trust the value read from the image. yes, i am aware of end of december (edge case) 
                    {
                        SetMaskedDate(takenOrModified);
                    }
                    else
                    {
                        SetMaskedDate(_lastSetDate);
                        ImageDateMskTxtBox.ForeColor = Color.Red;
                    }
                }
                else
                {
                    SetMaskedDate(takenOrModified);
                }
            }
            else
            {
                DateTime proposedDate = new ImageDateTakenCalculator(_currentImageProperties, _imageFileInfo).GetValueForDateTaken();
                SetMaskedDate(proposedDate);
                ImageDateMskTxtBox.ForeColor = Color.Red;
            }

            this.Text = currentImageName;
            _lastImage = currentImageName;
            Properties.Settings.Default.LastImage = currentImageName;
            Properties.Settings.Default.Save();
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            if (_imageIndex - 1 > -1)
            {
                _imageIndex--;
                LoadImage();
            }
        }

        private void SetDateBtn_Click(object sender, EventArgs e)
        {
            const string dateFormat = "dd/MM/yyyy";
            if (DateTime.TryParseExact(ImageDateMskTxtBox.Text, dateFormat, _hebCultureInfo,DateTimeStyles.None, out var newDateTaken))
            {
                var runExifTool = new RunExifTool(_imageFileInfo.FullName, false);
                bool operationResult = runExifTool.SetDateTaken(newDateTaken);
                if (!operationResult)
                {
                    MessageBox.Show($@"Failed setting {newDateTaken} as the date taken for {_currentImageProperties.Name}");
                    ImageDateMskTxtBox.ForeColor = Color.Red;
                }
                else
                {
                    ImageDateMskTxtBox.ForeColor = Color.Blue;
                    _lastSetDate = newDateTaken;
                }
            }
            else
            {
                MessageBox.Show($@"Failed parsing {ImageDateMskTxtBox.Text} as a date value. please set a date value in the format to {dateFormat}");
            }
        }

        private void SetDateMoveNextBtn_Click(object sender, EventArgs e)
        {
            SetDateBtn_Click(sender, e);
            NextBtn_Click(sender, e);
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(_currnetFolder);
        }
    }
}
