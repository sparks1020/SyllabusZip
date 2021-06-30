using SyllabusZip.Common.Data;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Helpers
{
    public class AssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public bool SaveAssignment(EditAssignmentViewModel assignmentedit, out Assignment assignment)
        {
            if (assignmentedit != null)
            {
                if (Guid.TryParse(assignmentedit.AssignmentId, out Guid newGuid))
                {
                    assignment = _context.Assignments
                    .Where(x => x.Id == newGuid)
                    .SingleOrDefault();
                    if (assignment != null)
                    {
                        assignment.Date = assignmentedit.Date;
                        assignment.Homework = assignmentedit.Homework;
                        assignment.Chapter = assignmentedit.Chapter;
                        assignment.Project = assignmentedit.Project;
                    }

                    _context.SaveChanges();
                    return true;
                }
            }
            // Return false if customeredit == null or CustomerID is not a guid
            assignment = null;
            return false;
        }

        public List<EditAssignmentViewModel> GetAssignments()
        {
            List<Assignment> assignments = new List<Assignment>();
            assignments = _context.Assignments.ToList();

            if (assignments != null)
            {
                List<EditAssignmentViewModel> assignmentsDisplay = new List<EditAssignmentViewModel>();
                foreach (var x in assignments)
                {
                    var assignmentDisplay = new EditAssignmentViewModel()
                    {
                        AssignmentId = x.Id.ToString(),
                        Date = x.Date,
                        Homework = x.Homework,
                        Chapter = x.Chapter,
                        Project = x.Project
                    };
                    assignmentsDisplay.Add(assignmentDisplay);
                }
                return assignmentsDisplay;
            }
            return null;
        }

        public EditAssignmentViewModel GetAssignment(Guid assignmentId)
        {
            if (assignmentId != Guid.Empty)
            {
                var assignment = _context.Assignments
                    .Where(x => x.Id == assignmentId)
                    .SingleOrDefault();
                if (assignment != null)
                {
                    var assignmentEdit = new EditAssignmentViewModel()
                    {
                        AssignmentId = assignment.Id.ToString("D"),
                        Date = assignment.Date,
                        Homework = assignment.Homework,
                        Chapter = assignment.Chapter,
                        Project = assignment.Project
                    };

                    return assignmentEdit;
                }
            }
            return null;
        }

    }
}
