using AutoMapper;
using System.Globalization;
using TableReservation.Models;
using TableReservation.Models.Enum;
using TableReservation.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TableReservation.Mappers
{
    public partial class MapperCofig : Profile
    {
        public MapperCofig()
        {
            CreateMap<Menus, ViewMenus>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ReverseMap().ForMember(des => des.Type,
                opt => opt.MapFrom(src => EnumMapper<MenuType>.MapType(src.Type)));
            CreateMap<Menus, MenuGetAddEditView>().ReverseMap().ForMember(des => des.Type,
              opt => opt.MapFrom(src => EnumMapper<MenuType>.MapType(src.Type)));
            CreateMap<Tables, ViewTables>().ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))).ReverseMap();
            CreateMap<Tables, TableGetAddEditView>().ReverseMap();
            CreateMap<Tables, EditStatusTableView>().ReverseMap();
            CreateMap<Reviews, ViewReviews>().ReverseMap();
            CreateMap<Reservations, ViewStaffReservation>().ReverseMap().ForMember(des => des.Status,
                opt => opt.MapFrom(src => EnumMapper<Status>.MapType(src.Status)));
            CreateMap<Reservations, ViewBooking>().ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
            CreateMap<Customers, CustomersGetAddDeleteView>().ReverseMap();
            CreateMap<Customers, ViewCustomers>()
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                 .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ReverseMap();
        }
    }
}
