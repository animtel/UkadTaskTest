using System.ComponentModel.DataAnnotations;

namespace MyTest1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class TestUrlModel : DbContext
    {
        // Контекст настроен для использования строки подключения "TestUrlModel" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "MyTest1.Models.TestUrlModel" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "TestUrlModel" 
        // в файле конфигурации приложения.
        public TestUrlModel()
            : base("TestUrlModel")
        {
            Database.SetInitializer(
                new CreateDatabaseIfNotExists<TestUrlModel>());
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

         public virtual DbSet<CheckUrl> CheckUrls { get; set; }
    }

   

    public class CheckUrl
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Host { get; set; }
        public string Url { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        
    }

    class JsonCollection
    {
        public List<CheckUrl> CheckUrls { get; set; }
    }
}