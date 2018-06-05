using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.DAL
{
    public class DAL
    {
        private string _strConnection = "data source=10.120.2.125:1561/TGIS;password=PTTOUR;persist security info=True;user id=PTTOUR";

        public string ConnectionString { 
            get {
                return _strConnection;
            }
            set {
                _strConnection = value;
            }
        }

        public DAL()
        {

        }

        public DAL(string pStrConnection)
        {
            _strConnection = pStrConnection;

        }

        public IDbConnection GetConnection()
        {
            IDbConnection result;
            result = new OracleConnection(_strConnection);
            return result;
        }

        public IDbCommand GetCommand(string pStrCommand , IDbConnection pConnection)
        {
            if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
            {
                return null;
            }
            IDbCommand result = null;
            var con = pConnection as OracleConnection;
            if (con == null)
            {
                return null;
            }
            result = new OracleCommand(pStrCommand, con);
            return result;
        }

        public IDataAdapter GetDataAdaptor(string pStrCommand, IDbConnection pConnection)
        {
            if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
            {
                return null;
            }            
            var con = pConnection as OracleConnection;
            if (con == null)
            {
                return null;
            }
            IDataAdapter result = null;
            result = new OracleDataAdapter(pStrCommand , con);
            return result;
        }

        public DataSet GetDataSet(string pStrCommand)
        {
            var con = GetConnection();
            var da = GetDataAdaptor(pStrCommand, con);
            var result = new DataSet();
            da.Fill(result);
            con.Close();
            con.Dispose();
            con = null;           
            da = null;
            GC.Collect();
            return result;
        }

        public IEnumerable<T> ReadData<T>(string pStrCommand, Func<IDataReader, T> pFuncReadData)
        {
            if (string.IsNullOrEmpty(pStrCommand) || pFuncReadData == null)
            {
                yield break;
            }
            var con = GetConnection();
            
            var com = GetCommand(pStrCommand, con);

            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }


            var reader = com.ExecuteReader();
            while (reader.Read())
            {
                var result = pFuncReadData(reader);
                if (!EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    yield return result;
                }
            }
            reader.Close();
            reader.Dispose();
            reader = null;

            con.Close();
            con.Dispose();
            con = null;

            com.Dispose();
            com = null;

            GC.Collect();
        }
    }
}