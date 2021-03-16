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
using System.Linq;
using System.Threading.Tasks;


namespace SQLtry
{
    class battle
    {
        //记录Battle名字和参与的用户Socket
        public static Dictionary<string, List<Socket>> saveBattleInfo = new Dictionary<string, List<Socket>> { };
        //记录Socket和对应的用户ID
        public static Dictionary<Socket, string> SocketAndClient = new Dictionary<Socket, string> { };

        //接收并记录该Socket的用户ID，返回该ID
        public static string recieveID(Socket SocketClient, string name)
        {
            byte[] account = new byte[1024 * 1024];
            List<Socket> saveOwner = saveBattleInfo[name];
            int ReceiveLength = 0;
            Console.WriteLine("开始接收客户端发来的账号");
            ReceiveLength = SocketClient.Receive(account, 0, account.Length, SocketFlags.None);
            string ReceiveAccount = Encoding.UTF8.GetString(account, 0, ReceiveLength);
            Console.WriteLine("账号" + ReceiveAccount);
            SocketAndClient.Add(SocketClient, ReceiveAccount);
            SocketClient.Send(Encoding.UTF8.GetBytes((saveOwner.Count+1).ToString()));
            SocketClient.Receive(account, 0, account.Length, SocketFlags.None);
            return ReceiveAccount;
        }

        public static void createBattle(Socket SocketClient, string name)
        {
            if (saveBattleInfo.ContainsKey(name))
            {
                string SendMessage = "failed";
                SocketClient.Send(Encoding.UTF8.GetBytes(SendMessage));
                Console.WriteLine("创建失败");
            }
            else
            {
                List<Socket> saveOwner = new List<Socket>();
                saveBattleInfo.Add(name, saveOwner);
                string SendMessage = "success";
                SocketClient.Send(Encoding.UTF8.GetBytes(SendMessage));
                Console.WriteLine("创建成功");
            }
        }

        //检查房间中的Socket是否仍然有效，若有无效连接，则删除
        //参数：为1，表示对全部的房间进行检查；否则，表示对特定的房间进行检查
        public static void checkAlive(string roomName)
        {
            List<Socket> checkSocket;
            if (roomName == "1")
            {
                for (int i = 0; i < saveBattleInfo.Count(); i++)
                {
                    checkSocket = saveBattleInfo.ElementAt(i).Value;
                    for (int j = 0; j < checkSocket.Count(); j++)
                    {
                        //如果连接已关闭、重置或终止，则返回 true
                        //此种情况就表示若客户端断开连接了，则此方法就返回true;否则，返回false。
                        if (checkSocket[j].Poll(1000, SelectMode.SelectRead))
                        {
                            checkSocket[j].Close();
                            checkSocket.RemoveAt(j);//移除该位置的Socket
                        }
                    }
                    if (checkSocket.Count() == 0)
                    {//没有用户在该房间等待，删除该房间
                        saveBattleInfo.Remove(saveBattleInfo.ElementAt(i).Key);
                    }
                }
            }
            else {
                checkSocket = saveBattleInfo[roomName];
                for (int j = 0; j < checkSocket.Count(); j++)
                {
                    //如果连接已关闭、重置或终止，则返回 true
                    //此种情况就表示若客户端断开连接了，则此方法就返回true;否则，返回false。
                    if (checkSocket[j].Poll(1000, SelectMode.SelectRead))
                    {
                        checkSocket.RemoveAt(j);//移除该位置的Socket
                    }
                }
            }
        }
        
        //SocketClient：要加入的客户端的Socket
        //name：该客户端要加入的Battle
        public static void joinBattle(Socket SocketClient, string name)
        {
            byte[] account = new byte[1024 * 1024];
            if (saveBattleInfo.ContainsKey(name)) //存在该Battle房间
            {
                List<Socket> checkSocket = saveBattleInfo[name];
                checkAlive(name);//检查该房间
                List<Socket> tempCheck = saveBattleInfo[name];
                try {
                    if (checkSocket.Count() == 3)
                    {
                        string SendMessage = "0"; //Battle的人数已满，无法加入
                        SocketClient.Send(Encoding.UTF8.GetBytes(SendMessage));
                        Console.WriteLine("0加入失败");
                    }
                    else //Battle人数未满，可以加入
                    {
                        saveBattleInfo[name] = checkSocket;
                        string SendMessage = "1"; //加入成功
                        SocketClient.Send(Encoding.UTF8.GetBytes(SendMessage));
                        Console.WriteLine("加入成功");

                        //记录该Socket的用户ID
                        string ReceiveAccount = recieveID(SocketClient, name);

                        for (int j = 0; j < checkSocket.Count(); j++)
                        { //向之前已经在房间中的客户端发送新加入的用户账号
                            checkSocket[j].Send(Encoding.UTF8.GetBytes("add"));
                            checkSocket[j].Receive(account, 0, account.Length, SocketFlags.None);
                            checkSocket[j].Send(Encoding.UTF8.GetBytes(ReceiveAccount));
                            checkSocket[j].Receive(account, 0, account.Length, SocketFlags.None);
                        }
                        for (int j = 0; j < checkSocket.Count(); j++)
                        { //向之前已经在房间中的客户端发送新加入的用户账号

                            SocketClient.Send(Encoding.UTF8.GetBytes(SocketAndClient[checkSocket[j]]));
                            SocketClient.Receive(account, 0, account.Length, SocketFlags.None);
                        }
                        SocketClient.Send(Encoding.UTF8.GetBytes(ReceiveAccount));
                        SocketClient.Receive(account, 0, account.Length, SocketFlags.None);
                        checkSocket.Add(SocketClient); //将该客户加入Battle

                        if (checkSocket.Count() == 3)
                        { //人数满，开始Battle
                            startBattle(name);
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("有异常出现");
                }
            }
            else
            {
                string SendMessage = "-1"; //没有此Battle，加入失败
                SocketClient.Send(Encoding.UTF8.GetBytes(SendMessage));
                Console.WriteLine("加入失败");
            }
        }

        //向Battle成员广播，可以Battle啦
        public static void broadCast(string name, string wordHead)
        {
            byte[] message = new byte[1024 * 1024];
            List<Socket> checkSocket = saveBattleInfo[name];
            for (int i = 0; i < 3; i++)
            {
                checkSocket[i].Send(Encoding.UTF8.GetBytes("start"));
                checkSocket[i].Receive(message, 0, message.Length, SocketFlags.None); //接收确认
                checkSocket[i].Send(Encoding.UTF8.GetBytes(wordHead)); //发送Battle的首字母
                checkSocket[i].Receive(message, 0, message.Length, SocketFlags.None); //接收确认
            }
        }

        //广播该用户输入的单词
        //word：当轮用户输入的单词
        //clientNum：当轮用户序号
        public static void broadCastWord(string word, string name, int clientNum)
        {
            byte[] message = new byte[1024 * 1024];
            List<Socket> checkSocket = saveBattleInfo[name];
            //找到该轮次用户的用户ID
            string sendMessage = SocketAndClient[checkSocket[clientNum]];

            for (int i = 0; i < 3; i++)
            {
                if (i == clientNum)
                    continue;
                checkSocket[i].Send(Encoding.UTF8.GetBytes(word)); //告知每个客户端该轮次用户输入的单词
                checkSocket[i].Receive(message, 0, message.Length, SocketFlags.None); //接收确认收到信号
            }
        }

        public static void endBattle(string name) {
            List<Socket> useSocket = saveBattleInfo[name];
            for (int i = 0; i < 3; i++) {
                SocketAndClient.Remove(useSocket[i]);
                useSocket[i].Shutdown(SocketShutdown.Both);
                useSocket[i].Close();
            }
            saveBattleInfo.Remove(name);
            checkAlive("1"); //去掉死掉的连接和房间
        }

        //传入的参数name为Battle名字
        public static void startBattle(string name)
        {

            Random rd = new Random();
            int wordHead = rd.Next(1, 27); //生成1~26之间的随机数，不包括27

            char capital = (char)('A' + wordHead - 1);
            char lowercase = (char)('a' + wordHead - 1);

            WordBiz word = new WordBiz();
            List<string> usableWordsC = word.getWordBattle(capital);
            List<string> usableWordsL = word.getWordBattle(lowercase);

            List<string> usableWords = usableWordsC.Union(usableWordsL).ToList<string>(); //剔除重复项，合并两个集合
            List<string> alreadyUsed = new List<string>();
            List<Socket> useSocket = saveBattleInfo[name];
            broadCast(name, capital.ToString()); //广播通知全体成员该轮次的首字母

            byte[] temp = new byte[1024 * 1024];
            byte[] Word = new byte[1024 * 1024];
            int ReceiveLength = 0;
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    for(int j = 0; j < 3; j++) { 
                        useSocket[j].Send(Encoding.UTF8.GetBytes((i+1).ToString())); //通知第i号客户
                        useSocket[j].Receive(Word, 0, Word.Length, SocketFlags.None); //接收确认收到信号
                    }
                    Console.WriteLine("开始接收客户端" + i + "发来的单词");
                   
                    for (int j = 0; j < 3; j++) {
                        useSocket[j].Send(Encoding.UTF8.GetBytes("aaa")); //通知第j号客户

                    }
                    ReceiveLength = useSocket[i].Receive(Word, 0, Word.Length, SocketFlags.None);
                    string RecieceWord = Encoding.UTF8.GetString(Word, 0, ReceiveLength); //接收到的该轮用户输入的单词
                    Console.WriteLine("接收到：" + RecieceWord);
                 
                    broadCastWord(RecieceWord, name, i); //向其他用户广播该用户输入的单词

                    if (RecieceWord == "ackackackack") {
                        endBattle(name);
                        return;
                    }

                    if (usableWords.Contains(RecieceWord)) //该单词可用
                    {
                        if (alreadyUsed.Contains(RecieceWord)) //该单词出现过，回答失败
                        {
                            string SendMessage = "-1"; //回答失败，该单词已经回答过
                            Console.WriteLine("答案重复");
                            useSocket[0].Send(Encoding.UTF8.GetBytes("-1"));
                            useSocket[1].Send(Encoding.UTF8.GetBytes("-1"));
                            useSocket[2].Send(Encoding.UTF8.GetBytes("-1"));
                            Console.WriteLine("决出输家");
                            endBattle(name);
                            break;
                        }
                        else
                        {
                            string SendMessage = "1"; //回答成功
                            useSocket[i].Send(Encoding.UTF8.GetBytes(SendMessage));
                            useSocket[i].Receive(Word, 0, Word.Length, SocketFlags.None);
                            Console.WriteLine("答案正确");
                            alreadyUsed.Add(RecieceWord);
                            broadCastWord("1", name, i); //通知其他Battle成员
                        }
                    }
                    else
                    {
                        string SendMessage = "0"; //回答失败，没有该单词
                        Console.WriteLine("答案未找到");
                        useSocket[0].Send(Encoding.UTF8.GetBytes("0"));
                        useSocket[1].Send(Encoding.UTF8.GetBytes("0"));
                        useSocket[2].Send(Encoding.UTF8.GetBytes("0"));
                        Console.WriteLine("决出输家");
                        endBattle(name);
                        return;
                    }
                }
            }
        }
    }
}
