using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.ViewModels.QueryViewModel;


namespace Apptivate_UQMS_WebApp.Services
{
    public interface IQueryService
    {
        //Task HandleFileUpload(IFormFile uploadedFile, int queryId);

        Task<object> GetAcademicQueryAsync(int queryTypeId, string firebaseUid);
        Task SubmitAdministrativeQueryAsync(QueryDto model, IFormFile uploadedFile, string firebaseUid);
        Task SubmitAcademicQueryAsync(QueryDto model, IFormFile uploadedFile, string firebaseUid);

        Task<object> GetStudentQueryAsync(int queryId, string firebaseUid);
        Task<object> GetEmailAsync(string firebaseUid);

        Task<object> GetStaffAssignmentQueryDetails(int queryId, string firebaseUid);

        Task SubmitSolutionToQueryAsync(QueryResolutions model, IFormFile uploadedFile, string firebaseUid);

        //Task<object> GetResolvedTicketDetails(int queryId, string firebaseUid);
        Task<ResolvedTicketAndQueryViewModel> GetResolvedTicketDetails(int queryId, string firebaseUid);

    }
}




     
   

