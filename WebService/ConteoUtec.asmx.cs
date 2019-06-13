using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Services;
using Newtonsoft.Json;
using System.Globalization;
using System.Collections;





namespace WebService
{
    /// <summary>
    /// Descripción breve de ConteoUtec
    /// </summary>
    [WebService(Namespace = "http://apoyo.conteoutec.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ConteoUtec : System.Web.Services.WebService
    {
        Conexion objConexion = new Conexion();

        //INICIAR SESION 
        [WebMethod]
        public int IniciarSesion(String usuario,String Clave)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
            int nivel = -1;

            try
            {
                cmd = new SqlCommand("SP_GET_USUARIOS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string UsuarioBd = dr["NOMBRE_USUARIO"].ToString();
                    string ClaveBd = dr["CLAVE"].ToString();
                    nivel = Convert.ToInt32(dr["NIVEL"]);

                    if (usuario.Equals(UsuarioBd) && Clave.Equals(ClaveBd))
                    {
                        conexion.Close();
                        cmd = new SqlCommand("SP_BITACORA_INICIO_DE_SESION", conexion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USER", usuario);
                        conexion.Open();
                        cmd.ExecuteNonQuery();

                        return nivel;
                    }
                    else return -1;
                }
                else return -1;                
            }
            catch (Exception x)
            {
                throw;                               
            }
            finally
            {
                conexion.Close();
            }           
        }



        //LISTAR FACULTADES
        [WebMethod]
        public DataSet ID_Facultad_UsuarioXML(string nombre_usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
     

            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_ID_FACULTAD", conexion);
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", nombre_usuario);
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ID_Facultad_UsuarioJSON(string nombre_usuario)
        {
            string json = JsonConvert.SerializeObject(ID_Facultad_UsuarioXML(nombre_usuario), Formatting.Indented);
            return json;
        }

        //VALIDAR SI USUARIO EXISTE
        [WebMethod]
        public bool ValidarUsuario(string usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
             

            try
            {
                cmd = new SqlCommand("SP_GET_ID_USUARIO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO",usuario);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return true;
                }
                else return false;
                
              
            }
            catch(Exception x)
            {
                return false;
                
            }
            finally
            {
                conexion.Close();
            }

          
        }

        //AGREGAR USUARIO DIFERENTE DE ADMIN_LAB Y INSTRUCTOR 
        [WebMethod]
        public bool AgregarUsuario(string usuario, string clave, string nivel, string nombre, string apellido, string facultad)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("SP_SET_USUARIOS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@PASS", clave);
                cmd.Parameters.AddWithValue("@NIVEL", nivel);
                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@APELLIDO", apellido);
                cmd.Parameters.AddWithValue("@FACULTAD", facultad);
                conexion.Open();
                cmd.ExecuteNonQuery();
                //si la consulta se efectua correctamente devolvemos verdadero
                return true;
            }
            catch (Exception x)
            {
                return false;
              
            }
            finally
            {
                conexion.Close();
            }
            
        }
        [WebMethod]
        public bool AgregarUsuarioDocente(string usuario, string clave, string nivel, string nombre, string apellido, string facultad, string codigo_empleado)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("SP_SET_USUARIO_DOCENTE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@PASS", clave);
                cmd.Parameters.AddWithValue("@NIVEL", nivel);
                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@APELLIDO", apellido);
                cmd.Parameters.AddWithValue("@FACULTAD", facultad);
                cmd.Parameters.AddWithValue("@CODIGO_EMPLEADO",codigo_empleado);

                conexion.Open();
                cmd.ExecuteNonQuery();
                //si la consulta se efectua correctamente devolvemos verdadero
                return true;
            }
            catch (Exception x)
            {
                return false;

            }
            finally
            {
                conexion.Close();
            }

        }
        //AGREGAR USUARIO ADMIN_LAB Y INSTRUCTOR 
        [WebMethod]
        public bool AgregarUsuario_AdminLab_Instructor(string usuario, string usuario_insert, string clave, string nivel, string nombre, string apellido, string facultad, string laboratorio)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_SET_USUARIOS_ADMIN_LAB_INSTRUCTOR", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@USUARIO_INSERT", usuario_insert);
                cmd.Parameters.AddWithValue("@PASS", clave);
                cmd.Parameters.AddWithValue("@NIVEL", nivel);
                cmd.Parameters.AddWithValue("@NOMBRE", nombre);
                cmd.Parameters.AddWithValue("@APELLIDO", apellido);
                cmd.Parameters.AddWithValue("@FACULTAD", facultad);
                cmd.Parameters.AddWithValue("@LABORATORIO", laboratorio);
                conexion.Open();
                cmd.ExecuteNonQuery();
                //si la consulta se efectua correctamente devolvemos verdadero
                return true;
            }

            catch (Exception x)
            {
                return false;
                throw x;
            }
            finally
            {
                conexion.Close();
            }

        }

        //LISTAR FACULTADES
        [WebMethod]
        public DataSet ListarFacultadesXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_FACULTADES", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ListarFacultadesJSON()
        {
            string json = JsonConvert.SerializeObject(ListarFacultadesXML(), Formatting.Indented);
            return json;
        }

        //LISTAR NIVELES 

        [WebMethod]
        public DataSet ListarNivelesXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_NIVELES", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ListarNivelesJSON()
        {
            string json = JsonConvert.SerializeObject(ListarNivelesXML(), Formatting.Indented);
            return json;
        }


        //LISTAR EDIFICIOS
        [WebMethod]
        public DataSet ListarEdificiosXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_EDIFICIOS", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ListarEdificiosJSON()
        {
            string json = JsonConvert.SerializeObject(ListarEdificiosXML(), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public int InsertarEdificio(string usuario, string Nombre, int Num_plantas, int num_aulas)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
            try
            {
                cmd = new SqlCommand("SP_SET_EDIFICIO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario.ToUpper());
                cmd.Parameters.AddWithValue("@NOMBRE", Nombre.ToUpper());
                cmd.Parameters.AddWithValue("@PLANTAS", Num_plantas);
                cmd.Parameters.AddWithValue("@AULAS", num_aulas);

                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    int id = int.Parse(dr["ID_EDIFICIO"].ToString());
                    return id;
                }
                return 0;


            }
            catch (Exception)
            {

                return 0;
            }
            finally
            {
                conexion.Close();
            }

        }

        //LISTAR ESCUELAS
        [WebMethod]
        public DataSet ListarEscuelasXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_ESCUELAS", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ListarEscuelasJSON()
        {
            string json = JsonConvert.SerializeObject(ListarEscuelasXML(), Formatting.Indented);
            return json;
        }

        //LISTAR LABORATORIOS
        [WebMethod]
        public DataSet ListarLaboratoriosXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_LABORATORIOS", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string ListarLaboratoriosJSON()
        {
            string json = JsonConvert.SerializeObject(ListarLaboratoriosXML(), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet ListarMateriasXML() {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_MATERIAS", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally {
                conexion.Close();
            }       

        }
        [WebMethod]
        public string ListarMateriasJSON()
        {
            string json = JsonConvert.SerializeObject(ListarMateriasXML(), Formatting.Indented);
            return json;
        }



        [WebMethod]
        public DataSet ListarMateriasPorCodigoXML(string Codigo)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataReader dr;
            
            SqlCommand cmd;
            try
            {                         
                cmd = new SqlCommand("SP_LIST_MATERIAS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CODIGO", Codigo.ToUpper());
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;            
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }

        }
        [WebMethod]
        public string ListarMateriasPorCodigoJSON(string Codigo)
        {
            string json = JsonConvert.SerializeObject(ListarMateriasPorCodigoXML(Codigo), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet ListarClasesXML()
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_CLASE", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }

        }
        [WebMethod]
        public string ListarClasesJSON()
        {
            string json = JsonConvert.SerializeObject(ListarClasesXML(), Formatting.Indented);
            return json;
        }
        [WebMethod]
        public DataSet ListarClasesPorCodigoXML(string Codigo)
        {
            SqlConnection conexion = objConexion.abrirConexion();

            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_LIST_CLASE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CODIGO", Codigo.ToUpper());
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }

        }
        [WebMethod]
        public string ListarClasesPorCodigoJSON(string Codigo)
        {
            string json = JsonConvert.SerializeObject(ListarClasesPorCodigoXML(Codigo), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet ListarUsuariosXML(){
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_ALL_USUARIO", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }       

        }
        [WebMethod]
        public string ListarUsuariosJSON() {
            string json = JsonConvert.SerializeObject(ListarUsuariosXML(), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet Get_an_UserXML(int ID) {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_AN_USER", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", ID);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }        
        }
        [WebMethod]
        public string Get_an_UserJSON(int ID) {
            string json = JsonConvert.SerializeObject(Get_an_UserXML(ID), Formatting.Indented);
            return json;        
        }

        [WebMethod]
        public DataSet Get_User_LikeXML(string usuario)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_USUARIO_LIKE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", usuario);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_User_LikeJSON(string usuario)
        {
            string json = JsonConvert.SerializeObject(Get_User_LikeXML(usuario), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet Get_an_User_Admin_IntructorXML(int ID)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_AN_USER_ADMIN_LAB_INSTRUCTOR", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO", ID);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_an_User_Admin_IntructorJSON(int ID)
        {
            string json = JsonConvert.SerializeObject(Get_an_User_Admin_IntructorXML(ID), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public DataSet Get_Clases_ActualesXML(string edificio , string hora , int Facultad)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime _date = DateTime.Now;
                string _dateString = _date.ToString("yyyy-MM-dd");
                string dia = System.DateTime.Now.DayOfWeek.ToString();
                dia = dia.Substring(0, 2);
                cmd = new SqlCommand("SP_GET_CURRENT_CLASES", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FECHA", _dateString);
                cmd.Parameters.AddWithValue("@EDIFICIO", edificio);
                cmd.Parameters.AddWithValue("@DIA_ACTUAL", getDay());
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@ID_FACULTAD", Facultad);


                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_ActualesJSON(string edificio , string hora, int Facultad)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_ActualesXML(edificio,hora, Facultad), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_Clases_ActualesAdminXML(string edificio, string hora)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime _date = DateTime.Now;
                string _dateString = _date.ToString("yyyy-MM-dd");
                string dia = System.DateTime.Now.DayOfWeek.ToString();
                dia = dia.Substring(0, 2);
                cmd = new SqlCommand("SP_GET_CURRENT_CLASES_ADMIN", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FECHA", _dateString);
                cmd.Parameters.AddWithValue("@EDIFICIO", edificio);
                cmd.Parameters.AddWithValue("@DIA_ACTUAL", getDay());
                cmd.Parameters.AddWithValue("@HORA", hora);


                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_ActualesAdminJSON(string edificio, string hora, int Facultad)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_ActualesAdminXML(edificio, hora), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_Clases_Actuales_DocenteXML(string hora, string Nombre_Usuario)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime _date = DateTime.Now;
                string _dateString = _date.ToString("yyyy-MM-dd");
                string dia = System.DateTime.Now.DayOfWeek.ToString();
                dia = dia.Substring(0, 2);
                cmd = new SqlCommand("SP_GET_CURRENT_CLASES_DOCENTES", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FECHA", _dateString);
                cmd.Parameters.AddWithValue("@DIA_ACTUAL", getDay());
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@USUARIO_DOCENTE", Nombre_Usuario);


                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_Actuales_DocenteJSON(string Nombre_usuario, string hora)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_Actuales_DocenteXML(hora, Nombre_usuario), Formatting.Indented);
            return json;
        }
        private string getDay() {

            string dia = System.DateTime.Now.DayOfWeek.ToString();

            switch (dia) {             
                case "Monday":
                    return "Lu";
                case "Tuesday":
                    return "Ma";
                case "Wednesday":
                    return "Mie";
                case "Thursday":
                    return "Jue";
                case "Friday":
                    return "Vie";
                case "Saturday":
                    return "Sab";
                case "Sunday":
                    return "Dom";
                default :
                    return dia.Substring(0, 2);
            }
        }
        [WebMethod]
        public DataSet Get_detalle_faltaXML() {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            try
            {
                conexion.Open();
                da = new SqlDataAdapter("SP_GET_DETALLE_FALTA", conexion);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }     
        }
        [WebMethod]
        public string Get_detalle_faltaJSON()
        {
            string json = JsonConvert.SerializeObject(Get_detalle_faltaXML(), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_Conteo_ifExistXML(int id_clase)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime _date = DateTime.Now;
                string _dateString = _date.ToString("yyyy-MM-dd");
                cmd = new SqlCommand("SP_GET_ASISTENCIA_IF_EXIST", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CLASE", id_clase);
                cmd.Parameters.AddWithValue("@FECHA_ACTUAL", _dateString);
                               conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Conteo_ifExistJSON(int id)
        {
            string json = JsonConvert.SerializeObject(Get_Conteo_ifExistXML(id), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_ALL_CONTEOSXML()
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime _date = DateTime.Now;
                string _dateString = _date.ToString("yyyy-MM-dd");
                cmd = new SqlCommand("SP_GET_ALL_CONTEOS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FECHA", _dateString);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Actualizar_Edificio_Nombre(string USuario, int ID_Edificio, string Nuevo_Nombre)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_UPDATE_EDIFICIO_NOMBRE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_EDIFICIO", ID_Edificio);
                cmd.Parameters.AddWithValue("@NUEVO_NOMBRE", Nuevo_Nombre.ToUpper());
                cmd.Parameters.AddWithValue("@USUARIO", USuario);             
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Actualizar_Edificio_Aulas(string USuario, int ID_Edificio, int Nueva_cantidad)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_UPDATE_EDIFICIO_AULAS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_EDIFICIO", ID_Edificio);
                cmd.Parameters.AddWithValue("@NUEVAS_AULAS", Nueva_cantidad);
                cmd.Parameters.AddWithValue("@USUARIO", USuario);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Actualizar_Edificio_Plantas(string USuario, int ID_Edificio, int Nueva_cantidad)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_UPDATE_EDIFICIO_PLANTAS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_EDIFICIO", ID_Edificio);
                cmd.Parameters.AddWithValue("@NUEVAS_PLANTAS", Nueva_cantidad);
                cmd.Parameters.AddWithValue("@USUARIO", USuario);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }


        [WebMethod]
        public string Get_ALL_CONTEOSJSON()
        {
            string json = JsonConvert.SerializeObject(Get_ALL_CONTEOSXML(), Formatting.Indented);
            return json;
        }

        [WebMethod]
        public bool Insertar_Conteo(string usuario , int id_clase , int cantidad) {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime Hoy = DateTime.Today;
                DateTime hora = DateTime.Now;
                string fecha_actual = Hoy.ToString("yyyy-MM-dd");
                string hora_actual = hora.ToString("HH:mm:ss");

                cmd = new SqlCommand("SP_SET_ASISTENCIA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@ID_CLASE", id_clase);
                cmd.Parameters.AddWithValue("@CANTIDA", cantidad);
                cmd.Parameters.AddWithValue("@FECHA", fecha_actual);
                cmd.Parameters.AddWithValue("@HORA", hora_actual);

                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public bool Actualizar_Conteo(string Nuevo_usuario, int id_Asistencia, int Nueva_cantidad)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime Hoy = DateTime.Today;
                DateTime hora = DateTime.Now;
                string fecha_actual = Hoy.ToString("yyyy-MM-dd");
                string hora_actual = hora.ToString("HH:mm:ss");

                cmd = new SqlCommand("SP_UPDATE_CONTEO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NUEVO_USUARIO", Nuevo_usuario);
                cmd.Parameters.AddWithValue("@ID_ASISTENCIA", id_Asistencia);
                cmd.Parameters.AddWithValue("@NUEVO_CONTEO", Nueva_cantidad);
                cmd.Parameters.AddWithValue("@NUEVA_FECHA", fecha_actual);
                cmd.Parameters.AddWithValue("@NUEVA_HORA", hora_actual);

                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Insertar_Falta(string usuario, int id_clase, string detalle , string comentario,string hora)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                DateTime Hoy = DateTime.Today;
                DateTime Hora = DateTime.Now;

                string fecha_actual = Hoy.ToString("yyyy-MM-dd");
                if (String.IsNullOrEmpty(hora)) {
                    hora = Hora.ToString("HH:mm:ss");
                }

                cmd = new SqlCommand("SP_SET_FALTA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@ID_CLASE", id_clase);
                cmd.Parameters.AddWithValue("@DETALLE", detalle);
                cmd.Parameters.AddWithValue("@FECHA", fecha_actual);
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@COMENTARIO", comentario);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Modificar_Clave(int usuario_update, string Nueva_Clave, string Usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {

                cmd = new SqlCommand("SP_UPDATE_CLAVE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO_UPDATE", usuario_update);
                cmd.Parameters.AddWithValue("@NUEVA_CLAVE", Nueva_Clave);
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", Usuario);
              
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Modificar_Facultad(int usuario_update, string Nueva_Facultad, string Usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {

                cmd = new SqlCommand("SP_UPDATE_FACULTAD", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO_UPDATE", usuario_update);
                cmd.Parameters.AddWithValue("@NUEVA_FACULTAD", Nueva_Facultad);
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", Usuario);

                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Modificar_Laboratorio(int usuario_update, string Nuevo_Laboratorio, string Usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {

                cmd = new SqlCommand("SP_UPDATE_LABORATORIO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO_UPDATE", usuario_update);
                cmd.Parameters.AddWithValue("@NUEVO_LABORATORIO", Nuevo_Laboratorio);
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", Usuario);

                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public bool Reset_Clave(int usuario_update, string Usuario)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {

                cmd = new SqlCommand("SP_RESET_CLAVE", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_USUARIO_RESET", usuario_update);
                cmd.Parameters.AddWithValue("@NOMBRE_USUARIO", Usuario);

                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }
       /**
        * 
        * retorna -1 ya hay otro ciclo comprendido en ese rango de fechas
        * retorna 1 se inserto 
        * retorna 0 fallo en la consulta o de conexion
        * 
        * **/

        [WebMethod]
        public int Habilitar_Ciclo(String usuario,string fecha_inicio , string fecha_fin, int ciclo)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
            string anio = fecha_inicio.ToString();
            anio = anio.Substring(0, 4);

            try
            {
                DateTime Hoy = DateTime.Today;                

                string fecha_actual = Hoy.ToString("yyyy-MM-dd");

                cmd = new SqlCommand("SP_CICLOS_HABILES", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CICLO", ciclo);
                cmd.Parameters.AddWithValue("@FECHA_INICIO", fecha_inicio);
                cmd.Parameters.AddWithValue("@FECHA_FIN", fecha_fin);

                conexion.Open();

               dr = cmd.ExecuteReader();

               if (!dr.Read())
               {
                   conexion.Close();
                   cmd = new SqlCommand("SP_ACTIVATE_CICLO", conexion);
                   cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.AddWithValue("@CICLO", ciclo);
                   cmd.Parameters.AddWithValue("@FECHA_INICIO", fecha_inicio);
                   cmd.Parameters.AddWithValue("@FECHA_FIN", fecha_fin);
                   cmd.Parameters.AddWithValue("@ANIO", anio);
                   cmd.Parameters.AddWithValue("@USUARIO", usuario);
                   conexion.Open();
                   cmd.ExecuteNonQuery();
                   return 1;

               }
               else {
                   return -1;
               }                     
                    
            }
            catch (Exception x)
            {
                return 0;

            }
            finally
            {
                conexion.Close();
            }

        }


       

        [WebMethod]
        public DataSet Get_ALL_CICLOSXML()
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {             
                cmd = new SqlCommand("SP_GET_CICLOS_HABILES", conexion);
                cmd.CommandType = CommandType.StoredProcedure;                
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_ALL_CICLOSJSON()
        {
            string json = JsonConvert.SerializeObject(Get_ALL_CICLOSXML(), Formatting.Indented);
            return json;
        }




        /**
         * 
         * retorna -1 , hay otro ciclo activo
         * retorna -2 rango de fecha entra en otro ciclo
         * retorna 1 se inserto 
         * retorna 0 fallo en la consulta o de conexion
         * retorna -3 rango de fechas no es correcto
         * 
         * **/

        [WebMethod]
        public int Modificar_Ciclo(string usuario , string fecha_inicio , string fecha_fin, int estado, string anio, int id_ciclo)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;

            DateTime Inicio = Convert.ToDateTime(fecha_inicio);

            DateTime Fin = Convert.ToDateTime(fecha_fin);

            if (Inicio > Fin)
            {
                return -3;
            }

            bool modificador = false;
            try
            {
                cmd = new SqlCommand("SP_GET_CICLO_ACTIVO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CICLO", id_ciclo);            
                conexion.Open();               
                dr = cmd.ExecuteReader();

                if (estado == 1)
                {
                    if (!dr.Read())
                    {
                        conexion.Close();
                        cmd = new SqlCommand("SP_VERIFICAR_FECHA_CICLO", conexion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FECHA_INICIO", fecha_inicio);
                        cmd.Parameters.AddWithValue("@FECHA_FIN", fecha_fin);
                        cmd.Parameters.AddWithValue("@ANIO", anio);
                        cmd.Parameters.AddWithValue("@ID_CICLO", id_ciclo);

                        conexion.Open();
                        dr = cmd.ExecuteReader();
                        if (!dr.Read())
                        {
                            modificador = true;
                        }
                        else {
                            return -2;
                        }
                    }
                   
                    }
                    else
                    {
                        conexion.Close();
                        cmd = new SqlCommand("SP_VERIFICAR_FECHA_CICLO", conexion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FECHA_INICIO", fecha_inicio);
                        cmd.Parameters.AddWithValue("@FECHA_FIN", fecha_fin);
                        cmd.Parameters.AddWithValue("@ANIO", anio);
                        cmd.Parameters.AddWithValue("@ID_CICLO", id_ciclo);

                        conexion.Open();
                        dr = cmd.ExecuteReader();
                        if (!dr.Read())
                        {
                            modificador = true;
                        }
                        else
                        {
                            return -2;
                        }
                }



                if (modificador)
                {
                    conexion.Close();
                    cmd = new SqlCommand("SP_UPDATE_CICLO", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CICLO", id_ciclo);
                    cmd.Parameters.AddWithValue("@FECHA_INICIO", fecha_inicio);
                    cmd.Parameters.AddWithValue("@FECHA_FIN", fecha_fin);
                    cmd.Parameters.AddWithValue("@ANIO", anio);
                    cmd.Parameters.AddWithValue("@ESTADO", estado);
                    cmd.Parameters.AddWithValue("@USUARIO", usuario);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    return 1;
                }
                else {
                    return -1;
                }

            }
            catch (Exception x)
            {
                return 0;
            }
            finally
            {
                conexion.Close();
            }

        }

        /**
         * 
         * retorna 0 rango de fechas no es valido
         * retorna -1 no se puede agregar mas parciales al ciclo
         * retorna -2 este parcial ya fue agregado 
         * retorna -3 la fecha interfiere con otros parciales
         * retorna -4 falla en consulta
         * retorna -5 fechas estan fuera de rango del ciclo
         * reorna 1 insetado
         * 
         * **/
        [WebMethod]
        public int Insertar_Parcial(String Usuario ,int  ID_Ciclo, string Fecha_Inicio , string Fecha_Fin, int Numero_Parcial ) {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
            int Cantida = 0;
            try
            {
                DateTime Inicio = Convert.ToDateTime(Fecha_Inicio);

                DateTime Fin = Convert.ToDateTime(Fecha_Fin);

                if (Inicio > Fin)
                {
                    return 0;
                }
                cmd = new SqlCommand("SP_GET_RANGO_CICLO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (!dr.Read())
                {
                    return -5;
                }
                conexion.Close();
                cmd = new SqlCommand("SP_GET_PARCIALES_CANTIDA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Cantida = Convert.ToInt32(dr["CANTIDAD"]);
                    conexion.Close();
                    if (Cantida >= 5)
                    {
                        return -1;
                    }
                    else
                    {
                        cmd = new SqlCommand("SP_GET_PARCIAL_NUMERO", conexion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                        cmd.Parameters.AddWithValue("@ID_PARCIAL", Numero_Parcial);
                        conexion.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            return -2;
                        }
                        else
                        {
                            conexion.Close();
                            cmd = new SqlCommand("SP_GET_PARCIAL_FECHA", conexion);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                            cmd.Parameters.AddWithValue("@ID_PARCIAL", Numero_Parcial);
                            cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                            cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                            conexion.Open();
                            dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                return -3;
                            }
                            else
                            {
                                ///inserta parcial
                                ///
                                conexion.Close();
                                cmd = new SqlCommand("SP_SET_FECHA_PARCIAL", conexion);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                                cmd.Parameters.AddWithValue("@ID_PARCIAL", Numero_Parcial);
                                cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                                cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                                cmd.Parameters.AddWithValue("@USUARIO", Usuario);
                                conexion.Open();
                                cmd.ExecuteNonQuery();

                                return 1;
                            }
                        }
                    }
                }
                else {
                    return -4;
                }
                

            }
            catch (Exception x)
            {
                return -4;
                throw;

            }
            finally
            {
                conexion.Close();
            }
           




        }

        /**
        * 
        * retorna 0 rango de fechas no es valido
        * retorna -2 la fecha interfiere con otros parciales
        * retorna -4 falla en consulta
        * retorna -5 fechas estan fuera de rango del ciclo
        * reorna 1 actualizado
        * 
        * **/

        [WebMethod]
        public int Actualizar_Parcial(String Usuario, int ID_Ciclo, string Fecha_Inicio, string Fecha_Fin, int ID_Fecha_Parcial)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            SqlDataReader dr;
         
            int Numero_Parcial =0;
            try
            {
                DateTime Inicio = Convert.ToDateTime(Fecha_Inicio);

                DateTime Fin = Convert.ToDateTime(Fecha_Fin);

                if (Inicio > Fin)
                {
                    return 0;
                }
                cmd = new SqlCommand("SP_GET_RANGO_CICLO", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (!dr.Read())
                {
                    return -5;
                }

                conexion.Close();
                cmd = new SqlCommand("SP_GET_NUMERO_PARCIAL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_FECHA_PARCIAL", ID_Fecha_Parcial);               
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Numero_Parcial =  Convert.ToInt32(dr["NUMERO_PARCIAL"]);
                }

                                
                conexion.Close();
                cmd = new SqlCommand("SP_GET_PARCIAL_FECHA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                cmd.Parameters.AddWithValue("@ID_PARCIAL", Numero_Parcial);
                cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return -2;
                }
                else
                {
                    ///actualizar parcial
                    ///
                    conexion.Close();
                    cmd = new SqlCommand("SP_UPDATE_FECHA_PARCIAL", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                    cmd.Parameters.AddWithValue("@ID_FECHA_PARCIAL", ID_Fecha_Parcial);
                    cmd.Parameters.AddWithValue("@FECHA_INICIO", Fecha_Inicio);
                    cmd.Parameters.AddWithValue("@fECHA_FIN", Fecha_Fin);
                    cmd.Parameters.AddWithValue("@USUARIO", Usuario);
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    return 1;
                }
                        
                    
                
                
            }
            catch (Exception x)
            {
                return -4;
                throw;

            }
            finally
            {
                conexion.Close();
            }
        }



        [WebMethod]
        public DataSet Get_Parciales_CicloXML(int ID_Ciclo)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_PARCIALES", conexion);
                cmd.Parameters.AddWithValue("@ID_CICLO", ID_Ciclo);
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Parciales_CicloJSON(int ID_Ciclo)
        {
            string json = JsonConvert.SerializeObject(Get_Parciales_CicloXML(ID_Ciclo), Formatting.Indented);
            return json;
        }




        [WebMethod]
        public DataSet Get_Edificio_DiaXML(int facultad)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_EDIFICIO_DIA", conexion);
                cmd.Parameters.AddWithValue("@DIA", getDay());
                cmd.Parameters.AddWithValue("@FACULTAD", facultad);
                cmd.CommandType = CommandType.StoredProcedure;
                
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Edificio_DiaJSON(int facultad)
        {
            string json = JsonConvert.SerializeObject(Get_Edificio_DiaXML(facultad), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_Edificio_Dia_AdminXML()
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;

            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_EDIFICIO_DIA_ADMIN", conexion);
                cmd.Parameters.AddWithValue("@DIA", getDay());
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Edificio_Dia_AdminJSON()
        {
            string json = JsonConvert.SerializeObject(Get_Edificio_Dia_AdminXML(), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public DataSet Get_Clases_ContadasXML(string fecha, int id_facultad, string hora)
        {
            string Fecha = fecha;
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            DateTime f = Convert.ToDateTime(fecha);
            Fecha = f.ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(Fecha))
            {
                 DateTime Hoy = DateTime.Today;
                 Fecha = Hoy.ToString("yyyy-MM-dd");
            }


            SqlCommand cmd;
            try
            {
                string dia =getDayByFecha(Fecha);
                cmd = new SqlCommand("SP_GET_CLASES_CON_ASISTENCIA", conexion);
                cmd.Parameters.AddWithValue("@FECHA", Fecha);
                cmd.Parameters.AddWithValue("@ID_FACULTAD", id_facultad);
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@DIA", dia);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_ContadasJSON(string fecha,int id_facultad, string hora)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_ContadasXML(fecha,id_facultad,hora), Formatting.Indented);
            return json;
        }




        [WebMethod]
        public DataSet Get_Clases_Sin_ConteoXML(string fecha, int id_facultad, string hora)
        {
            string Fecha = fecha;
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            DateTime f = Convert.ToDateTime(fecha);
            Fecha = f.ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(Fecha))
            {
                DateTime Hoy = DateTime.Today;
                Fecha = Hoy.ToString("yyyy-MM-dd");
            }


            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_CLASES_SIN_ASISTENCIA", conexion);
                cmd.Parameters.AddWithValue("@FECHA", Fecha);
                cmd.Parameters.AddWithValue("@ID_FACULTAD", id_facultad);
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@DIA", getDayByFecha(Fecha));

                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_Sin_ConteoJSON(string fecha, int id_facultad, string hora)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_Sin_ConteoXML(fecha,id_facultad,hora), Formatting.Indented);
            return json;
        }

        private string getDayByFecha(string fecha)
        {

            DateTime d = Convert.ToDateTime(fecha);
            int Dia = Convert.ToInt32(d.ToString("dd"));
            int Mes = Convert.ToInt32(d.ToString("MM"));
            int Anio = Convert.ToInt32(d.ToString("yyyy"));

            DateTime dateValue = new DateTime(Anio, Mes, Dia);
            string dia = dateValue.ToString("ddddddddd");

            switch (dia)
            {
                case "Monday":
                    return "Lu";
                case "Tuesday":
                    return "Ma";
                case "Wednesday":
                    return "Mie";
                case "Thursday":
                    return "Jue";
                case "Friday":
                    return "Vie";
                case "Saturday":
                    return "Sab";
                case "Sunday":
                    return "Dom";
                case "sábado" :
                    return "Sab";
                case "Sábado":
                    return "Sab";
                default:
                    return dia.Substring(0, 2);
            }
        }





        [WebMethod]
        public DataSet Get_Dias_ParcialesXML(int ID_Parcial)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;


            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_FECHA_HORA_PARCIALES", conexion);
                cmd.Parameters.AddWithValue("@ID_PARCIAL", ID_Parcial);
             
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Dias_ParcialesJSON(int ID_Parcial)
        {
            string json = JsonConvert.SerializeObject(Get_Dias_ParcialesXML(ID_Parcial), Formatting.Indented);
            return json;
        }


        /***
         * 
         * 0 error en consulta
         * -1 hay otra fecha ingual asignada
         * -2 la fecha no pertenece al rango del la semana de parcial
         * 1 segun TIPO si TIPO = 1 , se inserto , si TIPO= 2 , se actualizó
         * -3 rango equivocado
         * 
         ***/

        [WebMethod]
        public int Set_Dias_Parciales(int ID_Parcial,string fecha , string hora_inicio, string hora_fin,string usuario,int ID_Ciclo, int TIPO)
        {
            //dr = cmd.ExecuteReader();
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            SqlCommand cmd;
            SqlDataReader dr;
            DateTime Fecha = Convert.ToDateTime(fecha);
            string fecha_ = Fecha.ToString("yyyy-MM-dd");


            try
            {              
                    cmd = new SqlCommand("SP_VERIFICAR_DIA_HORA_PARCIAL", conexion);
                    cmd.Parameters.AddWithValue("@FECHA", fecha_);
                    cmd.Parameters.AddWithValue("@HORA_INICIO", hora_inicio);
                    cmd.Parameters.AddWithValue("@HORA_FIN", hora_fin);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return -1;
                    }
                    conexion.Close();
                               

                     cmd = new SqlCommand("SP_VERFICAR_FECHA_PARCIAL", conexion);
                    cmd.Parameters.AddWithValue("@ID_PARCIAL", ID_Parcial);
                    cmd.Parameters.AddWithValue("@FECHA", fecha_);
                    cmd.Parameters.AddWithValue("@CICLO", ID_Ciclo);                 
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        DateTime fecha1 = Convert.ToDateTime(hora_inicio);

                        DateTime fecha2 = Convert.ToDateTime(hora_fin);

                        if (fecha1 < fecha2)
                        {
                            conexion.Close();
                            cmd = new SqlCommand("SP_SET_DIA_HORA_PARCIAL", conexion);
                            cmd.Parameters.AddWithValue("@ID_FECHA_PARCIAL", ID_Parcial);
                            cmd.Parameters.AddWithValue("@FECHA", fecha_);
                            cmd.Parameters.AddWithValue("@HORA_INICIO", hora_inicio);
                            cmd.Parameters.AddWithValue("@HORA_FIN", hora_fin);
                            cmd.Parameters.AddWithValue("@USUARIO", usuario);
                            cmd.Parameters.AddWithValue("@CICLO", ID_Ciclo);
                            cmd.CommandType = CommandType.StoredProcedure;
                            conexion.Open();
                            cmd.ExecuteNonQuery();

                            return 1;
                        }
                        else {
                            return -3;
                        }
                       
                    }
                    else {

                        return -2;
                    }

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conexion.Close();
            }
        }




        /***
        * 
        * 0 error en consulta
        * -1 hay otra fecha ingual asignada
        * -2 la fecha no pertenece al rango del la semana de parcial
        * 1 segun TIPO si TIPO = 1 , se inserto , si TIPO= 2 , se actualizó
        * -3 rango equivocado
        * 
        * 
        * Parametro ID_Parcial es el ID_DIA_HORA_PARCIAL  de la Tabla DIA_HORA_PARCIAL
        * 
        ***/

        [WebMethod]
        public int Actualizar_Dias_Parciales(int ID_Parcial , string fecha, string hora_inicio, string hora_fin, string usuario)
        {
            //dr = cmd.ExecuteReader();
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            SqlCommand cmd;
            SqlDataReader dr;
            DateTime Fecha = Convert.ToDateTime(fecha);
            string fecha_ = Fecha.ToString("yyyy-MM-dd");



            try
            {

                cmd = new SqlCommand("SP_VALIDAR_HORA_UPDATE", conexion);
                cmd.Parameters.AddWithValue("@ID_DIA_HORA_PARCIAL", ID_Parcial);
                cmd.Parameters.AddWithValue("@HORA_INICIO", hora_inicio);
                cmd.Parameters.AddWithValue("@HORA_FIN", hora_fin);
                cmd.Parameters.AddWithValue("@FECHA", fecha_);
                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return -1;
                }
                else
                {
                    DateTime fecha1 = Convert.ToDateTime(hora_inicio);

                        DateTime fecha2 = Convert.ToDateTime(hora_fin);

                        if (fecha1 < fecha2)
                        {
                            conexion.Close();

                            cmd = new SqlCommand("SP_UPDATE_DIA_HORA_PARCIAL", conexion);
                            cmd.Parameters.AddWithValue("@ID_DIA_HORA_PARCIAL", ID_Parcial);
                            cmd.Parameters.AddWithValue("@HORA_INICIO", hora_inicio);
                            cmd.Parameters.AddWithValue("@HORA_FIN", hora_fin);
                            cmd.Parameters.AddWithValue("@USUARIO", usuario);
                            cmd.CommandType = CommandType.StoredProcedure;
                            conexion.Open();
                            cmd.ExecuteNonQuery();
                            return 1;
                        }
                        else {
                            return -3;
                        }
                }


              

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conexion.Close();
            }
        }


        [WebMethod]
        public DataSet Get_Fecha_ParcialesXML(int ID_Parcial)
        {
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataReader dr;
          
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_FECHA_HORA_PARCIALES", conexion);
                cmd.Parameters.AddWithValue("@ID_PARCIAL", ID_Parcial);

                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                dr= cmd.ExecuteReader();
                if (dr.Read())
                {
                    
                    ArrayList Fechas = new ArrayList();
                    var F_inicio = Convert.ToDateTime(dr["FECHA_INICIO"].ToString());
                    var F_fin = Convert.ToDateTime(dr["FECHA_FIN"].ToString());

                    for (var date = F_inicio; date <= F_fin; date = date.AddDays(1))
                    {
                        Fechas.Add(date);

                    }
                    int c = Fechas.Count;
                    string[,] fechas = new string[c, 2];
                    for (int i = 0; i < c; i++)
                    {
                        string f = Convert.ToDateTime(Fechas[i].ToString()).ToString("dd-MM-yyy");
                        fechas[i, 0] = f;

                    }
                    for (int j = 0; j < Fechas.Count; j++)
                    {
                        DateTime d = Convert.ToDateTime(Fechas[j].ToString());
                        int Dia = Convert.ToInt32(d.ToString("dd"));
                        int Mes = Convert.ToInt32(d.ToString("MM"));
                        int Anio = Convert.ToInt32(d.ToString("yyyy"));

                        DateTime dateValue = new DateTime(Anio, Mes, Dia);
                        string dia = dateValue.ToString("ddddddddd");

                        fechas[j, 1] = dia;

                    }

                    DataTable dt = new DataTable();
                   
                  
                    DataRow row; 
                      DataColumn column;


                     column = new DataColumn();
                        column.DataType = typeof(System.String);
                        column.ColumnName = "Fechas";
                        dt.Columns.Add(column);

                        // Create second column.
                        column = new DataColumn();
                        column.DataType = typeof(System.String);
                        column.ColumnName = "Dias";
                        dt.Columns.Add(column);



                    for (int i = 0; i < fechas.GetLength(0); i++)
                    {
                        
                            row = dt.NewRow();
                            row["Fechas"]=fechas[i, 0].Substring(0,10);
                            row["Dias"] = fechas[i,1];

                            dt.Rows.Add(row);

                    }
                        

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    return ds;
                }
                return null;
            
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Fecha_ParcialesJSON(int ID_Parcial)
        {
            string json = JsonConvert.SerializeObject(Get_Fecha_ParcialesXML(ID_Parcial), Formatting.Indented);
            return json;
        }


        /*ACTUALIZAR CLASES*/
        [WebMethod]
        public bool Actualizar_ClasesExcel(string usuario, string escuela, string codMateria, string dias, string codEmpleado, string aula, int nuevosInscritos)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_UPDATE_DATOS_CLASES", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@ESCUELA", escuela.ToUpper());
                cmd.Parameters.AddWithValue("@COD_MATERIA", codMateria);
                cmd.Parameters.AddWithValue("@DIAS", dias);
                cmd.Parameters.AddWithValue("@COD_EMPLEADO", codEmpleado);
                cmd.Parameters.AddWithValue("@AULA", aula);
                cmd.Parameters.AddWithValue("@NUEVOS_INSCRITOS", nuevosInscritos);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        /*INSERTAR CLASES*/
        [WebMethod]
        public bool Insertar_ClasesExcel(string usuario, string codMateria, string escuela, string nomDocente, string aula, string ciclo_anio, int seccion, int numInscritos, string dias, string horas)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_INSERT_DATOS_CLASES_EXCEL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@COD_MATERIA", codMateria);
                cmd.Parameters.AddWithValue("@ESCUELA", escuela.ToUpper());
                cmd.Parameters.AddWithValue("@DOCENTE", nomDocente);
                cmd.Parameters.AddWithValue("@AULA", aula);
                cmd.Parameters.AddWithValue("@CICLO_ANIO", ciclo_anio);
                cmd.Parameters.AddWithValue("@SECCION", seccion);
                cmd.Parameters.AddWithValue("@NUM_INSCRITOS", numInscritos);
                cmd.Parameters.AddWithValue("@DIAS", dias);
                cmd.Parameters.AddWithValue("@HORA", horas);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        /*INSERTAR AULAS*/
        [WebMethod]
        public bool Insertar_Aulas_Excel(string edif_aula, int capacidad)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_INSERT_AULA_EXCEL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EDIF_AULA", edif_aula);
                cmd.Parameters.AddWithValue("@CAPACIDAD", capacidad);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        /*INSERTAR DOCENTES*/
        [WebMethod]
        public bool Insertar_Docentes_Excel(string nom_docente, int cod_empleado)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_INSERT_DOCENTE_EXCEL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NOMBRE_DOCENTE", nom_docente);
                cmd.Parameters.AddWithValue("@COD_EMPLEADO", cod_empleado);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        /*Obtener id aula*/
        [WebMethod]
        public DataSet GET_ID_AULA_Excel(string aula)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_AULA_ID_EXCEL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AULA", aula);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }






        //metodo de retorno de comportamiento de asistencia por Escuela para grafica
        [WebMethod]
        public DataSet GET_REPORTE_ESCUELA_GRAFICA(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_ASISTENCIA_ESCUELAS_GRAFICA_", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }

        //metodo de retorno de comportamiento de asistencia por Facutad para grafica
        [WebMethod]
        public DataSet GET_REPORTE_FACULTAD_GRAFICA(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_ASISTENCIA_FACULTADES_GRAFICA_", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }


        //metodo de retorno de comportamiento de Inasistencia por Escuela para grafica
        [WebMethod]
        public DataSet GET_REPORTE_ESCUELA_GRAFICA_INASISTENCIA(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_INASISTENCIA_ESCUELAS_GRAFICA_", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }

        //metodo de retorno de comportamiento de Inasistencia por Facutad para grafica
        [WebMethod]
        public DataSet GET_REPORTE_FACULTAD_GRAFICAINASISTENCIA(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_INASISTENCIA_FACULTADES_GRAFICA_", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }




        //*********************************************************************************************//

        //metodo de retorno de comportamiento de asistencia por escuelas
        [WebMethod]
        public DataSet GET_REPORTE_ESCUELASXML(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_REPORTE_ESCUELAS", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }

        //metodo de retorno de comportamiento de asistencia por facultad
        [WebMethod]
        public DataSet GET_REPORTE_FACULTADXML(string fechaI, string fechaF)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_REPORTE_CLASES_NORMALES_FACULTAD", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaI", fechaI);
                cmd.Parameters.AddWithValue("@fechaF", fechaF);
                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }

        //Prueba subida excel
        [WebMethod]
        public bool Actualizar_DocentesExcel(int ID_Docente, string NombreDocente, string ApellidoDocente, int codEmpleado)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_ActualizarDocentesExcel", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_DOCENTE", ID_Docente);
                cmd.Parameters.AddWithValue("@NombreDocente", NombreDocente.ToUpper());
                cmd.Parameters.AddWithValue("@ApellidoDocente", ApellidoDocente);
                cmd.Parameters.AddWithValue("@COD_Empleado", codEmpleado);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }



        [WebMethod]
        public DataSet Get_DIA_HORA_ParcialXML(int ID_Parcial, string fecha)
        {
            DateTime Fecha =Convert.ToDateTime(fecha);

            string _dateString = Fecha.ToString("yyyy-MM-dd");

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try

            {
                cmd = new SqlCommand("SP_GET_DIA_HORA_PARCIAL_POR_ID_PARCIAL_FECHA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_PARCIAL", ID_Parcial);
                cmd.Parameters.AddWithValue("@FECHA", _dateString);

                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conexion.Close();
            }
        }

        [WebMethod]
        public string Get_DIA_HORA_ParcialJSON(int ID_Parcial, string fecha)
        {
            string json = JsonConvert.SerializeObject(Get_DIA_HORA_ParcialXML(ID_Parcial, fecha), Formatting.Indented);
            return json;
        }


        [WebMethod]
        public int Eliminar_Hora_Parcial(string usuario, int id_DIA_HORA)
        {

            SqlConnection conexion = objConexion.abrirConexion();
            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_DELETE_HORA_PARCIAL", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@ID_DIA_HORA_PARCIAL", id_DIA_HORA);
                conexion.Open();
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conexion.Close();
            }
        }




        [WebMethod]
        public DataSet Get_Clases_Con_FaltaXML(string fecha, int id_facultad, string hora)
        {
            string Fecha = fecha;
            SqlConnection conexion = objConexion.abrirConexion();
            SqlDataAdapter da;
            DateTime f = Convert.ToDateTime(fecha);
            Fecha = f.ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(Fecha))
            {
                DateTime Hoy = DateTime.Today;
                Fecha = Hoy.ToString("yyyy-MM-dd");
            }


            SqlCommand cmd;
            try
            {
                cmd = new SqlCommand("SP_GET_CLASES_CON_FALTAS", conexion);
                cmd.Parameters.AddWithValue("@FECHA", Fecha);
                cmd.Parameters.AddWithValue("@ID_FACULTAD", id_facultad);
                cmd.Parameters.AddWithValue("@HORA", hora);
                cmd.Parameters.AddWithValue("@DIA", getDayByFecha(Fecha));

                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                SqlDataAdapter theDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                theDataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conexion.Close();
            }
        }
        [WebMethod]
        public string Get_Clases_Con_FaltaJSON(string fecha, int id_facultad, string hora)
        {
            string json = JsonConvert.SerializeObject(Get_Clases_Con_FaltaXML(fecha, id_facultad, hora), Formatting.Indented);
            return json;
        }


    }
}
