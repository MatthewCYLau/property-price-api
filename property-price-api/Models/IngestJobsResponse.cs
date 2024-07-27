namespace property_price_api.Models;

public class IngestJobsResponse
{
    public IngestJobsResponse(PaginationMetadata paginationMetadata, List<IngestJob> ingestJobs)
    {
        PaginationMetadata = paginationMetadata;
        IngestJobs = ingestJobs;
    }

    public PaginationMetadata PaginationMetadata { get; set; }
    public List<IngestJob> IngestJobs { get; set; }

}
