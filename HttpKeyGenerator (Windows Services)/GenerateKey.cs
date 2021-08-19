using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Timers;
using System.IO;

namespace InternetAuth__Windows_Services_
{
    public class GenerateKey
    {
        private readonly Timer _checker;
        private int executeCode = 2;
        private bool isBlocked = true;
        private bool netCutOff = false;
        
        public GenerateKey()
        {
            _checker = new Timer(1000); //creating the timer for every second
            _checker.AutoReset = true; //make sure it resets after every elapse
            _checker.Elapsed += _checker_Elapsed; //the function that happens whenever the time reaches 1 second
        }

        private void _checker_Elapsed(object sender, ElapsedEventArgs e) //the part where we check from the text file
        {

            switch (executeCode)
            {
                case 0:
                    //execute the first check
                    //cut off the internet connection
                    Process.Start("ipconfig", "/release");

                    //log error
                    using (StreamWriter writer = File.AppendText(".\\ServiceLog.txt"))
                    {
                        writer.WriteLine("Internet has been blocked at time: '" + DateTime.Now.ToString() + "'");
                        writer.Close();
                    }
                    netCutOff = true;
                    isBlocked = false;
                    executeCode = 2; //make it go towards case 2
                    break;

                case 1:
                    Process.Start("ipconfig", "/renew");
                    using (StreamWriter writer = File.AppendText(".\\ServiceLog.txt"))
                    {
                        writer.WriteLine("Internet Conn. Service Started at time: '" + DateTime.Now.ToString() + "'");
                        writer.Close();
                    }
                    executeCode = 2;
                    netCutOff = false;
                    break;

                case 2:
                    using (StreamReader sr = new StreamReader(".\\BlockedUser.txt"))
                    {
                        string readLine;
                        while ((readLine = sr.ReadLine()) != null)
                        {
                            if (Environment.MachineName.ToString() == readLine && !netCutOff) //the desktop name exists in the list of 'Blocked Users'
                            {
                                executeCode = 0; //make it go towards disabling internet
                            }
                        }
                    }
                    using (StreamReader sr = new StreamReader(".\\BlockedUser.txt"))
                    {
                        string readLine;
                        while ((readLine = sr.ReadLine()) != null)
                        {
                            if (Environment.MachineName.ToString() != readLine && netCutOff && !isBlocked)
                            {
                                executeCode = 1; //towards enabling internet
                            }
                        }
                        break;
                    }

            }
        }

        //....(1)

        public void Stop()
        {
            _checker.Stop();
        }
        public void Start()
        {
            _checker.Start();
        }
    }
}
