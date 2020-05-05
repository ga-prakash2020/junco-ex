using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace ScheduledJob
{
    class Program
    {
        const string SEND_LINK_QUERY = "SELECT Email FROM Notifications WHERE [NotifiedOn] IS NULL";
        const string SEND_THANKYOU_NOTEQUERY = "SELECT Email FROM Notifications WHERE [NotifiedOn] IS NOT NULL AND [IsViewed] = 1";
        const string SEND_REMINDER_QUERY = "SELECT Email FROM Notifications WHERE [NotifiedOn] IS NOT NULL AND DATEDIFF(" +
            "day, [NotifiedOn], GETDATE()) <= 3  AND [IsViewed] = 0";

        const string UPDATE_LINK_SENT_STATUS_QUERY = "UPDATE Notifications SET [NotifiedOn] = GETDATE() WHERE Email = @Email";
        const string UPDATE_THANKYOU_QUERY = "UPDATE Notifications SET [IsThankYouSent] = 1 WHERE [IsViewed] = 1 AND Email = @Email";

        static string adminEmail = "";
        static string adminPassword = "";
        static string smtpHost = "";
        static int smtpHostPort;

        static void Main(string[] args)
        {
            adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            adminPassword = ConfigurationManager.AppSettings["adminPassword"];
            smtpHost = ConfigurationManager.AppSettings["smtpHost"];
            smtpHostPort = int.Parse(ConfigurationManager.AppSettings["smtpHostPort"]);

            Run();

            Console.ReadLine();
        }

        static void Run()
        {
            // Get recepients to send link
            string linkRecepients = GetMailIds(SEND_LINK_QUERY);

            if (!string.IsNullOrEmpty(linkRecepients))
            {
                if (SendEmail(adminEmail, linkRecepients, "Your link", "<a href='gmail.com>click here>'"))
                {
                    // Update notification sent status
                    var notificationRecepients = linkRecepients.Split(',');
                    foreach (var recepient in notificationRecepients)
                    {
                        UpdateNotificationSentStatus(recepient);
                    }
                }
            }

            // Get recepients to send thank you mail
            string thankYouRecepients = GetMailIds(SEND_THANKYOU_NOTEQUERY);
            if (!string.IsNullOrEmpty(thankYouRecepients))
            {
                if (SendEmail(adminEmail, thankYouRecepients, "Thank You", "Thank you for visting the link"))
                {
                    // Update thankyou sent status
                    var thankYouStatusRecepients = thankYouRecepients.Split(',');
                    foreach (var recepient in thankYouStatusRecepients)
                    {
                        UpdateThankYouStatus(recepient);
                    }
                }
            }

            // Get recepients to send reminder mail
            string reminderRecepients = GetMailIds(SEND_REMINDER_QUERY);
            if (!string.IsNullOrEmpty(reminderRecepients))
                SendEmail(adminEmail, reminderRecepients, "Reminder", "Please visit the link");
        }

        static string GetMailIds(string query)
        {
            string connString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
            StringBuilder sbRecipients = new StringBuilder();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connString))
                {
                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        sqlConn.Open();
                        using (var reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                sbRecipients.AppendFormat("{0},", reader["Email"].ToString());
                            }
                        }
                    }
                }
                return sbRecipients.ToString().TrimEnd(new char[] { ',' });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured on 'SendEmail': \nDetails: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace);
                return string.Empty;
            }
        }

        static bool UpdateNotificationSentStatus(string email)
        {
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Email", SqlDbType.VarChar)
                {
                    Value = email
                }
            };

            return ExecuteNonQuery(UPDATE_LINK_SENT_STATUS_QUERY, sqlParams);
        }

        static bool UpdateThankYouStatus(string email)
        {
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Email", SqlDbType.VarChar)
                {
                    Value = email
                }
            };

            return ExecuteNonQuery(UPDATE_THANKYOU_QUERY, sqlParams);
        }

        static bool ExecuteNonQuery(string query, SqlParameter[] sqlParams)
        {
            string connString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connString))
                {
                    using (SqlCommand command = new SqlCommand(query, sqlConn))
                    {
                        sqlConn.Open();
                        foreach (var param in sqlParams)
                        {
                            command.Parameters.Add(param);
                        }

                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception occured on 'SendEmail': \nDetails: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace);
                return false;
            }
        }

        static bool SendEmail(string from, string to, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(from);
                message.To.Add(to);
                message.Subject = subject; 
                message.Body = body;
                smtp.Port = smtpHostPort;
                smtp.Host = smtpHost;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, adminPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception occured on 'SendEmail': \nDetails: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}
