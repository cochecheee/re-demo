using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo_des
{
    public static class _3des
    {
        public static string Mahoa3DES(string plain, string k1, string k2, string k3)
        {
            plain = Expandplain(plain);

            string[] mh1 = Encrypt_all(plain, k1);
            string[] mh2 = Decrypt_all(mh1, k2);
            string mahoa2 = ToString(mh2);
            string[] mh3 = Encrypt_all(mahoa2, k3);

            return Binary_Hex(ToString(mh3));
        }

        public static string Giaima3DES(string cipher, string k1, string k2, string k3)
        {
            string[] arr = ToArrayString(Hex_Binary(cipher));
            string[] gm1 = Decrypt_all(arr, k3);
            string giaima1 = ToString(gm1);
            string[] gm2 = Encrypt_all(giaima1, k2);
            string[] gm3 = Decrypt_all(gm2, k1);
            string giaima3 = ConvertBinary_string(gm3);

            return giaima3;
        }

        #region Ham convert string thanh []string
        public static string[] ToArrayString(string s)
        {
            int n = s.Length / 64;
            string[] arr = new string[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = s.Substring(i * 64, 64);
            }

            return arr;
        }
        #endregion

        #region Convert Hex_Binary
        public static string Hex_Binary(string s)
        {
            StringBuilder binary = new StringBuilder();
            string tra = "0123456789ABCDEF";
            int n = s.Length;
            for (int i = 0; i < n; i++)
            {
                binary.Append(ConvertDec_Binary(tra.IndexOf(s[i])));
            }
            return binary.ToString();
        }
        #endregion

        #region Convert Binary_Hex
        public static string Binary_Hex(string s)
        {
            string tra = "0123456789ABCDEF";
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < s.Length; i += 4)
            {
                string needed = s.Substring(i, 4);
                int t = 8 * ((int)needed[0] - 48) + 4 * ((int)needed[1] - 48) + 2 * ((int)needed[2] - 48) + ((int)needed[3] - 48);
                hex.Append(tra[t]);
            }

            return hex.ToString();
        }
        #endregion

        #region Ham Convert []String thành string
        public static string ToString(string[] S)
        {
            string s = "";
            for (int i = 0; i < S.Length; i++)
            {
                s += S[i];
            }
            return s;
        }
        #endregion

        #region Ham Ma Hoa DES (input X la 64bit dang nhi phan)
        public static string Encrypt_DES(string X, string[] K)
        {
            X = IP(X);

            string L, R;
            L = X.Substring(0, 32);
            R = X.Substring(32, 32);


            for (int i = 0; i < 16; i++)
            {
                string F = Fiestel(R, K[i]);

                string L1 = R;
                string R1 = XOR(L, F);

                L = L1;
                R = R1;

            }

            return Invert_IP(L + R);


        }
        #endregion

        #region Ham mo rong chuoi plainText
        public static string Expandplain(string s)
        {
            int n = s.Length;
            if (n % 8 != 0)
            {
                n = n + 8 - n % 8;
                s = s.PadRight(n, '£');
            }

            s = ConvertString_Binary(s);
            return s;
        }
        #endregion

        #region Ham Ma hoa (input X tuy chon)
        public static string[] Encrypt_all(string s, string k)
        {
            string[] K = Keys(ConvertString_Binary(k));
            int n = s.Length;

            int i = 0;
            n = s.Length / 64;
            string[] X = new string[n];

            while (i < n)
            {
                X[i] = s.Substring(i * 64, 64);
                X[i] = Encrypt_DES(X[i], K);
                i++;
            }

            return X;
        }
        #endregion

        #region Ham Giai ma DES (input X la 64 bit dang nhi phan)//
        public static string Decrypt_DES(string X, string[] K)
        {
            X = IP(X);

            string L, R;
            L = X.Substring(0, 32);
            R = X.Substring(32, 32);

            for (int i = 0; i < 16; i++)
            {
                string F = Fiestel(L, K[15 - i]);

                string R1 = L;
                string L1 = XOR(R, F);

                L = L1;
                R = R1;
            }

            return Invert_IP(L + R);
        }
        #endregion

        #region Ham Giai ma all
        public static string[] Decrypt_all(string[] s, string k)
        {
            string[] K = Keys(ConvertString_Binary(k));

            string[] X = new string[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                X[i] = Decrypt_DES(s[i], K);
            }

            return X;
        }
        #endregion

        #region Ham doi Nhiphan sang ASCii
        public static string ConvertBinary_string(string[] binary)
        {
            StringBuilder str = new StringBuilder();

            for (int c = 0; c < binary.Length; c++)
            {
                string s = binary[c];
                for (int i = 0; i < 8; i++)
                {
                    string sub = s.Substring(i * 8, 8);
                    str.Append((char)Convert.ToInt16(sub, 2));
                }
            }


            return Chuanhoa(str);
        }
        #endregion

        #region ham chuan hoa
        public static string Chuanhoa(StringBuilder st)
        {
            string s = st.ToString();

            while (s.IndexOf("£") >= 0)
            {
                s = s.Remove(s.IndexOf("£"));
            }
            return s;
        }
        #endregion

        #region Ham doi chuoi sang Nhiphan
        public static string ConvertString_Binary(string s)
        {
            StringBuilder str = new StringBuilder();

            foreach (char c in s)
            {
                str.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }

            return str.ToString();
        }
        #endregion

        #region Ham sinh hoan vi IP 
        public static string IP(string X)
        {
            int[] P = { 58, 50, 42, 34, 26, 18, 10, 2,
                       60, 52, 44, 36, 28, 20, 12, 4,
                       62, 54, 46, 38, 30, 22, 14, 6,
                       64, 56, 48, 40, 32, 24, 16, 8,
                       57, 49, 41, 33, 25, 17, 9, 1,
                       59, 51, 43, 35, 27, 19, 11, 3,
                       61, 53, 45, 37, 29, 21, 13, 5,
                       63, 55, 47, 39, 31, 23, 15, 7 };

            int n = X.Length;

            char[] result = new char[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = X[P[i] - 1];
            }

            return new string(result);
        }
        #endregion

        #region Ham tim hoan vi nguoc IP_1 
        public static string Invert_IP(string X)
        {
            int[] P_invert = { 40, 8, 48, 16, 56, 24, 64, 32,
                               39, 7, 47, 15, 55, 23, 63, 31,
                               38, 6, 46, 14, 54, 22, 62, 30,
                               37, 5, 45, 13, 53, 21, 61, 29,
                               36, 4, 44, 12, 52, 20, 60, 28,
                               35, 3, 43, 11, 51, 19, 59, 27,
                               34, 2, 42, 10, 50, 18, 58, 26,
                               33, 1, 41, 9, 49, 17, 57, 25 };

            int n = X.Length;

            char[] result = new char[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = X[P_invert[i] - 1];
            }

            return new string(result);

        }
        #endregion

        #region Ham hoan vi mo rong E
        public static string Expand_E(string R)
        {
            //Phan mo rong bit
            int[] E = { 32, 1, 2, 3, 4, 5,
                        4, 5, 6, 7, 8, 9,
                        8, 9, 10, 11, 12, 13,
                        12, 13, 14, 15, 16, 17,
                        16, 17, 18, 19, 20, 21,
                        20, 21, 22, 23, 24, 25,
                        24, 25, 26, 27, 28, 29,
                        28, 29, 30, 31, 32, 1 };

            char[] Result = new char[48];

            for (int i = 0; i < 48; i++)
            {
                Result[i] = R[E[i] - 1];
            }
            return new string(Result);
        }
        #endregion

        #region Ham sinh hoan vi P trong ham F
        public static string P(string s)
        {
            int[] P = { 16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25 };
            char[] result = new char[P.Length];

            for (int i = 0; i < P.Length; i++)
            {
                result[i] = s[P[i] - 1];
            }
            return new string(result);
        }
        #endregion

        #region Phep dich bit LS
        public static string LS(string s, int k)
        {
            string str = "";

            if (k == 1)
            {
                for (int i = s.Length - 1; i >= 1; i--)
                {
                    str = s[i] + str;
                }
                str += s[0];
            }
            else
            {
                for (int i = s.Length - 1; i >= 2; i--)
                {
                    str = s[i] + str;
                }
                str += s[0];
                str += s[1];
            }

            return str;
        }
        #endregion

        #region Normalize K size
        public static string Normalize_L(string s)
        {
            return s.PadLeft(64, '0');
        }
        #endregion

        #region Ham sinh khoa 
        public static string[] Keys(string K)
        {
            K = Normalize_L(K);
            K = PC1(K);
            string[] Keys = new string[16];

            string C = K.Substring(0, 28);
            string D = K.Substring(28, 28);

            for (int i = 0; i < 16; i++)
            {
                if (i == 0 || i == 1 || i == 8 || i == 15) //Neu i bat dau tu 1. Thi i==1,2,9,16
                {
                    C = LS(C, 1);
                    D = LS(D, 1);
                }
                else
                {
                    C = LS(C, 2);
                    D = LS(D, 2);
                }
                //Thuc hien hoan vi PC2
                Keys[i] = PC2(C + D);
            }

            return Keys; // Co tat ca la 16 Keys
        }
        #endregion

        #region Ham hoan vi PC1 
        public static string PC1(string K)
        {
            int[] PC = { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };

            char[] c = new char[56];

            for (int i = 0; i < 56; i++)
            {
                c[i] = K[PC[i] - 1];
            }

            return new string(c);
        }
        #endregion

        #region Ham hoan vi PC2 
        public static string PC2(string K)
        {
            int[] PC2 = { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };

            char[] c = new char[48];

            for (int i = 0; i < 48; i++)
            {
                c[i] = K[PC2[i] - 1];
            }

            return new string(c);
        }
        #endregion

        #region Phep XOR
        public static string XOR(string A, string J)
        {
            int n = A.Length;
            string s = "";
            for (int i = 0; i < n; i++)
            {
                int t = (int)A[i] ^ (int)J[i];
                s += t.ToString();
            }

            return s;
        }
        #endregion

        #region Ham hop the vi
        public static string HopTheVi_S_box(string B, int[,] S)
        {
            //string result = "";

            int row = ((int)B[0] - 48) * 2 + ((int)B[5] - 48);
            int col = ((int)B[1] - 48) * 8 + ((int)B[2] - 48) * 4 + ((int)B[3] - 48) * 2 + ((int)B[4] - 48);

            //Doi sang chuoi nhi phan 4 bit
            return ConvertDec_Binary(S[row, col]);
        }
        #endregion

        #region Ham Convert Decimal_Binary chuoi 4 bit
        public static string ConvertDec_Binary(int Dec)
        {
            string s = "";

            while (Dec > 0)
            {
                int t = Dec % 2;
                s = t.ToString() + s;
                Dec /= 2;
            }

            s = s.PadLeft(4, '0');

            return s;
        }
        #endregion

        #region Ham F tra ve 32bit
        public static string Fiestel(string R, string K)
        {
            int[,] S1 = {
                            { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                            { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                            { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                            { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
                        };

            int[,] S2 = {
                            { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                            { 3, 13, 4, 13, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                            { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                            { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
                        };

            int[,] S3 = {
                            { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                            { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                            { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                            { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
                        };

            int[,] S4 = {
                            { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                            { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                            { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                            { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
                        };

            int[,] S5 = {
                            { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                            { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                            { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                            { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
                        };

            int[,] S6 = {
                            { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 15, 11 },
                            { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                            { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                            { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
                        };

            int[,] S7 = {
                            { 4, 11, 12, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                            { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                            { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                            { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
                        };

            int[,] S8 = {
                            { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                            { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                            { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                            { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
                        };

            string E = Expand_E(R); //Ham mo rong E. E luc nay co 48bit

            string result = XOR(E, K);

            string[] B = new string[8];
            string[] C = new string[8];

            for (int i = 1; i < 9; i++)
            {
                B[i - 1] = result.Substring((i - 1) * 6, 6);
            }

            C[0] = HopTheVi_S_box(B[0], S1); //C nay se chua 4 bit
            C[1] = HopTheVi_S_box(B[1], S2);
            C[2] = HopTheVi_S_box(B[2], S3);
            C[3] = HopTheVi_S_box(B[3], S4);
            C[4] = HopTheVi_S_box(B[4], S5);
            C[5] = HopTheVi_S_box(B[5], S6);
            C[6] = HopTheVi_S_box(B[6], S7);
            C[7] = HopTheVi_S_box(B[7], S8);

            result = "";
            for (int i = 0; i < 8; i++)
            {
                result += C[i];
            }

            result = P(result);

            return result;
        }
        #endregion
    }
}
