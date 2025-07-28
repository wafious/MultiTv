using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MultiTv
{
    public partial class SelectURL : Form
    {
        private string _url = string.Empty;
        public SelectURL()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        public string URL
        {
            set
            {
                _url = value;
            }
            get
            {
                return _url;
            }
        }

        private string LinkExctractor(string url)
        {
            ////////////////////
            string result = string.Empty;
            Uri uri = new Uri(url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322)";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";


            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                result = reader.ReadToEnd();

            }

            StringBuilder builder = new StringBuilder();

            foreach (var c in result)
            {
                builder.Append(c);
                if (c == '>')
                {
                    builder.Append('\r');
                }
            }

            foreach (string line in builder.ToString().Split('\r'))
            {

                if (line.Contains("flv_url"))
                {

                    int start = line.LastIndexOf("value=") + "value=".Length + 1;

                    int end = line.IndexOf(".flv") - start;

                    url = line.Substring(start, end) + ".flv";

                }

            }

            ///////////////////

            return url;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            URL = LinkExctractor(txtURL.Text.Trim());

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
