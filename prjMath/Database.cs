using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace System
{
    public class Database : DataContext
    {
        //TODO: Declare suas tabelas aqui
        //EXAMPLE: public Table<TipoDaTabela> NomeDaTabela
        public Table<Pontuacao> Pontuacao;

        //TODO: Faça suas inserções iniciais aqui
        private void OnCreateDatabase()
        {
        }

        #region Default Constructor and Methods

        #region Constructor
        public static Database Current { get; private set;}

        static Database()
        {
            Database.Current = new Database("Data Source=isostore:/Database.sdf");
        }

        private Database(string file)
            : base(file)
        {
            if (!DatabaseExists())
            {
                CreateDatabase();
                OnCreateDatabase();
            }
        }

        #endregion

        #region Insert

        public void Insert<T>(T entity) where T : class, new()
        {
            GetTable<T>().InsertOnSubmit(entity);
            SubmitChanges();
        }

        public void Insert<T>(IEnumerable<T> entities) where T : class, new()
        {
            GetTable<T>().InsertAllOnSubmit(entities);
            SubmitChanges();
        }

        public void Insert<T>(params T[] entities) where T : class, new()
        {
            GetTable<T>().InsertAllOnSubmit(entities);
            SubmitChanges();
        }

        #endregion

        #region Update

        public void Update()
        {
            SubmitChanges();
        }

        #endregion

        #region Delete

        public void Delete<T>(T entity) where T : class, new()
        {
            GetTable<T>().DeleteOnSubmit(entity);
            SubmitChanges();
        }

        public void Delete<T>(IEnumerable<T> entities) where T : class, new()
        {
            GetTable<T>().DeleteAllOnSubmit(entities);
            SubmitChanges();
        }

        public void Delete<T>(params T[] entities) where T : class, new()
        {
            GetTable<T>().DeleteAllOnSubmit(entities);
            SubmitChanges();
        }

        #endregion

        #region Select

        public List<T> SelectAll<T>() where T : class, new()
        {
            return GetTable<T>().ToList();
        }

        public List<T> Select<T>(Func<T, bool> predicate) where T : class, new()
        {
            return GetTable<T>().Where(predicate).ToList();
        }

        #endregion

        #endregion

        
    }

    //TODO: Declare suas classes aqui
    [Table]
    public class Pontuacao
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int codPontuacao;
        [Column]
        public string tempo { get; set; }
        [Column]
        public string acertos { get; set; }
    }
}
