using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        Timer timer = new Timer();

        protected override void OnStart(string[] args)
        {
            //Dosyamıza servis başlangıç zamanını yazdırıyoruz ve timer ayarlarını //yapıyoruz.
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        private void OnElapsedTime(Object source, ElapsedEventArgs e) 
        {
            WriteToFile("Service is recall at " + DateTime.Now);
        }


        private double Dolar = 0.0;
        private DataSet dsDovizKur;

        string ht;
        private void WriteToFile(string Message)
        {

            dsDovizKur = new DataSet();
            dsDovizKur.ReadXml(@"http://www.tcmb.gov.tr/kurlar/today.xml");
            DataRow dr = dsDovizKur.Tables[1].Rows[0];
            Dolar = Convert.ToDouble(dr[4].ToString().Replace('.', ','));

       
            try
            {
                SqlConnection baglanti = new SqlConnection("Server=.;Database=EfDb;User Id=sa;Password=060792;");
                SqlCommand komut = new SqlCommand("insert into tbl (t1)values (\'" + Convert.ToString(Dolar).Replace(',', '.') + "\')", baglanti);
                baglanti.Open();
                komut.ExecuteNonQuery();

                baglanti.Close();
            }
            catch (Exception e)
            {
                ht = e.Message;

            }





            string path = @"c:\yeni\Log";
        
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = @"c:\yeni\ss.txt";
           
            if (!File.Exists(filepath))
            {
            
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Convert.ToString(Dolar));
                    sw.WriteLine(ht);
                }

            }
            else
            {
           
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Convert.ToString(Dolar));
                    sw.WriteLine(ht);
                }
            }
        }
    }
}
