using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestOne.DAL;
using TestOne.Models;

namespace TestOne.Controllers
{
    [Route("api/[controller]")]
    public class BlogEntitiesController : ControllerBase
    {
        private readonly BlogsContext _context;

        public BlogEntitiesController(BlogsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogEntity>>> GetBlogsEntities()
        =>await _context.BlogsEntities.ToListAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogEntity(int id)
        {
            var postEntity = await _context.BlogsEntities.FindAsync(id);

            if (postEntity == null)
            {
                return NotFound();
            }

            return Ok(postEntity);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogEntity(int id,[FromBody] BlogEntity blogEntity)
        {
            if (id != blogEntity.BlogId)
            {
                return BadRequest();
            }

            _context.Entry(blogEntity).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (! await BlogEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { id = id });
        }


        [HttpPost]
        public async Task<IActionResult> PostBlogEntity([FromBody]BlogEntity blogEntity)
        {
            _context.BlogsEntities.Add(blogEntity);
            _context.SaveChanges();

            return CreatedAtAction("PostBlogEntity", new { id = blogEntity.BlogId }, blogEntity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogEntity(int id)
        {
            var blog = await _context.BlogsEntities.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            await DeleteBlogPost(id);

            _context.BlogsEntities.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> BlogEntityExists(int id)
        => await _context.BlogsEntities.AnyAsync(e => e.BlogId == id);
        private async Task DeleteBlogPost(int id)
        {
            foreach (var post in _context.PostsEntities.Where(x => x.ParentId == id))
            {
                _context.PostsEntities.Remove(post);
            }
            await _context.SaveChangesAsync();
        }

    }
}
