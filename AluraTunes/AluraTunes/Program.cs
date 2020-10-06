using System;
using System.Collections.Generic;
using System.Linq;

namespace AluraTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            IntroducaoAoLinqSelectFromWhere();
        }

        private static void IntroducaoAoLinqSelectFromWhere()
        {
            // Listar os gêneros Rock

            var generos = new List<Genero>
            {
                new Genero { Id = 1, Nome = "Rock" },
                new Genero { Id = 2, Nome = "Reggae" },
                new Genero { Id = 3, Nome = "Rock Progressivo" },
                new Genero { Id = 4, Nome = "Punk Rock" },
                new Genero { Id = 5, Nome = "Clássica" }
            };

            foreach (var genero in generos)
            {
                if (genero.Nome.Contains("Rock"))
                {
                    Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
                }
            }

            var query = from genero in generos
                        where genero.Nome.Contains("Rock")
                        select genero;

            // Linq = Language Integrated Query = Consulta integra à Linguagem

            Console.WriteLine();

            foreach (var genero in query)
            {
                Console.WriteLine("{0}\t{1}", genero.Id, genero.Nome);
            }

            Console.ReadKey();
        }
    }

    class Genero
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
