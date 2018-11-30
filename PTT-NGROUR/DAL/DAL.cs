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
        //Production
        private string _strConnection = "data source=10.120.2.151:1522/PTTGIS2;password=PTTOUR;persist security info=True;user id=PTTOUR";
        //Test
        //private string _strConnection = "data source=10.120.2.125:1561/TGIS;password=PTTOUR;persist security info=True;user id=PTTOUR;";
        public string ConnectionString
        {
            get
            {
                return _strConnection;
            }
            set
            {
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
            try
            {
                result = new OracleConnection(_strConnection);
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public IDbCommand GetCommand(string pStrCommand, IDbConnection pConnection)
        {
            try
            {
                if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
                {
                    return null;
                }

                IDbCommand result = null;
                if (!(pConnection is OracleConnection con))
                {
                    return null;
                }
                result = new OracleCommand(pStrCommand, con);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IDataAdapter GetDataAdaptor(string pStrCommand, IDbConnection pConnection)
        {
            try
            {
                if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
                {
                    return null;
                }
                if (!(pConnection is OracleConnection con))
                {
                    return null;
                }
                IDataAdapter result = null;
                result = new OracleDataAdapter(pStrCommand, con);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetDataSet(string pStrCommand)
        {
            try
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<T> ReadData<T>(string pStrCommand, Func<IDataReader, T> pFuncReadData)
        {
            if (string.IsNullOrEmpty(pStrCommand) || pFuncReadData == null)
            {
                yield break;
            }
            var con = GetConnection();
            if (con == null)
            {
                yield break;
            }
            var com = GetCommand(pStrCommand, con);
            if (com == null)
            {
                yield break;
            }
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

        public object ExecuteScalar(string pStrCommand)
        {
            using (var con = GetConnection())
            {
                return ExecuteScalar(pStrCommand, con);
            };
        }

        public object ExecuteScalar(string pStrCommand, IDbConnection pConnection)
        {
            try
            {
                if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
                {
                    return null;
                }
                if (!(pConnection is OracleConnection con))
                {
                    return null;
                }
                var com = GetCommand(pStrCommand, con);
                if (com == null)
                {
                    return null;
                }
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                var result = com.ExecuteScalar();

                com.Dispose();
                com = null;
                GC.Collect();
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void ExecuteNonQuery(string pStrCommand)
        {
            try
            {
                using (var con = GetConnection())
                {
                    ExecuteNonQuery(pStrCommand, con);
                };
            }
            catch(Exception ex)
            {

            }
        }
        public void ExecuteNonQuery(string pStrCommand, IDbConnection pConnection)
        {
            if (string.IsNullOrEmpty(pStrCommand) || pConnection == null)
            {
                return;
            }
            if (!(pConnection is OracleConnection con))
            {
                return;
            }
            var com = GetCommand(pStrCommand, con);
            if (com == null)
            {
                return;
            }
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            int result = com.ExecuteNonQuery();
            com.Dispose();
            com = null;
            GC.Collect();
        }
    }
}