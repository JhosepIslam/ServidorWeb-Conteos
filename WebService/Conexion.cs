using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace WebService
{
    public class Conexion
    {
        SqlConnection conexion;
        public SqlConnection abrirConexion()
        {//

            conexion = new SqlConnection("Data Source = localhost; Initial Catalog = UTEC_CONTEOS; trusted_connection = true");

            return conexion;
        }
    }
}