using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace demo_des
{
    public class des
    {
        #region mainprocess
        //lấy bit thứ ith trong 32 bit
        public ulong getbit(ulong K1, int i)
        {
            // Dịch phải K1 đi (32 - i) bit
            ulong b = K1 >> (32 - i);
            // Lấy bit thứ i từ trái sang phải
            ulong bit = b & 0x01;
            return bit;
        }
        //lấy bit thứ ith trong 28 bit
        public ulong getbit28(ulong K1, int i)
        {
            ulong b = K1 >> (28 - i);
            ulong bit = b & 0x01;
            return bit;
        }
        //tính hoán vị PC1 khi nhận được khóa
        public ulong PC1CD(ulong K1, ulong K2, int chiso1, int chiso2)
        {
            int[] pc1 = { 57, 49, 41, 33, 25, 17, 9, 1,
                58, 50, 42, 34, 26, 18, 10, 2, 59, 51,
                43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
                63, 55, 47, 39, 31, 23, 15, 7, 62, 54,
                46, 38, 30, 22, 14, 6, 61, 53, 45, 37,
                29, 21, 13, 5, 28, 20, 12, 4 };
            ulong pc1k1 = 0;

            for (int i = chiso1; i < chiso2; i++)
            {
                int vitri; ;
                ulong bit;

                if (pc1[i] > 32)
                {
                    vitri = pc1[i] - 32;
                    bit = getbit(K2, vitri);
                }
                else
                {
                    vitri = pc1[i];
                    bit = getbit(K1, vitri);
                }

                pc1k1 = (pc1k1 << 1) | bit;
            }

            return pc1k1;
        }
        //dịch vòng trái n-bit
        public ulong ShiftLeft(ulong C0, int s)
        {
            ulong C1;
            ulong sbit, bit28s;
            sbit = (C0 >> (28 - s)); //đề lấy s-bit đầu tiên
            bit28s = (C0 << s) & 0xFFFFFFF; //dịch trái s-bit và lấy 28bit

            C1 = bit28s | sbit; //bit28s + sbit

            return C1;
        }
        //thực hiện hoán vị PC2 sinh kháo Ki
        public ulong KPC2(ulong K1, ulong K2, int chiso1, int chiso2) //tra ve chuoi khoa 48 bit
        {
            int[] pc2 = {
                14, 17, 11, 24, 1, 5, 3,
                28, 15, 6, 21, 10, 23, 19,
                12, 4, 26, 8, 16, 7, 27,
                20, 13, 2, 41, 52, 31, 37,
                47, 55, 30, 40, 51, 45,
                33, 48, 44, 49, 39, 56,
                34, 53, 46, 42, 50, 36,
                29, 32 }; //48 bits
            ulong pc2k1 = 0;

            for (int i = chiso1; i < chiso2; i++)
            {
                int vitri; ;
                ulong bit;

                if (pc2[i] > 28)
                {
                    vitri = pc2[i] - 28;
                    bit = getbit28(K2, vitri);
                }
                else
                {
                    vitri = pc2[i];
                    bit = getbit28(K1, vitri);
                }

                pc2k1 = (pc2k1 << 1) | bit;
            }

            return pc2k1;
        }
        //hàm tạo 16 khóa
        public void GenKey(ulong K1, ulong K2, ulong[] key1, ulong[] key2)
        {
            /*
             *  Các bước sinh khóa:
             *  B1: Tính hoán vị PC1CD
             *  B2: Dịch vòng trái
             *  B3: Hoán vị PC2(CiDi)
             */
            ulong C0, D0;
            //Tính hoán vị PC1 khi nhận khóa
            C0 = PC1CD(K1, K2, 0, 28);
            D0 = PC1CD(K1, K2, 28, 56);
            int[] s = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

            //thực hiện 16 vòng sinh 16 khóa
            ulong C1, D1;
            for (int i = 0; i < 16; i++)
            {
                C1 = ShiftLeft(C0, s[i]);
                D1 = ShiftLeft(D0, s[i]);

                key1[i] = KPC2(C1, D1, 0, 24);
                key2[i] = KPC2(C1, D1, 24, 48);
                C0 = C1; D0 = D1;
            }
        }
        //hàm tính hoán vị IP
        public ulong IPM(ulong M1, ulong M2, int chiso1, int chiso2)
        {
            int[] IP = {
                58, 50, 42, 34, 26, 18, 10, 2,
                60, 52, 44, 36, 28, 20, 12, 4,
                62, 54, 46, 38, 30, 22, 14, 6,
                64, 56, 48, 40, 32, 24, 16, 8,
                57, 49, 41, 33, 25, 17, 9, 1,
                59, 51, 43, 35, 27, 19, 11, 3,
                61, 53, 45, 37, 29, 21, 13, 5,
                63, 55, 47, 39, 31, 23, 15, 7 };
            ulong ipm1 = 0;

            for (int i = chiso1; i < chiso2; i++)
            {
                int vitri;
                ulong bit;

                if (IP[i] > 32)
                {
                    vitri = IP[i] - 32;
                    bit = getbit(M2, vitri);
                }
                else
                {
                    vitri = IP[i];
                    bit = getbit(M1, vitri);
                }

                // Lưu bit vào biến ipm1
                ipm1 = (ipm1 << 1) | bit;
            }
            return ipm1;
        }
        //hàm mở rộng E trả về 48 bit từ 32 bit
        public ulong ER0(ulong R0, int chiso1, int chiso2)
        {
            int[] E = {
                32, 1, 2, 3, 4, 5, 4, 5,
                6, 7, 8, 9, 8, 9, 10, 11,
                12, 13, 12, 13, 14, 15, 16,
                17, 16, 17, 18, 19, 20, 21, 20,
                21, 22, 23, 24, 25, 24, 25,
                26, 27, 28, 29, 28, 29, 30, 31,
                32, 1 };
            ulong er1 = 0;

            for (int i = chiso1; i < chiso2; i++)
            {
                int vitri;
                ulong bit;

                vitri = E[i];
                bit = getbit(R0, vitri);

                ulong b = bit & 0x01;
                // Lưu bit vào biến ipm1
                er1 = (er1 << 1) | bit;
            }
            return er1;
        }
        //hèm tính thế S-box
        public ulong SubByte(ulong A1, ulong A2)
        {
            int[] S1 = {
                14, 4, 13, 1, 2, 15,
                11, 8, 3, 10, 6, 12,
                5, 9, 0, 7, 0, 15,
                7, 4, 14, 2, 13, 1,
                10, 6, 12, 11, 9, 5,
                3, 8, 4, 1, 14, 8,
                13, 6, 2, 11, 15, 12,
                9, 7, 3, 10, 5, 0,
                15, 12, 8, 2, 4, 9,
                1, 7, 5, 11, 3, 14,
                10, 0, 6, 13 };
            int[] S2 = { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10, 3, 13, 4,
                7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5, 0, 14, 7, 11, 10, 4, 13, 1, 5,
                8, 12, 6, 9, 3, 2, 15, 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 };
            int[] S3 = { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8, 13, 
                7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1, 13, 6, 4, 9, 8, 15,
                3, 0, 11, 1, 2, 12, 5, 10, 14, 7, 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 };
            int[] S4 = { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15, 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9, 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4, 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 };
            int[] S5 = { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9, 14, 11,
                2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6, 4, 2, 1, 11, 10, 13, 7, 8, 15, 9,
                12, 5, 6, 3, 0, 14, 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 };
            int[] S6 = { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11, 10, 15, 4, 2, 7, 
                12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8, 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13,
                11, 6, 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 };
            int[] S7 = { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1, 13, 
                0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6, 1, 4, 11, 13, 12, 3, 7,
                14, 10, 15, 6, 8, 0, 5, 9, 2, 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 };
            int[] S8 = { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7, 1, 15,
                13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2, 7, 11, 4, 1, 9, 12, 14,
                2, 0, 6, 10, 13, 15, 3, 5, 8, 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 };

            ulong B = 0;  //kết quả sau thế (32bit
            int[] chiso = new int[9];   //chi so tra trong bảng Sbox
            int[] S = new int[9];       //luu gia tri tra

            // Lấy 4 cặp 6 bit từ A1
            for (int i = 1; i <= 4; i++)
            {
                ulong b6i = (A1 >> (24 - 6 * i)) & 0x3F;
                int bit1 = (int)((b6i >> 5) & 0x01);
                int bit6 = (int)(b6i & 0x01);
                int hang = bit1 << 1 | bit6;
                int cot = (int)(b6i >> 1) & 0xF;

                chiso[i] = (hang << 4) | cot; // Chỉ số dùng để tra bảng S
            }

            // Lấy 4 cặp 6 bit từ A2
            for (int i = 5; i <= 8; i++)
            {
                ulong b6i = (A2 >> (24 - 6 * (i - 4))) & 0x3F;
                int bit1 = (int)((b6i >> 5) & 0x01);
                int bit6 = (int)(b6i & 0x01);
                int hang = bit1 << 1 | bit6;
                int cot = (int)(b6i >> 1) & 0xF;

                chiso[i] = (hang << 4) | cot;
            }

            // Tra bảng S và gắn các giá trị vào mảng S
            for (int i = 1; i <= 8; i++)
            {
                switch (i)
                {
                    case 1: S[i] = S1[chiso[i]]; break;
                    case 2: S[i] = S2[chiso[i]]; break;
                    case 3: S[i] = S3[chiso[i]]; break;
                    case 4: S[i] = S4[chiso[i]]; break;
                    case 5: S[i] = S5[chiso[i]]; break;
                    case 6: S[i] = S6[chiso[i]]; break;
                    case 7: S[i] = S7[chiso[i]]; break;
                    case 8: S[i] = S8[chiso[i]]; break;
                }
            }

            // Ghép 8 cặp 4 bit tạo thành B (32 bit)
            for (int i = 1; i <= 8; i++)
            {
                B = (B << 4) | (uint)S[i]; //chuyển s[i] sang hexa
            }

            return B;
        }
        //hàm tính hoán vị P
        public ulong HoanviP(ulong B)
        {
            int[] P = { 16, 7, 20, 21, 29, 12, 28,
                17, 1, 15, 23, 26, 5, 18, 31, 10,
                2, 8, 24, 14, 32, 27, 3, 9, 19,
                13, 30, 6, 22, 11, 4, 25 };
            ulong fp = 0;

            for (int i = 0; i < 32; i++)
            {
                int vitri = P[i];
                ulong bit = getbit(B, vitri);
                ulong b = bit & 0x01;
                fp = (fp << 1) | b;
            }

            return fp;
        }
        //một vòng lặp mã hóa & giải mã
        public ulong F(ulong L0, ulong R0, ulong key1, ulong key2)  //1 vòng lặp mã hóa, hàm Feistel F
        {
            // Mo rong nua phai R0
            ulong ER01, ER02;
            ER01 = ER0(R0, 0, 24);
            ER02 = ER0(R0, 24, 48);

            //phép XOR, tính A = E[R0] + K1.
            ulong A1, A2;
            A1 = key1 ^ ER01;
            A2 = key2 ^ ER02;
            // B = S1(A1) S2(A2) S3(A3) S4(A4) S5(A5) S6(A6) S7(A7) S8(A8)
            ulong B;
            B = SubByte(A1, A2);
            //Hoan vi P
            ulong FP;
            FP = HoanviP(B);
            return FP;

        }
        //hoán vị IP-1
        public ulong HoanviIP_1(ulong M1, ulong M2, int chiso1, int chiso2)
        {
            int[] IP1 = {
                40, 8, 48, 16, 56, 24, 64,
                32, 39, 7, 47, 15, 55, 23,
                63, 31, 38, 6, 46, 14, 54,
                22, 62, 30, 37, 5, 45, 13,
                53, 21, 61, 29, 36, 4, 44,
                12, 52, 20, 60, 28, 35, 3,
                43, 11, 51, 19, 59, 27, 34,
                2, 42, 10, 50, 18, 58, 26, 33,
                1, 41, 9, 49, 17, 57, 25 };
            ulong ipm1 = 0;
            for (int i = chiso1; i < chiso2; i++)
            {
                int vitri;
                ulong bit;
                if (IP1[i] > 32)
                {
                    vitri = IP1[i] - 32;
                    bit = getbit(M2, vitri);
                }
                else
                {
                    vitri = IP1[i];
                    bit = getbit(M1, vitri);

                }
                ulong b = bit & 0x01;
                ipm1 = (ipm1 << 1) | b;
            }
            return ipm1;
        }
        //hàm mã hóa DS
        public void MahoaDES(ulong M1, ulong M2, ulong K1, ulong K2, ref ulong C1, ref ulong C2)
        {
            ulong[] key1 = new ulong[16];
            ulong[] key2 = new ulong[16];
            GenKey(K1, K2, key1, key2);

            ulong L0, R0;
            L0 = IPM(M1, M2, 0, 32);
            R0 = IPM(M1, M2, 32, 64);
            ulong L1 = 0, R1 = 0;
            ulong FP;
            for (int i = 0; i < 16; i++)
            {
                FP = F(L0, R0, key1[i], key2[i]);
                R1 = FP ^ L0;
                L1 = R0;
                R0 = R1; L0 = L1;
            }

            C1 = HoanviIP_1(R1, L1, 0, 32);
            C2 = HoanviIP_1(R1, L1, 32, 64);

        }
        //hàm giải mã des
        public void GiaiMaDES(ulong M1, ulong M2, ulong K1, ulong K2, ref ulong C1, ref ulong C2)
        {
            ulong[] key1 = new ulong[16];
            ulong[] key2 = new ulong[16];
            GenKey(K1, K2, key1, key2);
            //Buoc 3. Thuc hien hoan vi IP
            ulong L0, R0;
            L0 = IPM(M1, M2, 0, 32);
            R0 = IPM(M1, M2, 32, 64);
            ulong L1 = 0, R1 = 0;
            ulong FP;
            for (int i = 0; i < 16; i++)
            {
                FP = F(L0, R0, key1[15 - i], key2[15 - i]);
                R1 = FP ^ L0;
                L1 = R0;
                R0 = R1;
                L0 = L1;
            }

            C1 = HoanviIP_1(R1, L1, 0, 32);
            C2 = HoanviIP_1(R1, L1, 32, 64);
        }
        //ma hóa chuỗi dài hơn 8 kí tự
        bool[] marked; //màng lưu vi6 trí dấu cách
        int n;
        public string mahoachuoi(string s, ulong K1, ulong K2)
        {

            string hexa = "";
            this.n = s.Length;
            this.marked = new bool[n]; //mang luu vị trí khoảng các
            for (int i = 0; i < n; i++)
            {
                if (s[i] == ' ')
                {
                    this.marked[i] = true;
                }
            }
            //chuỗi mới để mã hóa
            string newStr = "";
            for (int i = 0; i < n; i++)
            {
                if (s[i] != ' ') newStr += s[i];
            }
            //chuẩn hóa chuỗi về có thể chia hết cho 8
            while (newStr.Length % 8 != 0)
            {
                newStr += 'x';
            }

            for (int i = 0; i < newStr.Length; i += 8)
            {
                string t = newStr.Substring(i, 8);
                ulong M = ConvertStrToHexa(t);
                ulong M1 = chiathanh2nua(M, 0, 32);
                ulong M2 = chiathanh2nua(M, 32, 64);

                ulong C1 = 0, C2 = 0;
                MahoaDES(M1, M2, K1, K2, ref C1, ref C2);

                hexa += string.Format("{0:X16}", ghephainua(C1, C2));
            }
            return hexa;

        }
        public string giaimachuoi(string hexa, ulong K1, ulong K2)
        {
            string res = "";
            string temp = "";

            for (int i = 0; i < hexa.Length; i += 16)
            {
                string hex64 = hexa.Substring(i, 16);
                ulong hex64ulong = ulong.Parse(hex64, System.Globalization.NumberStyles.HexNumber);
                //chia thành 2 nửa
                ulong M1 = chiathanh2nua(hex64ulong, 0, 32);
                ulong M2 = chiathanh2nua(hex64ulong, 32, 64);

                ulong C1 = 0, C2 = 0;
                GiaiMaDES(M1, M2, K1, K2, ref C1, ref C2);

                string s = ConvertHexaToStr(ghephainua(C1, C2));

                temp += s;
            }
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                if (marked[i] == false)
                {
                    res += temp[index++];
                }
                else
                {
                    res += ' ';
                }
            }

            return res;
        }

        #endregion mainprocess

        #region convert
        public ulong chiathanh2nua(ulong a, int chiso1, int chiso2)
        {
            ulong res = 0;

            if (chiso1 == 0)
            {
                res = (a >> 32) & 0xFFFFFFFF;
            } else if (chiso1 == 32)
            {
                res = a & 0xFFFFFFFF;
            }

            return res;
        }
        public ulong ConvertStrToHexa(string str)
        {
            //chuyển chuỗi kí tự sang byte
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            //chuyển thành giá trị hex dưới dạng chuỗi
            string hex = BitConverter.ToString(bytes).Replace("-", "");
            //chuyền thành hexa val
            ulong result = Convert.ToUInt64(hex, 16);

            return result;
        } //đã test
        public string ConvertHexaToStr(ulong hex)
        {
            string hexString = hex.ToString("X");
            byte[] bytes = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return Encoding.UTF8.GetString(bytes);
        }
        public ulong ghephainua(ulong N1, ulong N2)
        {

            ulong result = 0;
            result = (N1 << 32) | N2;

            return result;
        } //ok
        #endregion convert
    }
}