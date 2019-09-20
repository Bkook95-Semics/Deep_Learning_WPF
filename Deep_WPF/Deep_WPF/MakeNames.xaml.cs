using System;
using System.Collections.Generic;
using System.IO;
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

namespace Deep_WPF
{
    /// <summary>
    /// MakeNames.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MakeNames : Window
    {
        int CIdxNum = 1;

        public delegate void TextEventHandler(string tcc, string tpt, string tfp, string tfn);    // string을 반환값으로 같는 대리자를 선언합니다.
        public event TextEventHandler WriteTextEvent;          // 대리자 타입의 이벤트 처리기를 설정합니다.


        public string tdarknetFolderPath
        {
            get;
            set;
        }

        public string tdarknetDataFolderMiddlePath
        {
            get;
            set;
        }
        public string tnamesFilePath
        {
            get;
            set;
        }

        public int tclassCount
        {
            get;
            set;
        }
        public string tnamesPathTxtbox
        {
            get;
            set;
        }
        public string tnamesFileName
        {
            get;
            set;
        }


        public MakeNames()
        {
            InitializeComponent();
            List<NamesContentData> items = new List<NamesContentData>();
        }

        public class NamesContentData
        {
            public int CIdx { get; set; }
            public string CName { get; set; }
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void addBtnClick(object sender, RoutedEventArgs e)
        {
            namesContent_lv.Items.Add(new NamesContentData() { CIdx = CIdxNum, CName = classNametoAddTbx.Text });
            CIdxNum++;
            //namesFileNameTbx.Text = "";
            classNametoAddTbx.Text = "";
        }

        private void removeBtnClick(object sender, RoutedEventArgs e)
        {
            CIdxNum--;
            namesContent_lv.Items.RemoveAt(CIdxNum - 1);
        }

        private void okBtnClick(object sender, RoutedEventArgs e)
        {
            // names 만들기
            //Train tr = (Train)this.Owner;
            //((Training)System.Windows.Application.Current.MainWindow).;
            string namesFullPath = tdarknetFolderPath + tdarknetDataFolderMiddlePath + namesFileNameTbx.Text;



            StreamWriter sw = new StreamWriter(namesFullPath + ".names", false, Encoding.UTF8);

            for (int i = 0; i < namesContent_lv.Items.Count; i++)
            {
                NamesContentData temp = namesContent_lv.Items[i] as NamesContentData;
                sw.WriteLine(temp.CName);
            }
            sw.Close();
            tclassCount = namesContent_lv.Items.Count;
            // 부모 보내줄거 


            tnamesPathTxtbox = namesFullPath + ".names";
            //보내줄거


            tnamesFilePath = tnamesPathTxtbox;
            //보내줄거 = 받아올거..
            tnamesFileName = tnamesFilePath.Substring(tnamesFilePath.LastIndexOf('\\') + 1);
            //보내줄거 = 받아올거

            //tr.Focus(); // 걸어줘야 부모창 안꺼짐


            WriteTextEvent(tclassCount.ToString(), tnamesPathTxtbox, tnamesFilePath, tnamesFileName);
            //tclassCount, tnamesPathTxtbox, tnamesFilePath, tnamesFileName

            this.Close();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //   WriteTextEvent(textBox1.Text);                                // 스트링값을 구독자(Form1을 지칭)에게 날려줍니다.
            this.Close();
        }





        // 델리게이트 사용전
        //private void okBtnClick(object sender, RoutedEventArgs e)
        //{
        //    // names 만들기
        //    //Train tr = (Train)this.Owner;
        //    //((Training)System.Windows.Application.Current.MainWindow).;
        //    string namesFullPath = train.darknetFolderPath + train.darknetDataFolderMiddlePath + namesFileNameTbx.Text;



        //    StreamWriter sw = new StreamWriter(namesFullPath + ".names", false, Encoding.UTF8);

        //    for (int i = 0; i < namesContent_lv.Items.Count; i++)
        //    {
        //        NamesContentData temp = namesContent_lv.Items[i] as NamesContentData;
        //        sw.WriteLine(temp.CName);
        //    }
        //    sw.Close();
        //    train.classCount = namesContent_lv.Items.Count;


        //    train.namesPathTxtbox.Text = namesFullPath + ".names";


        //    train.namesFilePath = train.namesPathTxtbox.Text;
        //    train.namesFileName = train.namesFilePath.Substring(train.namesFilePath.LastIndexOf('\\') + 1);

        //    //tr.Focus(); // 걸어줘야 부모창 안꺼짐
        //    this.Close();
        //}
    }
}

