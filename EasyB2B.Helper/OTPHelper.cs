using EasyB2B.Helper.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EasyB2B.Helper
{  

    public class OTPSettings
    {
        public int OTPLength { get; set; } = 6;
        public OTPTypes OTPCodeType { get; set; } = OTPTypes.NUMERIC;
    }

    public enum OTPTypes
    {
        [StringValue("1,2,3,4,5,6,7,8,9,0")]
        NUMERIC = 1,
        [StringValue("A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z")]
        ALPHABET = 2,
        [StringValue("~,!,@,#,$,%,^,&,*,+,?")]
        SPECIALCHARACTER = 3,
        [StringValue("1,2,3,4,5,6,7,8,9,0,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z")]
        ALPHANUMERIC = 4,
        [StringValue("A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,~,!,@,#,$,%,^,&,*,+,?")]
        ALPHASPECIALCHARACTER =  5,
        [StringValue("1,2,3,4,5,6,7,8,9,0,~,!,@,#,$,%,^,&,*,+,?")]
        NUMERICSPECIALCHARACTER = 6,
        [StringValue("1,2,3,4,5,6,7,8,9,0,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,~,!,@,#,$,%,^,&,*,+,?")]
        ALPHANUMERICWITHSPECIALCHARACTER = 7       
    }



    public class OTPHelper
    {


        public static string GetNewOTP()
        {
            return GetNewOTP(new OTPSettings());
        }

        public static string GetNewOTP(OTPSettings oTPSettings)
        {
            Random rng = new Random();

            string OtpCharacters = OTPCharacters(oTPSettings);

            string OTPPassword = OTPGenerator(OtpCharacters, rng.Next(10).ToString());

            return OTPPassword;
        }

        public static string OTPCharacters(OTPSettings oTPSettings)
        {
            string NewCharacters = string.Empty;

            string allowedChars = StringEnum.GetStringValue(oTPSettings.OTPCodeType);

            if (oTPSettings.OTPLength > 0 && string.IsNullOrEmpty(allowedChars))
            {
                char[] sep = { ',' };
                string[] arr = allowedChars.Split(sep);

                string IDString = string.Empty;
                string temp = string.Empty;

                //utilize the "random" class
                Random rand = new Random();


                for (int i = 0; i < Convert.ToInt32(oTPSettings.OTPLength); i++)
                {
                    temp = arr[rand.Next(0, arr.Length)];
                    IDString += temp;
                    NewCharacters = IDString;
                }
            }
            return NewCharacters;
        }



        public static string OTPGenerator(string uniqueIdentity, string uniqueCustomerIdentity)
        {
            int length = 6;
            string oneTimePassword = "";
            DateTime dateTime = DateTime.Now;
            string _strParsedReqNo = dateTime.Day.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Month.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Year.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Hour.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Minute.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Second.ToString();
            _strParsedReqNo = _strParsedReqNo + dateTime.Millisecond.ToString();
            _strParsedReqNo = _strParsedReqNo + uniqueCustomerIdentity;


            _strParsedReqNo = uniqueIdentity + uniqueCustomerIdentity;
            using (MD5 md5 = MD5.Create())
            {
                //Get hash code of entered request id in byte format.
                byte[] _reqByte = md5.ComputeHash(Encoding.UTF8.GetBytes(_strParsedReqNo));
                //convert byte array to integer.
                int _parsedReqNo = BitConverter.ToInt32(_reqByte, 0);
                string _strParsedReqId = Math.Abs(_parsedReqNo).ToString();
                //Check if length of hash code is less than 9.
                //If so, then prepend multiple zeros upto the length becomes atleast 9 characters.
                if (_strParsedReqId.Length < 9)
                {
                    StringBuilder sb = new StringBuilder(_strParsedReqId);
                    for (int k = 0; k < (9 - _strParsedReqId.Length); k++)
                    {
                        sb.Insert(0, '0');
                    }
                    _strParsedReqId = sb.ToString();
                }
                oneTimePassword = _strParsedReqId;
            }

            if (oneTimePassword.Length >= length)
            {
                oneTimePassword = oneTimePassword.Substring(0, length);
            }
            return oneTimePassword;
        }
    }
}
