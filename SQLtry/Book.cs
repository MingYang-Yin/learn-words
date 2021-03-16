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
    class Book
    {
        BookBiz getWord = new BookBiz();
        byte[] temp = new byte[1024 * 1024];
        List<Words> wordList;

        //开始发送
        public void startGet(Socket SocketClient)
        {
            SocketClient.Send(Encoding.UTF8.GetBytes("needClientId")); //发送
            int ReceiveLength = SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None);
            string clientID = Encoding.UTF8.GetString(temp, 0, ReceiveLength);
            wordList = getWord.getBookAll(clientID);
            send(SocketClient);
            return;
        }

        //开始删除
        public void startDel(Socket SocketClient)
        {
            SocketClient.Send(Encoding.UTF8.GetBytes("needClientId")); //发送确认消息
            int ReceiveLength = SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None);
            string clientID = Encoding.UTF8.GetString(temp, 0, ReceiveLength); //接收用户名
            SocketClient.Send(Encoding.UTF8.GetBytes("needClientId")); //发送确认消息
            ReceiveLength = SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None);
            string wordToDel = Encoding.UTF8.GetString(temp, 0, ReceiveLength); //接收用户名
            wordList = getWord.getBookAll(clientID);
            for (int i = 0; i < wordList.Count; i++) {
                if (wordList[i].Word == wordToDel) {
                    getWord.deleteBook(wordList[i].wordId, clientID);
                    break;
                }
            }
            SocketClient.Send(Encoding.UTF8.GetBytes("ack")); //接收确认消息
            return;
        }

        //发送单词和释义
        public void send(Socket SocketClient) {
            int num = wordList.Count;
            SocketClient.Send(Encoding.UTF8.GetBytes(num.ToString())); //发送该用户单词本中的数量
            SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None); //接收响应消息
            for (int i = 0; i < num; i++) {
                SocketClient.Send(Encoding.UTF8.GetBytes(wordList[i].Word)); //发送单词
                SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None); //接收响应消息
                sendMeaning(wordList[i].meaning, SocketClient);
                SocketClient.Receive(temp, 0, temp.Length, SocketFlags.None); //接收响应消息
            }
        }

        //处理并发送意思
        public void sendMeaning(string OriMeaning, Socket SocketClient)
        {
            string[] arr, brr, finalArr;
            arr = OriMeaning.Split("<br>", StringSplitOptions.RemoveEmptyEntries); //以<br>符号分割，得到第一个释义
            brr = arr[0].Split("1)", StringSplitOptions.RemoveEmptyEntries); //以<br>符号分割，得到第一个释义的除去1的部分
            finalArr = brr[0].Split("."); //以.分割，得到第一个释义的中文部分
            if (finalArr.Length == 1)
            {
                SocketClient.Send(Encoding.UTF8.GetBytes(finalArr[0].ToString())); //发送
            }
            else
            {
                SocketClient.Send(Encoding.UTF8.GetBytes(finalArr[1].ToString())); //发送
            }
        }

    }
}
