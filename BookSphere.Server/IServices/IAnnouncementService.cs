using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IAnnouncementService
{
          //Get all active announcements
          Task<List<AnnouncementDto>> GetActiveAnnouncementsAsync();

          //Get announcement by Id
          Task<AnnouncementDto> GetAnnouncementByIdAsync(Guid announcementId); // Renamed for clarity

          //Create new announcement
          Task<AnnouncementDto> CreateAnnouncementAsync(Guid? userId, CreateAnnouncementDto createAnnouncementDto); // userId is nullable (system announcements might not have a user)

          //Update an announcment
          Task<AnnouncementDto> UpdateAnnouncementAsync(Guid userId, Guid announcementId, CreateAnnouncementDto updateAnnouncementDto); // Added userId for authorization

          // Delete an announcement
          Task<bool> DeleteAnnouncementAsync(Guid userId, Guid announcementId); // Added userId for authorization

          //Activate/Deactivate an announcment
          Task<AnnouncementDto> ToggleAnnouncementStatusAsync(Guid userId, Guid announcementId, bool isActive); // Added userId for authorization

          //Get announcements by type
          Task<List<AnnouncementDto>> GetAnnouncementsByTypeAsync(string type);
}
