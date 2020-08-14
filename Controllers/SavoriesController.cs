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
  public class SavoriesController : Controller
  {
    private readonly SweetAndSavoryContext _db;
    private readonly UserManager<ApplicationUser> _userManager; 

    public SavoriesController(UserManager<ApplicationUser> userManager, SweetAndSavoryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      List<Savory> userSavories = _db.Savories.ToList();
      return View(userSavories);
    }

    [Authorize] 
    public ActionResult Create()
    {
      ViewBag.SweetId = new SelectList(_db.Sweets, "SweetId", "Name");
      return View();
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(Savory savory, int SweetId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      savory.User = currentUser;
      _db.Savories.Add(savory);
      if (SweetId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SweetId = SweetId, SavoryId = savory.SavoryId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisSavory= _db.Savories
          .Include(savory => savory.Sweets)
          .ThenInclude(join => join.Sweet)
          .Include(savory => savory.User)
          .FirstOrDefault(savory => savory.SavoryId == id);
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      ViewBag.IsCurrentUser = userId != null ? userId == thisSavory.User.Id : false;
      return View(thisSavory);
    }

    [Authorize]
    public async Task<ActionResult> Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSavory = _db.Savories.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(savory => savory.SavoryId == id);
      if (thisSavory == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.SweetId = new SelectList(_db.Sweets, "SweetId", "Name"); 
      return View(thisSavory);
    }

    [HttpPost]
    public ActionResult Edit(Savory savory, int SweetId)
    {
      if (SweetId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SweetId = SweetId, SavoryId = savory.SavoryId });
      }
      _db.Entry(savory).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = savory.SavoryId});
    }

    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSavory = _db.Savories.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(savory => savory.SavoryId == id);
      if (thisSavory == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisSavory);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisSavory = _db.Savories.FirstOrDefault(savory => savory.SavoryId == id);
      _db.Savories.Remove(thisSavory);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<ActionResult> AddSweet(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisSavory = _db.Savories.Where(entry => entry.User.Id == currentUser.Id).FirstOrDefault(savory => savory.SavoryId == id);
      if (thisSavory == null)
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.SweetId = new SelectList(_db.Sweets, "SweetId", "Name"); 
      return View(thisSavory);
    }

    [HttpPost]
    public ActionResult AddSweet(Savory savory, int SweetId)
    {
      if (SweetId != 0)
      {
        _db.SweetsSavories.Add(new SweetSavory() { SweetId = SweetId, SavoryId = savory.SavoryId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = savory.SavoryId});
    }

    [HttpPost]
    public ActionResult DeleteSweet(int joinId)
    {
      var joinEntry = _db.SweetsSavories.FirstOrDefault(entry => entry.SweetSavoryId == joinId);
      _db.SweetsSavories.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}