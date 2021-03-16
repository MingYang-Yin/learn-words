using System;
using System.Collections.Generic;
using System.Text;
using Dao;
using entity;

namespace Biz
{

    class UserBiz
    {
        UserDao userDao = new UserDao();
        /*
         * 登录函数
         * 登录成功返回User
         */
        public User login(string userId, string password)
        {
            User user = userDao.getUserByIdAP(userId, password);
            return user;
        } 
        /*
         * 注册函数
         * 已经存在用户返回-2
         * 注册成功返回1
         * 其他失败情况返回其他值
         */
        public int register(string userId, string password)
        {
            return userDao.insertUserByIdAP(userId, password);
        }
        /*
         * 获取相应ID的record
         * 返回对应userId的记录值
         */
        public int getRecord(string userId)
        {
            User u = userDao.getUserById(userId);
            return u.record;
        }
        /*
         * 对应userId的record自增1
         */
         public int incRecord(string userId)
        {
            int re = getRecord(userId);
            re++;
            int i = userDao.setRecord(userId, re);
            return i;
        }

    }

    class WordBiz
    {
        WordDao wordDao = new WordDao();
        /*
         * 通过record获取学习单词列表
         * 传入参数为 userId
         */
        
        public List<Words> getWordLearn(string userId)
        {
            UserDao u = new UserDao();

            return wordDao.recordWord(u.getUserById(userId).record);
        }

        /*
         * 通过record获取已经学习过的单词列表
         * 传入参数为userId
         */
         public List<Words> getWordHasLearned(string userId)
        {
            UserDao u = new UserDao();

            return wordDao.learnedWord(u.getUserById(userId).record);
        }

        /*
         * 传入随机首字母
         * 获取一系列该首字母的单词
         */
        public List<string> getWordBattle(char a)
        {
            return wordDao.battleJustWord(a);
        }

    }
    class NotBiz
    {
        NotDao notDao = new NotDao();
        //根据用户ID得到所有的不会的单词
        public List<Words> getNotAll(string userId)
        {
            return notDao.getNotById(userId);

        }
        //学习不会的单词（得到15个）
        public List<Words> getNotLearn(string userId)
        {

            List<Words> words = getNotAll(userId);
            int wordsLen = words.Count;
            if (wordsLen <= 15)
            {
                return words;
            }
            else{
                List<Words> words1 = new List<Words>();
                for(int i = 0; i < 15; i++)
                {
                    words1.Add(words[i]);
                }
                return words1;
            }

        }
        //增加不会的单词
        public int addNot(int wordId, string userId)
        {
            return notDao.insertById(wordId, userId);
        }
        //删除不会的单词
        public int deleteNot(int wordId, string userId)
        {
            return notDao.deleteById(wordId, userId);
        }

        //查重
        //如果重复返回false
        //如果不重复返回true
        public bool multiNot(string word, string userId)
        {
            List<Words> words = getNotAll(userId);
            for(int i = 0; i < words.Count; i++)
            {
                if (words[i].Word == word)
                {
                    return false;
                }
            }
            return true;
        }

    }

    class BookBiz
    {
        BookDao bookDao = new BookDao();
        //根据用户ID得到所有单词本的单词
        public List<Words> getBookAll(string userId)
        {
            return bookDao.getBookById(userId);
        }
        //学习单词本中的单词（得到15个）
        public List<Words> getBookLearn(string userId)
        {
            List<Words> words = getBookAll(userId);
            int wordsLen = words.Count;
            if (wordsLen <= 15)
            {
                return words;
            }
            else
            {
                List<Words> words1 = new List<Words>();
                for (int i = 0; i < 15; i++)
                {
                    words1.Add(words[i]);
                }
                return words1;
            }
        }

        //查重，如果重复返回false
        public bool doubleBook(string word, string userId)
        {
            List<Words> words = getBookAll(userId);
            for(int i = 0; i < words.Count; i++)
            {
                if (words[i].Word == word)
                {
                    return false;
                }
            }
            return true;
        }
        //增加单词本的单词
        public int addBook(int wordId , string userId)
        {
            return bookDao.insertById(wordId, userId);
        }
        //删除单词本的单词
        public int deleteBook(int wordId, string userId)
        {
            return bookDao.deleteById(wordId, userId);
        }
    }
}
