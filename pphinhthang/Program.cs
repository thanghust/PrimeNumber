﻿using System;
using System.Collections.Generic;
using System.Text;
using Alchemi.Core;
using Alchemi.Core.Owner;


namespace TrapezoidRule
{
    class TrapezoidRule : GApplication
    {
        public static GApplication App = new GApplication();
        private static int[] matrix;//Mang chua so can kiem tra
        private static int NumPerThread;//So luong so trong 1 thread
        private static DateTime start;

        [STAThread]
        static void Main(string[] args)
        {
            int n;
            string host;
            Random random = new Random();
            Console.Write("Host[localhost]:");
            host = Console.ReadLine();
            if (host.Length < 1)
            {
                host = "localhost";
            }
            Console.Write("Dua vao so luong so can kiem tra:");
            n = Int32.Parse(Console.ReadLine());
            Console.Write("Dua vao so luong so cho 1 thread:");
            NumPerThread = Int32.Parse(Console.ReadLine());
            matrix = new int[n];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = random.Next();
            }
            int NumRemain = n; // NumRemain =  số luong số cần kiểm tra
            int NumCur = 0; // 
            while (NumRemain > 0)
            {
                int NumberOfThread;
                if (NumRemain > NumPerThread) // số lượng số cần check >  số lượng số của 1 luồng
                {
                    NumberOfThread = NumPerThread;
                }
                else
                {
                    NumberOfThread = NumRemain;
                }
                int[] Nums = new int[NumberOfThread];
                for (int i = 0; i < NumberOfThread; i++)
                {
                    Nums[i] = matrix[NumCur];
                    NumCur++;
                }
                App.Threads.Add(new TrapezoidRuleCount(NumCur - NumPerThread, NumCur - NumPerThread + 1, 5)); // NumCur-NumperThread, NumCur-NumperThread + 1
                NumRemain -= NumberOfThread;
            }
            // thiết lập liên kết
            App.Connection = new GConnection("localhost", 9000, "user", "user");
            // Thêm vào các mô đun cần thiết
            App.Manifest.Add(new ModuleDependency(typeof(TrapezoidRule).Module));
            // Thêm vào sự kiện threadFinish
            App.ThreadFinish += new GThreadFinish(App_ThreadFinish);

            App.ApplicationFinish += new GApplicationFinish(App_ApplicationFinish);
            start = DateTime.Now;
            Console.WriteLine("Thread started!");
            App.Start();
            Console.ReadLine();

        }

        private static void App_ThreadFinish(GThread thread)
        {
            TrapezoidRuleCount pnc = (TrapezoidRuleCount)thread;
            Console.WriteLine("So{0}-{1} hoan thanh", pnc.StartNums,pnc.EndNums);
        }

        private static void App_ApplicationFinish()
        {
            Console.WriteLine("Hoan thanh sau {0} seconds.", DateTime.Now - start);
        }
    }
    [Serializable]
    class TrapezoidRuleCount : GThread
    {
        public int StartNums;
        public int EndNums;
        public int n;
        public TrapezoidRuleCount(int StartNums, int EndNums , int n)
        {
            this.StartNums = StartNums;
            this.EndNums = EndNums;
            this.n = n;
        }
        public override void Start()
        {
            double ans ;
            double h;
            h = (EndNums - StartNums) / n;
            double[] X = new double[n+1];
            double[] Y = new double[n+1];

            for (int i = 0; i < n+1; i++)
            {
                X[i] = StartNums + i*h;
                X.Append(X[i]);
                Y[i] = Math.Exp(X[i]);
                Y.Append(Y[i]);
            }
            ans = 0;
            for (int i = 0;i < n+1;i++)
            {
                if ((i == 0) || (i == n))    
                {
                    ans = ans + Y[i];
                }
                else
                {
                    ans = ans + 2 * Y[i];
                }
            }
            ans = (ans * h)/ 2;
            Console.WriteLine(ans);
        }
    }
}
