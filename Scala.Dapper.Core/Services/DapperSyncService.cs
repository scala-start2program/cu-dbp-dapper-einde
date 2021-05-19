using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Scala.Dapper.Core.Entities;


namespace Scala.Dapper.Core.Services
{
    public class DapperSyncService
    {
        // ==============
        // CRUD AUTEURS
        // ==============
        public List<Auteur> GetAuteurs()
        {
            List<Auteur> auteurs = new List<Auteur>();
            string sql = "select id, naam from auteurs order by naam";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                auteurs = connection.Query<Auteur>(sql).ToList();
                connection.Close();
            }
            return auteurs;
        }
        public bool AuteurToevoegen(Auteur auteur)
        {
            string sql = "insert into auteurs (id, naam) values (@id, @naam)"; 
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, auteur);
                connection.Close();
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool AuteurWijzigen(Auteur auteur)
        {
            string sql = "update auteurs set naam = @naam where id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, auteur);
                connection.Close();
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool AuteurVerwijderen(Auteur auteur)
        {
            if (IsAuteurInGebruik(auteur))
                return false;
            string sql = "delete from auteurs where id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, auteur);
                connection.Close();
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }

        }
        public bool IsAuteurInGebruik(Auteur auteur)
        {
            string sql = "select count(*) from boeken where auteurId = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql, auteur);
                connection.Close();
                if (count == 0)
                    return false;
                else
                    return true;
            }
        }
        public bool BestaatAuteurId(string auteurId)
        {
            string sql = $"select count(*) from auteurs where id = '{auteurId}'";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql);
                connection.Close();
                if (count == 0)
                    return false;
                else
                    return true;
            }

            // HOEWEL IN COMMENTAAR, DE CODE HIERONDER VERDIENT DE VOORKEUR
            //
            //string sql = "select count(*) from auteurs where id = @id";
            //using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            //{
            //    connection.Open();
            //    int count = connection.ExecuteScalar<int>(sql, new { id = auteurId });
            //    connection.Close();
            //    if (count == 0)
            //        return false;
            //    else
            //        return true;
            //}

        }
        public Auteur ZoekAuteurViaNaam(string naam)
        {
            Auteur auteur;
            string sql = "select * from auteurs where naam = @zoeknaam";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                auteur = connection.QueryFirst<Auteur>(sql, new { zoeknaam = naam });
                connection.Close();
            }
            return auteur;
        }
        public Auteur ZoekAuteurViaId(string auteurId)
        {
            Auteur auteur; 
            string sql = "select * from auteurs where id = @zoekid";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                auteur = connection.QueryFirst<Auteur>(sql, new { zoekid = auteurId });
                connection.Close();
            }
            return auteur;

        }
        // ==============
        // CRUD UITGEVERS
        // ==============
        public List<Uitgever> GetUitgevers()
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    List<Uitgever> uitgevers = connection.GetAll<Uitgever>().ToList();
                    connection.Close();
                    uitgevers = uitgevers.OrderBy(p => p.Naam).ToList();
                    return uitgevers;
                }
                catch
                {
                    return null;
                }
            }
        }
        public bool UitgeverToevoegen(Uitgever uitgever)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                //var nieuweAutonummeringsWaarde = connection.Insert(uitgever);
                try
                {
                    connection.Open();
                    connection.Insert(uitgever);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool UitgeverWijzigen(Uitgever uitgever)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Update(uitgever);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
        public bool UitgeverVerwijderen(Uitgever uitgever)
        {
            if (IsUitgeverInGebruik(uitgever))
                return false;
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Delete(uitgever);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool IsUitgeverInGebruik(Uitgever uitgever)
        {
            string sql = "select count(*) from boeken where uitgeverId = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql, uitgever);
                connection.Close();
                if (count == 0)
                    return false;
                else
                    return true;
            }

        }
        public bool BestaatUitgeverId(string uitgeverId)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Uitgever uitgever = connection.Get<Uitgever>(uitgeverId);
                    connection.Close();
                    if (uitgever == null)
                        return false;
                    else
                        return true;
                }
                catch
                {
                    return false;
                }
            }

        }
        public Uitgever ZoekUitgeverViaNaam(string naam)
        {
            string sql = "select * from uitgevers where naam = @naam";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Uitgever uitgever = connection.QueryFirst(sql, new { zoeknaam = naam });
                    connection.Close();
                    return uitgever;
                }
                catch
                {
                    return null;
                }
            }
        }
        public Uitgever ZoekUitgeverViaId(string uitgeverId)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Uitgever uitgever = connection.Get<Uitgever>(uitgeverId);
                    connection.Close();
                    return uitgever;
                }
                catch
                {
                    return null;
                }
            }

        }
        // ==============
        // CRUD BOEKEN
        // ==============
        public List<Boek> GetBoeken(Auteur auteur = null, Uitgever uitgever = null)
        {
            List<Boek> boeken = new List<Boek>();
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                if (auteur == null && uitgever == null)
                    boeken = connection.Query<Boek>("Select * from boeken order by titel").ToList();
                else if (auteur != null && uitgever == null)
                    boeken = connection.Query<Boek>("Select * from boeken where AuteurId = @auteurId order by titel", new { auteurId = auteur.Id }).ToList();
                else if (auteur == null && uitgever != null)
                    boeken = connection.Query<Boek>("Select * from boeken where UitgeverId = @uitgeverId  order by titel", new { uitgeverId = uitgever.Id }).ToList();
                else
                    boeken = connection.Query<Boek>("Select * from boeken where AuteurId = @auteurId and UitgeverId = @uitgeverId  order by titel", new { auteurId = auteur.Id, uitgeverId = auteur.Id }).ToList();
                connection.Close();
            }
            return boeken;

        }
        public bool BoekToevoegen(Boek boek)
        {
            if (!BestaatAuteurId(boek.AuteurId))
                return false;
            if (!BestaatUitgeverId(boek.UitgeverId))
                return false;


            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Insert(boek);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
        public bool BoekWijzigen(Boek boek)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Update(boek);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
        public bool BoekVerwijderen(Boek boek)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Delete(boek);
                    connection.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
    }
}
