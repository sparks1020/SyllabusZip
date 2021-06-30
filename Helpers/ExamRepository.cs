using SyllabusZip.Common.Data;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Helpers
{
    public class ExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public bool SaveExam(EditExamViewModel examedit, out Exam exam)
        {
            if (examedit != null)
            {
                if (Guid.TryParse(examedit.ExamId, out Guid newGuid))
                {
                    exam = _context.Exams
                    .Where(x => x.Id == newGuid)
                    .SingleOrDefault();
                    {
                        exam.Date = examedit.Date;
                        exam.ExamType = examedit.ExamType;
                    }

                    _context.SaveChanges();
                    return true;
                }
            }
            // Return false if customeredit == null or CustomerID is not a guid
            exam = null;
            return false;
        }

        public List<EditExamViewModel> GetExams()
        {
            List<Exam> exams = new List<Exam>();
            exams = _context.Exams.ToList();

            if (exams != null)
            {
                List<EditExamViewModel> examsDisplay = new List<EditExamViewModel>();
                foreach (var x in exams)
                {
                    var assignmentDisplay = new EditExamViewModel()
                    {
                        ExamId = x.Id.ToString(),
                        Date = x.Date,
                        ExamType = x.ExamType
                    };
                    examsDisplay.Add(assignmentDisplay);
                }
                return examsDisplay;
            }
            return null;
        }

        public EditExamViewModel GetExam(Guid examid)
        {
            if (examid != Guid.Empty)
            {
                var exam = _context.Exams
                    .Where(x => x.Id == examid)
                    .SingleOrDefault();
                if (exam != null)
                {
                    var examEdit = new EditExamViewModel()
                    {
                        ExamId = exam.Id.ToString("D"),
                        Date = exam.Date,
                        ExamType = exam.ExamType
                    };

                    return examEdit;
                }
            }
            return null;
        }

    }
}
