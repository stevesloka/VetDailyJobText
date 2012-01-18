using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LData
{
    public class BizPhoneUser
    {
        public static bool DoesPinExist(int pin)
        {
            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);
            return db.PhoneUsers.FirstOrDefault(p => p.PIN == pin) == null ? false : true;
        }

        public static bool DoesPhoneNumberExist(string phoneNumber)
        {
            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);
            return db.PhoneUsers.FirstOrDefault(p => p.PhoneNumber == phoneNumber) == null ? false : true;
        }

        public static void SaveUserPrefs(string phoneNumber, int zipCode, string keyWords)
        {
            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);

            //Get the record from db
            LData.PhoneUser user = db.PhoneUsers.FirstOrDefault(p => p.PhoneNumber == phoneNumber);

            if (user != null)
            {
                user.ZipCode = zipCode;
                user.KeyWords = keyWords;

                db.SubmitChanges(); //save back to db
            }
        }

        public static void GetUserPrefs(string phoneNumber, out int zipCode, out string keyWords)
        {
            zipCode = 0;
            keyWords = "";

            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);

            //Get the record from db
            LData.PhoneUser user = db.PhoneUsers.FirstOrDefault(p => p.PhoneNumber == phoneNumber);

            if (user != null)
            {
                zipCode = user.ZipCode == null ? 0 : (int)user.ZipCode;
                keyWords = user.KeyWords;
            }
        }

        public static string CreatePhoneUser(string phone, int pin)
        {
            //TODO -- Error Check!
            string errorMessage = "";

            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);
            
            //Verify phone number doesn't exist
            if (!DoesPhoneNumberExist(phone))
            {
                LData.PhoneUser user = new PhoneUser();
                user.ID = Guid.NewGuid();
                user.PhoneNumber = phone;
                user.PIN = pin;
                user.DateCreated = DateTime.Now;
                user.DateLastAccessed = Constants.NullDate;
                user.DateActivated = DateTime.Now;

                db.PhoneUsers.InsertOnSubmit(user);
                db.SubmitChanges();
            }
            else
            {
                errorMessage = "I'm sorry, you already have an account!";
            }

            return errorMessage;
        }

        public static bool DoesUserHaveCorrectCredentials(string phone, int pin)
        {
             LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);
             return db.PhoneUsers.FirstOrDefault(p => p.PhoneNumber == phone && p.PIN == pin) == null ? false : true;
        }

        public static void UpdateLastAccessedDate(string phoneNumber)
        {
            LData.DBDataContext db = new DBDataContext(Constants.AppConnectionString);

             //Get the record from db
            LData.PhoneUser user = db.PhoneUsers.FirstOrDefault(p => p.PhoneNumber == phoneNumber);

            if (user != null)
            {
                user.DateLastAccessed = DateTime.Now;
                db.SubmitChanges();
            }
        }
    }
}
