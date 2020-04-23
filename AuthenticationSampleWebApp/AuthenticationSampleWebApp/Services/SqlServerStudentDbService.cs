using AuthenticationSampleWebApp.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSampleWebApp.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        public bool CheckUserPassword(LoginRequestDto index)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "select * from student where IndexNumber=@index AND Password=@password";
                com.Parameters.AddWithValue("index", index.Login);
                com.Parameters.AddWithValue("password", index.Haslo);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return false;
                }
            }
            return true;
        }

        public void AddToken(string requestLogin, string refToken)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "UPDATE Student SET Token=@token WHERE IndexNumber=@index";
                com.Parameters.AddWithValue("index", requestLogin);
                com.Parameters.AddWithValue("token", refToken);
                var dr = com.ExecuteReader();
            }

        }

        public void ChangeToken(string refToken, string newRefToken)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "UPDATE Student SET Token=@newRefToken WHERE Token=@refToken";
                com.Parameters.AddWithValue("refToken", refToken);
                com.Parameters.AddWithValue("newRefToken", newRefToken);
                var dr = com.ExecuteReader();
            }
        }

        public string GetSalt(string index)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT salt FROM Student WHERE IndexNumber=@index";
                com.Parameters.AddWithValue("index", index);
                var dr = com.ExecuteReader();
                return dr.ToString();
            }
        }

        //====wrocic
        public void EnrollStudent(EnrollStudentRequest request)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.CommandText = "select IdStudies from studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                    }

                    int idstudies = (int)dr["IdStudies"];

                    com.CommandText = "select * from Enrollment where semester=1 AND idstudy=@idstudy";
                    com.Parameters.AddWithValue("idstudy", idstudies);
                    dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        com.CommandText =
                            "Insert Into Enrollment VALUES ((SELECT MAX(IdEnrollment) FROM Enrollment) + 1,"
                            + "1,@idstudy,@date";
                        com.Parameters.AddWithValue("idstudy", idstudies);
                        com.Parameters.AddWithValue("date", DateTime.Now);
                        com.ExecuteReader();
                        com.CommandText = "select * from Enrollment where semester=1 AND idstudy=@idstudy";
                        dr = com.ExecuteReader();
                    }

                    var idEnrollment = (int)dr["IdEnrollment"];

                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, Birthdate, IdEnrollment) "
                                      + "VALUES(@Index, @Fname, @Lname,@BDate,@Studies)";
                    com.Parameters.AddWithValue("index", request.IndexNumber);
                    com.Parameters.AddWithValue("fname", request.FirstName);
                    com.Parameters.AddWithValue("lname", request.LastName);
                    com.Parameters.AddWithValue("bdate", request.Birthdate);
                    com.Parameters.AddWithValue("studies", idEnrollment);
                    com.ExecuteNonQuery();

                    tran.Commit();

                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                }
            }
        }

        public void PromoteStudents(PromoteStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public bool CheckIndexNumber(string index)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select * from student where IndexNumber=@index";
                com.Parameters.AddWithValue("index", index);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckToken(string token)
        {
            var conBuilder = new SqlConnectionStringBuilder();
            conBuilder.DataSource = "db-mssql";
            conBuilder.InitialCatalog = "s17025";
            conBuilder.IntegratedSecurity = true;
            string ConString = conBuilder.ConnectionString;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select * from student where token=@token";
                com.Parameters.AddWithValue("token", token);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return false;
                }
            }
            return true;
        }






    }



}

