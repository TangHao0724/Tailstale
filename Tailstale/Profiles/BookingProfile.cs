using AutoMapper;
using Tailstale.Controllers;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using Tailstale.partial;
using WebApplication1.DTO;


namespace Tailstale.Profiles
{
    public class BookingProfile:Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingDetail, BookingDetailDTO>()
               .ForMember(dest => dest.roomPrice, opt => opt.MapFrom(src => src.room.roomPrice.Value))
                 .ForMember(dest => dest.roomName, opt => opt.MapFrom(src => src.room.FK_roomType.roomType1))
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
                opt => opt.MapFrom(src => src.BookingDetails));
            CreateMap<Room, RoomDTO>()
               .ForMember(
               dest => dest.roomType,
               opt => opt.MapFrom(src => src.FK_roomType));
               



        }
        
    }
}
