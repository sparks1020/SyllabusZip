using Microsoft.AspNetCore.Mvc;
using SyllabusZip.Common.Data;
using SyllabusZip.Helpers;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SyllabusZip.Controllers
{
    public class EditorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EditorController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
            
        }

        public IActionResult Edit(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                bool isGuid = Guid.TryParse(id, out Guid customerId);
                if (isGuid && customerId != Guid.Empty)
                {
                    return View();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public ActionResult EditAssignments(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                bool isGuid = Guid.TryParse(id, out Guid customerId);
                if (isGuid && customerId != Guid.Empty)
                {
                    var repo = new AssignmentRepository(_context);
                    var model = repo.GetAssignment(customerId);
                    return View(model);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAssignments(EditAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = new AssignmentRepository(_context);
                bool saved = repo.SaveAssignment(model, out Assignment changedAssignment);
                if (saved)
                {
                    bool isGuid = Guid.TryParse(model.AssignmentId, out Guid assignmentid);
                    if (isGuid)
                    {
                        var modelUpdate = repo.GetAssignment(assignmentid);
                        if (changedAssignment != null)
                        {
                            return RedirectToAction("Assignments", "Home", new { syllabusId = changedAssignment.SyllabusId });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult EditExams(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                bool isGuid = Guid.TryParse(id, out Guid examid);
                if (isGuid && examid != Guid.Empty)
                {
                    var repo = new ExamRepository(_context);
                    var model = repo.GetExam(examid);
                    return View(model);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditExams(EditExamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = new ExamRepository(_context);
                bool saved = repo.SaveExam(model, out Exam changedExam);
                if (saved)
                {
                    bool isGuid = Guid.TryParse(model.ExamId, out Guid syllabusid);
                    if (isGuid)
                    {
                        if (changedExam != null)
                        {
                            var modelUpdate = repo.GetExam(syllabusid);
                            return RedirectToAction("Exams", "Home", new { syllabusId = changedExam.SyllabusId });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult EditMaterials(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                bool isGuid = Guid.TryParse(id, out Guid syllabusId);
                if (isGuid && syllabusId != Guid.Empty)
                {
                    var repo = new MaterialsRepository(_context);
                    var model = repo.GetMaterials(syllabusId);
                    return View(model);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMaterials(EditMaterialsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = new MaterialsRepository(_context);
                bool saved = repo.SaveMaterial(model, out Materials changedMaterial, Guid.Parse(model.SyllabusId));
                if (saved)
                {
                    bool isGuid = Guid.TryParse(model.SyllabusId, out Guid syllabusid);
                    if (isGuid)
                    {
                        if (changedMaterial != null)
                        {
                            var modelUpdate = repo.GetMaterials(syllabusid);
                            return RedirectToAction("Materials", "Home", new { syllabusId = changedMaterial.SyllabusId });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult EditContact(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                bool isGuid = Guid.TryParse(id, out Guid syllabusId);
                if (isGuid && syllabusId != Guid.Empty)
                {
                    var repo = new ContactRepository(_context);
                    var model = repo.GetContact(syllabusId);
                    return View(model);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditContacts(EditContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var repo = new ContactRepository(_context);
                bool saved = repo.SaveContact(model, out ContactInfo changedContact);
                if (saved)
                {
                    bool isGuid = Guid.TryParse(model.ContactId, out Guid syllabusid);
                    if (isGuid)
                    {
                        if (changedContact != null)
                        {
                            var modelUpdate = repo.GetContact(syllabusid);
                            return RedirectToAction("Course", "Home", new { syllabusId = changedContact.SyllabusId });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return BadRequest();
        }
    }
}
