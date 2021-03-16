using System;
using Operation;
namespace entity
{
    class User
    {
        /*
        userId varchar(15)
        password varchar(45)
        record int
        */
        [DataReaderModel("userId")]
        public String userId { get; set; }

        [DataReaderModel("password")]
        public String password { get; set; }
        [DataReaderModel("record")]
        public int record { get; set; }
        
    }

    [Serializable]
    class Words
    {
        /*
        wordId int
        word varchar(30)
        GQS varvhar(35) 过去式
        GQFC varvhar(40) 过去分词 
        XZFC varvhar(40) 现在分词
        FS varvhar(40) 复数
        meaning longtext 释义
        lx longtext 例句
         */
        [DataReaderModel("wordId")]
        public int wordId { get; set; }
        [DataReaderModel("Word")]
        public String Word { get; set; }
        [DataReaderModel("GQS")]
        public String GQS { get; set; }
        [DataReaderModel("GQFC")]
        public String GQFC { get; set; }
        [DataReaderModel("XZFC")]
        public String XZFC { get; set; }
        [DataReaderModel("FS")]
        public String FS { get; set; }
        [DataReaderModel("meaning")]
        public String meaning { get; set; }
        [DataReaderModel("lx")]
        public String lx { get; set; }
    }

    class Unknown
    {
        /*
        notId int
        wordId int
        userId varchar(15)
         */
        [DataReaderModel("notId")]
        public int notId { get; set; }
        [DataReaderModel("wordId")]
        public int wordId { get; set; }
        [DataReaderModel("userId")]
        public String userId { get; set; }
    }

    class Book
    {
        /*
        bookId int
        wordId int
        userId varchar(15)
         */
        [DataReaderModel("bookId")]
        public int bookId { get; set; }
        [DataReaderModel("wordId")]
        public int wordId { get; set; }
        [DataReaderModel("userId")]
        public String userId { get; set; }
    }
}