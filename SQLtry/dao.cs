using Operation;
using entity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Dao
{
    class UserDao
    {
        string conStr = "server=localhost;port=3306;database=word;user id='root';password='B04316MySQL';Allow User Variables=True";

        /*public List<Person> GetAllPersonList(int index = 1, int pageSize = 10)
        {
            index = (index - 1) * 10;
            string sql = $"SELECT * FROM t_appoint_user limit {index},{pageSize}";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Person>(sql);
        }*/

        //通过Id查询
        public User getUserById(string id)
        {
            string sql = $"SELECT * FROM user WHERE userId = '{id}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClass<User>(sql);
        }

        //登录功能
        public User getUserByIdAP(string id, string password)
        {
            string sql = $"SELECT * FROM user WHERE userId = '{id}' AND password = '{password}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            
            return sqlOperation.getClass<User>(sql);
        }

        //注册功能
        public int insertUserByIdAP(string id, string password)
        {
            User user1 = getUserById(id);
            if (user1 != null)
            {
                return -2;
            }
            else
            {
                string sql = $"INSERT INTO user (userId, password, record) VALUES ('{id}', '{password}', 1)";
                MySqlOperation sqlOperation = new MySqlOperation(conStr);
                return sqlOperation.ExecSQL(sql);
            }
        }

        //设置record
        public int setRecord(string id, int record)
        {
            string sql = $"UPDATE user SET record = '{record}' WHERE userId = '{id}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.ExecSQL(sql);
        }
    }

    class WordDao
    {
        string conStr = "server=localhost;port=3306;database=word;user id='root';password='B04316MySQL';Allow User Variables=True";
        //通过用户的record查询单词
        public List<Words> recordWord(int record)
        {
            string sql = $"SELECT * FROM words WHERE wordId BETWEEN ('{record}'-1)*15 AND '{record}'*15";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Words>(sql);
        }
        //通过用户的record返回所有已经学过的单词
        public List<Words> learnedWord(int record)
        {
            string sql = $"SELECT * FROM words WHERE wordId < ('{record}'-1)*15";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Words>(sql);
        }
        //通过首字母查询单词
        public List<Words> battleWord(char c)
        {
            string sql = $"SELECT * FROM words WHERE word LIKE '{c}%'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Words>(sql);
        }
        //通过首字母返回纯单词列表！！！！！！
        public List<string> battleJustWord(char c)
        {
            List<string> w = new List<string>();
            List<Words> words = battleWord(c);
            for(int i = 0; i < words.Count; i++)
            {
                w.Add(words[i].Word);
            }
            return w;
        }



    }

    class NotDao
    {
        string conStr = "server=localhost;port=3306;database=word;user id='root';password='B04316MySQL';Allow User Variables=True";
        //通过用户id查询不会的单词
        public List<Words> getNotById(string userId)
        {
            string sql = $"SELECT words.wordId, words.Word, words.GQS, words.GQFC, words.XZFC, words.FS, words.meaning, words.lx FROM unknown, words WHERE unknown.wordId=words.wordId AND unknown.userId = '{userId}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Words>(sql);
        }

        //通过wordId和userId删除
        public int deleteById(int wordId, string userId)
        {
            string sql = $"DELETE FROM word.unknown WHERE wordId='{wordId}' AND userId = '{userId}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.ExecSQL(sql);
        }
        //通过userI和wordId增加
        public int insertById(int wordId, string userId)
        {
            string sql = $"INSERT INTO word.unknown (userId, wordId) VALUES ('{userId}', {wordId})";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.ExecSQL(sql);
        }

    }

    class BookDao
    {
        string conStr = "server=localhost;port=3306;database=word;user id='root';password='B04316MySQL';Allow User Variables=True";
        //通过用户id查询生词本--------------------------------------------------------------------------------------------------
        public List<Words> getBookById(string userId)
        {
            string sql = $"SELECT words.wordId, words.Word, words.GQS, words.GQFC, words.XZFC, words.FS, words.meaning, words.lx FROM book, words WHERE book.wordId=words.wordId AND book.userId = '{userId}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.getClassList<Words>(sql);
        }
        //通过wordId和userId删除-----------------------------------------------------------------------------------------------------------
        public int deleteById(int wordId, string userId)
        {
            string sql = $"DELETE FROM book WHERE wordId='{wordId}' AND userId = '{userId}'";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.ExecSQL(sql);
        }
        //通过userI和wordId增加------------------------------------------------!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!------------------------------------------------------------------
        public int insertById(int wordId, string userId)
        {
            string sql = $"INSERT INTO book (userId, wordId) VALUES ('{userId}', {wordId})";
            MySqlOperation sqlOperation = new MySqlOperation(conStr);
            return sqlOperation.ExecSQL(sql);
        }
    }

}
