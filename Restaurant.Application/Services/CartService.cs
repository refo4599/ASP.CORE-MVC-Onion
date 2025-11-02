using Restaurant.Application.Interfaces;
using Restaurant.Application.ViewModels;
using Restaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.AddCartAsync(cart);
            }
            return cart;
        }

        public async Task<(bool Success, string Message, int RemainingStock)> AddToCartAsync(string userId, CartItemVM productVM)
        {
            var product = await _productRepository.GetByIdAsync(productVM.ProductId);
            if (product == null || !product.IsAvailable || productVM.Quantity > product.InStock)
            {
                return (false, $" Product '{product?.Name ?? "Unknown"}' not available or insufficient stock.", product?.InStock ?? 0);
            }

            var cart = await GetCartByUserIdAsync(userId);
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productVM.ProductId);

            if (item != null)
                item.Quantity += productVM.Quantity;
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productVM.ProductId,
                    Quantity = productVM.Quantity
                });
            }

            await _cartRepository.SaveChangesAsync();
            await _productRepository.DecreaseStockAsync(productVM.ProductId, productVM.Quantity);

            var remainingStock = product.InStock - productVM.Quantity;

            return (true, $" '{product.Name}' added to cart successfully!", remainingStock);
        }


        public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && quantity > 0)
            {
                int diff = quantity - item.Quantity;
                if (diff > 0)
                {
                    await _productRepository.DecreaseStockAsync(productId, diff);
                }
                item.Quantity = quantity;
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            cart.Items.RemoveAll(x => x.ProductId == productId);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }

        public async Task<decimal> GetTotalAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            return cart.Items.Sum(x => x.Quantity * (x.Product?.Price ?? 0));
        }

        public async Task<List<OrderItems>> GetCartItemsAsync(string userId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);

            return cartItems.Select(c => new OrderItems
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Product.Price,
                Product = c.Product
            }).ToList();
        }
    }
}
