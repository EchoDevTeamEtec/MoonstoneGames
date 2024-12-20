using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.ViewModels;

namespace MoonstoneTCC.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraController(IJogoRepository jogoRepository, 
            CarrinhoCompra carrinhoCompra)
        {
            _jogoRepository = jogoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        public IActionResult Index()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;

            var carrinhoCompraVM = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoCompraVM);
        }
        [Authorize]
        public IActionResult AdicionarItemNoCarrinhoCompra (int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos.FirstOrDefault(p=> p.JogoId == jogoId);
            if(jogoSelecionado != null)
            {
                _carrinhoCompra.AdicionarAoCarrinho(jogoSelecionado);
            }
            return RedirectToAction("Index");
        }
        [Authorize]

        public IActionResult RemoverItemDoCarrinhoCompra(int jogoId)
        {
            var jogoSelecionado = _jogoRepository.Jogos
                                  .FirstOrDefault(p => p.JogoId == jogoId);

            if(jogoSelecionado != null)
            {
                _carrinhoCompra.RemoverDoCarrinho(jogoSelecionado);
            }
            return RedirectToAction("Index");
        }

    }
}
