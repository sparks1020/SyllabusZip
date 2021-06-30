using SyllabusZip.Common.Data;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Helpers
{
    public class ContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public bool SaveContact(EditContactViewModel contactedit, out ContactInfo contact)
        {
            if (contactedit != null)
            {
                if (Guid.TryParse(contactedit.ContactId, out Guid newGuid))
                {
                    contact = _context.Contact
                    .Where(x => x.Id == newGuid)
                    .SingleOrDefault();
                    if (contact != null)
                    {
                        contact.ClassTime = contactedit.ClassTime;
                        contact.Teacher = contactedit.Teacher;
                        contact.Office = contactedit.Office;
                        contact.OfficeHours = contactedit.Office_Hours;
                        contact.Email = contactedit.Email;
                        contact.Phone = contactedit.Phone;
                        contact.Mailbox = contactedit.Mailbox;
                    }

                    _context.SaveChanges();
                    return true;
                }
            }
            // Return false if customeredit == null or CustomerID is not a guid
            contact = null;
            return false;
        }

        public List<EditContactViewModel> GetContact()
        {
            List<ContactInfo> contact = new List<ContactInfo>();
            contact = _context.Contact.ToList();

            if (contact != null)
            {
                List<EditContactViewModel> contactsDisplay = new List<EditContactViewModel>();
                foreach (var x in contact)
                {
                    var contactDisplay = new EditContactViewModel()
                    {
                        ContactId = x.Id.ToString(),
                        ClassTime = x.ClassTime,
                        Teacher = x.Teacher,
                        Office = x.Office,
                        Office_Hours = x.OfficeHours,
                        Email = x.Email,
                        Phone = x.Phone,
                        Mailbox = x.Mailbox
                    };
                    contactsDisplay.Add(contactDisplay);
                }
                return contactsDisplay;
            }
            return null;
        }

        public EditContactViewModel GetContact(Guid contactId)
        {
            if (contactId != Guid.Empty)
            {
                var contact = _context.Contact
                    .Where(x => x.Id == contactId)
                    .SingleOrDefault();
                if (contact != null)
                {
                    var contactEdit = new EditContactViewModel()
                    {
                        ContactId = contact.Id.ToString("D"),
                        ClassTime = contact.ClassTime,
                        Teacher = contact.Teacher,
                        Office = contact.Office,
                        Office_Hours = contact.OfficeHours,
                        Email = contact.Email,
                        Phone = contact.Phone,
                        Mailbox = contact.Mailbox
                    };

                    return contactEdit;
                }
            }
            return null;
        }

    }
}
