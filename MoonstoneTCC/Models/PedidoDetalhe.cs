﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MoonstoneTCC.Models
{
    public class PedidoDetalhe
    {
        public int PedidoDetalheId { get; set; }
        public int PedidoId { get; set; }
        public int JogoId { get; set; }
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco {  get; set; }

        public virtual  Jogo Jogo { get; set; }
        public virtual Pedido Pedido { get; set; }
    }
}
