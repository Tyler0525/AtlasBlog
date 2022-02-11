#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasBlog.Data;
using AtlasBlog.Models;
using AtlasBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AtlasBlog.Services;

namespace AtlasBlog.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;



        public BlogPostsController(ApplicationDbContext context, SlugService slugService, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
            _slugService = slugService;
            
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPosts.Include(b => b.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug)
        {
            if(string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Slug == slug);
           
            
            
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create

        
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,BlogPostState,Body")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                var slug = _slugService.UrlFriendly(blogPost.Title, 100);

                //I have to ensure that the Slug is unique before allowing it to be stored in the Datbase
                //If it is unique it can be used, otherwise throw custom error letting user know
                var isUnique = !_context.BlogPosts.Any(b => b.Slug == slug);

                if(isUnique)
                {
                    blogPost.Slug = slug;
                }
                else
                {
                    //Then Slug cannot be used and error must be shown to user
                    ModelState.AddModelError("Title", "This Title cannot be used(duplicate Slug)");
                    ModelState.AddModelError("", "Title cannot be used");
                    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
                    return View(blogPost);
                }
               

                blogPost.Created = DateTime.UtcNow;
                   
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Slug,Title,IsDeleted,Abstract,BlogPostState,Body,Created")] BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            

            if (ModelState.IsValid)
            {
                try
                {
                    //If the Slug has changed I need to do this check

                    var slug = _slugService.UrlFriendly(blogPost.Title, 100);
                    if (blogPost.Slug != slug)
                    {
                        //I have to ensure that the Slug is unique before allowing it to be stored in the DB
                        //If it is unique, it can be used otherwise we have to throw a custom error letting user know what's up
                        var isUnique = !_context.BlogPosts.Any(b => b.Slug == slug);
                        if (isUnique)
                        {
                            blogPost.Slug = slug;
                        }
                        else
                        {
                            //The slug cannot be used and an error must be shown to the user
                            ModelState.AddModelError("Title", "This Title cannot be used (duplicate Slug)");
                            ModelState.AddModelError("", "Title cannot be used");
                            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
                            return View(blogPost);
                        }

                    }

                    blogPost.Updated = DateTime.UtcNow;
                    blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", blogPost.BlogId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
