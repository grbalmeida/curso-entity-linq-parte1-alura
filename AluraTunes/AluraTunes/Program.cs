using AluraTunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AluraTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqToEntitiesWhere();
        }

        private static void LinqToEntitiesWhere()
        {
            using (var contexto = new AluraTunesEntities())
            {
                var textoBusca = "Led";

                contexto.Database.Log = Console.WriteLine;

                var query = from artista in contexto.Artistas
                            where artista.Nome.Contains(textoBusca)
                            select artista;

                foreach (var artista in query)
                {
                    Console.WriteLine("{0}\t{1}", artista.ArtistaId, artista.Nome);
                }
            }


            Console.ReadKey();
        }

        private static void LinqToEntitiesContextoJoinTakeLogSql()
        {
            using (var contexto = new AluraTunesEntities())
            {
                var query = from genero in contexto.Generos
                            select genero;

                foreach (var genero in query)
                {
                    Console.WriteLine("{0}\t{1}", genero.GeneroId, genero.Nome);
                }

                var faixaEGenero = from genero in contexto.Generos
                                   join faixa in contexto.Faixas
                                   on genero.GeneroId equals faixa.GeneroId
                                   select new { genero, faixa };

                faixaEGenero = faixaEGenero.Take(10);

                contexto.Database.Log = Console.WriteLine;

                Console.WriteLine();

                foreach (var item in faixaEGenero)
                {
                    Console.WriteLine("{0}\t{1}", item.faixa.Nome, item.genero.Nome);
                }
            }

            Console.ReadKey();
        }

        private static void LinqToXml()
        {
            XElement root = XElement.Load(@"..\..\Data\AluraTunes.xml");

            var queryXML = from genero in root.Element("Generos").Elements("Genero")
                           select genero;
        
            foreach (var genero in queryXML)
            {
                Console.WriteLine("{0}\t{1}", genero.Element("GeneroId").Value, genero.Element("Nome").Value);
            }

            var query = from genero in root.Element("Generos").Elements("Genero")
                        join musica in root.Element("Musicas").Elements("Musica")
                            on genero.Element("GeneroId").Value equals musica.Element("GeneroId").Value
                        select new
                        {
                            Musica = musica.Element("Nome").Value,
                            Genero = genero.Element("Nome").Value
                        };

            Console.WriteLine();

            foreach (var musicaGenero in query)
            {
                Console.WriteLine("{0}\t{1}", musicaGenero.Musica, musicaGenero.Genero);
            }

            Console.ReadKey();
        }

        private static void LinqComJoins()
        {
            // Listar músicas

            var generos = new List<Genero>
            {
                new Genero { Id = 1, Nome = "Rock" },
                new Genero { Id = 2, Nome = "Reggae" },
                new Genero { Id = 3, Nome = "Rock Progressivo" },
                new Genero { Id = 4, Nome = "Punk Rock" },
                new Genero { Id = 5, Nome = "Clássica" }
            };

            var musicas = new List<Musica>
            {
                new Musica { Id = 1, Nome = "Sweet Child O' Mine", GeneroId = 1 },
                new Musica { Id = 2, Nome = "I Shot The Sheriff", GeneroId = 2 },
                new Musica { Id = 3, Nome = "Danúbio Azul", GeneroId = 5 }
            };

            var musicaQuery = from musica in musicas
                              join genero in generos on musica.GeneroId equals genero.Id
                              select new
                              {
                                  musica.Id,
                                  musica.Nome,
                                  genero
                              };

            foreach (var musica in musicaQuery)
            {
                Console.WriteLine("{0}\t{1}\t{2}", musica.Id, musica.Nome, musica.genero.Nome);
            }

            Console.ReadKey();
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

    class Musica
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int GeneroId { get; set; }
    }
}
