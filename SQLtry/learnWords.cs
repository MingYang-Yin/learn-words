using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Dao;
using entity;
using Biz;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace SQLtry
{

    class learnWords
    {
        static WordBiz wordsToLearn = new WordBiz();//储存要学的15个单词
        static joinBook tempAddBook = new joinBook();
        public static void sendWords(Socket SocketClient, List<Words> wordsToRemember, string UserAccount)
        {
            Socket ReceiveSocket = (Socket)SocketClient;
            //接收客户端发来的消息
            byte[] account = new byte[1024 * 1024];
            byte[] unknownWord = new byte[1024 * 1024];
            byte[] activate = new byte[1024 * 1024];
            try
            {
                //向客户端发送Ready信号
                string SendMessage;
                //获取Client的IP地址
                for (int i = 0; i < 15; i++)
                { //发送15个单词的单词部分
                    SendMessage = wordsToRemember[i].Word;
                    ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                    ReceiveSocket.Receive(activate, 0, activate.Length, SocketFlags.None);
                }
                listen(ReceiveSocket, UserAccount, wordsToRemember);
                //关闭Socket连接
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("监听出现异常!!!");
                Console.WriteLine("客户端" + ReceiveSocket.RemoteEndPoint + "已经连接中断" + "\r\n" +
                    ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                ReceiveSocket.Shutdown(SocketShutdown.Both);
                ReceiveSocket.Close();
                return;
            }
        }

        public static void learn(Socket SocketClient)
        {
            byte[] account = new byte[1024 * 1024];
            int ReceiveLength = 0;
            SocketClient.Send(Encoding.UTF8.GetBytes("notice"));

            Console.WriteLine("开始接收客户端发来的账号");
            ReceiveLength = SocketClient.Receive(account, 0, account.Length, SocketFlags.None);
            string ReceiveAccount = Encoding.UTF8.GetString(account, 0, ReceiveLength);
            Console.WriteLine("账号" + ReceiveAccount);
            sendWords(SocketClient, wordsToLearn.getWordLearn(ReceiveAccount), ReceiveAccount);
        }

        public static void listen(Socket ReceiveSocket, string UserAccount, List<Words> wordsToRemember)
        {
            int ReceiveLength = 0;
            byte[] unknownWord = new byte[1024 * 1024];
            string SendMessage;
            UserBiz userBiz = new UserBiz();
            while (true)
            {
                ReceiveLength = ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None);
                string RecieceUnknownWord = Encoding.UTF8.GetString(unknownWord, 0, ReceiveLength);
                string[] arr;
                if (RecieceUnknownWord == "111") //接收到了干扰信息
                {
                    continue;
                }
                if (RecieceUnknownWord == "addBook")
                {
                    ReceiveSocket.Send(Encoding.UTF8.GetBytes("needWords"));
                    ReceiveLength = ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None);
                    RecieceUnknownWord = Encoding.UTF8.GetString(unknownWord, 0, ReceiveLength);
                    BookBiz check = new BookBiz();
                    if (check.doubleBook(RecieceUnknownWord, UserAccount)) {
                        tempAddBook.addToBook(UserAccount, RecieceUnknownWord, wordsToRemember);
                    }
                    ReceiveSocket.Send(Encoding.UTF8.GetBytes("ack"));
                }
                else if (RecieceUnknownWord != "okk") //用户没有学习完毕
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (wordsToRemember[i].Word == RecieceUnknownWord)
                        {
                            int wordID = wordsToRemember[i].wordId;
                            NotBiz addUnknownWord = new NotBiz();
                            int a = addUnknownWord.addNot(wordID, UserAccount);
                            Console.WriteLine("successfully add to database?" + a);
                            SendMessage = wordsToRemember[i].meaning; //发送释义
                            arr = SendMessage.Split("<br>"); //去除释义中的<br>符号
                            SendMessage = "";

                            for (int k = 0; k < arr.Length; k++)
                            {
                                SendMessage += arr[k];
                            }
                            SendMessage = SendMessage.Replace((char)13, (char)32);
                            SendMessage = SendMessage.Replace((char)10, (char)32);
                            ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                            ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认

                            SendMessage = wordsToRemember[i].lx;
                            if (SendMessage == "")
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes("noLx"));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }
                            else
                            {
                                arr = SendMessage.Split("/n");
                                string[] arr1 = arr[0].Split("/");
                                string[] arr2 = arr[1].Split("/");
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(arr1[0])); //发送例句的英文部分
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(arr2[0])); //发送例句的中文部分
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }

                            SendMessage = wordsToRemember[i].GQS;
                            if (SendMessage == "")
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes("noGQS"));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }
                            else
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }

                            SendMessage = wordsToRemember[i].GQFC;
                            if (SendMessage == "")
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes("noGQFS"));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }
                            else
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }

                            SendMessage = wordsToRemember[i].XZFC;
                            if (SendMessage == "")
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes("noXZFC"));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }
                            else
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }

                            SendMessage = wordsToRemember[i].FS;
                            if (SendMessage == "")
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes("noFS"));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }
                            else
                            {
                                ReceiveSocket.Send(Encoding.UTF8.GetBytes(SendMessage));
                                ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None); //接收确认
                            }

                            break;
                        }
                    }
                }
                else
                {
                    userBiz.incRecord(UserAccount);
                    break;
                }
            }
            ReceiveLength = ReceiveSocket.Receive(unknownWord, 0, unknownWord.Length, SocketFlags.None);
            string RecieceUnknownWord1 = Encoding.UTF8.GetString(unknownWord, 0, ReceiveLength);
            if (RecieceUnknownWord1 == "continue1")
            {
                learn(ReceiveSocket);
            }
            return;
        }
    }
}

