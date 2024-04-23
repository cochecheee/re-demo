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
using System.Xml.Serialization;
using static System.Windows.Forms.DataFormats;

namespace demo_des
{
    public partial class TrangChu : Form
    {
        public TrangChu()
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
        private void btn_login_Click(object sender, EventArgs e)
        {
            //kiểm tra input đầu vào
            if (string.IsNullOrEmpty(txt_login_username.Text) || string.IsNullOrEmpty(txt_login_password.Text))
            {
                MessageBox.Show("Nhập đầy đủ thông tin để đăng nhập..");
                return;
            }
            else
            {
                //giải mã trong chuổi
                FirebaseResponse response = client.Get("usersclb/");
                Dictionary<string, thanhvien> result = response.ResultAs<Dictionary<string, thanhvien>>();
                bool check = false;
                foreach (var u in result)
                {
                    string username = u.Value.Username;
                    string password = u.Value.Password;

                    string decUsername = _3des.Giaima3DES(username, Key.Key1, Key.Key2, Key.Key3);
                    string decPassword = _3des.Giaima3DES(password, Key.Key1, Key.Key2, Key.Key3);

                    if (txt_login_username.Text == decUsername && txt_login_password.Text == decPassword) //them pass
                    {
                        //Form1 f1 = new Form1();
                        //f1.ShowDialog();

                        //mở form thông tin lên
                        MessageBox.Show("Đăng nhập thành công...");
                        check = true;
                        ThongTin thongtin = new ThongTin();
                        thongtin.lbl_username.Text = txt_login_username.Text; //update username
                        thongtin.ShowDialog();
                        this.Hide();
                    }
                    
                }
                if(!check)
                {
                    MessageBox.Show("Không tồn tại tài khoản vui lòng đăng kí...");
                }
            }

        }
        //khởi tạo firebase
        private void TrangChu_Load(object sender, EventArgs e)
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
        private void layKhoaTuFireBase()
        {
            FirebaseResponse response = client.Get("key/");
            Key = response.ResultAs<key>();
        }
        private void btn_signup_Click(object sender, EventArgs e)
        {
            //kiểm tra input đầu vào
            if (string.IsNullOrEmpty(txt_signup_username.Text) || string.IsNullOrEmpty(txt_signup_password.Text)
                || string.IsNullOrEmpty(txt_signup_phone.Text) || string.IsNullOrEmpty(cb_club.Text)
                || string.IsNullOrEmpty(txt_signup_address.Text) ||
                (!rbtn_female.Checked && !rbtn_gender_khac.Checked && !rbtn_male.Checked))
            {
                MessageBox.Show("Điền đẩy đủ thông tin cá nhân để đăng nhập..");
                return;
            }
            else
            {
                // cập nhật dữ liệu lên firebase
                string username = txt_signup_username.Text;
                string password = txt_signup_password.Text;
                string phone = txt_signup_phone.Text;
                string address = txt_signup_address.Text;
                string clb = cb_club.Text;
                string gender = "";
                if (rbtn_female.Checked)
                {
                    gender = "Female";
                }
                else if (rbtn_male.Checked)
                {
                    gender = "Male";
                }
                else
                {
                    gender = "Other";
                }
                try
                {
                    //ma hóa 3des dữ liệu rồi cập nhật lên firebase
                    string encUsername = _3des.Mahoa3DES(username, Key.Key1, Key.Key2, Key.Key3);
                    string encPassword = _3des.Mahoa3DES(password, Key.Key1, Key.Key2, Key.Key3);
                    string encPhone = _3des.Mahoa3DES(phone, Key.Key1, Key.Key2, Key.Key3);
                    string encAddress = _3des.Mahoa3DES(address, Key.Key1, Key.Key2, Key.Key3);
                    string encClb = _3des.Mahoa3DES(clb, Key.Key1, Key.Key2, Key.Key3);
                    string encGender = _3des.Mahoa3DES(gender, Key.Key1, Key.Key2, Key.Key3);

                    //tạo 1 user mới
                    var newUser = new thanhvien()
                    {
                        Username = encUsername,
                        Password = encPassword,
                        Phone = encPhone,
                        Address = encAddress,
                        Gender = encGender,
                        Clb = encClb,
                    };
                    //cập nhật lên firebase
                    FirebaseResponse response = client.Set("usersclb/" + txt_signup_username.Text, newUser);
                    thanhvien result = response.ResultAs<thanhvien>();
                    MessageBox.Show("Update thành công");
                }
                catch
                {
                    MessageBox.Show("Error from line 114 -> 140");
                }
            }
        }
    }
}
