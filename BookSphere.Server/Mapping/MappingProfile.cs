using System;
using AutoMapper;
using BookSphere.DTOs;
using BookSphere.Models;

namespace BookSphere.Mapping;

public class MappingProfile : Profile
{
          public MappingProfile()
          {
                    //User Mappings
                    CreateMap<User, UserDto>()
                              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));
                    CreateMap<RegisterDto, User>()
                              .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                              .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                              .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Member"))
                              .ForMember(dest => dest.RegisteredDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                              .ForMember(dest => dest.SuccessfulOrder, opt => opt.MapFrom(src => 0))
                              .ForMember(dest => dest.HasStackableDiscount, opt => opt.MapFrom(src => false));
                    
                    //Book Mappings
                    CreateMap<Book, BookDto>()
                              .ForMember(dest => dest.DiscountedPrice,
                                        opt => opt.MapFrom(src => src.Price - (src.Price * src.DiscountPercentage / 100)));
                    CreateMap<Book, BookDetailsDto>()
                              .ForMember(dest => dest.DiscountedPrice,
                                        opt => opt.MapFrom(src => src.Price - (src.Price * src.DiscountPercentage / 100)))
                              .ForMember(dest => dest.AverageRating,
                                        opt => opt.MapFrom(src => src.Reviews.Count > 0 ? src.Reviews.Average(r => r.Rating) : 0))
                              .ForMember(dest => dest.ReviewCount,
                                        opt => opt.MapFrom(src => src.Reviews.Count));
                    CreateMap<CreateBookDto, Book>();
                    CreateMap<UpdateBookDto, Book>();

                    //WhiteList Mapppings
                    CreateMap<WhiteList, WhiteListDto>()
                              .ForMember(dest => dest.Items,
                                        opt => opt.MapFrom(src => src.WhiteListItems));
                    CreateMap<WhiteListItem, WhiteListItemDto>()
                              .ForMember(dest => dest.Book,
                                        opt => opt.MapFrom(src => src.Book));

                    //Review Mappings
                    CreateMap<Review, ReviewDto>()
                              .ForMember(dest => dest.UserName,
                                        opt => opt.MapFrom(src => src.User.FullName));
                    CreateMap<CreateReviewDto, Review>();

                    //Announcement mappings
                    CreateMap<Announcement, AnnouncementDto>();
                    CreateMap<CreateAnnouncementDto, Announcement>()
                              .ForMember(dest => dest.CreatedDate,
                                        opt => opt.MapFrom(src => DateTime.UtcNow))
                              .ForMember(dest => dest.IsActive,
                                        opt => opt.MapFrom(src => true));

                    // Cart mappings
                    CreateMap<Cart, CartDto>()
                              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                              .ForMember(dest => dest.SubTotal, opt => opt.Ignore()) // Will be calculated in service
                              .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore()) // Will be calculated in service
                              .ForMember(dest => dest.Total, opt => opt.Ignore()) // Will be calculated in service
                              .ForMember(dest => dest.QualifiesForBulkDiscount, opt => opt.Ignore()) // Calculated in service
                              .ForMember(dest => dest.HasLoyaltyDiscount, OPT => OPT.Ignore()); // Calculated in service
                    CreateMap<CartItem, CartItemDto>()
                              .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                              .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Book.Price))
                              .ForMember(dest => dest.DiscountedPrice,
                                        opt => opt.MapFrom(src => src.Book.Price - (src.Book.Price * src.Book.DiscountPercentage / 100)))
                              .ForMember(dest => dest.Subtotal,
                                        opt => opt.MapFrom(src => (src.Book.Price - (src.Book.Price * src.Book.DiscountPercentage / 100)) * src.Quantity));

                    // Order Mappings
                    CreateMap<Order, OrderDto>()
                              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                              .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.TotalAmount - src.FinalAmount));
                    CreateMap<OrderItem, OrderItemDto>()
                              .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                              .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book.Author))
                              .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book.ISBN))
                              .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.SubTotal));
          }
}
