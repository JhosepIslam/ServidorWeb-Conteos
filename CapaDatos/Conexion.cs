using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace CapaDatos
{
    public class Conexion
    {
        SqlConnection conexion;
        public SqlConnection abrirConexion()
        {
            conexion = new SqlConnection("Data Source = .; Initial Catalog = UTEC_CONTEOS; trusted_connection = true");
            return conexion;
        }
    }
}
