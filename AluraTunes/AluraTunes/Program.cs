using AluraTunes.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;

namespace AluraTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqMetodosExtensao();
        }

        private static void LinqMetodosExtensao()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var vendaMedia = contexto.NotasFiscais.Average(nf => nf.Total);

                Console.WriteLine("Venda Média: {0}", vendaMedia);

                var query = from notaFiscal in contexto.NotasFiscais
                            select notaFiscal.Total;

                var contagem = query.Count();

                var queryOrdenada = query.OrderBy(total => total);

                var elementoCentral = queryOrdenada.Skip(contagem / 2).First();

                var mediana = elementoCentral;

                Console.WriteLine("Mediana: {0}", mediana);
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesMinMaxAvg()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;
                
                // 3 idas até o banco de dados

                //var maiorVenda = contexto.NotasFiscais.Max(nf => nf.Total);
                //var menorVenda = contexto.NotasFiscais.Min(nf => nf.Total);
                //var vendaMedia = contexto.NotasFiscais.Average(nf => nf.Total);

                //Console.WriteLine("A maior venda é de R$ {0}", maiorVenda);
                //Console.WriteLine("A menor venda é de R$ {0}", menorVenda);
                //Console.WriteLine("A venda média é de R$ {0}", vendaMedia);

                var vendas = (from notaFiscal in contexto.NotasFiscais
                             group notaFiscal by 1 into agrupado
                             select new
                             {
                                 MaiorVenda = agrupado.Max(nf => nf.Total),
                                 MenorVenda = agrupado.Min(nf => nf.Total),
                                 VendaMedia = agrupado.Average(nf => nf.Total)
                             }).Single();

                // 1 ida até o banco de dados

                Console.WriteLine("A maior venda é de R$ {0}", vendas.MaiorVenda);
                Console.WriteLine("A menor venda é de R$ {0}", vendas.MenorVenda);
                Console.WriteLine("A venda média é de R$ {0}", vendas.VendaMedia);
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesGroupBy()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var query = from itemNotaFiscal in contexto.ItemsNotasFiscal
                            where itemNotaFiscal.Faixa.Album.Artista.Nome == "Led Zeppelin"
                            group itemNotaFiscal by itemNotaFiscal.Faixa.Album into agrupado
                            let vendasPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                            orderby vendasPorAlbum descending
                            select new
                            {
                                TituloDoAlbum = agrupado.Key.Titulo,
                                TotalPorAlbum = vendasPorAlbum
                            };

                foreach (var agrupado in query)
                {
                    Console.WriteLine(
                        "{0}\t{1}",
                        agrupado.TituloDoAlbum.PadRight(40),
                        agrupado.TotalPorAlbum
                    );
                }
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesSum()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var query = from itemNotaFiscal in contexto.ItemsNotasFiscal
                            where itemNotaFiscal.Faixa.Album.Artista.Nome == "Led Zeppelin"
                            select new
                            {
                                totalDoItem = itemNotaFiscal.Quantidade * itemNotaFiscal.PrecoUnitario
                            };

                //foreach (var itemNotaFiscal in query)
                //{
                //    Console.WriteLine(itemNotaFiscal.totalDoItem);
                //}

                var totalDoArtista = query.Sum(q => q.totalDoItem);

                Console.WriteLine("Total do artista: R$ {0}", totalDoArtista);
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesCount()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var query = from faixa in contexto.Faixas
                            where faixa.Album.Artista.Nome == "Led Zeppelin"
                            select faixa;

                //var quantidade = query.Count();

                //Console.WriteLine("Led Zeppelin tem {0} músicas no banco de dados.", quantidade);

                var quantidade = contexto.Faixas.Count();

                Console.WriteLine("O banco de dados tem {0} faixas de música", quantidade);

                quantidade = contexto.Faixas.Where(f => f.Album.Artista.Nome == "Led Zeppelin").Count();

                Console.WriteLine("Led Zeppelin tem {0} músicas no banco de dados.", quantidade);

                quantidade = contexto.Faixas.Count(f => f.Album.Artista.Nome == "Led Zeppelin");

                Console.WriteLine("Led Zeppelin tem {0} músicas no banco de dados.", quantidade);
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesRefinandoConsultas()
        {
            using (var contexto = new AluraTunesEntities())
            {                
                GetFaixas(contexto, "Led Zeppelin", "");

                Console.WriteLine();

                GetFaixas(contexto, "Led Zeppelin", "Graffiti");
            }

            Console.ReadKey();
        }

        private static void GetFaixas(AluraTunesEntities contexto, string buscaArtista, string buscaAlbum)
        {
            var query = from faixa in contexto.Faixas
                        where faixa.Album.Artista.Nome.Contains(buscaArtista)
                        && (!string.IsNullOrEmpty(buscaAlbum) ? faixa.Album.Titulo.Contains(buscaAlbum) : true)
                        orderby faixa.Album.Titulo, faixa.Nome descending
                        select faixa;

            foreach (var faixa in query)
            {
                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }
        }

        private static void LinqToEntitiesSemJoin()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var textoBusca = "Led";

                var query = from album in contexto.Albums
                            where album.Artista.Nome.Contains(textoBusca)
                            select new
                            {
                                NomeArtista = album.Artista.Nome,
                                NomeAlbum = album.Titulo
                            };

                foreach (var album in query)
                {
                    Console.WriteLine("{0}\t{1}", album.NomeArtista, album.NomeAlbum);
                }
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesJoin()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var textoBusca = "Led";

                var query = from artista in contexto.Artistas
                            join album in contexto.Albums
                            on artista.ArtistaId equals album.ArtistaId
                            where artista.Nome.Contains(textoBusca)
                            select new
                            {
                                NomeArtista = artista.Nome,
                                NomeAlbum = album.Titulo
                            };

                foreach (var item in query)
                {
                    Console.WriteLine("{0}\t{1}", item.NomeArtista, item.NomeAlbum);
                }
            }

            Console.ReadKey();
        }

        private static void LinqToEntitiesSintaxeDeMetodo()
        {
            using (var contexto = new AluraTunesEntities())
            {
                contexto.Database.Log = Console.WriteLine;

                var textoBusca = "Led";

                var query = contexto.Artistas.Where(a => a.Nome.Contains(textoBusca));

                foreach (var artista in query)
                {
                    Console.WriteLine("{0}\t{1}", artista.ArtistaId, artista.Nome);
                }
            }

            Console.ReadKey();
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
