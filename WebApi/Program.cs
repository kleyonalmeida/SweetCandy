using System;
using Domain.Entities;

internal class Program
{
  private static void Main(string[] args)
  {
    Console.WriteLine("SweetCandy - WebApi (projeto mínimo)");

    // Exemplo de uso rápido (apenas para build): criar uma instância de entidade.
    var insumo = new Supply { Name = "Açúcar", Quantity = 10m, Price = 5m };
    Console.WriteLine($"Insumo: {insumo.Name}, qtd={insumo.Quantity}, price={insumo.Price}");
  }
}
