using AutoMapper;
using InvoiceGenerator.Data;
using InvoiceGenerator.Data.Entities;
using InvoiceGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.AutoMapperProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserViewModel, ApplicationUser>();
            CreateMap<ApplicationUser,EditUserViewModel>();

            CreateMap<tblInvoice, InvoiceIndexItemVewModel>();
                

            CreateMap<InvoiceItemViewModel, tblInvoiceItems>();
            CreateMap<tblInvoiceItems, InvoiceItemViewModel>();  
            CreateMap<InvoiceVewModel, tblInvoice>()
                .ForMember(I => I.Items, x => x.MapFrom(x => x.Items)) 
                .ForMember(m => m.RowVersion, x => x.MapFrom(y => Convert.FromBase64String(y.RowVersion)));
            CreateMap<tblInvoice, InvoiceVewModel>()
                .ForMember(I => I.Items, x => x.MapFrom(x => x.Items))
                .ForMember(m => m.RowVersion, x => x.MapFrom(y => Convert.ToBase64String(y.RowVersion)));
        }
    }
}
