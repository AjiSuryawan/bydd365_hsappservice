using bydd365_hsappservice.models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace bydd365_hsappservice
{
    internal class MyJob : IJob
    {
        public void MainExecute()
        {
            try
            {
                string emailconfig = ConfigurationManager.AppSettings["emailconfig"];
                string filename = ConfigurationManager.AppSettings["filename"];
                string filePath = Path.Combine(emailconfig, filename);

                // Read JSON content from the first file
                string jsonFilePath1 = ConfigurationManager.AppSettings["jsonbyodconfig"];
                string jsonContent1 = File.ReadAllText(jsonFilePath1);

                // Deserialize JSON into object
                ByodConfig configObject1 = JsonConvert.DeserializeObject<ByodConfig>(jsonContent1);

                // Read JSON content from the second file
                string jsonFilePath2 = ConfigurationManager.AppSettings["jsond365config"];
                string jsonContent2 = File.ReadAllText(jsonFilePath2);

                // Deserialize JSON into object
                ByodConfig configObject2 = JsonConvert.DeserializeObject<ByodConfig>(jsonContent2);

                // Compare table counts for each table
                for (int i = 0; i < configObject1.TableListName.Count; i++)
                {
                    string tableName = configObject1.TableListName[i];
                    int count1 = ExecuteCountQuery(configObject1.DBServer, configObject1.DBName, tableName,
                        configObject1.DBUser, configObject1.DBPass, configObject1.Timeout);
                    int count2 = ExecuteCountQuery(configObject2.DBServer, configObject2.DBName, tableName,
                        configObject2.DBUser, configObject2.DBPass, configObject2.Timeout);

                    if (count1 != count2)
                    {
                        Console.WriteLine($"Table {tableName} has different counts:");
                        Console.WriteLine($"Database 1 count: {count1}");
                        Console.WriteLine($"Database 2 count: {count2}");

                        // just for email testing
                        string jsonContent = File.ReadAllText(filePath);
                        Console.WriteLine($"JSON Content:\n{jsonContent}");
                        EmailConfig model = JsonConvert.DeserializeObject<EmailConfig>(jsonContent);
                        Console.WriteLine($"Session ID: {model.Sessionid}");
                        Console.WriteLine($"Interface Name: {model.InterfaceName}");
                        Console.WriteLine($"Company ID: {model.CompanyId}");
                        Console.WriteLine($"SMTP Server: {model.HoyaSmtp.SmtpServer}");
                        Console.WriteLine($"Has Attachment: {model.SmtpStorageAttachment.HasAttach}");

                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient(model.HoyaSmtp.SmtpServer, model.HoyaSmtp.SmtpPort);

                        mail.From = new MailAddress(model.HoyaSmtp.SmtpFrom);
                        mail.Subject = model.HoyaSmtp.SmtpSubject + ":" + DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                        mail.IsBodyHtml = true;
                        mail.Body = model.HoyaSmtp.SmtpBody.Replace("\\r\\n", Environment.NewLine);
                        mail.To.Add(model.HoyaSmtp.SmtpTo);
                        mail.CC.Add(model.HoyaSmtp.SmtpCc);
                        /*string filePathattachment = Path.Combine(model.SmtpStorageAttachment.FolderPath, model.SmtpStorageAttachment.Files[0]); // Replace with the actual file path
                        Attachment attachment = new Attachment(filePathattachment);
                        mail.Attachments.Add(attachment);*/


                        try
                        {
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(model.HoyaSmtp.SmtpUser, model.HoyaSmtp.SmtpPassword);
                            SmtpServer.Send(mail);
                            Console.WriteLine("Email sent successfully with attachment.");


                        }
                        catch (SmtpException ex)
                        {
                            Console.WriteLine($"SMTP Exception: {ex.Message}");
                            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                        }


                    }
                    else
                    {
                        Console.WriteLine($"Table {tableName} has same counts in both databases:");
                        Console.WriteLine($"Count: {count1}");


                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private int ExecuteCountQuery(string dbServer, string dbName, string tableName, string dbUser, string dbPass, int timeOut)
        {
            int count = 0;
            string connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};Encrypt=True;TrustServerCertificate=True;Integrated Security=True;Connection Timeout={timeOut}";
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                using (var command = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", connection))
                {
                    count = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing count query for table {tableName}: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return count;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            MainExecute();
        }
    }
}
