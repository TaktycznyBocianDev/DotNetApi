using System.Collections;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{

    class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration congif)
        {
            _config = congif;
        }

        // public T LoadDataSingle<T>(string sql)
        // {
        //     IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        //     return dbConnection.QuerySingle<T>(sql);
        // }

        public T LoadDataSingle<T>(string sql, object parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql, parameters);
        }

        public IEnumerable<T> LoadData<T>(string sql, object parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql, parameters);
        }

        public bool ExecuteSql(string sql, object parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql, parameters) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql, object parameters = null)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
           return dbConnection.Execute(sql, parameters);
        }
    }

}