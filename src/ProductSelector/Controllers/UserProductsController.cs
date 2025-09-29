using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductSelector.Models;
using PulumiInfra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProductSelector.Controllers
{
    public class UserProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PulumiAutomationService _pulumiService;

        public UserProductsController(AppDbContext context)
        {
            _context = context;
            _pulumiService = new PulumiAutomationService();
        }

        // GET: UserProducts
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserProducts.Include(t => t.User).Include(t=>t.Product).ToListAsync());
        }

        // GET: UserProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProduct = await _context.UserProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProduct == null)
            {
                return NotFound();
            }

            return View(userProduct);
        }

        // GET: UserProducts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProductId,ConfigJson")] UserProduct userProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProduct);
        }

        // GET: UserProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProduct = await _context.UserProducts.FindAsync(id);
            if (userProduct == null)
            {
                return NotFound();
            }
            return View(userProduct);
        }

        // POST: UserProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,ConfigJson")] UserProduct userProduct)
        {
            if (id != userProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProductExists(userProduct.Id))
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
            return View(userProduct);
        }

        // GET: UserProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProduct = await _context.UserProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProduct == null)
            {
                return NotFound();
            }

            return View(userProduct);
        }

        // POST: UserProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProduct = await _context.UserProducts.FindAsync(id);
            if (userProduct != null)
            {
                _context.UserProducts.Remove(userProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        public async Task<IActionResult> Deploy(int userId, bool destroy = false)
        {
            var userProduct = _context.UserProducts.Include(t => t.Product).Where(t => t.UserId == userId).ToList();
            List<int> productIds = _context.UserProducts.Where(t => t.UserId == userId).Select(up => up.ProductId).ToList();

            // Map productIds (like "ProductA", "ProductB") → folder paths
            var productPaths = _context.Products.Where(t => productIds.Contains(t.Id)).Select(t => t.ProjectPath).ToList(); //new List<string>();

            var stackName = $"user-{userId}-stack";
            List<UserProductPort> portList = userProduct.Select(t => new UserProductPort { Port = t.Port, ProductId = t.ProductId, UserId = t.UserId, ProjectPath = t.Product.ProjectPath }).ToList();
            var result = await _pulumiService.DeployForUserAsync(stackName, productPaths, userId.ToString(), portList, destroy);

            // Collect outputs into a dictionary for response
            var outputs = new Dictionary<string, object?>();
            foreach (var kvp in result.Outputs)
            {
                outputs[kvp.Key] = kvp.Value.Value;
            }

            return Ok(new
            {
                message = "Deployment finished",
                outputs
            });
        }

        private bool UserProductExists(int id)
        {
            return _context.UserProducts.Any(e => e.Id == id);
        }
    }
}
