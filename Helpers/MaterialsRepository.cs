using SyllabusZip.Common.Data;
using SyllabusZip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Helpers
{
    public class MaterialsRepository
    {
        private readonly ApplicationDbContext _context;

        public MaterialsRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public bool SaveMaterial(EditMaterialsViewModel materialedit, out Materials material, Guid syllabusId)
        {
            if (materialedit != null)
            {
                //Delete existing records
                var materials_record = _context.Materials.Where(materials_record => materials_record.SyllabusId == syllabusId);


                foreach (var detail in materials_record)
                {
                    _context.Materials.Remove(detail);
                }


                //Insert materialedit record into database
                var new_material = new Materials()
                {
                    SyllabusId = syllabusId,
                    Id = Guid.NewGuid(),
                    Material_Value = materialedit.Material_Value,
                };
                _context.Materials.Add(new_material);
                _context.SaveChanges();

                material = new_material;
                return true;
            }

            material = null;
            return false;
        }

        public EditMaterialsViewModel GetMaterials(Guid SyllabusId)
        {
            if (SyllabusId != Guid.Empty)
            {
                var material = _context.Materials
                    .Where(x => x.SyllabusId == SyllabusId)
                    .ToList();
                if (material != null)
                {
                    var materialEdit = new EditMaterialsViewModel()
                    {
                        SyllabusId = SyllabusId.ToString("D"),
                        Material_Value = string.Join(" ", material.Select(m => m.Material_Value))
                    };

                    return materialEdit;
                }
            }
            return null;
        }

    }
}
