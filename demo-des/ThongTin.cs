using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo_des
{
    public partial class ThongTin : Form
    {
        public ThongTin()
        {
            InitializeComponent();
        }
        //kết nối project với firebase
        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "C4n3cmu3NJVytNiNwmOt1lSiyknFm2GakZXJXsr5",
            BasePath = "https://fir-des-647b5-default-rtdb.firebaseio.com/",
        };
        IFirebaseClient client;
        key Key;

        private void ThongTin_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FirebaseClient(config);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối firebase..");
            }

            try
            {
                layKhoaTuFireBase();
            }
            catch
            {
                MessageBox.Show("Lỗi từ lấy khóa..");
            }
        }

        private void btn_view_Click(object sender, EventArgs e)
        {
            //lấy thông tin thông qua lbl_username
        }
        private void layKhoaTuFireBase()
        {
            FirebaseResponse response = client.Get("key/");
            Key = response.ResultAs<key>();
        }
    }
}
