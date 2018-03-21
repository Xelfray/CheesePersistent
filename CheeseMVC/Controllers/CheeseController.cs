using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{  
    public class CheeseController : Controller
    {
        private readonly CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {                  
            IList<Cheese>cheeses = context.Cheeses.Include(c => c.Category).ToList();

            return View(cheeses);
        }
        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory =
                    context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                Cheese newCheese = new Cheese()
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
           
        }

        public IActionResult Remove()
        {
            List<Cheese> cheeses = context.Cheeses.ToList();
            return View(cheeses);
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach(int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(cheese => cheese.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        public IActionResult Category(int id)
        {
            if (id==0)
            {
                return Redirect("/Category");
            }

            CheeseCategory theCategory = context.Categories.Include(cat => cat.Cheeses).Single(cat => cat.ID == id);

            return View("Index", theCategory.Cheeses);
        }
    }
}