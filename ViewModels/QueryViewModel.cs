using static Apptivate_UQMS_WebApp.Models.QueryModel;


namespace Apptivate_UQMS_WebApp.ViewModels
{
    public class QueryViewModel
    {
        


        public enum QueryStatus
        {
            Pending,     // Query submitted but not yet assigned
            Ongoing,     // Query assigned to staff and in progress
            Resolved,    // Query has been resolved
            Closed       // Query is closed and requires no further action
        }





    }
}
