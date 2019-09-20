using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Deep_WPF
{


    /// <summary>
    /// Training.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Training : System.Windows.Controls.UserControl
    {

        //변수
        #region

        //경로 변수
        public string darknetFolderPath;
        public string darknetDataFolderMiddlePath = "\\build\\darknet\\x64\\data\\";
        public string trainFilePath;
        public string trainFileName;
        public string namesFilePath;
        public string namesFileName;
        public string dataFilePath;
        public string dataFileName;
        public string cfgFilePath;
        public string cfgFileName;
        public string convFilePath;
        public string convFileName;
        public int classCount = 0;
        public string drive;
        public int driveidx;
        public string startCode;
        #endregion

        public Training()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dataOption.Visibility = Visibility.Visible;
        }

        private void DarknetPathBtnclick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result.ToString() == "OK")
            {

                darknetPathTxtbox.Text = dialog.SelectedPath;
                darknetFolderPath = darknetPathTxtbox.Text;

                driveidx = darknetFolderPath.IndexOf(':'); 
                drive = darknetFolderPath.Remove(driveidx);

            }

        }

        private void TrainingPathBtnclick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "D:\\";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "txt파일 (.txt)|*.txt";
            DialogResult result = dlg.ShowDialog();
            if (result.ToString() == "OK")
            {

                trainPathTxtbox.Text = dlg.FileName;

                trainFilePath = trainPathTxtbox.Text;
                trainFileName = trainFilePath.Substring(trainFilePath.LastIndexOf('\\') + 1);
                
                string trainFullPath = darknetFolderPath +
                    darknetDataFolderMiddlePath + trainFileName;
                
                CopyFileInside(trainFilePath, trainFullPath);
                
                trainPathTxtbox.Text = trainFullPath;

                trainFilePath = trainPathTxtbox.Text;
                trainFileName = trainFilePath.Substring(trainFilePath.LastIndexOf('\\') + 1);
            }
        }

        void CopyFileInside(string _filePath, string _destPath)
        {
            string[] lines = File.ReadAllLines(_filePath);
            int readNum = lines.Length;
            StreamWriter sw = new StreamWriter(_destPath, false);
            for (int i = 0; i < readNum; i++)
            {
                sw.WriteLine(lines[i]);
            }
            sw.Close();
        }


        private void NamesPathBtnclick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "D:\\";

            dlg.DefaultExt = ".names";
            dlg.Filter = "names파일 (.names)|*.names";
            DialogResult result = dlg.ShowDialog();
            if (result.ToString() == "OK")
            {
                namesPathTxtbox.Text = dlg.FileName;

                namesFilePath = namesPathTxtbox.Text;
                namesFileName = namesFilePath.Substring(namesFilePath.LastIndexOf('\\') + 1);

                string[] lines;
                lines = File.ReadAllLines(namesFilePath);
                int readNum = lines.Length;
                
                int cnt = 0;
                for (int i = 0; i < readNum; i++)
                {
                    lines[i].TrimStart();
                    if (lines[i] == "")
                    {
                        continue;
                    }
                    cnt += 1;
                }
                classCount = cnt;
                
                string destPath = darknetFolderPath +
                    darknetDataFolderMiddlePath + namesFileName;
                
                CopyFileInside(namesFilePath, destPath);
                
                namesPathTxtbox.Text = destPath;

                namesFilePath = namesPathTxtbox.Text;
                namesFileName = namesFilePath.Substring(namesFilePath.LastIndexOf('\\') + 1);

            }
        }


        private void DataPathBtnclick(object sender, RoutedEventArgs e)
        {
            //.data 경로
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "D:\\";
            dlg.DefaultExt = ".data";
            dlg.Filter = "data파일 (.data)|*.data";
            DialogResult result = dlg.ShowDialog();
            if (result.ToString() == "OK")
            {
                dataPathTxtbox.Text = dlg.FileName;

                dataFilePath = dataPathTxtbox.Text;
                dataFileName = dataFilePath.Substring(dataFilePath.LastIndexOf('\\') + 1);
                
                string destPath = darknetFolderPath +
                    darknetDataFolderMiddlePath + dataFileName;
                
                CopyFileInside(dataFilePath, destPath);
                
                dataPathTxtbox.Text = destPath;

                dataFilePath = dataPathTxtbox.Text;
                dataFileName = dataFilePath.Substring(dataFilePath.LastIndexOf('\\') + 1);

            }
        }


        private void CfgPathBtnclick(object sender, RoutedEventArgs e)
        {
            //.cfg 경로
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "D:\\";
            dlg.DefaultExt = ".cfg";
            dlg.Filter = "cfg파일 (.cfg)|*.cfg";
            DialogResult result = dlg.ShowDialog();

            if (result.ToString() == "OK")
            {
                cfgPathTxtbox.Text = dlg.FileName;

                cfgFilePath = cfgPathTxtbox.Text;
                cfgFileName = cfgFilePath.Substring(cfgFilePath.LastIndexOf('\\') + 1);
                
                string destPath = darknetFolderPath +
                    darknetDataFolderMiddlePath + cfgFileName;
                
                CopyFileInside(cfgFilePath, destPath);
                
                cfgPathTxtbox.Text = destPath;

                cfgFilePath = cfgPathTxtbox.Text;
                cfgFileName = cfgFilePath.Substring(cfgFilePath.LastIndexOf('\\') + 1);
            }
        }

        private void ConvPathBtnclick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "D:\\";
            DialogResult result = dlg.ShowDialog();
            if (result.ToString() == "OK")
            {
                convPathTxtbox.Text = dlg.FileName;


                convFilePath = convPathTxtbox.Text;
                convFileName = convFilePath.Substring(convFilePath.LastIndexOf('\\') + 1);
            }

        }

        private void NamesMakeBtnclick(object sender, RoutedEventArgs e)
        {
            namesOption.Visibility = Visibility.Visible;
            makeNamesNewWindow();
        }



        private void FrmNew_WriteTextEvent(string tcc, string tpt, string tfp, string tfn)
        {
            //throw new NotImplementedException();
        }
        public void makeNamesNewWindow()
        {
            MakeNames makeNames = new MakeNames();


            makeNames.tdarknetFolderPath = darknetFolderPath;
            makeNames.tdarknetDataFolderMiddlePath = darknetDataFolderMiddlePath;
            makeNames.tnamesFilePath = namesFilePath;
            makeNames.tnamesPathTxtbox = namesPathTxtbox.Text;            

            makeNames.WriteTextEvent += FrmNew_WriteTextEvent;
            makeNames.WriteTextEvent += new MakeNames.TextEventHandler(frmNew_WriteTextEvent);

            makeNames.ShowDialog();

            
        }


        void frmNew_WriteTextEvent(string tcc, string tpt, string tfp, string tfn)
        {
            classCount = Convert.ToInt32(tcc);
            namesPathTxtbox.Text = tpt;
            namesFilePath = tfp;
            namesFileName = tfn;
           
        }


        private void DataMakeBtnclick(object sender, RoutedEventArgs e)
        {
            dataOption.Visibility = Visibility.Visible;
        }



        private void CfgMakeBtnclick(object sender, RoutedEventArgs e)
        {

            cfgOption.Visibility = Visibility.Visible;

        }

        private void ConvMakeBtnclick(object sender, RoutedEventArgs e)
        {

        }



        private void MakeDataOkBtnClick(object sender, RoutedEventArgs e)
        {
            string[] dataLine = {
                "classes = "+classCount,
                "train = data/"+trainFileName,
                "valid = data/validpin.txt",
                "names = data/"+namesFileName,
                "backup = backup/"
            };


            string dataFullPath = darknetFolderPath +
                darknetDataFolderMiddlePath + dataFileNameTbx.Text + ".data";

            int readNum = dataLine.Length;
            StreamWriter sw = new StreamWriter(dataFullPath, false);
            for (int i = 0; i < readNum; i++)
            {
                sw.WriteLine(dataLine[i]);
            }
            sw.Close();

            dataFileNameTbx.Text = "";

            dataPathTxtbox.Text = dataFullPath;


            dataFilePath = dataPathTxtbox.Text;
            dataFileName = dataFilePath.Substring(namesFilePath.LastIndexOf('\\') + 1);

            dataOption.Visibility = Visibility.Collapsed;

        }

        private void MakeCfgOkBtnClick(object sender, RoutedEventArgs e)
        {

            string[] cfgTinyYolo =
                {
                "[net]",
                "# Testing",
                "#batch=1",
                "#subdivisions=1",
                "# Training",
                "batch=64",
                "subdivisions=2",
                "width=416",
                "height=416",
                "channels=3",
                "momentum=0.9",
                "decay=0.0005",
                "angle=0",
                "saturation = 1.5",
                "exposure = 1.5",
                "hue=.1",
                "",
                "learning_rate=0.001",
                "burn_in=1000",
                "max_batches = 500200",
                "policy=steps",
                "steps = 400000,450000",
                "scales=.1,.1",
                "",
                "[convolutional]",
                 "batch_normalize=1",
                "filters=16",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=2",
                "",
                "[convolutional]",
                "batch_normalize=1",
                "filters=32",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=2",
                "",
                "[convolutional]",
                 "batch_normalize=1",
                "filters=64",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=2",
                "",
                "[convolutional]",
                 "batch_normalize=1",
                "filters=128",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=2",
                "",
                "[convolutional]",
                 "batch_normalize=1",
                "filters=256",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=2",
                "",
                "[convolutional]",
                "batch_normalize=1",
                "filters=512",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[maxpool]",
                "size = 2",
                "stride=1",
                "",
                "[convolutional]",
                 "batch_normalize=1",
                "filters=1024",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "###########",
                "",
                "[convolutional]",
                "batch_normalize = 1",
                "filters=256",
                "size=1",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[convolutional]",
                "batch_normalize = 1",
                "filters=512",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[convolutional]",
                "size = 1",
                "stride=1",
                "pad=1",
                "filters=255",
                "activation=linear",
                "",
                "",
                "",
                "[yolo]",
                "mask = 3,4,5",
                "anchors = 10,14,  23,27,  37,58,  81,82,  135,169,  344,319",
                "classes=80",
                "num=6",
                "jitter=.3",
                "ignore_thresh = .7",
                "truth_thresh = 1",
                "random=1",
                "",
                "[route]",
                "layers = -4",
                "",
                "[convolutional]",
                "batch_normalize=1",
                "filters=128",
                "size=1",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[upsample]",
                "stride = 2",
                "",
                "[route]",
                "layers = -1, 8",
                "",
                "[convolutional]",
                "batch_normalize=1",
                "filters=256",
                "size=3",
                "stride=1",
                "pad=1",
                "activation=leaky",
                "",
                "[convolutional]",
                "size = 1",
                "stride=1",
                "pad=1",
                "filters=255",
                "activation=linear",
                "",
                "[yolo]",
                "mask = 0,1,2",
                "anchors = 10,14,  23,27,  37,58,  81,82,  135,169,  344,319",
                "classes=80",
                "num=6",
                "jitter=.3",
                "ignore_thresh = .7",
                "truth_thresh = 1",
                "random=1"
            };

            cfgTinyYolo[5] = "batch=" + batchTbx.Text;
            cfgTinyYolo[6] = "subdivisions=" + subdivisionTbx.Text;
            cfgTinyYolo[7] = "width=" + witdhTbx.Text;
            cfgTinyYolo[8] = "height=" + heighTbx.Text;
            //cfgTinyYolo[9] = "channels= 3";
            cfgTinyYolo[126] = "filters=" + ((classCount + 5) * 3);
            cfgTinyYolo[134] = "classes=" + classCount;
            cfgTinyYolo[170] = "filters=" + ((classCount + 5) * 3);
            cfgTinyYolo[176] = "classes=" + classCount;

            string cfgFullFath = darknetFolderPath + darknetDataFolderMiddlePath
                + cfgFileNameTbx.Text + ".cfg";

            int readNum = cfgTinyYolo.Length;
            StreamWriter sw = new StreamWriter(cfgFullFath, false);
            for (int i = 0; i < readNum; i++)
            {
                sw.WriteLine(cfgTinyYolo[i]);
            }
            sw.Close();
            cfgPathTxtbox.Text = cfgFullFath;
            cfgFilePath = cfgPathTxtbox.Text;
            cfgFileName = cfgFilePath.Substring(cfgFilePath.LastIndexOf('\\') + 1);

            cfgOption.Visibility = Visibility.Collapsed;

        }

        private void DarknetPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            trainingPathBtn.IsEnabled = true;
            namesMakeBtn.IsEnabled = true;

            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }


        }

        private void NamesPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (trainPathTxtbox.Text != "" && namesPathTxtbox.Text != "")
            {
                makeDataOkBtn.IsEnabled = true;
                dataMakeBtn.IsEnabled = true;
            }
            cfgMakeBtn.IsEnabled = true;


            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }


        }

        private void TrainPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (trainPathTxtbox.Text != "" && namesPathTxtbox.Text != "")
            {
                makeDataOkBtn.IsEnabled = true;
                dataMakeBtn.IsEnabled = true;
            }
            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }
        }

        private void ConvPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }
        }

        private void CfgPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }
        }

        private void DataPathTxtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //train start enabled
            if (darknetPathTxtbox.Text != "" && trainPathTxtbox.Text != "" && namesPathTxtbox.Text != ""
                && dataPathTxtbox.Text != "" && cfgPathTxtbox.Text != "" && convPathTxtbox.Text != "")
            {
                trainingStartBtn.IsEnabled = true;
            }
        }
        
        private void TrainingStartBtnclick(object sender, RoutedEventArgs e)
        {
            string darknetExe = "";

            //No Gpu 체크확인 후 경로설정
            if (gpuChk.IsChecked == true)
            {
                darknetExe = "darknet_no_gpu.exe";
            }
            else
            {
                darknetExe = "darknet";
            }



            // 다크넷 읽기 띄어쓰기 없음
            startCode = darknetExe + " detector train "
                + dataFilePath.Substring(dataFilePath.IndexOf(':') + 1) + " "
                + cfgFilePath.Substring(cfgFilePath.IndexOf(':') + 1) + " "
                + convFilePath.Substring(convFilePath.IndexOf(':') + 1);
            


            ////cmd 비동기 출력
            Thread thread1 = new Thread(threadmethod);
            thread1.Start();

        }
        
        //비동기 끝나고 터짐
        public void threadmethod()
        {
            Process process;
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";

            process.StartInfo.Arguments = "/c " + drive + ": && cd " + darknetFolderPath + "/build/darknet/x64 && " + startCode;


            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardError = false;

            
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.WaitForExit();

            process.Close();
        }
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            SetTextBoxText(outLine.Data.ToString());
            Thread.Sleep(100);
        }


        private delegate void SetTextBoxTextInvoker(string text);
        private void SetTextBoxText(string text)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { TestBox.Text += text + "\n"; }));
         
        }
        

    }
}
