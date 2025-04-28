using System;
using BookSphere.DTOs;

namespace BookSphere.IServices;

public interface IAnnouncementService
{
          //Get all active announcements
          Task<List<AnnouncementDto>> GetActiveAnnouncementsAsync();

          //Get announcement by Id
          Task<AnnouncementDto> GetAnnouncementDto(Guid announcementId);

          //Create new announcement
          Task<AnnouncementDto> CreateAnnouncementAsync(Guid adminId, CreateAnnouncementDto createAnnouncementDto);

          //Update an announcment
          Task<AnnouncementDto> UpdateAnnouncementAsync(Guid announcementId, CreateAnnouncementDto updateAnnouncementDto);

          // Delete an announcement
          Task<bool> DeleteAnnouncementAsync(Guid announcementId);

          //Activate/Deactivate an announcment
          Task<AnnouncementDto> ToggleAnnouncementStatudAsync(Guid announcementId, bool isActive);

          //Get announcements by type
          Task<List<AnnouncementDto>> GetAnnouncementsByTypeAsync(string type);
}
