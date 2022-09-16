using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestOne.DAL;
using TestOne.Models;

namespace TestOne.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostEntitiesController : ControllerBase
    {
        private readonly BlogsContext _context;

        public PostEntitiesController(BlogsContext context)
        {
            _context = context;
        }

        // GET: api/PostEntities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostEntity>>> GetPostsEntities()
        {
            return await _context.PostsEntities.ToListAsync();
        }
        
        // GET: api/PostEntities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostEntity>> GetPostEntity(int id)
        {
            var postEntity = await _context.PostsEntities.FindAsync(id);

            if (postEntity == null)
            {
                return NotFound();
            }

            return postEntity;
        }

        // PUT: api/PostEntities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostEntity(int id, PostEntity postEntity)
        {
            if (id != postEntity.PostId)
            {
                return BadRequest();
            }

            _context.Entry(postEntity).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(postEntity.PostId);
        }

        // POST: api/PostEntities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostEntity>> PostPostEntity(PostEntity postEntity)
        {
            _context.PostsEntities.Add(postEntity);
            _context.SaveChanges();

            return CreatedAtAction("GetPostEntity", new { id = postEntity.PostId }, postEntity);
        }

        // DELETE: api/PostEntities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostEntity(int id)
        {
            var postEntity = await _context.PostsEntities.FindAsync(id);
            if (postEntity == null)
            {
                return NotFound();
            }

            _context.PostsEntities.Remove(postEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostEntityExists(int id)
        {
            return _context.PostsEntities.Any(e => e.PostId == id);
        }
    }
}
