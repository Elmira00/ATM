using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class BankCard
    {
        public BankCard(string bankName, string username, string pAN, string pIN, DateTime expireDate)
        {
            BankName = bankName;
            Username = username;
            PAN = pAN;
            PIN = pIN;
            Random random = new Random();
            CVC = random.Next(100,999).ToString();
            ExpireDate = expireDate;
            Balance = random.Next(600,1000);
        }

        public string BankName { get; set; }
        public string Username { get; set; }
        public string PAN { get; set; }//kartdaki 16 reqemli kod
        public string PIN{ get; set; }//kartdaki 4 reqemli kod
        public  string CVC { get; set; }//kartin arxasindaki 3 reqemli kod//random
        public DateTime ExpireDate { get; set; }
        public double Balance { get; set; }//random

    }
    public class Client
    {
        public Client(int ıd, string name, string surname, int age, decimal salary, BankCard bankCard)
        {
            Id = ıd;
            Name = name;
            Surname = surname;
            Age = age;
            Salary = salary;
            BankCard = bankCard;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public decimal Salary{ get; set; }
        public BankCard    BankCard { get; set; }
        public string[] ClientReports { get; set; }
        public int ClientReportCount { get; set; } = 0;
        public bool IsExpiredTimeAvailable()
        {
            if (BankCard.ExpireDate > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public void AddClientReport(in string text,in DateTime date)
        {
            string newReport = (ClientReportCount+1).ToString()+"-->"+text + date.ToString() + "\n\n";
            string[] temp = new string[++ClientReportCount];
            if (ClientReports != null)
            {
                ClientReports.CopyTo(temp, 0);
            }
            temp[temp.Length - 1] = newReport;
            ClientReports = temp;
        }
        public void ShowClientReport()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            foreach (var item in ClientReports)
            {
                Console.WriteLine(item);
            }
            Console.ResetColor();
        }
    }
    public class Bank 
    {
        public Client[] Clients { get; set; }
        public int ClientCount { get; set; } = 0;
         
        public Bank()
        {

        }
        
        public Client GetClientWithPin(in string pin)
        {
            Client client = null;
            for (int i = 0; i < ClientCount; i++)
            {
                if (Clients[i].BankCard.PIN == pin)
                {
                    client = Clients[i];
                    break;
                }
            }
                return client;
        }
        public void AddClient(in Client newClient)
        {
            Client[] temp = new Client[++ClientCount];
            if (Clients != null)
            {
                Clients.CopyTo(temp, 0);
            }
            temp[temp.Length - 1] = newClient;
            Clients = temp;
        }
        public void CheckBalance(in string pin)
        {
            var client = GetClientWithPin(pin);
            if (client.IsExpiredTimeAvailable())
            {
                Console.WriteLine($"BALANCE : {client.BankCard.Balance}");
            }
            client.AddClientReport("Balance was checked", DateTime.Now);
        }
        public double WithdrawCash(in string pin,in double enteredMoney)
        {
            var client = GetClientWithPin(pin);
            if (enteredMoney < client.BankCard.Balance)
            {
                client.BankCard.Balance -= enteredMoney;
            }
            else
            {
                throw new Exception("! ! ! There is not enough money in the balance ! ! !");
            }
            client.AddClientReport("Cash was withdrawn", DateTime.Now);
            return enteredMoney;
        }
        public void  TransferFunds(in string pinSender, in string pinReceiver,in double amountOfTheMoney)
        {
            Client clientSender = GetClientWithPin(pinSender);
            if(amountOfTheMoney<= clientSender.BankCard.Balance)
            {           
            Client clientReceiver = GetClientWithPin(pinReceiver);
            clientSender.BankCard.Balance -= amountOfTheMoney;
            clientReceiver.BankCard.Balance += amountOfTheMoney;
            clientSender.AddClientReport($"{amountOfTheMoney} $ was transformed to {clientReceiver.Name} {clientReceiver.Surname}'s card  ", DateTime.Now);
            }
            else
            {
                throw new Exception("There is less than money in the balance");
            }
        }
        public void ShowReport(in string pin)
        {
            var client = GetClientWithPin(pin);
            client.ShowClientReport();
        }
        public void Check(in string pin)
        {
            Client client = GetClientWithPin(pin);
            if (client == null)
            {
                throw new Exception("Invalid Pin");
            }
            if (!client.IsExpiredTimeAvailable())
            {
                throw new Exception ("Check expired time");
            }
            
        }
    }

    public class Controller
    {
        public static void Start()
        {
            DateTime expireDate1 = DateTime.Now.AddDays(+5);
            DateTime expireDate2 = DateTime.Now.AddDays(-1);
            DateTime expireDate3 = DateTime.Now.AddMonths(+3);
            DateTime expireDate4 = DateTime.Now.AddYears(3);

            BankCard bankCard1 = new BankCard("Kapital", "Mrs.Elmira Ahmadova", "1234567890123123", "1267", expireDate1);
            BankCard bankCard2 = new BankCard("Kapital", "Mr.Nihad Shahbazov", "7239451628364710", "3032", expireDate2);
            BankCard bankCard3 = new BankCard("Kapital", "Mr.Eli Isayev", "1927304739201827", "6743", expireDate3);
            BankCard bankCard4 = new BankCard("Kapital", "Mrs.Firuza Mammadova", "1725394735241829", "9232", expireDate4);

            Client client1 = new Client(1, "Elmira", "Ahmadova", 18, 6500, bankCard1);
            Client client2 = new Client(2, "Nihad", "Shahbazov", 29, 3890, bankCard2);
            Client client3 = new Client(3, "Eli", "Isayev", 27, 2000, bankCard3);
            Client client4 = new Client(4, "Firuza", "Mammadova", 34, 890, bankCard4);

            Bank myBank = new Bank();
            myBank.AddClient(client1);
            myBank.AddClient(client2);
            myBank.AddClient(client3);
            myBank.AddClient(client4);

            void ShowFirstPage()
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n--------------------> A T M <--------------------\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Enter Pin : ");
                Console.ResetColor();

            }
            void ShowSecondPage()
            {

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine();
                Console.WriteLine("Check Balance -->[1]");
                Console.WriteLine("Withdraw Funt -->[2]");
                Console.WriteLine("Show Report   -->[3]");
                Console.WriteLine("Transform Funt-->[4]");
                Console.WriteLine("Exit-->[5]");
                Console.ResetColor();
                Console.WriteLine();

            }
            while (true)
            {
                Console.Clear();
                ShowFirstPage();
                string pin = Console.ReadLine();
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        Console.Clear();
                        myBank.Check(pin);
                        var client = myBank.GetClientWithPin(pin);
                        
                        Console.WriteLine($"Welcome {client.Name} {client.Surname}! ");
                        ShowSecondPage();
                        string select = Console.ReadLine();

                        if (select == "1")
                        {
                            myBank.CheckBalance(pin);
                            Thread.Sleep(1000);
                        }
                        else if (select == "2")
                        {
                            Console.WriteLine("10$-->[1]");
                            Console.WriteLine("20$-->[2]");
                            Console.WriteLine("50$-->[3]");
                            Console.WriteLine("500$-->[4]");
                            Console.WriteLine("Custom amount-->[5]");
                            string subSelect = Console.ReadLine();
                            if (subSelect == "1")
                            {
                                myBank.WithdrawCash(pin, 10);
                                Console.ReadKey();
                            }
                            else if (subSelect == "2")
                            {
                                myBank.WithdrawCash(pin, 20);
                                Console.ReadKey();
                            }
                            else if (subSelect == "3")
                            {
                                myBank.WithdrawCash(pin, 50);
                                Console.ReadKey();
                            }

                            else if (subSelect == "4")
                            {
                                myBank.WithdrawCash(pin, 500);
                                Console.ReadKey();
                            }
                            else if (subSelect == "5")
                            {
                                Console.Write("Enter amount of money : ");
                                int amountOfMoney = Convert.ToInt32(Console.ReadLine());
                                myBank.WithdrawCash(pin, amountOfMoney);
                                Console.ReadKey();
                            }
                        }
                        else if (select == "3")
                        {
                            myBank.ShowReport(pin);
                            Console.ReadKey();
                        }
                        else if (select == "4")
                        {
                            Console.Write("Enter receiver's pin : ");
                            string pin2 = Console.ReadLine();
                            myBank.Check(pin2);
                            Console.Write("Enter amount of money : ");
                            int amountOfMoney = Convert.ToInt32(Console.ReadLine());
                            myBank.TransferFunds(pin, pin2, amountOfMoney);
                            Console.ReadKey();
                        }
                        else if (select == "5")
                        {
                            break;
                        }
                        else { throw new Exception("Invalid select"); }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                }

            }

        }
    }

    public class Program
    {
        static void Main(string[] args)
        {

            Controller.Start();



        }
    }
}
