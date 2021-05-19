using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace Scala.Dapper.Core.Entities
{
    [Table("Boeken")]
    public class Boek
    {
        [ExplicitKey]
        public string Id { get; private set; }
        public string Titel { get; set; }
        public string AuteurId { get; set; }
        public string UitgeverId { get; set; }
        public int Jaar { get; set; }
        public Boek()
        {
            Id = Guid.NewGuid().ToString();
        }
        public Boek(string titel, string auteurId, string uitgeverId, int jaar) : this()
        {
            Titel = titel;
            AuteurId = auteurId;
            UitgeverId = uitgeverId;
            Jaar = jaar;
        }
        public Boek(string id, string titel, string auteurId, string uitgeverId, int jaar)
        {
            Id = id;
            Titel = titel;
            AuteurId = auteurId;
            UitgeverId = uitgeverId;
            Jaar = jaar;
        }
        public override string ToString()
        {
            return $"{Titel}";
        }

    }
}
