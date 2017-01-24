using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;
using MusicStore.ViewModels;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {

        private readonly AppSettings _appSettings;

        public CustomerController(MusicStoreContext dbContext, IOptions<AppSettings> options)
        {
            DbContext = dbContext;
            _appSettings = options.Value;
        }

        public MusicStoreContext DbContext { get; }

        // GET: /<controller>/
        public ActionResult Index(string musicGenre, string searchString)
        {
            var GenreLST = new List<string>();

            var GenreQry = from d in DbContext.Genres
                           orderby d.Name
                           select d.Name;

            GenreLST.AddRange(GenreQry.Distinct());
            ViewBag.musicGenre = new SelectList(GenreLST);

            var musics = from m in DbContext.Albums.Include(a => a.Genre).Include(a => a.Artist)
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                musics = musics.Where(s => s.Title.Contains(searchString));
            }

            if (string.IsNullOrEmpty(musicGenre))
                return View(musics);
            else
            {
                return View(musics.Where(x => x.Genre.Name == musicGenre));
            }

        }

        public async Task<IActionResult> Edit(int id)
        {
            var album = await DbContext.Albums.
                Where(a => a.AlbumId == id).
                FirstOrDefaultAsync();

            if (album == null)
            {
                return NotFound();
            }

            ViewBag.GenreId = new SelectList(DbContext.Genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(DbContext.Artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        //// POST: /StoreManager/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(
        //    [FromServices] IMemoryCache cache,
        //    Album album,
        //    CancellationToken requestAborted)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        DbContext.Update(album);
        //        await DbContext.SaveChangesAsync(requestAborted);
        //        //Invalidate the cache entry as it is modified
        //        cache.Remove(GetCacheKey(album.AlbumId));
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.GenreId = new SelectList(DbContext.Genres, "GenreId", "Name", album.GenreId);
        //    ViewBag.ArtistId = new SelectList(DbContext.Artists, "ArtistId", "Name", album.ArtistId);
        //    return View(album);
        //}



    }
}
