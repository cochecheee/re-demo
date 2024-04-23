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
            try
            {
                //lấy thông tin thông qua lbl_username
                string username = lbl_username.Text;
                FirebaseResponse response = client.Get("usersclb/" + username);
                var tv = response.ResultAs<thanhvien>();

                string decUsername = _3des.Giaima3DES(tv.Username, Key.Key1, Key.Key2, Key.Key3);
                string decPhone = _3des.Giaima3DES(tv.Phone, Key.Key1, Key.Key2, Key.Key3);
                string decAddress = _3des.Giaima3DES(tv.Address, Key.Key1, Key.Key2, Key.Key3);
                string decClb = _3des.Giaima3DES(tv.Clb, Key.Key1, Key.Key2, Key.Key3);
                string decGender = _3des.Giaima3DES(tv.Gender, Key.Key1, Key.Key2, Key.Key3);

                txt_address.Text = decAddress;
                txt_gender.Text = decGender;
                txt_phone.Text = decPhone;
                txt_club.Text = decClb;
                txt_username.Text = decUsername;
            }
            catch {
                MessageBox.Show("Sai ở btn view ne..");
            }

        }
        private void layKhoaTuFireBase()
        {
            FirebaseResponse response = client.Get("key/");
            Key = response.ResultAs<key>();
        }
    }
}
