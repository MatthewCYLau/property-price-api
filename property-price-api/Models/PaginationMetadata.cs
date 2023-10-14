namespace property_price_api.Models
{
	public class PaginationMetadata
	{
        public PaginationMetadata(int totalRecords, int currentPage, int totalPages)
        {
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public int TotalRecords { get; set; }
        public int? CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

