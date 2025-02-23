﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Context;
using MoonstoneTCC.Models;
using MoonstoneTCC.ViewModels;
using ReflectionIT.Mvc.Paging;


namespace MoonstoneTCC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPedidosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult PedidoJogos(int? id)
        {
            var pedido = _context.Pedidos
                        .Include(pd => pd.PedidoItens)
                        .ThenInclude(j => j.Jogo)
                        .FirstOrDefault(p => p.PedidoId == id);

            if (pedido == null)
            {
                Response.StatusCode = 404;
                return View("PedidoNoFound", id.Value);
            }

            PedidoJogoViewModel pedidoJogos = new PedidoJogoViewModel()
            {
                Pedido = pedido,
                PedidoDetalhes = pedido.PedidoItens
            };
            return View(pedidoJogos);
        }

        // GET: Admin/AdminPedidos
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Pedidos.ToListAsync());
        //}
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var resultado = _context.Pedidos.AsNoTracking()
                                      .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.Nome.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 6, pageindex, sort, "Nome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }


        // GET: Admin/AdminPedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminPedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PedidoId,Nome,Sobrenome,Endereco1,Endereco2,Cep,Estado,Cidade,Telefone,Email,PedidoEnviado,PedidoEntregueEm")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,Nome,Sobrenome,Endereco1,Endereco2,Cep,Estado,Cidade,Telefone,Email,PedidoEnviado,PedidoEntregueEm")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.PedidoId == id);
        }


        [HttpGet]
        public IActionResult ExportarPedidoTexto(int id)
        {
            // Busca o pedido no banco de dados
            var pedido = _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(i => i.Jogo)
                .FirstOrDefault(p => p.PedidoId == id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            // Monta o conteúdo do arquivo TXT
            var conteudo = $"Pedido #{pedido.PedidoId}\n" +
                           $"Nome: {pedido.Nome}\n" +
                           $"Endereço: {pedido.Endereco1}, {pedido.Endereco2}\n" +
                           $"Cidade: {pedido.Cidade} - {pedido.Estado}\n" +
                           $"CEP: {pedido.Cep}\n" +
                           $"Telefone: {pedido.Telefone}\n" +
                           $"Email: {pedido.Email}\n" +
                           $"Itens do Pedido:\n";

            foreach (var item in pedido.PedidoItens)
            {
                conteudo += $"- {item.Jogo.Nome}: {item.Quantidade}x R${item.Preco:F2}\n";
            }

            // Converte o conteúdo para bytes e retorna como arquivo para download
            var bytes = System.Text.Encoding.UTF8.GetBytes(conteudo);
            return File(bytes, "text/plain", $"Pedido_{pedido.PedidoId}.txt");
        }


        public IActionResult BuscarPedidos(string filter)
        {
            var pedidos = _context.Pedidos.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                pedidos = pedidos.Where(p =>
                    p.Nome.Contains(filter) ||
                    p.Endereco1.Contains(filter) ||
                    p.Endereco2.Contains(filter) ||
                    p.Cidade.Contains(filter) ||
                    p.Cep.Contains(filter) ||
                    p.Telefone.Contains(filter) ||
                    p.Email.Contains(filter));
            }

            // Executa a consulta e retorna os resultados
            var resultados = pedidos.ToList();

            // Retorna a view com os resultados
            return View(resultados);
        }



    }
}
