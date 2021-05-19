using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace Scala.Dapper.Core.Entities
{
    [Table("Uitgevers")]
    public class Uitgever
    {
        [ExplicitKey]
        public string Id { get; private set; }
        public string Naam { get; set; }
        public Uitgever()
        {
            Id = Guid.NewGuid().ToString();
        }
        public Uitgever(string naam)
        {
            Id = Guid.NewGuid().ToString();
            Naam = naam;
        }
        public Uitgever(string id, string naam)
        {
            Id = id;
            Naam = naam;
        }
        public override string ToString()
        {
            return $"{Naam}";
        }
    }
}
