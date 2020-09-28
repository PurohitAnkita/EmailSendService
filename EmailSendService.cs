using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EmailSendService
{
    /// <summary>
    /// Service to asynchronously send email to large users
    /// </summary>
    public class EmailSendService
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the file location of Csv file:");
                string filePath = Console.ReadLine();

                List<User> usersList = File.ReadAllLines(filePath).Skip(1).Select(x => new User(x)).ToList();

                sendEmailAsync(usersList);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Send Email async
        /// </summary>
        /// <param name="usersList">list of users</param>
        private static async void sendEmailAsync(List<User> usersList)
        {
            Console.WriteLine("Sending email started in background....");
            int validEmailSendCount = await sendEmail(usersList);            
            Console.WriteLine("Successfully sent email to {0} users", validEmailSendCount);
        }

        /// <summary>
        /// Send email in background
        /// </summary>
        /// <param name="usersList">list of users</param>
        /// <returns></returns>
        private static async Task<int> sendEmail(List<User> usersList)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            string subject = string.Empty;
            string body = string.Empty;

            int validEmail = 0;

            await Task.Run(() => 
            {                
                if (usersList != null && usersList.Any())
                {
                    foreach (User user in usersList)
                    {                        
                        bool isValid = regex.Match(user.emailId).Success;

                        if (isValid)
                        {
                            validEmail++;
                            Thread.Sleep(250);
                        }
                        else
                        {
                            Console.WriteLine(String.Format("Invalid email address: {0} ", user.emailId));
                        }
                    }
                }
            });

            return validEmail;
        }
    }

    /// <summary>
    /// Class to model User data
    /// </summary>
    public class User
    {
        /// <summary>
        /// User name
        /// </summary>
        public string userName;

        /// <summary>
        /// Email address
        /// </summary>
        public string emailId;

        /// <summary>
        /// Sets the users name and email address from the csv file
        /// </summary>
        /// <param name="line">user data in csv</param>
        public User(string line)
        {
            string[] values = line.Split(',');
            userName = values[0];
            emailId = values[1];
        }
    }
}
