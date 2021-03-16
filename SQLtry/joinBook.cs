using System;
using System.Collections.Generic;
using System.Text;
using Biz;
using entity;


namespace SQLtry
{
    //向单词本中添加词
    class joinBook
    {
        BookBiz useBook = new BookBiz();
        public int addToBook(string clientID, string wordToAdd, List<Words> wordsToRemember) {
            for (int i = 0; i < wordsToRemember.Count; i++) {
                if (wordsToRemember[i].Word == wordToAdd) {
                    useBook.addBook(wordsToRemember[i].wordId, clientID);
                    return 1;
                }
            }
            return 0;
        }
    }
}
