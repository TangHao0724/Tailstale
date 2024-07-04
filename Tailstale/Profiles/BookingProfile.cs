using AutoMapper;
using Tailstale.Controllers;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using WebApplication1.DTO;

namespace Tailstale.Profiles
{
    public class BookingProfile:Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingDetail, BookingDetailDTO>()
               .ForMember(dest => dest.roomName, opt => opt.MapFrom(src => src.room.roomType.ToString()))
               .ForMember(dest => dest.roomPrice, opt => opt.MapFrom(src => src.room.roomPrice.Value))
               ;

            CreateMap<Booking, BookingDTO>()
                .ForMember(
                dest => dest.BookingStatus,
                opt => opt.MapFrom(src => src.bookingStatusNavigation.status_name))
                .ForMember(
                dest => dest.BookingTotal,
                opt => opt.MapFrom(src => src.BookingDetails.Sum(d => d.bdTotal)))
                .ForMember(
                dest => dest.BookingDetailDTOs,
                opt => opt.MapFrom(src => src.BookingDetails))
                ;

            //var configuration = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Booking, BookingDTO>()
            //        .ForMember(dest => dest.BookingStatus,
            //    opt => opt.MapFrom(src => src.bookingStatusNavigation.status_name))
            //    .ForMember(dest => dest.BookingTotal,
            //    opt => opt.MapFrom(src => src.BookingDetails.Sum(d => d.bdTotal)))
            //    .ForMember(dest => dest.BookingDetailDTOs,
            //    opt => opt.MapFrom(src => src.BookingDetails));

            //    cfg.CreateMap<BookingDetail, BookingDetailDTO>().ForMember(dest => dest.roomName, opt => opt.MapFrom(src => src.room.roomType.ToString()))
            //   .ForMember(dest => dest.roomPrice, opt => opt.MapFrom(src => src.room.roomPrice.Value));




            //});
            //_mapper = configuration.CreateMapper();
        }
        }
}
