using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace QuickStarts
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //Generate keys
        RSACryptoServiceProvider rsaGenKeys = new RSACryptoServiceProvider();
        string privateXml = "<RSAKeyValue><Modulus>yYMKx73kAaDEnCfeE5pYbfSLu6tI+bm3Haqn9DJiCiJXhiPGvb6RRRICj77p8il8nZyWpM7dhzkRoAWRkjLhymh/o4NB4K+NWwWsqHd6fWbyFMBDSUmNQsCt7pCo8Rlyh0qxkwoBqrzDbIrpdrQFZrxfTQEtOjGm/E85EO5FJi8=</Modulus><Exponent>AQAB</Exponent><P>7AKsdxAhKng+BtvDGBbQ+B/MFASa2LsrrgjXLD7qMPmOIJcHNJgPw3/BKYRDR+aH3eupL8djSQADuC2mPpWKNw==</P><Q>2pRZWKMvyj4BWkyIhUtvBKRPeuD215/Tqbz4EJqxwrWe3ztImk+4AEhmT/ueXNhT17Ez5Bx+Hc96LXt/G/znyQ==</Q><DP>C6keKEvNt7F1LxWQkBIghQHdLWgE1ox43gJlzzPUklLiKM4NdxXEQZ6ARrLYOCc1s0s/crLhPuwXQvROcG0nBQ==</DP><DQ>1os8M3Nhqio4W7C5Y9SJ8M1yshL0Vusq7/Vqq6mbh5mLimN/PO+4lJZ7zlAUvd8cVJ87ZtDMvWfz7YbFWexgeQ==</DQ><InverseQ>50ahdTangznlam9M4f5MKUWqnWPU62y02ANj25ISpPWQsUgeH2jIas/7ue4WUfy0Fb8YFf2K6Hw6ZEqx/OCpEw==</InverseQ><D>nbodWQSwD/onilbGQ6++4anVKbrDLvyHcQgf2EsLZAVHXq5oJCKikF6tdJgpg6unZ7KmUy+8Q9iOjCNvsvByu9jHO5DM2IGVV4SpOghXitKuU2upgEjAB/V4IvU1xt79DpKmOHhtJbRe8MvVSaAgRjevP26Kx55VmBPGW81KxjE=</D></RSAKeyValue>";
        string publicXml = "<RSAKeyValue><Modulus>yYMKx73kAaDEnCfeE5pYbfSLu6tI+bm3Haqn9DJiCiJXhiPGvb6RRRICj77p8il8nZyWpM7dhzkRoAWRkjLhymh/o4NB4K+NWwWsqHd6fWbyFMBDSUmNQsCt7pCo8Rlyh0qxkwoBqrzDbIrpdrQFZrxfTQEtOjGm/E85EO5FJi8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] toEncryptData = Encoding.ASCII.GetBytes("hello world");

            //rsaGenKeys.FromXmlString(privateXml);
            //rsaGenKeys.FromXmlString(publicXml);

            //Encode with public key
            RSACryptoServiceProvider rsaPublic = new RSACryptoServiceProvider();
            
            rsaPublic.FromXmlString(publicXml);
            byte[] encryptedRSA = rsaPublic.Encrypt(toEncryptData, false);
            string EncryptedResult = Encoding.Default.GetString(encryptedRSA);


            //Decode with private key
            var rsaPrivate = new RSACryptoServiceProvider();
            rsaPrivate.FromXmlString(privateXml);
            byte[] decryptedRSA = rsaPrivate.Decrypt(encryptedRSA, false);
            string originalResult = Encoding.Default.GetString(decryptedRSA);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            privateXml = rsaGenKeys.ToXmlString(true);

        }

        string strPublicKeyExponent = "010001";
        string strPublicKeyModulus = "99F10A3266084499AB457FB0A14E95E17DC49B2C8B477456C2517AC3E7414213D5383D1DF193E5FADAED98D9217E83A5A6D5246AC63D7FECE54693F16B514F19904F068C27DFBEF350DBA64847A98DF1645A381D7A29A46262A60812B0D5EE24762CA6F1E6810B2F806B87A4FA71500107392C30644ECA66489EEB2EA991014D";

        private void button3_Click(object sender, EventArgs e)
        {
            publicXml = rsaGenKeys.ToXmlString(false);

            RSAParameters parameter = rsaGenKeys.ExportParameters(true);
            strPublicKeyExponent = BytesToHexString(parameter.Exponent);
            strPublicKeyModulus = BytesToHexString(parameter.Modulus);
        }

        private string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);
            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }
    }
}
