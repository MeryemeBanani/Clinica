using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Clinica
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["ClinicaContext"].ConnectionString;

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT Ruolo From Utenti where UtenteID = @username", sqlConnection);
                cmd.Parameters.AddWithValue("username", username);
                sqlConnection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string ruolo = reader["Ruolo"].ToString();
                    roles.Add(ruolo);

                }

                sqlConnection.Close();

            }catch(Exception ex)
            {
                sqlConnection.Close();
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] roles = GetRolesForUser(username);
            return roles.Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}