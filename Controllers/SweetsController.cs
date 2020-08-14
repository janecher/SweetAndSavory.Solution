using Microsoft.AspNetCore.Mvc;
using SweetAndSavory.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SweetAndSavory.Controllers
{
  public class SweetsController : Controller
  {
    private readonly SweetAndSavoryContext _db;
    private readonly UserManager<ApplicationUser> _userManager; 

    public SweetsController(UserManager<ApplicationUser> userManager, SweetAndSavoryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      List<SweetSavory> userSweets = _db.Sweets.ToList();
      return View(userSweets);
    }

    [Authorize] 
    public ActionResult Create()
    {
      ViewBag.SavoryId = new SelectList(_db.Savories, "SavoryId", "Name");
      return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(Sweet sweet, int SavoryId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      savory.User = currentUser;
      _db.Sweets.Add(sweet);
      if (SavoryId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SavoryId = SavoryId, SweetId = sweet.SweetId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisSweet= _db.Sweets
          .Include(sweet => sweet.Savories)
          .ThenInclude(join => join.Savory)
          .Include(sweet => sweet.User)
          .FirstOrDefault(sweet => sweet.SweetId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.IsCurrentUser = userId != null ? userId == thisSweet.User.Id : false;
      return View(thisSweet);
    }

    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSweet = _db.Sweets.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(sweet => sweet.SweetId == id);
      if (thisSweet == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.SavoryId = new SelectList(_db.Savories, "SavoryId", "Name"); 
      return View(thisSweet);
    }

    [HttpPost]
    public ActionResult Edit(Sweet sweet, int SavoryId)
    {
      if (SavoryId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SavoryId = SavoryId, SweetId = sweet.SweetId });
      }
      _db.Entry(sweet).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = sweet.SweetId});
    }

    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSweet = _db.Sweets.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(sweet => sweet.SweetId == id);
      if (thisSweet == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisSweet);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisSweet = _db.Sweets.FirstOrDefault(sweet => sweet.SweetId == id);
      _db.Sweets.Remove(thisSweet);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> AddSavory(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSweet = _db.Sweets.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(sweet => sweet.SweetId == id);
      if (thisSweet == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.SavoryId = new SelectList(_db.Savories, "SavoryId", "Name"); 
      return View(thisSweet);
    }

    [HttpPost]
    public ActionResult AddSavory(Sweet sweet, int SavoryId)
    {
      if (SavoryId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SavoryId = SavoryId, SweetId = sweet.SweetId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = sweet.SweetId});
    }

    [HttpPost]
    public ActionResult DeleteSavory(int joinId)
    {
      var joinEntry = _db.SweetsSavories.FirstOrDefault(entry => entry.SweetSavoryId == joinId);
      _db.SweetsSavories.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}