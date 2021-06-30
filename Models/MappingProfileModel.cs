using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class MappingProfileModel : Profile
    {
        public MappingProfileModel()
        {
            CreateMap<UserRegistrationModel, UserModel>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
